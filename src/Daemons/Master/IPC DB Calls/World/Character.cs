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
//using System.Collections.Generic;

//using CharacterStats = System.Tuple<ushort, ushort, ushort, ushort, ushort, ushort, ulong, System.Tuple<uint, uint, uint, uint, uint, byte, ushort, System.Tuple<ushort, byte, ushort, ushort, ulong, uint>>>;

#endregion

namespace Minerva
{
    partial class IPCProtocol
    {
        void CharacterList(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var account = data.ReadInt32();

            var serverDB = sync.GetServerDBHandler(serverId);
            var charNum = serverDB.GetCharactersCount(account);

            var packet = new IPCWriter(IPC.GetCharacterList);
            packet.Write(charNum);

            serverDB.GetCharacters(account);

            while (serverDB.ReadRow())
            {
                packet.Write((serverDB["id"] as int?).Value);
                packet.Write((serverDB["slot"] as byte?).Value);
                packet.Write(serverDB["name"] as string);
                packet.Write((serverDB["level"] as uint?).Value);
                packet.Write((serverDB["class"] as byte?).Value);
                packet.Write((serverDB["face"] as byte?).Value);
                packet.Write((serverDB["hair"] as byte?).Value);
                packet.Write((serverDB["colour"] as byte?).Value);
                packet.Write((serverDB["gender"] as bool?).Value);
                packet.Write((serverDB["map"] as byte?).Value);
                packet.Write((serverDB["x"] as byte?).Value);
                packet.Write((serverDB["y"] as byte?).Value);
                packet.Write((serverDB["created"] as DateTime?).Value.ToBinary());

                packet.Write(serverDB["head"] as byte[]);
                packet.Write(serverDB["body"] as byte[]);
                packet.Write(serverDB["hands"] as byte[]);
                packet.Write(serverDB["feet"] as byte[]);
                packet.Write(serverDB["righthand"] as byte[]);
                packet.Write(serverDB["lefthand"] as byte[]);
                packet.Write(serverDB["back"] as byte[]);
            }

            receiver.Send(packet);
        }

        /*CharacterStats GetStats(IPCReceiver receiver, IPCReader data, DatabaseHandler database)
        {
            serverdbs[server].GetStats(charId);
            serverdbs[server].ReadRow();

            return new CharacterStats((serverdbs[server]["curhp"] as ushort?).Value,
                (serverdbs[server]["maxhp"] as ushort?).Value,
                (serverdbs[server]["curmp"] as ushort?).Value,
                (serverdbs[server]["maxmp"] as ushort?).Value,
                (serverdbs[server]["cursp"] as ushort?).Value,
                (serverdbs[server]["maxsp"] as ushort?).Value,
                (serverdbs[server]["exp"] as ulong?).Value,
                new Tuple<uint, uint, uint, uint, uint, byte, ushort, Tuple<ushort, byte, ushort, ushort, ulong, uint>>((serverdbs[server]["str_stat"] as uint?).Value,
                    (serverdbs[server]["int_stat"] as uint?).Value,
                    (serverdbs[server]["dex_stat"] as uint?).Value,
                    (serverdbs[server]["honour"] as uint?).Value,
                    (serverdbs[server]["rank"] as uint?).Value,
                    (serverdbs[server]["swordrank"] as byte?).Value,
                    (serverdbs[server]["swordxp"] as ushort?).Value,
                    Tuple.Create((serverdbs[server]["swordpoints"] as ushort?).Value,
                        (serverdbs[server]["magicrank"] as byte?).Value,
                        (serverdbs[server]["magicxp"] as ushort?).Value,
                        (serverdbs[server]["magicpoints"] as ushort?).Value,
                        (serverdbs[server]["alz"] as ulong?).Value,
                        (serverdbs[server]["pnt_stat"] as uint?).Value
                    )
                )
            );
        }*/

        void CreateCharacter(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();
            var name = data.ReadString();
            var _class = data.ReadByte();
            var gender = data.ReadBoolean();
            var face = data.ReadByte();
            var hair = data.ReadByte();
            var colour = data.ReadByte();

            var stats = sync.GetInitialCharStats(_class);
            var items = sync.GetInitialCharItems(_class);
            var skills = sync.GetInitialCharSkills(_class);
            var quickslots = sync.GetInitialCharQuickSlots(_class);

            var serverDB = sync.GetServerDBHandler(serverId);
            var status = serverDB.CreateCharacter(id, slot, name, _class, gender, face, hair, colour, stats.ToArray(), items, skills, quickslots);

            var packet = new IPCWriter(IPC.CreateCharacter);
            packet.Write((byte)status);

            receiver.Send(packet);
        }

