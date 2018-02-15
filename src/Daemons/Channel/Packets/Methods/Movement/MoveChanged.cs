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
using System.Collections.Generic;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void AlterDestination(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var turnX = packet.ReadUShort();
            var turnY = packet.ReadUShort();
            var endX = packet.ReadUShort();
            var endY = packet.ReadUShort();
            //var unkX = packet.ReadShort();
            //var unkY = packet.ReadShort();
            //var unk0 = packet.ReadShort();

            Character character = client.Metadata["fullchar"] as Character;
            var id = character.id;
            var timestamp = (uint)client.Metadata["timestamp"];

            character.x = (byte)turnX;
            character.y = (byte)turnY;
            client.Metadata["dest_x"] = endX;
            client.Metadata["dest_y"] = endY;

            var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

            builder.New(0xD4);
            {
                builder += id;
                builder += Environment.TickCount - timestamp;
                builder += turnX;
                builder += turnY;
                builder += endX;
                builder += endY;
            }

            foreach (var c in clients)
            {
                c.Send(builder, "NFY_MoveChanged");
            }

            NewMobsList(packet, builder, client, events);
            //NewUserList(packet, builder, client, events);

            CharacterManagement.UpdatePosition(client.Metadata["syncServer"] as SyncReceiver, (int)client.Metadata["server"], client.AccountID, character.slot, character.map, character.x, character.y);

        }
    }
}