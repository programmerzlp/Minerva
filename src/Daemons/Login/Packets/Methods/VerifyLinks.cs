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
    partial class PacketProtocol
    {
        #region PacketInfo
        /*
         * VerifyLinks Packet
         * -------------------------
         * Client2Server Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * int      : padding
         * ushort   : opcode
         * 
         * uint     : auth key
         * ushort   : index
         * byte     : channel id
         * byte     : server id
         * uint     : magic key
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * byte     : channel id
         * byte     : server id
         * byte     : success       #1 = true, 0 = false
        */
        #endregion

        public static void VerifyLinks(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            int timestamp = packet.ReadInt();
            short count = packet.ReadShort();
            byte channel = packet.ReadByte();
            byte server = packet.ReadByte();
            int magickey = packet.ReadInt();

            var conf = client.Metadata["conf"] as Configuration;

            if (conf == null)
                return;

            int magic = conf.MagicKey;

            // skipping on debug mode
            if (!conf.Debug && magickey != magic)
            {
                Log.Notice("Bad Client! Client MagicKey: {0}, Server MagicKey: {1}", magickey, magic);
                return;
            }

            ulong key = ((ulong)count << 32) + (ulong)timestamp;

            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            Authentication.AddUser(syncServer, key, client.AccountID);

            builder.New(0x66);
            {
                builder += channel;         // channel
                builder += server;          // server
                builder += (byte)1;         // success
            }

            client.Send(builder, "VerifyLinks");
            
            (client.Metadata["timer"] as System.Timers.Timer).Stop();
        }
    }
}