        void DeleteCharacter(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();

            var serverDB = sync.GetServerDBHandler(serverId);
            var packet = new IPCWriter(IPC.DeleteCharacter);
            var status = serverDB.DeleteCharacter(id, slot);

            packet.Write(status);

            receiver.Send(packet);
        }

        void GetFullCharacter(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();

            var serverDB = sync.GetServerDBHandler(serverId);
            var packet = new IPCWriter(IPC.GetFullCharacter);

            serverDB.GetCharacter(id, slot);
            serverDB.ReadRow();

            var charId = (serverDB["id"] as int?).Value;

            packet.Write(charId);
            packet.Write((serverDB["slot"] as byte?).Value);
            packet.Write(serverDB["name"] as string);
            packet.Write((serverDB["level"] as uint?).Value);
            packet.Write((serverDB["class"] as byte?).Value);
            packet.Write((serverDB["face"] as byte?).Value);
            packet.Write((serverDB["hair"] as byte?).Value);
            packet.Write((serverDB["colour"] as byte?).Value);
            packet.Write((serverDB["gender"] as bool?).Value);
            packet.Write((serverDB["map"] as byte?).Value);
            packet.Write((serverDB["x"] as byte?).Value);
            packet.Write((serverDB["y"] as byte?).Value);
            packet.Write((serverDB["created"] as DateTime?).Value.ToBinary());

            packet.Write(serverDB["head"] as byte[]);
            packet.Write(serverDB["body"] as byte[]);
            packet.Write(serverDB["hands"] as byte[]);
            packet.Write(serverDB["feet"] as byte[]);
            packet.Write(serverDB["righthand"] as byte[]);
            packet.Write(serverDB["lefthand"] as byte[]);
            packet.Write(serverDB["back"] as byte[]);

            packet.Write(serverDB["card"] as byte[]);
            packet.Write(serverDB["neck"] as byte[]);
            packet.Write(serverDB["finger1"] as byte[]);
            packet.Write(serverDB["finger2"] as byte[]);
            packet.Write(serverDB["finger3"] as byte[]);
            packet.Write(serverDB["finger4"] as byte[]);
            packet.Write(serverDB["leftear"] as byte[]);
            packet.Write(serverDB["rightear"] as byte[]);
            packet.Write(serverDB["leftwrist"] as byte[]);
            packet.Write(serverDB["rightwrist"] as byte[]);
            packet.Write(serverDB["belt"] as byte[]);
            packet.Write(serverDB["charm"] as byte[]);
            packet.Write(serverDB["lefteffector"] as byte[]);
            packet.Write(serverDB["righteffector"] as byte[]);
            packet.Write(serverDB["cornalina"] as byte[]);
            packet.Write(serverDB["talisman"] as byte[]);

            packet.Write(serverDB["leftarcane"] as byte[]);
            packet.Write(serverDB["rightarcane"] as byte[]);

            serverDB.GetStats(charId);
            serverDB.ReadRow();

            packet.Write((serverDB["curhp"] as ushort?).Value);
            packet.Write((serverDB["maxhp"] as ushort?).Value);
            packet.Write((serverDB["curmp"] as ushort?).Value);
            packet.Write((serverDB["maxmp"] as ushort?).Value);
            packet.Write((serverDB["cursp"] as ushort?).Value);
            packet.Write((serverDB["maxsp"] as ushort?).Value);
            packet.Write((serverDB["exp"] as ulong?).Value);

            packet.Write((serverDB["str_stat"] as uint?).Value);
            packet.Write((serverDB["int_stat"] as uint?).Value);
            packet.Write((serverDB["dex_stat"] as uint?).Value);
            packet.Write((serverDB["pnt_stat"] as uint?).Value);

            packet.Write((serverDB["honour"] as uint?).Value);
            packet.Write((serverDB["rank"] as uint?).Value);
            packet.Write((serverDB["swordrank"] as byte?).Value);
            packet.Write((serverDB["swordxp"] as ushort?).Value);
            packet.Write((serverDB["swordpoints"] as ushort?).Value);
            packet.Write((serverDB["magicrank"] as byte?).Value);
            packet.Write((serverDB["magicxp"] as ushort?).Value);
            packet.Write((serverDB["magicpoints"] as ushort?).Value);

            packet.Write((serverDB["alz"] as ulong?).Value);
            packet.Write((serverDB["wexp"] as ulong?).Value);
            packet.Write((serverDB["honor"] as ulong?).Value);

            var itemCount = serverDB.GetItemCount(charId);
            serverDB.GetInventory(charId);

            packet.Write(itemCount);

            while (serverDB.ReadRow())
            {
                packet.Write((byte[])serverDB["item"] as byte[]);
                packet.Write((serverDB["amount"] as ushort?).Value);
                packet.Write((serverDB["slot"] as byte?).Value);
            }

            var skillCount = serverDB.GetSkillCount(charId);
            serverDB.GetSkills(charId);

            packet.Write(skillCount);

            while (serverDB.ReadRow())
            {
                packet.Write((serverDB["skill"] as ushort?).Value);
                packet.Write((serverDB["level"] as byte?).Value);
                packet.Write((serverDB["slot"] as byte?).Value);
            }

            var quickSlotCount = serverDB.GetQuickSlotCount(charId);
            serverDB.GetQuickslots(charId);

            packet.Write(quickSlotCount);

            while (serverDB.ReadRow())
            {
                packet.Write((serverDB["skill"] as byte?).Value);
                packet.Write((serverDB["slot"] as byte?).Value);
            }

            receiver.Send(packet);
        }

