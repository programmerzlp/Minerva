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

using CharacterItems = System.Collections.Generic.List<System.Tuple<byte[], ushort, byte>>;

#endregion

namespace Minerva
{
    public partial class PipeServer
    {
        public List<byte[]> GetEquipment(int server, int charId)
        {
            var result = new List<byte[]>();

            serverdbs[server].GetEquipment(charId);
            serverdbs[server].ReadRow();

            result.Add(serverdbs[server]["head"] as byte[]);
            result.Add(serverdbs[server]["body"] as byte[]);
            result.Add(serverdbs[server]["hands"] as byte[]);
            result.Add(serverdbs[server]["feet"] as byte[]);
            result.Add(serverdbs[server]["righthand"] as byte[]);
            result.Add(serverdbs[server]["lefthand"] as byte[]);
            result.Add(serverdbs[server]["back"] as byte[]);

            return result;
        }

        public void GetInventory(int server, int charId)
        {
            serverdbs[server].GetInventory(charId);

            var items = new CharacterItems();

            while (serverdbs[server].ReadRow())
            {
                items.Add(Tuple.Create(serverdbs[server]["item"] as byte[], (serverdbs[server]["amount"] as ushort?).Value, (serverdbs[server]["slot"] as byte?).Value));
            }
        }

        public void MoveItem(int server, int charId, int oldslot, int newslot)
        {
            serverdbs[server].MoveItem(charId, oldslot, newslot);
        }

        public void RemoveItem(int server, int charId, int slot)
        {
            serverdbs[server].RemoveItem(charId, slot);
        }

        public void AddItem(int server, int charId, int slot, byte[] item, int amount = 0)
        {
            serverdbs[server].AddItem(charId, slot, item, amount);
        }

        public Tuple<byte[], int> GetItem(int server, int charId, int slot)
        {
            serverdbs[server].GetItem(charId, slot);
            serverdbs[server].ReadRow();

            return Tuple.Create(serverdbs[server]["item"] as byte[], (serverdbs[server]["amount"] as int?).Value);
        }

        public void EquipItem(int server, int charId, int itemslot, string equipslot)
        {
            serverdbs[server].EquipItem(charId, itemslot, equipslot);
        }
    }
}
