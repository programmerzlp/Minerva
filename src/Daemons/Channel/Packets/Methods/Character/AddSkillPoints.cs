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
        public static void AddSkillPoints(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var sync = client.Metadata["syncServer"] as SyncReceiver;
            var server = (int)client.Metadata["server"];
            Character character = client.Metadata["fullchar"] as Character;

            var skill = packet.ReadUShort();
            var slot = packet.ReadByte();
            var oldlevel = packet.ReadUShort();
            var newlevel = packet.ReadUShort();

            if ((newlevel == oldlevel + 1) || (newlevel == oldlevel - 1))
            {
                CharacterManagement.UpdateSkillPoints(sync, server, character.id, skill, newlevel, slot);
                builder.New(0x0A56);
                client.Send(builder, "AddSkillPoints");
            }
            else //Punishment for hackers :D
            {
                var map = client.Metadata["map"] as IMap;
                CharacterManagement.UpdatePosition(sync, server, client.AccountID, character.slot, character.map, character.x, character.y);
                client.Disconnect();
            }
        }
    }
}
