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
        public static void StorageExchangeMove(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var unk1 = packet.ReadInt();
            var oldslot = packet.ReadInt();
            var unk3 = packet.ReadInt();
            var newslot = packet.ReadInt();

            builder.New(0x875);
            {
                builder += 1;
                builder += (byte)0;
            }

            client.Send(builder, "StorageExchangeMove");

            if (unk1 == 1 && unk3 == 0) // unequip code?
            {
                NFY_ItemUnEquip(packet, builder, client, events, oldslot);
            }
            else if (unk1 == 0 && unk3 == 1) // equip code?
            {
                NFY_ItemEquip(packet, builder, client, events, unk3);
            }

        }
    }
}
