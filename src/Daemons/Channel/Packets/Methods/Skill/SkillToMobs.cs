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

        public static void SkillToMobs(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {

            var map = client.Metadata["map"] as IMap;
            var mapmobs = (client.Metadata["map"] as IMap).GetSurroundingMobs(client, 3);
            var mobdata = map.GetMobsData;




            var skillid = packet.ReadUShort();
            var unk = packet.ReadInt();
            var unk1 = packet.ReadByte();
            var unk2 = packet.ReadUShort();
            var unk3 = packet.ReadShort();
            var unk4 = packet.ReadByte();
            var dmg = packet.ReadUInt();
            var unk5 = packet.ReadByte();
            var unk6 = packet.ReadShort();
            var unk7 = packet.ReadShort();
            var unk8 = packet.ReadShort();
            var unk9 = packet.ReadShort();
            var mobid = packet.ReadShort();
            var unk11 = packet.ReadByte();
            var unk12 = packet.ReadShort();
            var unk13 = packet.ReadShort();

#if DEBUG
            Log.Notice("SkillID:" + skillid + " SkillSlot:" + unk + " unk1:" + unk1 + " unk2:" + unk2 + " unk3:" + unk3 + " unk4:" + unk4 + " dmg:" + dmg + " unk5:" + unk5 + " unk6:" + unk6 + " unk7:" + unk7 + " unk8:" + unk8 + " unk9:" + unk9 + " MobID:" + mobid + " unk11:" + unk11 + " unk12:" + unk12 + " unk13:" + unk13 + "");
#endif

            //packet.Skip(6);
            var attack = packet.ReadUShort();

            Character character = client.Metadata["fullchar"] as Character;
            uint hp = character.stats.curhp;
            uint mp = character.stats.curmp;

            SkillLoader s = new SkillLoader();
            var skilllist = s.LoadSkillData(skillid);
            var sid = skilllist.Find(x => x.ID == skillid);

            var char_atk = (int)client.Metadata["patk"];

            if (sid.Type == 1)
            {
                char_atk = (int)client.Metadata["patk"];
            }
            if (sid.Type == 2)
            {
                char_atk = (int)client.Metadata["matk"];
            }

            var mi = mapmobs.Find(x => x.Id == mobid);
            var md = mobdata[mi.SId];

            uint attack_rate = Convert.ToUInt32(new Random().Next(1, 10));

            var normal_atk = char_atk + (sid.Attack + ( (sid.Amp / 100) * sid.Attack)) ;

            var mobdef = new Random().Next(md.Defense, md.DefRate);

            var normal_damage = normal_atk - mobdef;
            var crit_damage = normal_damage + ( normal_damage * sid.Critical);

            var damage = 0;
            
            var atack_result = 0;

            if (attack_rate <= 5)
            {
                atack_result = 2;
                damage = normal_damage;
                character.stats.exp = character.stats.exp + (ulong)damage * 2;
            }
            if (attack_rate == 6)
            {
                atack_result = 1;
                damage = crit_damage;
                character.stats.exp = character.stats.exp + (ulong)damage * 2;
            }
            if (attack_rate == 7)
            {
                atack_result = 27;
            }

            if (attack_rate >= 8)
            {
                atack_result = 21;
            }

            //Damage Maked
            if (damage < md.Defense && damage > 0)
            {
                damage = new Random().Next(1, 5);
            }

            if (damage >= mi.CurrentHP && damage > 0)
            {
                damage = mi.CurrentHP;
                mi.CurrentHP = 0;
                //map.DropItem(mi.CurrentPosX, mi.CurrentPosX, 13, 1,1);
            }
            else
            {
                mi.CurrentHP = mi.CurrentHP - damage;
            }

          
            var exp = character.stats.exp;
            var skillexp = character.stats.swordrank + sid.Attack;


            builder.New(0x00AE);
            {

                builder += (ushort)skillid;     //skillid
                builder += hp;                  //Hp
                builder += mp;                  //Mp
                builder += (ushort)0;           //SP
                builder += (ulong)exp;          //Exp 
                builder += (ulong)0;            //OXP
                builder += (ulong)skillexp;            //SkillEXP

                builder += new byte[26];

                builder += (uint)0xFFFFFFFF;
                builder += (byte)0;
                builder += hp;
                builder += (uint)0;

                builder += (byte)1;

                builder += (byte)mobid;

                builder += (byte)0;

                builder += (byte)map.ID;
                builder += (byte)2;
                builder += (byte)2;

                builder += (byte)atack_result;

                builder += (int)damage;

                builder += (int)mi.CurrentHP;

                builder += new byte[12];
                builder += (byte)1;



            }

            client.Send(builder, "SkillToMobs");    

            if (mi.CurrentHP == 0)
            {

                mi.Spawn = Environment.TickCount + md.Respawn * 10;

                mapmobs.Remove(mi);
                
            }
        }
    }
}
