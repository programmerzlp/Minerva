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
        public static void SkillToUser(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

            Character character = client.Metadata["fullchar"] as Character;
            var id = character.id;

            var skill = packet.ReadShort();

            if (skill == 0x014C)
            {
                var unk2 = packet.ReadByte();
                var unk3 = packet.ReadShort();
                var unk4 = packet.ReadInt();

                builder.New(0xAF);
                {
                    builder += skill;
                    builder += (short)0x08;
                    builder += unk3;
                    builder += unk4;
                }

                client.Send(builder, "SkillToUser");

                builder.New(0xDD);
                {
                    builder += skill;
                    builder += id;
                    builder += 0x0D;
                    builder += (short)0x0200;
                    builder += unk3;
                    builder += unk4;
                }

                foreach (var c in clients)
                {
                    c.Send(builder, "NFY_SkillToUser");
                }
            }

            if (skill == 0x014B)
            {
                var unk2 = packet.ReadByte();
                var unk3 = packet.ReadShort();
                var unk4 = packet.ReadInt();

                builder.New(0xAF);
                {
                    builder += skill;
                    builder += 0x14;
                    builder += unk3;
                    builder += unk4;
                }

                client.Send(builder, "SkillToUser");

                builder.New(0xDD);
                {
                    builder += skill;
                    builder += id;
                    builder += (short)0x0C;
                    builder += (int)0x040;
                    builder += (byte)2;
                    builder += unk3;
                    builder += unk4;
                }

                foreach (var c in clients)
                {
                    c.Send(builder, "NFY_SkillToUser");
                }
            }

            if (skill == 0x014A)
            {
                var unk2 = packet.ReadByte();
                var unk3 = packet.ReadShort();
                var unk4 = packet.ReadInt();

                builder.New(0xAF);
                {
                    builder += skill;
                    builder += 0x13;
                    builder += unk3;
                    builder += unk4;
                }

                client.Send(builder, "SkillToUser");

                builder.New(0xDD);
                {
                    builder += skill;
                    builder += id;
                    builder += (short)0x0820;
                    builder += (int)0xC8;
                    builder += (byte)2;
                    builder += unk3;
                    builder += unk4;
                }

                foreach (var c in clients)
                {
                    c.Send(builder, "NFY_SkillToUser");
                }
            }

            if (skill == 0x00D1)
            {
                var unk2 = packet.ReadByte();
                var x = packet.ReadShort();
                var y = packet.ReadShort();

                builder.New(0xAF);
                {
                    builder += skill;
                    builder += 0;
                    builder += (short)0x039C;
                }

                client.Send(builder, "SkillToUser");

                builder.New(0xDD);
                {
                    builder += skill;
                    builder += id;
                    builder += 1;
                    builder += x;
                    builder += y;
                }

                foreach (var c in clients)
                {
                    c.Send(builder, "NFY_SkillToUser");
                }
            }

            if (skill == 0x00D0)
            {
                var unk2 = packet.ReadByte();
                var x = packet.ReadShort();
                var y = packet.ReadShort();

                builder.New(0xAF);
                {
                    builder += skill;
                    builder += 0;
                    builder += (short)0x0395;
                }

                client.Send(builder, "SkillToUser");

                builder.New(0xDD);
                {
                    builder += skill;
                    builder += id;
                    builder += 1;
                    builder += x;
                    builder += y;
                }

                foreach (var c in clients)
                {
                    c.Send(builder, "NFY_SkillToUser");
                }
            }
        }
    }
}