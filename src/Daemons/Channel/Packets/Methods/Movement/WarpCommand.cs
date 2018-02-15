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
        public static void WarpCommand(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {

            Character character = client.Metadata["fullchar"] as Character;
            var npc = packet.ReadUShort();
            var unk1 = packet.ReadUShort();
            var unk2 = packet.ReadByte();
            var unk3 = packet.ReadByte();
            var unk4 = packet.ReadByte();

            var y = packet.ReadByte();
            var x = packet.ReadByte();
            var unk5 = packet.ReadByte();
            var unk6 = packet.ReadByte();

            var warpID = packet.ReadUInt();

            var unk7 = packet.ReadByte();
            var unk8 = packet.ReadByte();
            var unk9 = packet.ReadByte();
            var unk10 = packet.ReadByte();
            var unk11 = packet.ReadByte();
            var unk12 = packet.ReadByte();
            var unk13 = packet.ReadByte();
            var unk14 = packet.ReadByte();

            var map = (client.Metadata["map"] as IMap);
            var fee = character.stats.alz;
            var alive = (warpID == 0 && npc != 54 && npc != 63);

            int[] dest = null;

            if (alive)
                dest = map.GetWarpDestination(npc, 0);
            else if (npc == 201)
                warpID = (uint)map.ID;
            else
                dest = map.GetDeathDestination();

            if (dest!=null)
            {
                warpID = (byte)dest[0];
                x = (byte)dest[1];
                y = (byte)dest[2];
            }

            map.GetSurroundingClients(client, 1);
            map.RemoveClient(client);
            events.Warped("world.WarpCommand", client, (int)warpID, x, y);

            builder.New(0x00F4); // TODO: Load x,y values from WarpList
            {
                builder += (ushort)x;
                builder += (ushort)y;
                builder += (uint)character.stats.exp;
                builder += 0;                           //axp
                builder += fee;                         //Alz (for death penalty and fee?)
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (ulong)0;
                builder += warpID;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
                builder += (byte)1;
            }

            #region PacketAnalisys  
#if DEBUG
            string notice = "";

            /*for (int i = 0; i< packet.Size-10; i++)
            {
                notice += packet.ReadByte()+" ";   
            }
            */

            notice += npc + " " + unk1 + " " + unk2 + " " + unk3 + " " + unk4 + " " + x + " " + y + " " + unk5 + " " + unk6 + " " + warpID + " "
                + unk7 + " " + unk8 + " " + unk9 + " " + unk10 + " " + unk11 + " " + unk12 + " " + unk13 + " " + unk14;
            Log.Notice(notice);
#endif
            #endregion

            client.Send(builder, "WarpCommand");

            character.map = (byte)warpID;
            character.x = x;
            character.y = y;

            CharacterManagement.UpdatePosition(client.Metadata["syncServer"] as SyncReceiver, (int)client.Metadata["server"], client.AccountID, character.slot, (byte)warpID, character.x, character.y);

            NewMobsList(packet, builder, client, events);
            //NewUserList(packet, builder, client, events);
        }
    }
}