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

namespace Minerva
{
    class SendChannels
    {
        #region PacketInfo
        /*
         * ServerState Packet
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * byte     : server count
         * 
         * foreach server
         *      int      : server id
         *      byte[3]  : unk1
         *      byte     : channel count
         *      foreach channel
         *          byte     : server id
         *          byte     : channel id
         *          ushort   : current players   #unconfirmed
         *          byte[21] : unk1
         *          byte     : unk2
         *          ushort   : max players       #unconfirmed
         *          uint     : channel ip
         *          ushort   : channel port
         *          byte     : unk3
         *          int      : channel type
         *          byte[3]  : unk4
        */
        #endregion

        public static void SendChannelList(ClientHandler client)
        {
            // hmm ..? the timer should be killed, or else it will just goes here until server is restarted...
            if (client == null || client.RemoteEndPoint == null)
                return;

            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var channels = ServerList.GetChannels(syncServer);

            if (channels == null)
                return;
            
            var p = new PacketBuilder();

            p.New(0x79);
            {
                p += (byte)channels.Length;

                foreach (var server in channels)
                {
                    p += server.id;
                    p += new byte[3];
                    p += (byte)server.channels.Length;

                    foreach (var channel in server.channels)
                    {
                        p += (byte)server.id;
                        p += (byte)channel.id;
                        p += (ushort)channel.curPlayers;
                        p += (ushort)0;
                        p += (ushort)0xffff;
                        p += (ushort)0;     // Capella Cur
                        p += (ushort)0;     // Proc Cur
                        p += (uint)0;       // Unknown
                        p += (ushort)0;     // Capella Cur
                        p += (ushort)0;     // Proc Cur
                        p += (ushort)0;     // Unknown
                        p += (byte)0;    // Lvl Min
                        p += (byte)0;    // Lvl Min+This
                        p += (byte)0x00;    // Min Rank
                        p += (byte)0xFF;    
                        p += (ushort)channel.maxPlayers;
                        p += channel.ip;
                        p += (ushort)channel.port;
                        p += (ulong)channel.type;
                    }
                }
            }

            client.Send(p, "ServerState");
        }
    }
}
