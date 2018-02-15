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

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void ServerEnv(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            builder.New(0x1D0);
            {
                builder += (ushort)0xC8;        // MaxLevel
                builder += (byte)0x01;          // UseDummy
                builder += (byte)0x01;          // Allow CashShop
                builder += (byte)0x00;          // Allow NetCafePoint
                builder += (ushort)0x0A;        // MaxRank
                builder += (ushort)0x28;        // Limit Loud Character Lv
                builder += (ushort)0x05;        // Limit Loud Mastery Lv
                builder += (ulong)0x0DF8475800; // Limit Inventory Alz Save
                builder += (ulong)0x0DF8475800; // Limit Warehouse Alz Save
                builder += (ulong)0x09502F9000;  
                builder += 1;
                builder += new byte[3];
                builder += 1;
                builder += (short)0x00;
                builder += (short)0x0A;
                builder += (short)0x0A;
                builder += 0x0101;
                builder += (byte)0x01;          // Min Level For Normal Chat
                builder += (byte)0x0A;
                builder += 100;
                builder += 0x3B9ACA00;
                builder += (short)0x07;
                builder += (short)0x00;
                builder += 0x010000;
                builder += 0xEE6B2800;
                builder += 0;
                builder += 0x88CA6C00;
                builder += 0xFFFFFFFF;
                builder += 0;
                builder += 2;
                builder += 1;
                builder += 10;
                builder += 15;
                builder += 2;
                builder += 109;
                builder += 110;
                builder += new byte[253];
            }

            client.Send(builder, "ServerEnv");
            
            /*
            UnknownA0B(packet, builder, client, events);
            NFY_ChargeInfo(packet, builder, client, events);
            Unknown9D6(packet, builder, client, events);
            Unknown9E0(packet, builder, client, events);
            Unknown8C8(packet, builder, client, events);*/
        }
    }
}