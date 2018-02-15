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
         * Connect2Svr Packet
         * -------------------------
         * Client2Server Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * int      : padding
         * ushort   : opcode
         * 
         * uint     : auth key          #timestamp
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * uint     : xor seed
         * uint     : auth key          #timestamp
         * ushort   : index             #user idx
         * ushort   : xor key index
        */
        #endregion

        public static void Connect(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            uint key = 0x49B4DDD1;
            ushort step = 0x1621;

            uint timestamp = (uint)client.Metadata["timestamp"];
            ushort count = (ushort)client.Metadata["count"];
            
            builder.New(0x65);
            {
                builder += key;         // XOR Seed
                builder += timestamp;   // AuthKey
                builder += count;       // Index
                builder += step;        // XOR Key Index
            }

            client.ChangeKey(key, step);

            client.Send(builder, "Connect2Svr");
        }
    }
}