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
    public class MobEntity
    {
        public uint Id;
        public ushort SId;
        public ushort CurrentPosX;
        public ushort CurrentPosY;
        public ushort EndPosX;
        public ushort EndPosY;
        public int Level;
        public int MaxHP;
        public int CurrentHP;
        public int NextTime;
        public bool IsMoving;
        public Random rnd;
        public int Spawn;



        public MobEntity(uint id, ushort sid, ushort startx, ushort starty, ushort endx, ushort endy, int level, int maxhp, int currhp, int respawn)
        {
            Id = id;
            SId = sid;
            CurrentPosX = startx;
            CurrentPosY = starty;
            EndPosX = endx;
            EndPosY = endy;
            Level = level;
            MaxHP = maxhp;
            CurrentHP = currhp;
            NextTime = Environment.TickCount;
            IsMoving = false;
            rnd = new Random();
            Spawn = respawn;
        }


    }

    public class MobData
    {
        public int I;
        public int Lev;
        public int Exp;
        public int HP;
        public int Defense;
        public int AtkRate;
        public int DefRate;
        public int Hpregen;
        public int Respawn;
        public int PatkMin;
        public int PatkMax;

        public MobData(int i, int level, int exp, int hp, int defense, int atkrate, int defrate, int hpregen, int respawn, int patkmin, int patkmax)
        {

            I = i;
            Lev = level;
            Exp = exp;
            HP = hp;
            Defense = defense;
            AtkRate = atkrate;
            DefRate = defrate;
            Hpregen = hpregen;
            Respawn = respawn;
            PatkMin = patkmin;
            PatkMax = patkmax;
        }

    }
}