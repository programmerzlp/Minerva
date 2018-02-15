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
        public static void ChangeChannel(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            if (client == null || client.RemoteEndPoint == null) return;
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            Character character = client.Metadata["fullchar"] as Character;

            Authentication.UpdateOnline(syncServer, client.AccountID, false);
            var newServer = packet.ReadByte();
            var newChannel = packet.ReadByte();

            builder.New(0x085D);
            {
                builder += (int)1;
            }
            
            client.Send(builder,"ChangeChannel");
        }
    }
}
