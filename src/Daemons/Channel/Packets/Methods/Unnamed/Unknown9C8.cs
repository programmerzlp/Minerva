/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    http://board.thedivinityproject.com
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

using DItem = Minerva.Structures.Database.Item;
using CItem = Minerva.Structures.Client.Item;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void Unknown9C8(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            Character character = client.Metadata["fullchar"] as Character;
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var clients = client.Metadata["clients"] as Dictionary<ulong, ClientHandler>;
            var AccID = client.AccountID;
            var magic = (ulong)client.Metadata["magic"];
            var ip = client.RemoteEndPoint;

            
            builder.New(0x09C8);
            client.Send(builder, "Unknown9C8");

            var id = character.id;

            if(id==-999999998) {
                builder.New(0xC9);
                {
                    builder += id;
                    builder += (byte)0x12;
                }

                foreach (var c in clients.Values)
                {
                    c.Send(builder, "DelUserList");
                }

                Authentication.UpdateOnline(syncServer, AccID, true);
            }
            else
            {
                Authentication.UpdateOnline(syncServer, AccID, false);
            }

            Remove(clients, syncServer, events, magic, client, ip, AccID);

        }

        public static void Remove(Dictionary<ulong, ClientHandler> clients, SyncReceiver syncServer, EventHandler events, ulong magic, ClientHandler client, System.Net.EndPoint ip, int AccID) {
            try
            {
                foreach (ClientHandler cli in clients.Values)
                {
                    if (cli.RemoteEndPoint == null)
                    {
                        clients.Remove((ulong)cli.Metadata["magic"]);
                        Authentication.GetUser(syncServer, (ulong)cli.Metadata["magic"]);
                        client.Disconnect();
                    }
                    else if (cli.RemoteEndPoint==ip)
                    {
                        clients.Remove(magic);
                        Authentication.GetUser(syncServer, magic);
                        events.ClientDisconnected(events, client);
                    }
                }
            }
            catch (Exception)
            {
                Log.Notice("Account {0} already removed",AccID);
            }
        }
    }
}
