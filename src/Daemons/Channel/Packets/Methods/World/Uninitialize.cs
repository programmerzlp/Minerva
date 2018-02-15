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

using System.Collections.Generic;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void LeaveWorld(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var unk = packet.ReadByte();
            packet.Skip(1);
            var mapID = packet.ReadByte();

            Character character = client.Metadata["fullchar"] as Character;
            var id = character.id;

            if(id== -999999998) return;

            var clients = client.Metadata["clients"] as Dictionary<ulong, ClientHandler>;
            var map = client.Metadata["map"] as IMap;
            var server = (int)client.Metadata["server"];
            var slot = character.slot;
            var x = character.x;
            var y = character.y;
            var magic = (ulong)client.Metadata["magic"];

            var syncServer = client.Metadata["syncServer"] as SyncReceiver;

            CharacterManagement.UpdatePosition(syncServer, server, client.AccountID, slot, character.map, x, y);

            builder.New(0xC9);
            {
                builder += id;
                builder += (byte)0x12;
            }

            character.id = 0;

            foreach (var c in clients.Values)
            {
                c.Send(builder, "DelUserList");
            }

            builder.New(0x8F);
            {
                builder += (byte)0;
            }

            client.Send(builder, "Uninitialze");
            map.RemoveClient(client);
        }
    }
}