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
        public static void GetCharacters(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var server = (int)client.Metadata["server"];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var id = Authentication.GetUser(syncServer, (ulong)client.Metadata["magic"]);


            if (id <= 0)
            {
                client.Disconnect();
                Log.Notice("Client disconnected!");
                return;
            }


            client.AccountID = id;

            var characters = CharacterManagement.GetCharacters(syncServer, server, id);
            var subpass = SubpassManagement.GetSubpass(syncServer, id);
            var slotorder = CharacterManagement.GetSlotOrder(syncServer, server, id);

            if (slotorder == -1)
                slotorder = 0x00123456;

            client.Metadata["slotorder"] = slotorder;

            builder.New(0x85);
            {
                if (!subpass)
                    builder += 0;   // not exist
                else
                    builder += 1;   // exists

                builder += new byte[9];
                builder += (byte)1;
                builder += 0;       // selected char id
                builder += slotorder;
                builder += 8;       //open 7th and 8th slot

                for (int i = 0; i < characters.Length; i++)
                {
                    var charId = characters[i].id;
                    var style = (uint)characters[i]._class;
                    style += (uint)(characters[i].face << 8);
                    style += (uint)(characters[i].colour << 13);
                    style += (uint)(characters[i].hair << 17);
                    style += (!characters[i].gender) ? 0 : (uint)(1 << 26);

                    TimeSpan date = (characters[i].created - new DateTime(1970, 1, 1, 0, 0, 0));

                    var eq = characters[i].equipment;
                    int head = (eq.head != null) ? (int)(BitConverter.ToUInt32(eq.head, 0) + (eq.head[0x02] * 0x2000)) : (ushort)0;
                    int body = (eq.body != null) ? (int)(BitConverter.ToUInt32(eq.body, 0) + (eq.body[0x02] * 0x2000)) : (ushort)0;
                    int hands = (eq.hands != null) ? (int)(BitConverter.ToUInt32(eq.hands, 0) + (eq.hands[0x02] * 0x2000)) : (ushort)0;
                    int feet = (eq.feet != null) ? (int)(BitConverter.ToUInt32(eq.feet, 0) + (eq.feet[0x02] * 0x2000)) : (ushort)0;
                    int righthand = (eq.righthand != null) ? (int)(BitConverter.ToUInt32(eq.righthand, 0) + (eq.righthand[0x02] * 0x2000)) : (ushort)0;
                    int lefthand = (eq.lefthand != null) ? (int)(BitConverter.ToUInt32(eq.lefthand, 0) + (eq.lefthand[0x02] * 0x2000)) : (ushort)0;
                    int back = (eq.back != null) ? (int)(BitConverter.ToUInt32(eq.back, 0) + (eq.back[0x02] * 0x2000)) : (ushort)0;

                    builder += charId;
                    builder += (long)date.TotalSeconds;     // created
                    builder += style;
                    builder += characters[i].level;
                    builder += 1;
                    builder += 0;
                    builder += 0;
                    builder += (byte)0;
                    builder += characters[i].map;
                    builder += (ushort)characters[i].x;
                    builder += (ushort)characters[i].y;

                    builder += (long)head;
                    builder += (long)0;
                    builder += (long)body;
                    builder += (long)0;
                    builder += (long)hands;
                    builder += (long)0;
                    builder += (long)feet;
                    builder += (long)0;
                    builder += (long)righthand;
                    builder += (long)0;
                    builder += (long)lefthand;
                    builder += (long)0;
                    builder += (long)back;
                    builder += (long)0;

                    builder += new byte[588];

                    builder += (byte)(characters[i].name.Length + 1);
                    builder += characters[i].name;
                    builder += 0;
                    builder += 0;
                    builder += (byte)0;
                }
            }

            client.Send(builder, "GetMyChartr");
            Authentication.UpdateOnline(syncServer, id, true);
        }
    }
}