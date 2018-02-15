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
    partial class PacketProtocol
    {
        /*
         * CSC_ATTCKTOMOBS Packet
         * -------------------------
         * Client2Server Structure:
         * 
         * ushort  : magic code
         * ushort  : size
         * int     : padding
         * ushort  : opcode
         * 
         * ushort  : MobID
         * byte    : MapID
         * byte    : Unkn1             #so far its value as been 2
         * byte    : Unkn2             #so far its value as been 2 as well, so it could be just a short (512)
         * byte    : Unkn3             #so far its value as been 0
         * -------------------------
         * Server2Client Structure:
         * 
         * ushort  : magic code
         * ushort  : size
         * ushort  : opcode
         * 
         * ushort  : MobID
         * byte    : Unkn1
         * byte    : MapID
         * byte    : Unkn2
         * uint    : CurHp             #Current HP/Life character has.
         * uint    : CurMp             #Current MP/Mana character has.
         * byte[2] : Unkn4             #so far its value as been 00 00
         * byte    : AtkResult         #Enum AttackResult -> Critical = 1; Normal = 2; Block = 1B; Miss = 15;
         * byte    : HitCounter        #Counts number of attacks made to mob
         * byte    : Unkn5             #Usually value between 1 and 3
         * byte[6] : Unkn6             #00
         * uint    : Dmg               #Damage inflicted on mob, Miss = 0; Dmg = Attack - Mob Defense; if(dmg<=0 && !Miss) dmg = 1;
         * uint    : MobHp             #MobHp -= Dmg
         * ushort  : AP
         * uint    : AXP
         * byte    : TakeDmg           #Hp Cost/Hp Sacrifice; Boolean;
         * uint    : HpHover           #Hp where 0/20 sp is displayed if TakeDmg = 1.
         * uint    : DmgTaken          #Amount of dmg recieved/hp sacrificed if TakeDmg = 1.
         * -------------------------
         * Server sends Notify packet OpCode 0x00E1;
         */

        public static void NormalAttack(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var map = client.Metadata["map"] as IMap;
            var mapmobs = (client.Metadata["map"] as IMap).GetSurroundingMobs(client, 3);
            var mobdata = map.GetMobsData;

            var MobID = packet.ReadUShort();
            var unk1 = packet.ReadByte();
            var MapID = packet.ReadByte();
            var unk2 = packet.ReadUShort();

            Character character = client.Metadata["fullchar"] as Character;
            var cid = character.id;
            var server = (int)client.Metadata["server"];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var slot = character.slot;

#if DEBUG
            Log.Message(string.Format("'{0}' '{1}' '{2}' '{3}'", MobID, unk1, MapID, unk2), ConsoleColor.Red, "PACKET RECV");
#endif

            var mi = mapmobs.Find(x => x.Id == MobID);

            var md = mobdata[mi.SId];


            //uint defense = Convert.ToUInt32(new Random().Next(1, 10));
            uint attack_rate = Convert.ToUInt32(new Random().Next(1, 10));
            //uint atk = character.stats.str_stat*2;   //Attempt dmg formula
            var atk = (int)client.Metadata["patk"];
            //uint dmg = atk - defense;

            var mobdef = new Random().Next(md.Defense, md.DefRate);
            var normal_damage = atk - mobdef;
            var crit_damage = normal_damage + (normal_damage * 20 / 100);
            var dmg = 0;

            var atack_result = 0;

            if (attack_rate > 0)
            {
                if (attack_rate < 7)
                {
                    if (attack_rate <= 5)
                    {
                        atack_result = 2;
                        dmg = normal_damage;
                        character.stats.exp = character.stats.exp + (ulong)dmg * 2;
                    }
                    if (attack_rate == 6)
                    {
                        atack_result = 1;
                        dmg = crit_damage;
                        character.stats.exp = character.stats.exp + (ulong)dmg * 2;
                    }
                }
                if (attack_rate == 7)
                {
                    atack_result = 27;
                    dmg = 0;
                }
                if (attack_rate >= 8)
                {
                    atack_result = 21;
                    dmg = 0;
                }

            }

            if (dmg < md.Defense && dmg < 0)
            {
                dmg = new Random().Next(1, atk);
                character.stats.exp = character.stats.exp + 1;
            }
            if (dmg >= mi.CurrentHP)
            {
                dmg = mi.CurrentHP;
                mi.CurrentHP = 0;
                //map.DropItem(mi.CurrentPosX, mi.CurrentPosX, 13, 1,1);
            }
            else
            {
                mi.CurrentHP = mi.CurrentHP - dmg;
            }

            ushort maxhp = character.stats.maxhp;
            ushort maxmp = character.stats.maxhp;

            ushort curhp = character.stats.curhp;
            ushort curmp = character.stats.curmp;

                builder.New(0x00B0);
                {
                    builder += (short)MobID;
                    builder += (byte)unk1;
                    builder += (byte)MapID;
                    builder += (byte)unk2;
                    builder += (uint)curhp;
                    builder += (uint)curmp;
                    builder += new byte[2];             // Unknown4 ( so far always 00 00 )
                    builder += (byte)atack_result;      // AttackResult;    
                    builder += (byte)0x1B;              // HitCounter
                    builder += (byte)0x3;               // Unknown5
                    builder += new byte[6];           // Unknown6

                    
                    builder += (ulong)0;                // OXP 
                    //builder += (uint)0;               // EXP
                    builder += (uint)dmg;               // DMG take on mob
                    builder += (ushort)mi.CurrentHP;    // MobHp
                    builder += (byte)0;                 // TakeDmg
                    builder += (uint)0;                 // HpHover
                    builder += (uint)0;                 // DmgTaken
                    builder += (uint)curhp;
                    builder += (uint)0;
                }



                if (curhp > 0)
                {
                    character.stats.curhp = curhp;
                    character.stats.curmp = curmp;
                }

                else
                {
                    #region Death PacketInfo
                    /*
                     * Death PacketInfo
                     * Server sends ErrorCode Msg (15 = Dead Menu where player can ressurect)
                     * Server sends Packet 0x042B  
                     */
                    #endregion

                    character.stats.curhp = maxhp;
                    character.stats.curmp = maxmp;

                    var p = client.CreatePacket("ErrorCode", packet.Opcode, (ushort)0, (ushort)15, (ushort)map.ID);
                    client.Send(p, "ErrorCode");

                    builder.New(0x042B);
                    {
                        builder += 0;
                        builder += 0;
                        builder += 0;
                    }

                    client.Send(builder, "unk2");
                }
            

#if DEBUG
            string notice = "";
            for (int i = 0; i < builder.Size; i++)
            {
                notice += builder.Data[i].ToString("X2") + " ";
            }

            Log.Notice(notice);
#endif

            client.Send(builder, "NormalAttack");

            if (mi.CurrentHP == 0)
            {
                mi.Spawn = Environment.TickCount + md.Respawn * 10;
            }

        }
    }
}