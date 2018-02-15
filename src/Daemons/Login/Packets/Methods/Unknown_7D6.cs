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

using System;
using System.Text;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        #region PacketInfo
        /*
         * Unknown_7D6 Packet
         * -------------------------
         * Client2Server Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * int      : padding
         * ushort   : opcode
         * 
         * int      : unk1
         * byte[]   : encrypted data    #array size depends on RSA keylength, see PublicKey packet
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * int      : unk1              #is set to 1 upon successful login; otherwise 0
         * int      : unk2
         * byte     : unk3       
         * int      : status            #account status
         * -------------------------
         * If login was successful, server sends another Unknown_7D6 packet:
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * int      : unk1              #is set to 1 upon successful login; otherwise 0
         * int      : unk2
         * byte     : unk3       
         * int      : status            #account status
         * int      : unk4
         * byte     : unk5
         * int      : unk6
         * long     : unk7
         * byte     : unk8
         * int      : unk9
         * byte     : unk10
         * int      : unk11
         * int      : unk12
         * int      : unk13
         * char[32] : auth key          #possibly used for cash shop
         * short    : unk14
         * byte     : server count
         * 
         * foreach server
         *      byte     : server id
         *      byte     : character count
         * 
         * byte[]   : unk15             #array length is 248
        */
        #endregion

        public static void Unknown_7D6(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            packet.Skip(4);

            var rsa = client.Metadata["rsa"] as RSA;

            if (rsa == null)
            {
                Log.Error("Unable to decrypt RSA data!");
                client.Disconnect();
                client = null;
                return;
            }

            byte[] encrypted = packet.ReadBytes(rsa.KeyLength / 8); // keylength is in bits
            byte[] details = rsa.Decrypt(encrypted);
            byte[] tmpdata = new byte[16];

            if (details == null)
            {
                Log.Error("Unable to decrypt RSA data!");
                client.Disconnect();
                client = null;
                return;
            }

            Array.Copy(details, tmpdata, 16);
            string name = Encoding.UTF8.GetString(tmpdata).Trim('\0');

            Array.Copy(details, 65, tmpdata, 0, 16);
            string pass = Encoding.UTF8.GetString(tmpdata).Trim('\0');

            Log.Notice("Login authentication: " + name);

            string ip = client.RemoteEndPoint.ToString().Split(':')[0];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var data = Authentication.FetchAccount(syncServer, name, pass);
            bool kick = false;

            if (data == null || data.id == -1)
            {
                builder.New(0x7D6);
                {
                    builder += 0;
                    builder += 0;
                    builder += (byte)0;
                    builder += (int)AccountStatus.OutOfService;
                }

                events.FailedLogin("login.AuthAccount", new LoginEventArgs(name, pass, ip, LoginResult.OutOfService));
                client.Send(builder, "Unknown_7D6");
                return;
            }

            builder.New(0x7D6);
            {
                switch (data.status)
                {
                    case AuthStatus.Unverified:
                        builder += 0;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Unverified;
                        break;
                    case AuthStatus.Banned:
                        builder += 0;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Banned;
                        break;
                    case AuthStatus.IncorrectName:
                    case AuthStatus.IncorrectPass:
                        builder += 0;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Incorrect;
                        var tmp = data.status == AuthStatus.IncorrectName ? LoginResult.UnknownUsername : LoginResult.WrongPassword;
                        events.FailedLogin("login.AuthAccount", new LoginEventArgs(name, pass, ip, tmp));
                        break;
                    case AuthStatus.Verified:
                        client.AccountID = data.id;
                        Authentication.UpdateIPDate(syncServer, client.AccountID, ip, DateTime.Now);
                        Authentication.UpdateOnline(syncServer, client.AccountID, true);
                        builder += 1;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Normal;
                        events.SuccessfulLogin("login.AuthAccount", new LoginEventArgs(name, pass, ip, LoginResult.Success, client.AccountID));
                        break;
                    default:
                        kick = true;
                        break;
                }
            }

            client.Send(builder, "Unknown_7D6");

            if (kick)
                client.Disconnect();

            if (data.id > 0 && data.status == AuthStatus.Verified)
            {
                builder.New(0x7D6);
                {
                    builder += 1;
                    builder += 0x0900;
                    builder += (byte)0;

                    builder += (int)AccountStatus.Normal;
                    builder += 0x4B6359;
                    builder += (byte)1;
                    builder += 0x67;
                    builder += (long)0;
                    builder += (byte)0;
                    builder += 0x43A56B60;
                    builder += (byte)0;
                    builder += 1;
                    builder += 0;
                    builder += 0;
                    builder += "0A47130EA1D04A4D99D58F094B7E88C3";           // AuthKey
                    builder += (short)0;
                    builder += (byte)1;                                      // Char Num of all servers
                    builder += (byte)1;                                      // Server ID
                    builder += (byte)1;                                      // CharNum
                }

                client.Send(builder, "Unknown_7D6");

                URLToClient(packet, builder, client, events);
                SystemMessg(packet, builder, client, events);

                SendChannels.SendChannelList(client);

                var timer = new System.Timers.Timer(5000);
                timer.AutoReset = true;
                timer.Elapsed += (sender, e) => { SendChannels.SendChannelList(client); };

                timer.Start();
                client.Metadata["timer"] = timer;
            }
        }
    }
}