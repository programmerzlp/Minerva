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

using PacketInfo = System.Tuple<string, Minerva.PacketMethod>;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static string Requires()
        {
            return null;
        }

        public static void Initialise(Dictionary<ushort, PacketInfo> methods, Dictionary<string, PacketConstructor> constructors)
        {
            
            #region World server
            methods[0x008C] = new PacketInfo("Connect2Svr", Connect);
            methods[0x008D] = new PacketInfo("VerifyLinks", VerifyLinks);
            methods[0x0094] = new PacketInfo("GetSvrTime", GetServerTime);
            methods[0x0144] = new PacketInfo("ChargeInfo", ChargeInfo);
            methods[0x0145] = new PacketInfo("NFY_ChargeInfo", NFY_ChargeInfo);
            methods[0x01D0] = new PacketInfo("ServerEnv", ServerEnv);
            methods[0x03D9] = new PacketInfo("CharacterSelect", CharacterSelect);
            methods[0x0840] = new PacketInfo("ChannelSelectList", ChannelSelectList);
            methods[0x085D] = new PacketInfo("ChangeChannel", ChangeChannel);
            methods[0x087D] = new PacketInfo("UpdateHelpInfo", UpdateHelpInfo);
            methods[0x09FE] = new PacketInfo("Ping", Ping);

            #endregion

            #region Character management
            methods[0x0085] = new PacketInfo("GetMyChartr", GetCharacters);
            methods[0x0086] = new PacketInfo("NewMyChartr", CreateCharacter);
            methods[0x0087] = new PacketInfo("DelMyChartr", DeleteCharacter);
            methods[0x0092] = new PacketInfo("SaveQuickSlot", SaveQuickSlot);
            methods[0x0142] = new PacketInfo("ChangeStyle", ChangeStyle);
            methods[0x0320] = new PacketInfo("CheckUserPrivacyData", CheckUserPrivacyData);
            methods[0x07D1] = new PacketInfo("SetCharacterSlotOrder", SetCharacterSlotOrder);
            methods[0x0875] = new PacketInfo("StorageExchangeMove", StorageExchangeMove);
            methods[0x08BD] = new PacketInfo("SpecialInventoryList", SpecialInventoryList);
            methods[0x0A56] = new PacketInfo("AddSkillPoints", AddSkillPoints);
            methods[0x0A58] = new PacketInfo("AddStatPoints", AddStatPoints);
            #endregion

            #region Subpass management
            methods[0x0406] = new PacketInfo("SubPasswordSet", SubPasswordSet);
            methods[0x0408] = new PacketInfo("SubPasswordCheckRequest", SubPasswordCheckRequest);
            methods[0x040A] = new PacketInfo("SubPasswordCheck", SubPasswordCheck);
            methods[0x040C] = new PacketInfo("SubPasswordFindRequest", SubPasswordFindRequest);
            methods[0x040E] = new PacketInfo("SubPasswordFind", SubPasswordFind);
            methods[0x0410] = new PacketInfo("SubPasswordDelRequest", SubPasswordDelRequest);
            methods[0x0412] = new PacketInfo("SubPasswordDel", SubPasswordDel);
            methods[0x0414] = new PacketInfo("SubPasswordChangeQARequest", SubPasswordChangeQARequest);
            methods[0x0416] = new PacketInfo("SubPasswordChangeQA", SubPasswordChangeQA);
            methods[0x0870] = new PacketInfo("CharacterDeleteCheckSubPassword", CharacterDeleteCheckSubPassword);
            #endregion

            #region World management
            methods[0x008E] = new PacketInfo("Initialized", EnterWorld);
            methods[0x008F] = new PacketInfo("Uninitialize", LeaveWorld);
            #endregion
            
            #region Chat management
            methods[0x00C3] = new PacketInfo("MessageEvnt", MessageEvent);
            #endregion

            #region CashItems management
            methods[0x01A2] = new PacketInfo("QueryCashItem", QueryCashItem);
            methods[0x01A3] = new PacketInfo("RecvCashItem", RecvCashItem);
            #endregion

            #region Movement management
            methods[0x00BE] = new PacketInfo("MoveBegined", MoveToLocation);
            methods[0x00BF] = new PacketInfo("MoveEnded", ArrivedAtLocation);
            methods[0x00C0] = new PacketInfo("MoveChanged", AlterDestination);
            methods[0x00C2] = new PacketInfo("MoveTilePos", ChangeMapCell);
            methods[0x0187] = new PacketInfo("ChangeDirection", ChangeDirection);
            methods[0x0191] = new PacketInfo("KeyMoveBegined", KeyMoveBegined);
            methods[0x0195] = new PacketInfo("KeyMoveChanged", KeyMoveChanged);

            //methods[0x0191] = new PacketInfo("Unknown191", Unknown191);
            //methods[0x0193] = new PacketInfo("Unknown193", Unknown193);
            //methods[0x0194] = new PacketInfo("Unknown194", Unknown194);
            #endregion

            #region Skill management
            methods[0x00AF] = new PacketInfo("SkillToUser", SkillToUser);
            methods[0x0136] = new PacketInfo("SkillToActs", SkillToActs);
            #endregion

            #region In-Progress Packets
            methods[0x00F4] = new PacketInfo("WarpCommand", WarpCommand); // TODO: Improve Packet and create/read WarpList
            methods[0x09F3] = new PacketInfo("MeritSystem", MeritSystem);

            #region Quest management
            methods[0x011A] = new PacketInfo("QuestStart", QuestStart);
            methods[0x011B] = new PacketInfo("QuestEnd", QuestEnd);
            methods[0x0140] = new PacketInfo("QuestNextStep", QuestNextStep);
            methods[0x02F8] = new PacketInfo("QuestReward", QuestReward);
            #endregion

#if DEBUG
            methods[0x00AE] = new PacketInfo("SkillToMobs", SkillToMobs);
            methods[0x00B0] = new PacketInfo("NormalAttack", NormalAttack);

            methods[0x08CA] = new PacketInfo("ReqCraft", ReqCraft);

            methods[0x0A05] = new PacketInfo("CashNPC", CashNPC);
            methods[0x0389] = new PacketInfo("PetInfo", PetInfo);

            constructors["PC_WarpCommand"] = PC_WarpCommand;
            constructors["ErrorCode"] = PC_ErrorCode;
#endif
            #endregion

            #region Ep8+ Packets
                methods[0x086C] = new PacketInfo("Unknown_86C", Unknown_86C);   //ServerLoudMsg?
                methods[0x08C8] = new PacketInfo("Unknown8C8", Unknown8C8);
                methods[0x09C8] = new PacketInfo("Unknown9C8", Unknown9C8);
                methods[0x09D6] = new PacketInfo("Unknown9D6", Unknown9D6);     //VerifyTimeoutCheck
                methods[0x09E0] = new PacketInfo("Unknown9E0", Unknown9E0);
                methods[0x0A0B] = new PacketInfo("UnknownA0B", UnknownA0B);
                methods[0x0CE9] = new PacketInfo("UnknownCE9", UnknownCE9);

            //methods[0x01E5] = new PacketInfo("Unknown1E5", Unknown1E5);
            //methods[0x0843] = new PacketInfo("Unknown843", Unknown843);
            //methods[0x09D8] = new PacketInfo("Unknown9D8", Unknown9D8);
            //methods[0x09DA] = new PacketInfo("Unknown9DA", Unknown9DA);



            #endregion

            constructors["MobsMoveBgn"] = PC_MobsMoveBgn;
            constructors["MobsMoveEnd"] = PC_MobsMoveEnd;
            //constructors[""] = ;
            //constructors["ItemDisposed"] = PC_ItemDisposed;
            //constructors["ItemDropped"] = PC_ItemDropped;
            constructors["UpdateStats"] = PC_UpdateStats;
            //constructors["MobSpawned"] = PC_MobSpawned;
        }
    }
}