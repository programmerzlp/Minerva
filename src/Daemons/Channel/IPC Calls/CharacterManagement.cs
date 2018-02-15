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
    class CharacterManagement
    {
        public static Character[] GetCharacters(SyncReceiver sync, int server, int id)
        {
            var packet = sync.CreateIPC(IPC.GetCharacterList);
            packet.Write((byte)server);
            packet.Write(id);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return null;

            var num = recv.ReadInt32();
            var charList = new Character[num];

            for (int i = 0; i < num; i++)
            {
                charList[i] = new Character();
                charList[i].account = id;
                charList[i].id = recv.ReadInt32();
                charList[i].slot = recv.ReadByte();
                charList[i].name = recv.ReadString();
                charList[i].level = recv.ReadUInt32();
                charList[i]._class = recv.ReadByte();
                charList[i].face = recv.ReadByte();
                charList[i].hair = recv.ReadByte();
                charList[i].colour = recv.ReadByte();
                charList[i].gender = recv.ReadBoolean();
                charList[i].map = recv.ReadByte();
                charList[i].x = recv.ReadByte();
                charList[i].y = recv.ReadByte();
                charList[i].created = DateTime.FromBinary(recv.ReadInt64());

                charList[i].equipment = new Equipment();
                charList[i].equipment.head = recv.ReadBytes(15);
                charList[i].equipment.body = recv.ReadBytes(15);
                charList[i].equipment.hands = recv.ReadBytes(15);
                charList[i].equipment.feet = recv.ReadBytes(15);
                charList[i].equipment.righthand = recv.ReadBytes(15);
                charList[i].equipment.lefthand = recv.ReadBytes(15);
                charList[i].equipment.back = recv.ReadBytes(15);
            }

            return charList;
        }

        public static CreateCharacterStatus CreateCharacter(SyncReceiver sync, int server, int id, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour)
        {
            var packet = sync.CreateIPC(IPC.CreateCharacter);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write(slot);
            packet.Write(name);
            packet.Write(_class);
            packet.Write(gender);
            packet.Write(face);
            packet.Write(hair);
            packet.Write(colour);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return CreateCharacterStatus.DBError;

            var status = (CreateCharacterStatus)recv.ReadByte();

            switch (status)
            {
                case CreateCharacterStatus.NameInUse: return CreateCharacterStatus.NameInUse;
                case CreateCharacterStatus.SlotInUse: return CreateCharacterStatus.SlotInUse;
                case CreateCharacterStatus.Success: return CreateCharacterStatus.Success;
                default: return CreateCharacterStatus.DBError;
            }
        }

        public static DeleteCharacterStatus DeleteCharacter(SyncReceiver sync, int server, int id, int slot)
        {
            var packet = sync.CreateIPC(IPC.DeleteCharacter);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write((byte)slot);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return DeleteCharacterStatus.DBError;

            var status = (DeleteCharacterStatus)recv.ReadByte();

            switch (status)
            {
                case DeleteCharacterStatus.Success: return DeleteCharacterStatus.Success;
                default: return DeleteCharacterStatus.DBError;
            }
        }

        public static void UpdatePosition(SyncReceiver sync, int server, int id, byte slot, byte map, byte x, byte y)
        {
            var packet = sync.CreateIPC(IPC.UpdatePosition);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write(slot);
            packet.Write(map);
            packet.Write(x);
            packet.Write(y);

            sync.Send(packet);
        }

        public static Character GetFullCharacter(SyncReceiver sync, int server, int id, int slot)
        {
            var packet = sync.CreateIPC(IPC.GetFullCharacter);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write((byte)slot);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return null;

            Character character = new Character();
            character.account = id;
            character.id = recv.ReadInt32();
            character.slot = recv.ReadByte();
            character.name = recv.ReadString();
            character.level = recv.ReadUInt32();
            character._class = recv.ReadByte();
            character.face = recv.ReadByte();
            character.hair = recv.ReadByte();
            character.colour = recv.ReadByte();
            character.gender = recv.ReadBoolean();
            character.map = recv.ReadByte();
            character.x = recv.ReadByte();
            character.y = recv.ReadByte();
            character.created = DateTime.FromBinary(recv.ReadInt64());

            character.equipment = new Equipment();
            character.equipment.head = recv.ReadBytes(15);
            character.equipment.body = recv.ReadBytes(15);
            character.equipment.hands = recv.ReadBytes(15);
            character.equipment.feet = recv.ReadBytes(15);
            character.equipment.righthand = recv.ReadBytes(15);
            character.equipment.lefthand = recv.ReadBytes(15);
            character.equipment.back = recv.ReadBytes(15);

            character.equipment.card = recv.ReadBytes(15);

            character.equipment.neck = recv.ReadBytes(15);
            character.equipment.finger1 = recv.ReadBytes(15);
            character.equipment.finger2 = recv.ReadBytes(15);
            character.equipment.finger3 = recv.ReadBytes(15);
            character.equipment.finger4 = recv.ReadBytes(15);
            character.equipment.leftear = recv.ReadBytes(15);
            character.equipment.rightear = recv.ReadBytes(15);
            character.equipment.leftwrist = recv.ReadBytes(15);
            character.equipment.rightwrist = recv.ReadBytes(15);

            character.equipment.belt = recv.ReadBytes(15);
            character.equipment.charm = recv.ReadBytes(15);
            character.equipment.lefteffector = recv.ReadBytes(15);
            character.equipment.righteffector = recv.ReadBytes(15);
            character.equipment.cornalina = recv.ReadBytes(15);
            character.equipment.talisman = recv.ReadBytes(15);

            character.equipment.leftarcane = recv.ReadBytes(15);
            character.equipment.rightarcane = recv.ReadBytes(15);

            character.stats = new Stats();
            character.stats.curhp = recv.ReadUInt16();
            character.stats.maxhp = recv.ReadUInt16();
            character.stats.curmp = recv.ReadUInt16();
            character.stats.maxmp = recv.ReadUInt16();
            character.stats.cursp = recv.ReadUInt16();
            character.stats.maxsp = recv.ReadUInt16();

            character.stats.exp = recv.ReadUInt64();


            character.stats.str_stat = recv.ReadUInt32();
            character.stats.int_stat = recv.ReadUInt32();
            character.stats.dex_stat = recv.ReadUInt32();
            character.stats.pnt_stat = recv.ReadUInt32();

            character.stats.honour = recv.ReadUInt32();
            character.stats.rank = recv.ReadUInt32();

            character.stats.swordrank = recv.ReadByte();
            character.stats.swordxp = recv.ReadUInt16();
            character.stats.swordpts = recv.ReadUInt16();
            character.stats.magicrank = recv.ReadByte();
            character.stats.magicxp = recv.ReadUInt16();
            character.stats.magicpts = recv.ReadUInt16();

            character.stats.alz = recv.ReadUInt64();
            character.stats.wexp = recv.ReadUInt64();
            character.stats.honor = recv.ReadUInt64();


            var num = recv.ReadInt32();
            character.inv = new Inventory[num];

            for (int i = 0; i < num; i++)
            {
                character.inv[i] = new Inventory();
                character.inv[i].item = recv.ReadBytes(15);
                character.inv[i].amount = recv.ReadUInt16();
                character.inv[i].slot = recv.ReadByte();
            }

            num = recv.ReadInt32();
            character.skills = new Skills[num];

            for (int i = 0; i < num; i++)
            {
                character.skills[i] = new Skills();
                character.skills[i].skill = recv.ReadUInt16();
                character.skills[i].level = recv.ReadByte();
                character.skills[i].slot = recv.ReadByte();
            }

            num = recv.ReadInt32();
            character.qslots = new QuickSlots[num];

            for (int i = 0; i < num; i++)
            {
                character.qslots[i] = new QuickSlots();
                character.qslots[i].skill = recv.ReadByte();
                character.qslots[i].slot = recv.ReadByte();
            }

            return character;
        }

        public static void SetQuickSlots(SyncReceiver sync, int server, int id, int quickslot, int position)
        {
            var packet = sync.CreateIPC(IPC.SetQuickSlots);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write(quickslot);
            packet.Write(position);

            sync.Send(packet);
        }

        public static void SetSlotOrder(SyncReceiver sync, int server, int id, int order)
        {
            var packet = sync.CreateIPC(IPC.SetSlotOrder);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write(order);

            sync.Send(packet);
        }

        public static int GetSlotOrder(SyncReceiver sync, int server, int id)
        {
            var packet = sync.CreateIPC(IPC.GetSlotOrder);
            packet.Write((byte)server);
            packet.Write(id);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return -1;

            var slotorder = recv.ReadInt32();
            return slotorder;
        }

        public static void UpdateStatPoints(SyncReceiver sync, int server, int id, int str, int intel, int dex, int points)
        {
            var packet = sync.CreateIPC(IPC.UpdateStatPoints);
            packet.Write((byte)server);
            packet.Write(id);
            packet.Write(str);
            packet.Write(intel);
            packet.Write(dex);
            packet.Write(points);

            sync.Send(packet);
        }

        public static void UpdateSkillPoints(SyncReceiver sync, int server, int charId, ushort skillid, ushort level, byte slot)
        {
            var packet = sync.CreateIPC(IPC.UpdateSkillPoints);
            packet.Write((byte)server);
            packet.Write(charId);
            packet.Write(skillid);
            packet.Write(level);
            packet.Write(slot);

            sync.Send(packet);
        }

        
        public static CashItem[] GetCashItem(SyncReceiver sync, int server, int id)
        {
            var packet = sync.CreateIPC(IPC.GetCashItemList);
            packet.Write((byte)server);
            packet.Write(id);

            sync.Send(packet);


            var recv = sync.ReadIPC();

            if (recv == null)
                return null;

            var itens = recv.ReadInt32();

            var ItemList = new CashItem[itens];

            for (int i = 0; i < itens; i++)
            {
                ItemList[i] = new CashItem();
                ItemList[i].ID = recv.ReadInt32();
                ItemList[i].itemid = recv.ReadInt32();
                ItemList[i].itemopt = recv.ReadBytes(8);
                ItemList[i].itemopt2 = recv.ReadInt32();
                ItemList[i].duration = recv.ReadInt32();
            }
            return ItemList;

        }

        public static void SetCashItem(SyncReceiver sync, int server, int ID)
        {
            var packet = sync.CreateIPC(IPC.SetCashItem);
            packet.Write((byte)server);
            packet.Write(ID);

            sync.Send(packet);
        }

    }
}
