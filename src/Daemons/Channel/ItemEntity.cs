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
    public class ItemEntity
    {
        public ushort ID;
        public uint Owner;
        public uint Party;
        public byte Bonus;
        public uint Amount;
        public uint Expiration;
        public uint Craft;
        public uint CraftBonus;
        public uint Upgrades;
        public uint Upgrade1;
        public uint Upgrade2;
        public uint Upgrade3;
        public uint Upgrade4;

        int tickcount;
        int lifetime;

        public ItemEntity(int lifetime, ushort id, uint owner, uint party, byte bonus, uint amount, uint expiration, byte craft, byte craftBonus, byte upgrades, byte upgrade1, byte upgrade2, byte upgrade3, byte upgrade4)
        {
            this.lifetime = lifetime;

            ID = id;
            Owner = owner;
            Party = party;
            Bonus = bonus;
            Amount = amount;
            Expiration = expiration;
            Craft = craft;
            CraftBonus = craftBonus;
            Upgrades = upgrades;
            Upgrade1 = upgrade1;
            Upgrade2 = upgrade2;
            Upgrade3 = upgrade3;
            Upgrade4 = upgrade4;

            tickcount = Environment.TickCount;
        }

        public bool UpdateOrDie()
        {
            var ticks = Environment.TickCount;
            var elapsed = ticks - tickcount;

            lifetime -= elapsed;
            tickcount = ticks;

            return (lifetime <= 0);
        }
    }
}