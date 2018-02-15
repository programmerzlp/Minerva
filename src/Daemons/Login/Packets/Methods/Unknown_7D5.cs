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
         * Unknown_7D5 Packet
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * int      : unk1          #might be ip address, unconfirmed
         * int      : unk2
         * byte     : unk3
        */
        #endregion

        public static void Unknown_7D5(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            builder.New(0x7D5);
            {
                builder += (ushort)0xD4C0;
                builder += 1;
                builder += (short)0x00;
                builder += (byte)0xB7;
            }

            client.Send(builder, "Unknown_7D5");
        }
    }
}