        void UpdateCharacterPosition(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();
            var map = data.ReadByte();
            var x = data.ReadByte();
            var y = data.ReadByte();

            var serverDB = sync.GetServerDBHandler(serverId);
            serverDB.UpdateCharacterPosition(id, slot, map, x, y);
        }

        void SetQuickSlots(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var quickslot = data.ReadInt32();
            var position = data.ReadInt32();

            var serverDB = sync.GetServerDBHandler(serverId);
            serverDB.SetQuickSlots(id, quickslot, position);
        }

        void SetSlotOrder(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var order = data.ReadInt32();

            var serverDB = sync.GetServerDBHandler(serverId);
            serverDB.SetSlotOrder(id, order);
        }

        void GetSlotOrder(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();

            var serverDB = sync.GetServerDBHandler(serverId);
            var packet = new IPCWriter(IPC.GetSlotOrder);
            var order = serverDB.GetSlotOrder(id);

            packet.Write(order);

            receiver.Send(packet);
        }

        void UpdateStatPoints(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var str = data.ReadInt32();
            var dex = data.ReadInt32();
            var intel = data.ReadInt32();
            var pnt = data.ReadInt32();

            var serverDB = sync.GetServerDBHandler(serverId);
            serverDB.UpdateStatPoints(id, str, dex, intel, pnt);
        }

        void UpdateSkillPoints(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();
            var skillid = data.ReadUInt16();
            var level = data.ReadUInt16();
            var slot = data.ReadByte();

            var serverDB = sync.GetServerDBHandler(serverId);
            serverDB.UpdateSkillPoints(id, skillid, level, slot);
        }


        void GetCashItemList(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var id = data.ReadInt32();


            var serverDB = sync.GetServerDBHandler(serverId);
            var CashCount = serverDB.GetCashItemCount(id);

            var packet = new IPCWriter(IPC.GetCashItemList);

            packet.Write(CashCount);

            serverDB.GetCashItem(id);

            while (serverDB.ReadRow())
            {
                packet.Write((serverDB["ID"] as int?).Value);
                packet.Write((serverDB["itemid"] as int?).Value);
                packet.Write(serverDB["itemopt"] as byte[]);
                packet.Write((serverDB["itemopt2"] as int?).Value);
                packet.Write((serverDB["duration"] as int?).Value);
            }

            receiver.Send(packet);
        }

        void SetCashItem(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var ID = data.ReadInt32();

            var serverDB = sync.GetServerDBHandler(serverId);
            serverDB.SetCashItem(ID);
        }
    }
}
