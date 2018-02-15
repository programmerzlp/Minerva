/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    http://board.thedivinityproject.com
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

using DItem = Minerva.Structures.Database.Item;
using CItem = Minerva.Structures.Client.Item;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void EnterWorld2(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            if(packet.Size<14) return;
            var id = packet.ReadInt();

            var server = (int)client.Metadata["server"];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            ServerData[] servers = GetChannels(syncServer);
            ChannelData channelData = null;

            foreach(var svr in servers){
                if (svr.id==server)
                {
                    foreach(var chn in svr.channels)
                    {
                        if (chn.id == (int)client.Metadata["channel"])
                            channelData = chn;
                    }
                }
            }

            if(channelData==null) return;

            var charslot = id % 8;
            var ip = BitConverter.ToUInt32(Configuration.channelIp.GetAddressBytes(), 0);
            var chport = Configuration.channelPort;

            if(client.AccountID<=0)
            {
                client.AccountID = Authentication.GetUser(syncServer, (ulong)client.Metadata["magic"]);
            }
            

            Character character = CharacterManagement.GetFullCharacter(syncServer, server, client.AccountID, charslot);
            client.Metadata["fullchar"] = character;
            client.Metadata["id"] = (int)id;

            client.Metadata["map"] = null;

            var equipment = character.equipment.GetEquipment();

            var level = character.level;
            var map = character.map;
            var x = (ushort)character.x;
            var y = (ushort)character.y;

            client.Metadata["dest_x"] = x;
            client.Metadata["dest_y"] = y;




            var style = (int)character._class;
            style += (int)character.stats.rank << 3;
            style += (character.face << 8);
            style += (character.colour << 13);
            style += (character.hair << 17);
            style += (!character.gender) ? 0 : (1 << 26);
            style += 152;

            client.Metadata["style"] = style;
            client.Metadata["equipment"] = equipment;


            var ditems = new List<byte[]>();
            var citems = new List<byte[]>();

            foreach (var i in character.inv)
            {
                var di = (DItem)(i.item.ToStructure<DItem>());
                var slot = i.slot;
                var ci = (CItem)(di.ToClient(slot));
                var item = ci.ToByteArray();

                if (i.amount != 0)
                    BitConverter.GetBytes(i.amount).CopyTo(item, 8);

                ditems.Add(di.ToByteArray());
                citems.Add(item);
            }
            
            client.Metadata["inventory"] = ditems;

            builder.New(0x8E, true);
            {
                builder += new byte[58];
                builder += (byte)0x14;
                builder += (byte)server;
                builder += (short)channelData.id;
                builder += new byte[22];
                builder += (byte)255;
                builder += (short)channelData.maxPlayers;
                builder += ip;                                          // channel ip
                builder += (ushort)chport;                              // channel port
                builder += (long)0;
                builder += 0x01000BF1;
                builder += (int)map;                                    // map
                builder += 0;
                builder += x;                                           // x
                builder += y;                                           // y
                builder += character.stats.exp;                         // exp
                builder += character.stats.alz;                         // alz
                builder += (ulong)0;                                        // wexp
                builder += level;                                       // level
                builder += 0;
                builder += character.stats.str_stat;                    // str
                builder += character.stats.dex_stat;                    // dex
                builder += character.stats.int_stat;                    // int
                builder += character.stats.pnt_stat;                    // points
                builder += character.stats.swordrank;                   // sword rank
                builder += (byte)0;                                     // magic rank **UNUSED**
                builder += new byte[6];
                builder += (uint)character.stats.maxhp;                 // max hp
                builder += (uint)character.stats.curhp;                 // curr hp
                builder += (uint)character.stats.maxmp;                 // max mp
                builder += (uint)character.stats.curmp;                 // curr mp
                builder += character.stats.maxsp;                       // max sp
                builder += character.stats.cursp;                       // curr sp
                builder += 8;                                           // max rage
                builder += (uint)0;                                     // DP
                builder += (ushort)0x2A30;
                builder += (ushort)0;
                builder += (uint)0;
                builder += (uint)0;                                     //SkillExpBars
                builder += (uint)0;                                     //SkillExp
                builder += (uint)0;                                     //SkillPoints
                builder += 0x0857;                                      //Unk 0x33
                builder += (uint)0;                                     //Unknw
                builder += (uint)999;                                     //Honor Points
                builder += (uint)0;
                builder += (uint)0;
                builder += (uint)0;
                builder += (uint)0;
                builder += (uint)0;
                builder += (ushort)0;
                builder += ip;                                          // unk server ip
                builder += (ushort)chport;                              // unk server port
                builder += ip;                                          // unk server ip Possible AuctionServer
                builder += (ushort)chport;                              // unk server port Possible AuctionServer
                builder += ip;                                          // unk server ip
                builder += (ushort)chport;                              // unk server port
                builder += (ushort)0xBBEE;
                builder += new byte[5];
                builder += 0x3FFFFF;                                           // warp codes?
                builder += 0x3FFFFF;                                       // map codes?
                builder += style;                                       // style
                builder += new byte[164];
                builder += (ushort)equipment.Count;                     // equip count    
                builder += (ushort)citems.Count;                        // items count
                builder += (ushort)0;
                builder += (ushort)character.skills.Length;             // skills count
                builder += (ushort)character.qslots.Length;             // quickslots count
                builder += new byte[1540];
                builder += 0x03E8;                                      // help window..?
                builder += 59;
                /*
                builder += new byte[1975];
                builder += (int)1;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;                                      // 2 -> Unlimited Warp Active until 2000-00:00:00?
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;                                      // 1 or 0
                builder += (int)0;
                builder += (int)0;                                      // 0 or 1
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (int)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (int)0;                                      // My Merit Points (See UI for more accurate perception)
                builder += (int)0;                                      // Merit Point (See UI for more accurate perception)
                builder += (int)0;                                     // if 15 and the value bellow Force Gems is >2000 then Unlimited Warp Active until 2000-00:00:00?
                builder += (int)0;                                      // Force Gems
                builder += (int)0;                                   // Unknown Value Max (9999)?
                */

                /*builder += new byte[2103];
                /*builder += (uint)0xFFFFFFFE;
                builder += new byte[2059];
                builder += (uint)0;                                     // Raise Spirit (millisecs)
                builder += new byte[16];
                builder += (int)0;                                      // My Merit Points (See UI for more accurate perception)
                builder += (int)0;                                      // Merit Point (See UI for more accurate perception)
                builder += (int)0;                                      // Unknown
                builder += (int)0;                                      // Force Gems
                builder += (int)0;                                      // Unknown*/

                builder += new byte[2115];

                builder += (byte)(character.name.Length + 1);           // name length + 1 
                builder += character.name;                              // name

                foreach (var e in equipment)
                {
                    builder += e.ToByteArray();
                }

                foreach (var i in citems)
                {
                    builder += i;
                }

                foreach (var s in character.skills)
                {
                    builder += (short)s.skill;                          // Skill ID
                    builder += s.level;                                 // Skill Level
                    builder += s.slot;                                  // Skill Slot
                    builder += (byte)0;
                }

                foreach (var q in character.qslots)
                {
                    builder += (short)q.skill;                          // Skill ID (referenced by Skill Slot)
                    builder += (short)q.slot;                           // Slot (in quickslot bars)
                }

                builder += 0x20;
                builder += (ushort)0x21;
                builder += (ushort)0x01;
                builder += 0x0BC9;
                builder += (byte)0x01;
                builder += (byte)0x01;
                builder += (byte)0x00;
                builder += 0x0435;
                builder += 0;
            }

            client.Send(builder, "Initialised");
            events.Warped("world.Initialized", client, map, x, y);
            Authentication.UpdateOnline(syncServer, client.AccountID, true);

            NewUserList(packet, builder, client, events);
            //NewMobsList(packet, builder, client, events);
        }
    }
}