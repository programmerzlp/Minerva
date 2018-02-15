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
        public static void NewUserList(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

            if (clients.Count > 1)
            {
                builder.New(0xC8);
                {
                    builder += (byte)1;

                    foreach (var c in clients)
                    {

                        builder += (byte)0x00;

                        Character character = client.Metadata["fullchar"] as Character;

                        //if (c == client) continue;
                        //if (character.id == 0) continue;

                        var cid = character.id;
                        var level = character.level;
                        var timestamp = (uint)client.Metadata["timestamp"];
                        var x = (ushort)character.x;
                        var y = (ushort)character.y;
                        var dx = (ushort)client.Metadata["dest_x"];
                        var dy = (ushort)client.Metadata["dest_y"];
                        var style = (int)client.Metadata["style"];
                        var name = character.name;

                        builder += cid;

                        builder += (byte)0x00;
                        builder += (byte)0x8A;
                        builder += (byte)0xED;


                        builder += (short)0x0100;
                        builder += level;
                        builder += (short)0;

                        builder += (int)192;
                        builder += (int)192;
                        builder += (int)0;
                        builder += (int)0;
                        builder += 0x01C2;
                        //builder += (short)0;
                        //builder += (int)(Environment.TickCount - (int)timestamp);
                        builder += x;
                        builder += y;
                        builder += dx;                      // end x
                        builder += dy;                      // end y
                        builder += (byte)0;                 // pk penalty
                        builder += 0;
                        builder += (short)0;
                        builder += style;
                        //builder += (byte)0;                 // animation id..?   1 = dance, 2 = sit, 3 = dance, 4 = ?, 5 = dance2
                        builder += 0;
                        builder += 0;
                        builder += 4;
                        builder += new byte[21];
                        builder += (byte)(name.Length + 1);
                        builder += name;
                        builder += (byte)0;

                        builder += (byte)1;
                        builder += (byte)105;
                        builder += new byte[19];
                        builder += (byte)2;
                        builder += (byte)150;
                        builder += new byte[19];
                        builder += (byte)3;
                        builder += (byte)195;
                        builder += new byte[19];
                        builder += (byte)4;
                        builder += (byte)90;
                        builder += new byte[19];
                    }
                }

                foreach (var c in clients)
                {
                    if (c == client)
                    {
                        continue;
                    }
                    c.Send(builder, "NFY_NewUserList");
                }

#if DEBUG
                string notice = "";
                for (int i = 0; i < builder.Size; i++)
                {
                    notice += builder.Data[i].ToString("X2") + " ";
                }

                Log.Notice("Clients: " + clients.Count + " | PCKT: " + notice);
#endif

            }
            else
            {
                builder.New(0xC8);
                {
                    Character character = client.Metadata["fullchar"] as Character;
                    var cid = character.id;
                    var level = character.level;
                    var timestamp = (uint)client.Metadata["timestamp"];
                    var x = (ushort)character.x;
                    var y = (ushort)character.y;
                    var dx = (ushort)client.Metadata["dest_x"];
                    var dy = (ushort)client.Metadata["dest_y"];
                    var style = (int)client.Metadata["style"];
                    var name = character.name;
                    /*
                    builder += (short)0x3001;
                    builder += cid;
                    builder += (short)9;
                    builder += (short)0x0100;
                    builder += level;
                    builder += 0x01C2;
                    //builder += (int)(Environment.TickCount - (int)timestamp);
                    builder += x;
                    builder += y;
                    builder += dx;                      // end x
                    builder += dy;                      // end y
                    builder += (byte)0;                 // pk penalty
                    builder += 0;
                    builder += (short)0;
                    builder += style;
                    builder += (byte)0;                 // animation id..?
                    builder += new byte[2];
                    builder += 4;
                    builder += new byte[21];
                    builder += (byte)(name.Length + 1);
                    builder += name;
                    builder += (byte)0;
                    builder += (short)0x6901;
                    builder += new byte[7];
                    builder += (byte)2;
                    builder += (byte)150;
                    builder += new byte[7];
                    builder += (byte)3;
                    builder += (byte)195;
                    builder += new byte[7];
                    builder += (byte)4;
                    builder += (byte)90;
                    builder += new byte[7];*/



                    builder += cid;

                    builder += (byte)0x00;
                    builder += (byte)0x8A;
                    builder += (byte)0xED;


                    builder += (short)0x0100;
                    builder += level;
                    builder += (short)0;

                    builder += (int)192;
                    builder += (int)192;
                    builder += (int)0;
                    builder += (int)0;
                    builder += 0x01C2;
                    builder += (short)0;
                    //builder += (int)(Environment.TickCount - (int)timestamp);
                    builder += x;
                    builder += y;
                    builder += dx;                      // end x
                    builder += dy;                      // end y
                    builder += (byte)0;                 // pk penalty
                    builder += 0;
                    builder += (short)0;
                    builder += style;
                    //builder += (byte)0;                 // animation id..?
                    builder += 0;
                    builder += 0;
                    builder += 4;
                    builder += new byte[21];
                    builder += (byte)(name.Length + 1);
                    builder += name;
                    builder += (byte)0;

                    builder += (byte)1;
                    builder += (byte)105;
                    builder += new byte[19];
                    builder += (byte)2;
                    builder += (byte)150;
                    builder += new byte[19];
                    builder += (byte)3;
                    builder += (byte)195;
                    builder += new byte[19];
                    builder += (byte)4;
                    builder += (byte)90;
                    builder += new byte[19];

                    client.Send(builder, "NFY_NewUserList");
                }

#if DEBUG
                string notice = "";
                for (int i = 0; i < builder.Size; i++)
                {
                    notice += builder.Data[i].ToString("X2") + " ";
                }

                Log.Notice(notice);
#endif
            }
        }
    }
}