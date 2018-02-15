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

using System.Runtime.InteropServices;

#endregion

namespace Minerva.Structures.Database
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Item : IDatabaseStructure
    {
        public uint ID;
        /*public byte Bonus;
        public byte IsBound;
        public byte CraftType;
        public byte CraftBonus;
        public byte Upgrades;*/
        public byte Upgrade1;
        public byte Upgrade2;
        public byte Upgrade3;
        public byte Upgrade4;
        public uint ExpirationDate;

        public dynamic ToClient(dynamic d = null)
        {
            dynamic result = null;

            if (d is EquipmentSlots)
                result = new Client.Equipment();
            else
                result = new Client.Item();
            
            result.ID = ID;
            /*
            result.ID += (ushort)(Bonus * 2 << 12);
            result.ID += (ushort)(IsBound << 12);

            if (Upgrades != 0)
            {
                result.Upgrade1 = Upgrade1;
                result.Upgrade1 += 1 << 4;

                if (!(Upgrades > 1))
                    return result;

                if (Upgrade2 == Upgrade1)
                    result.Upgrade1 += 1 << 4;
                else
                {
                    result.Upgrade2 = Upgrade2;
                    result.Upgrade2 += 1 << 4;
                }

                if (!(Upgrades > 2))
                    return result;

                if (Upgrade3 == Upgrade1)
                    result.Upgrade1 += 1 << 4;
                else if (Upgrade3 == Upgrade2)
                    result.Upgrade2 += 1 << 4;

                if (!(Upgrades > 3))
                    return result;

                if (Upgrade4 == Upgrade1)
                    result.Upgrade1 += 1 << 4;
                else if (Upgrade4 == Upgrade2)
                    result.Upgrade2 += 1 << 4;
            }

            result.Craft = CraftType;
            result.Craft += (byte)(CraftBonus << 4);

            result.Upgrades = (byte)(Upgrades << 4);
            */
            //result.Craft += (byte)(CraftBonus << 4);

            if (d is EquipmentSlots)
                result.Slot = (byte)d;
            else
                result.Slot = (ushort)d;

            result.ExpirationDate = ExpirationDate;

            return result;
        }
    }
}