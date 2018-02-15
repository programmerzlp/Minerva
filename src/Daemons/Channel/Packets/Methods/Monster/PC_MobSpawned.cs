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

namespace Minerva
{
    partial class PacketProtocol
    {
        public static PacketBuilder PC_MobSpawned(object[] args)
        {
            // mobs[i].Id, mobs[i].SId, mobs[i].CurrentPosX, mobs[i].CurrentPosY, mobs[i].EndPosX, mobs[i].EndPosY, mobs[i].CurrentHP, mobs[i].MaxHP, mobs[i].Level
            var id = System.Convert.ToUInt16(args[0]);
            var mid = (ushort)args[1];
            var startx = (ushort)args[2];
            var starty = (ushort)args[3];
            var endx = (ushort)args[4];
            var endy = (ushort)args[5];
            var hp = (ushort)args[6];
            var maxhp = (ushort)args[7];
            var level = (byte)args[8];


            var p = new PacketBuilder();

            p.New(0xCA);
            {
                p += (byte)0x01;        // count
                p += (ushort)0x0003;

                p += id;        // uniq id?
                p += startx;    // start x
                p += starty;    // start y
                p += endx;    // end x
                p += endy;    // end y
                p += mid;       // mob species id   
                p += (ushort)0x4CA0;
                p += maxhp;    // max hp
                p += (ushort)0x4CA0;
                p += hp;    // current hp
                p += (byte)0;        // moving speed multiplier?
                p += level;        // level
                p += new byte[7];

                return p;
            }
        }
    }
}