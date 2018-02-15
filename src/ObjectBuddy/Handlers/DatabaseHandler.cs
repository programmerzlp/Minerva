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
    public class DatabaseHandler
    {
        IDatabaseProtocol protocol;

        public DatabaseHandler(string type, string ip, string db, string user, string pass)
        {
            var file = string.Format("libs/{0}.dll", type);
            var asm = System.Reflection.Assembly.LoadFrom(file);
            var args = new object[] { ip, db, user, pass };
            var ptype = asm.GetType("Minerva.DatabaseProtocol");
            this.protocol = Activator.CreateInstance(ptype, args) as IDatabaseProtocol;

            this.protocol.Connect();
        }

        ~ DatabaseHandler()
        {
            protocol.Disconnect();
        }

        public object this[string key] { get { return protocol[key]; } }
        public bool ReadRow() { return protocol.ReadRow(); }

        #region Login

        public bool FetchAccount(string user) { return protocol.FetchAccount(user); }
        public void UpdateIPDate(int id, string ip, DateTime date) { protocol.UpdateIPDate(id, ip, date); }
        public void UpdateOnline(int id, bool online) { protocol.UpdateOnline(id, online); }
        public bool VerifyPassword(int id, string pass) { return protocol.VerifyPassword(id, pass); }

        #endregion

        #region World

        #region SubPass

        public bool      GetSubPass(int account) { return protocol.GetSubPass(account); }
        public int       GetSubPassQuestion(int account) { return protocol.GetSubPassQuestion(account); }
        public void      SetSubPass(int account, string subpw, int question, string answer) { protocol.SetSubPass(account, subpw, question, answer); }
        public bool      CheckSubPwAnswer(int account, string answer) { return protocol.CheckSubPwAnswer(account, answer); }
        public bool      CheckSubPw(int account, string pass) { return protocol.CheckSubPw(account, pass); }
        public void      RemoveSubPass(int account) { protocol.RemoveSubPass(account); }
        public DateTime? GetSubPassTime(int id) { return protocol.GetSubPassTime(id); }
        public void      SetSubPassTime(int id, int time) { protocol.SetSubPassTime(id, time); }

        #endregion

        #region Character

        public int   GetCharactersCount(int account) { return protocol.GetCharactersCount(account); }
        public void  GetCharacters(int account) { protocol.GetCharacters(account); }
        public int   GetSkillCount(int characterId) { return protocol.GetSkillCount(characterId); }
        public int   GetQuickSlotCount(int characterId) { return protocol.GetQuickSlotCount(characterId); }
        public void  GetSkills(int characterId) { protocol.GetSkills(characterId); }
        public void  GetQuickslots(int characterId) { protocol.GetQuickslots(characterId); }
        public void  SetQuickSlots(int characterId, int quickslot, int skill) { protocol.SetQuickSlots(characterId, quickslot, skill); }
        public void  GetStats(int characterId) { protocol.GetStats(characterId); }
        public int   CreateCharacter(int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour, int[] initialStats, List<Tuple<byte[], ushort, byte>> initialItems, List<int[]> initialSkills, List<int[]> initialQuickslots) { return protocol.CreateCharacter(account, slot, name, _class, gender, face, hair, colour, initialStats, initialItems, initialSkills, initialQuickslots); }
        public byte  DeleteCharacter(int account, int slot) { return protocol.DeleteCharacter(account, slot); }
        public void  GetCharacter(int account, int slot) { protocol.GetCharacter(account, slot); }
        public void  UpdateCharacterPosition(int account, byte slot, byte map, byte x, byte y) { protocol.UpdateCharacterPosition(account, slot, map, x, y); }
        public int   GetSlotOrder(int account) { return protocol.GetSlotOrder(account); }
        public void  SetSlotOrder(int account, int order) { protocol.SetSlotOrder(account, order); }
        public void  UpdateStatPoints(int charId, int str, int intel, int dex, int pnt) { protocol.UpdateStatPoints(charId, str, intel, dex, pnt); }
        public void  UpdateSkillPoints(int charId, ushort skillid, ushort level, byte slot) { protocol.UpdateSkillPoints(charId, skillid, level, slot); }

        #endregion

        #region Inventory

        public int GetItemCount(int charId) { return protocol.GetItemCount(charId); }
        public void GetInventory(int charId) { protocol.GetInventory(charId); }
        public void GetEquipment(int charId) { protocol.GetEquipment(charId); }
        public void MoveItem(int charId, int oldslot, int newslot) { protocol.MoveItem(charId, oldslot, newslot); }
        public void RemoveItem(int charId, int slot) { protocol.RemoveItem(charId, slot); }
        public void AddItem(int charId, int slot, byte[] item, int amount) { protocol.AddItem(charId, slot, item, amount); }
        public void GetItem(int charId, int slot) { protocol.GetItem(charId, slot); }
        public void EquipItem(int charId, int itemslot, string equipslot) { protocol.EquipItem(charId, itemslot, equipslot); }
        public void GetCashItem(int account) { protocol.GetCashItem(account); }
        public int GetCashItemCount(int account) { return protocol.GetCashItemCount(account); }
        public void SetCashItem(int ID) { protocol.SetCashItem(ID); }

        #endregion

        #endregion
    }
}