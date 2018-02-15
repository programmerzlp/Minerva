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
    partial class SyncHandler
    {
        Dictionary<int, List<int>> initialCharStats;
        Dictionary<int, List<Tuple<byte[], ushort, byte>>> initialCharItems;
        Dictionary<int, List<int[]>> initialCharSkills;
        Dictionary<int, List<int[]>> initialCharQuickslots;


        public void SetInitialCharStats(Dictionary<int, List<int>> initialCharStats)
        {
            this.initialCharStats = initialCharStats;
        }

        public void SetInitialCharItems(Dictionary<int, List<Tuple<byte[], ushort, byte>>> initialCharItems)
        {
            this.initialCharItems = initialCharItems;
        }

        public void SetInitialCharSkills(Dictionary<int, List<int[]>> initialCharSkills)
        {
            this.initialCharSkills = initialCharSkills;
        }

        public void SetInitialCharQuickSlots(Dictionary<int, List<int[]>> initialCharQuickslots)
        {
            this.initialCharQuickslots = initialCharQuickslots;
        }

        public List<int> GetInitialCharStats(byte _class)
        {
            lock (initialCharStats)
            {
                return initialCharStats[_class];
            }
        }

        public List<Tuple<byte[], ushort, byte>> GetInitialCharItems(byte _class)
        {
            lock (initialCharItems)
            {
                return initialCharItems[_class];
            }
        }

        public List<int[]> GetInitialCharSkills(byte _class)
        {
            lock (initialCharSkills)
            {
                return initialCharSkills[_class];
            }
        }

        public List<int[]> GetInitialCharQuickSlots(byte _class)
        {
            lock (initialCharQuickslots)
            {
                return initialCharQuickslots[_class];
            }
        }
    }
}