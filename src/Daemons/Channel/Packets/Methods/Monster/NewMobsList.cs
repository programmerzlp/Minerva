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
        
        public static void NewMobsList(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var mobs = (client.Metadata["map"] as IMap).GetSurroundingMobs(client, 3);

            if (mobs.Count > 0)
            {
                int times = mobs.Count / 10;
                int current = 0;
                int start = 0;

                if (times > 1)
                {
                    while (current < times)
                    {
                        NFY_MobList(builder, client, mobs, start, start + 10, 10);
                        start += 10;
                        current ++;
                    }

                    int left = mobs.Count - times * 10;
                    if (left > 0)
                    {
                        NFY_MobList(builder, client, mobs, start, mobs.Count, left);
                    }
                }
                else
                {
                    NFY_MobList(builder, client, mobs, 0, mobs.Count, mobs.Count);
                }
            }
        }

        static void NFY_MobList(PacketBuilder builder, ClientHandler client, List<MobEntity> mobs, int start, int end, int count)
        {
            var map = client.Metadata["map"] as IMap;
      
            builder.New(0xCA);
            {
                builder += (byte)(count);       // count

                for (int i = start; i < end; i++)
                {
                    
                    var m = mobs[i];
                    if (m.CurrentHP > 0) {

                        builder += (ushort)m.Id;            // uniq id?
                        builder += (byte)map.ID;       // Map id
                        builder += (byte)0x02;          // 
                        builder += m.CurrentPosX;   // start x
                        builder += m.CurrentPosY;   // start y
                        builder += m.EndPosX;       // end x
                        builder += m.EndPosY;       // end y
                        builder += m.SId;           // mob species id   
                        builder += m.MaxHP;           // max hp
                        builder += m.CurrentHP;       // current hp
                        builder += (byte)0;              // moving speed multiplier?
                        builder += (byte)m.Level;           // level
                        builder += new byte[8];

                    }
                   
                }
            }

            client.Send(builder, "NFY_NewMobsList");
        }
    }
}


