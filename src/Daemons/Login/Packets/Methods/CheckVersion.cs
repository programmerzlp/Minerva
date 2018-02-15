/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    https://github.com/dignityteam/minerva
    http://www.ragezone.com


    This file is part of Minerva.

    Minerva is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Minerva is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System.Timers;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        #region PacketInfo
        /*
         * CheckVersion Packet
         * -------------------------
         * Client2Server Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * int      : padding
         * ushort   : opcode
         * 
         * int      : version1         #client version
         * int      : version2         #debug
         * int      : version3         #reserved
         * int      : version4         #reserved
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * int      : version1         #client version
         * int      : version2         #debug
         * int      : version3         #reserved
         * int      : version4         #reserved
        */
        #endregion

        public static void CheckVersion(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            int version = packet.ReadInt();

            var conf = client.Metadata["conf"] as Configuration;

            if (conf == null)
                return;

            int clientVersion = conf.ClientVersion;

            // i dont like this one, should be way better: client.GetIpAddress();
            string ip = client.RemoteEndPoint.ToString().Split(':')[0];
            bool kick = false;

            if (conf.Debug)
            {
                // skipping on debug mode

                builder.New(0x7A);
                {
                    builder += version;     // Client Version
                    builder += 0;           // Debug
                    builder += 0;           // Reserved
                    builder += 0;           // Reserved
                }

                events.VersionMismatch("login.CheckVersion", new VersionCheckEventArgs(version, ip, VersionCheckResult.Match));

                client.Send(builder, "CheckVersion");
                return;
            }

            var result = VersionCheckResult.None;

            if (version != clientVersion)
            {
                result = version > clientVersion ? VersionCheckResult.NewerClient : VersionCheckResult.OlderClient;
                kick = true;
                Log.Notice(string.Format("Failed CheckVersion Ip: {0} Client: {1}; Server: {2}", ip, version, clientVersion));
            }
            else
                result = VersionCheckResult.Match;

            events.VersionMismatch("login.CheckVersion", new VersionCheckEventArgs(version, ip, result));

            builder.New(0x7A);
            {
                builder += clientVersion;   // Client Version
                builder += 0;               // Debug
                builder += 0;               // Reserved
                builder += 0;               // Reserved
            }

            client.Send(builder, "CheckVersion");

            if (kick)
            {
                // we have to be sure that client will be disconnected
                // if client didnt closed it's connection, server will close after 800ms
                var waitKick = new Timer(800);
                waitKick.Elapsed += (s, _e) =>
                {
                    client.Disconnect();
                    waitKick.Stop();
                };

                waitKick.Start();
                return;
            }

            int id = -2;

            var sleep = new Timer(500);
            sleep.Elapsed += (s, _e) => 
            {
                if (id == -2)
                {
                    var syncServer = client.Metadata["syncServer"] as SyncReceiver;
                    id = Authentication.GetUser(syncServer, (ulong)client.Metadata["magic"]);
                }

                if (id > 0)
                {
                    client.AccountID = id;
                    SendChannels.SendChannelList(client);

                    var timer = new Timer(5000);
                    timer.AutoReset = true;
                    timer.Elapsed += (sender, e) => { SendChannels.SendChannelList(client); };

                    timer.Start();
                    client.Metadata["timer"] = timer;
                }

                sleep.Stop();
            };

            sleep.Start();
        }
    }
}