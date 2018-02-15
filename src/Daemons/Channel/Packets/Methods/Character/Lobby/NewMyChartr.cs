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
        public static void CreateCharacter(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var style = packet.ReadInt();
            packet.Skip(2);
            var slot = packet.ReadByte();
            var nameLength = packet.ReadByte();
            var name = packet.ReadString(nameLength);

            var _class = (byte)(style & 0xFF ^ 8);
            style >>= 8;
            var colour = (byte)((style & 0xFF) >> 5);
            var face = (byte)(style & 0x1F);
            style >>= 8;
            var hair = (byte)((style & 0xFF) >> 1);
            style >>= 8;
            var gender = (style != 0);

            var server = (int)client.Metadata["server"];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var slotorder = (int)client.Metadata["slotorder"];

            var slots = new int[8]
            {
                slotorder / 0x10000000 % 0x10,
                slotorder / 0x1000000 % 0x10,
                slotorder / 0x100000 % 0x10,
                slotorder / 0x10000 % 0x10,
                slotorder / 0x1000 % 0x10,
                slotorder / 0x100 % 0x10,
                slotorder / 0x10 % 0x10,
                slotorder % 0x10,
            };

            for (int i = 0; i < 8; i++)
            {
                if (slots[i] == slot)
                {
                    slot = (byte)i;
                    break;
                }
            }

            slot = (byte)slots[slot];

            var status = CharacterManagement.CreateCharacter(syncServer, server, client.AccountID, slot, name, _class, gender, face, hair, colour);

            builder.New(0x86);
            {
                builder += client.AccountID * 8 + slot;
                builder += (byte)status;
            }

            client.Send(builder, "NewMyChartr");
        }
    }
}