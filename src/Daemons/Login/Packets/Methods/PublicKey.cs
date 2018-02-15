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
         * SystemMessg Packet
         * -------------------------
         * Client2Server Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * int      : padding
         * ushort   : opcode
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort   : magic code
         * ushort   : size
         * ushort   : opcode
         * 
         * byte     : unk1
         * ushort   : public key length
         * byte[]   : public key
         * -------------------------
         * Key Length determines the length of encrypted data in Unknown_7D6 packet
        */
        #endregion

        public static void PublicKey(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var rsa = new RSA();

            client.Metadata["rsa"] = rsa;

            builder.New(0x7D1);
            {
                builder += (byte)1;
                builder += rsa.KeyLength;
                builder += rsa.PublicKey;
            }

            client.Send(builder, "PublicKey");
        }
    }
}