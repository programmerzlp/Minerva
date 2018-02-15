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
    public interface IDatabaseProtocol
    {
        object this[string key] { get; }

        void Connect();
        void Disconnect();
        bool ReadRow();

        #region Login

        bool FetchAccount(string user);
        void UpdateIPDate(int id, string ip, DateTime date);
        void UpdateOnline(int id, bool online);
        bool VerifyPassword(int id, string pass);

        #endregion

        #region World

        #region SubPass

        bool GetSubPass(int id);
        void SetSubPass(int id, string subpw, int question, string answer);
        int  GetSubPassQuestion(int id);
        bool CheckSubPwAnswer(int id, string answer);
        bool CheckSubPw(int id, string pass);
        void RemoveSubPass(int id);
        DateTime? GetSubPassTime(int id);
        void SetSubPassTime(int id, int time);

        #endregion

        #region Character

        int   GetCharactersCount(int account);
        void  GetCharacters(int account);
        int   GetSkillCount(int character);
        void  GetSkills(int character);
        int   GetQuickSlotCount(int character);
        void  GetQuickslots(int character);
        void  SetQuickSlots(int character, int quickslot, int skill);
        void  GetStats(int character);
        int   CreateCharacter(int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour, int[] initialStats, List<Tuple<byte[], ushort, byte>> initialItems, List<int[]> initialSkills, List<int[]> initialQuickslots);
        byte  DeleteCharacter(int account, int slot);
        void  GetCharacter(int account, int slot);
        void  UpdateCharacterPosition(int account, byte slot, byte map, byte x, byte y);
        int   GetSlotOrder(int account);
        void  SetSlotOrder(int account, int order);
        void  UpdateStatPoints(int charId, int str, int intel, int dex, int points);
        void  UpdateSkillPoints(int charId, ushort skillid, ushort level, byte slot);

        #endregion

        #region Inventory

        int  GetItemCount(int charId);
        void GetInventory(int charId);
        void GetEquipment(int charId);
        void MoveItem(int charId, int oldslot, int newslot);
        void RemoveItem(int charId, int slot);
        void AddItem(int charId, int slot, byte[] item, int amount);
        void GetItem(int charId, int slot);
        void EquipItem(int charId, int itemslot, string equipslot);
        int GetCashItemCount(int account);
        void GetCashItem(int account);
        void SetCashItem(int ID);

        #endregion

        #endregion
    }
}