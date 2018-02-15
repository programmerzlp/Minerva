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

using MySql.Data.MySqlClient;

#endregion

namespace Minerva
{
    public partial class DatabaseProtocol : IDatabaseProtocol
    {
        public int GetCharactersCount(int account)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT COUNT(id) FROM characters WHERE characters.account = '{0}'", account);
            var tmp = cmd.ExecuteScalar();
            cmd.Dispose();

            return Convert.ToInt32(tmp);
        }

        public void GetCharacters(int account)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM characters, characters_equipment WHERE characters.account = '{0}' AND characters_equipment.charId = characters.id ORDER BY characters.slot", account);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public int GetSkillCount(int characterId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT COUNT(id) FROM characters_skills WHERE charId = '{0}'", characterId);
            var tmp = cmd.ExecuteScalar();
            cmd.Dispose();

            return Convert.ToInt32(tmp);
        }

        public void GetSkills(int charId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT skill, level, slot FROM characters_skills WHERE charId = {0}", charId);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public int GetQuickSlotCount(int characterId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT COUNT(id) FROM characters_quickslots WHERE charId = '{0}'", characterId);
            var tmp = cmd.ExecuteScalar();
            cmd.Dispose();

            return Convert.ToInt32(tmp);
        }

        public void GetQuickslots(int charId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT skill, slot FROM characters_quickslots WHERE charId = '{0}'", charId);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public void SetQuickSlots(int charId, int quickslot, int skill)
        {
            Cleanup();

            var cmd = sql.CreateCommand();

            if (skill == -1)
                cmd.CommandText = string.Format("DELETE FROM characters_quickslots WHERE charID ='{0}' AND slot = '{1}'", charId, quickslot);
            else
                cmd.CommandText = string.Format("REPLACE INTO characters_quickslots SET charID = '{0}', skill='{1}', slot = '{2}'", charId, skill, quickslot);

            cmd.ExecuteScalar();
            cmd.Dispose();
        }


        public void UpdateSkillPoints(int charId, ushort skillid, ushort level, byte slot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();

            cmd.CommandText = string.Format("Update characters_skills set level='" + level + "' where skill='{0}' and slot='{1}' and charId='{2}'", skillid, slot, charId);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }


        public void UpdateStatPoints(int charId, int str, int intel, int dex, int pnt)
        {
            Cleanup();

            var cmd = sql.CreateCommand();

            cmd.CommandText = string.Format("UPDATE characters_stats SET str_stat = '" + str + "', int_stat = '" + intel + "', dex_stat = '" + dex + "', pnt_stat = pnt_stat+'" + pnt + "' WHERE charId = '{0}'", charId);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public void GetStats(int charId)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM characters_stats WHERE charId = '{0}' LIMIT 1", charId);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public int CreateCharacter(int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour, int[] initialStats, List<Tuple<byte[], ushort, byte>> initialItems, List<int[]> initialSkills, List<int[]> initialQuickslots)
        {
            Cleanup();

            var cmd = sql.CreateCommand();

            // Check if the name is taken.
            cmd.CommandText = string.Format("SELECT id FROM characters WHERE name = '{0}' LIMIT 1", name);

            if (cmd.ExecuteScalar() != null)
            {
                cmd.Dispose();
                return 0x03;    // Character name already taken.
            }

            // Check if the character slot is taken.
            cmd.CommandText = string.Format("SELECT id FROM characters WHERE slot = {0} AND account = {1} LIMIT 1", slot, account);

            if (cmd.ExecuteScalar() != null)
            {
                cmd.Dispose();
                return 0x01;    // Character slot already taken.
            }

            try
            {
                var _gender = (gender == true) ? 1 : 0;
                var charId = account * 8 + slot;
                var query = string.Format("INSERT INTO characters (id, account, slot, name, level , class, gender, face, hair, colour, map, x, y, created)" +
                    "VALUES ('{0}', '{1}', '{2}', '{3}', 1 , '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12:yyyy-MM-dd hh:mm:ss}')",
                    charId, account, slot, name, _class, _gender, face, hair, colour, initialStats[0], initialStats[1], initialStats[2], DateTime.Now);
                cmd.CommandText = query;
                cmd.ExecuteScalar();
                cmd.Dispose();

                query = string.Format("INSERT INTO characters_equipment (charId, body, hands, feet, lefthand, righthand) VALUES ('{0}', @body, @hands, @feet, @lefthand, @righthand)", charId);
                cmd.CommandText = query;
                cmd.Parameters.Add("@body", MySqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[3]);
                cmd.Parameters.Add("@hands", MySqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[4]);
                cmd.Parameters.Add("@feet", MySqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[5]);
                cmd.Parameters.Add("@lefthand", MySqlDbType.Binary, 15).Value = (initialStats[7] != -1) ? BitConverter.GetBytes(initialStats[7]) : new byte[15];
                cmd.Parameters.Add("@righthand", MySqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[6]);
                cmd.ExecuteScalar();
                cmd.Dispose();

                query = string.Format("INSERT INTO characters_stats (charId, curhp, maxhp, curmp, maxmp, str_stat, int_stat, dex_stat)" +
                    "VALUES ('{0}', '{1}', '{1}', '{2}', '{2}', '{3}', '{4}', '{5}')",
                    charId, initialStats[8], initialStats[9], initialStats[10], initialStats[11], initialStats[12]);
                cmd.CommandText = query;
                cmd.ExecuteScalar();
                cmd.Dispose();

                foreach (var i in initialItems)
                {
                    cmd.CommandText = string.Format("INSERT INTO characters_items (charId, item, amount, slot) VALUES ({0}, @item{2}, {1}, {2})", charId, i.Item2, i.Item3);
                    cmd.Parameters.Add(string.Format("@item{0}", i.Item3), MySqlDbType.Binary, 15).Value = i.Item1;
                    cmd.ExecuteNonQuery();
                }

                foreach (var s in initialSkills)
                {
                    cmd.CommandText = string.Format("INSERT INTO characters_skills (charId, skill, level, slot) VALUES ({0}, {1}, {2}, {3})", charId, (ushort)s[0], (byte)s[1], (byte)s[2]);
                    cmd.ExecuteNonQuery();
                }

                foreach (var q in initialQuickslots)
                {
                    cmd.CommandText = string.Format("INSERT INTO characters_quickslots (charId, skill, slot) VALUES ({0}, {1}, {2})", charId, (byte)q[0], (byte)q[1]);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = string.Format("INSERT IGNORE INTO slotorder (id) VALUE ({0})", account);
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.Dispose();

                return 0xA1;    // Character creation succeeded.
            }
            catch (Exception e)
            {
                Log.Error(e.Message, "[MySQL::Character::CreateCharacter()]");
            }

            cmd.Parameters.Clear();
            cmd.Dispose();
            return 0x00;  // Unknown DB error.
        }

        public byte DeleteCharacter(int account, int slot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT id FROM characters WHERE slot = {0} AND account = {1} LIMIT 1", (byte)slot, account);
            var dbresult = cmd.ExecuteScalar();

            if (dbresult == null)
            {
                cmd.Dispose();
                return 1;    // Unknown DB error.
            }

            int charId = Convert.ToInt32(dbresult);

            try
            {
                cmd.CommandText = string.Format("DELETE FROM characters WHERE id = {0}; " +
                                                "DELETE FROM characters_equipment WHERE charId = {0};" +
                                                "DELETE FROM characters_items WHERE charId = {0};" +
                                                "DELETE FROM characters_quickslots WHERE charId = {0};" +
                                                "DELETE FROM characters_skills WHERE charId = {0};" +
                                                "DELETE FROM characters_stats WHERE charId = {0};",
                                               charId);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return 0xA1;
            }
            catch (Exception e)
            {
                Log.Error(e.Message,"[MySQL::Character::DeleteCharacter()]");
            }

            cmd.Dispose();
            return 0;
        }

        public void GetCharacter(int account, int slot)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM characters, characters_equipment WHERE characters.account = {0} AND characters.slot = {1} AND characters.id = characters_equipment.charId LIMIT 1", account, slot);
            reader = cmd.ExecuteReader();
            cmd.Dispose();
        }

        public void UpdateCharacterPosition(int account, byte slot, byte map, byte x, byte y)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE characters SET map = '{0}', x ='{1}', y = '{2}' WHERE account = '{3}' AND slot = '{4}'", map, x, y, account, slot);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public void SetSlotOrder(int account, int order)
        {
            Cleanup();
            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE slotorder SET slotorder = '{0}' WHERE id = '{1}'", order, account);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public int GetSlotOrder(int account)
        {
            Cleanup();
            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT slotorder FROM slotorder WHERE id = '{0}' LIMIT 1", account);
            var result = cmd.ExecuteScalar();

            if (result == null)
            {
                cmd.Dispose();
                return -1;
            }

            cmd.Dispose();
            return (int)result;
        }
    }
}
