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
    struct WarpIndex
    {
        public int X;
        public int Y;
        public int CapX;
        public int CapY;
        public int ProX;
        public int ProY;
        public int WCode;    // no idea what this is, yet
        public int Fee;
        public int WorldID;
        public int Level;

        public WarpIndex(int x, int y, int capx, int capy, int prox, int proy, int wcode, int fee, int world, int level) : this()
        {
            X = x;
            Y = y;
            CapX = capx;
            CapY = capy;
            ProX = prox;
            ProY = proy;
            WCode = wcode;
            Fee = fee;
            WorldID = world;
            Level = level;
        }
    }

    struct WarpList
    {
        public int Order;
        public int Type;
        public int TargetID;
        public int Level;
        public int Fee;
        public int ItemID;
        public int ItemOpt;
        public int QuestID;
        public bool IsGPSVisible;

        public WarpList(int order, int type, int target, int level, int fee, int item, int opt, int quest, bool gps) : this()
        {
            Order = order;
            Type = type;
            TargetID = target;
            Level = level;
            Fee = fee;
            ItemID = item;
            ItemOpt = opt;
            QuestID = quest;
            IsGPSVisible = gps;
        }
    }
}