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
        public static void ArrivedAtLocation(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var x = packet.ReadUShort();
            var y = packet.ReadUShort();

            Character character = client.Metadata["fullchar"] as Character;
            var id = character.id;

            character.x = (byte)x;
            character.y = (byte)y;

            var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);
            (client.Metadata["map"] as IMap).GetTile(client, x, y);

            builder.New(0xD3);
            {
                builder += id;
                builder += x;
                builder += y;
            }

            foreach (var c in clients)
            {
                c.Send(builder, "NFY_MoveEnded");
            }

            NewMobsList(packet, builder, client, events);
            //NewUserList(packet, builder, client, events);

            CharacterManagement.UpdatePosition(client.Metadata["syncServer"] as SyncReceiver, (int)client.Metadata["server"], client.AccountID, character.slot, character.map, character.x, character.y);

        }
    }
}