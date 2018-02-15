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
        public static void CharacterSelect(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            

            builder.New(0x03D9);
            {
                builder += (byte)1;
            }

            Character character = client.Metadata["fullchar"] as Character;
            CharacterManagement.UpdatePosition(client.Metadata["syncServer"] as SyncReceiver, (int)client.Metadata["server"], client.AccountID, character.slot, character.map, character.x, character.y);

            client.Send(builder, "CharacterSelect");

            

           
            if (character.id!= -999999998) {
                character.id = -999999998;
            }else
            {
                Log.Error("Character ID Already -999999998!", "[Channel::CharacterSelect::CharacterSelect()]");
            }

            

        }
    }
}