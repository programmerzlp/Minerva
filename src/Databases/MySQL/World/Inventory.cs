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

using MySql.Data.MySqlClient;

#endregion

namespace Minerva
{
    public partial class DatabaseProtocol : IDatabaseProtocol
    {
        public int GetItemCount(int characterId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT COUNT(id) FROM characters_items WHERE charId = '{0}'", characterId);
            var tmp = cmd.ExecuteScalar();
            cmd.Dispose();

            return Convert.ToInt32(tmp);
        }

        public void GetInventory(int characterId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT item, amount, slot FROM characters_items WHERE charId = {0}", characterId);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public void GetEquipment(int charId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM characters_equipment WHERE charId = {0} LIMIT 1", charId);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public void MoveItem(int charId, int oldslot, int newslot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE characters_items SET slot = '{1}' WHERE charId = '{0}' AND [slot] = '{2}'", charId, newslot, oldslot);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void RemoveItem(int charId, int slot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("DELETE characters_items WHERE charId = '{0}' AND slot = '{1}'", charId, slot);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void AddItem(int charId, int slot, byte[] item, int amount)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("INSERT INTO characters_items (charId, item, amount, slot) VALUES ('{0}', @item, '{1}', '{2}')", charId, (ushort)amount, (byte)slot);
            cmd.Parameters.Add("@item", MySqlDbType.Binary, 15).Value = item;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void GetItem(int charId, int slot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM characters_items WHERE charId = '{0}' AND slot = '{1}' LIMIT 1", charId, slot);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public void EquipItem(int charId, int itemslot, string equipslot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT item FROM characters_items WHERE charId = '{0}' AND slot = '{1}' LIMIT 1", charId, itemslot);

            var item = cmd.ExecuteScalar() as byte[];
            cmd.CommandText = string.Format("UPDATE characters_equipment SET {1} = @item WHERE charId = '{0}'; " +
                                            "DELETE characters_items WHERE charId = '{0}' AND slot = '{2}'", charId, equipslot, itemslot);
            cmd.Parameters.Add("@item", MySqlDbType.Binary, 15).Value = item;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public int GetCashItemCount(int account)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT COUNT(ID) FROM cash_item WHERE usernum = '{0}' AND recv='0'", account);
            var tmp = cmd.ExecuteScalar();
            cmd.Dispose();

            return Convert.ToInt32(tmp);
        }

        public void GetCashItem(int account)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM cash_item WHERE usernum = '{0}' AND recv='0'", account);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public void SetCashItem(int ID)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE cash_item SET recv='1' WHERE ID = '{0}' AND recv='0'", ID);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}
