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
using System.Xml.Linq;
using System.IO;

#endregion

namespace Minerva
{
    class MapLoader
    {
        public MapLoader()
        {
            /* */
            var mobsdata2 = new List<MobData>();


        }
       

        public Dictionary<int, Map> LoadMaps()
        {
            var maps = new Dictionary<int, Map>();
            Log.Message("Loading maps...", Log.DefaultFG);

            var xml = XDocument.Load("data/cabal.xml");
            var node = xml.Root.Element("map");
            var deadReturn = new Dictionary<int, int[]>();

            foreach (var n in node.Elements("map_index"))
            {
                deadReturn.Add(int.Parse(n.Attribute("world_id").Value),
                    new[] 
                    {
                        int.Parse(n.Attribute("dead_warp").Value),
                        int.Parse(n.Attribute("return_warp").Value)
                    }
                );
            }

            node = xml.Root.Element("warp_point");
            var warps = new List<WarpIndex>();

            foreach (var n in node.Elements("warp_index"))
            {
                warps.Add(new WarpIndex(int.Parse(n.Attribute("x").Value),
                                        int.Parse(n.Attribute("y").Value),
                                        int.Parse(n.Attribute("nation1x").Value),
                                        int.Parse(n.Attribute("nation1y").Value),
                                        int.Parse(n.Attribute("nation2x").Value),
                                        int.Parse(n.Attribute("nation2y").Value),
                                        int.Parse(n.Attribute("w_code").Value),
                                        int.Parse(n.Attribute("Fee").Value),
                                        int.Parse(n.Attribute("WorldIdx").Value),
                                        int.Parse(n.Attribute("level").Value)));
            }

            node = xml.Root.Element("warp_npc");

            var warpNPCs = new Dictionary<int, Dictionary<int, List<WarpList>>>();

            foreach (var n in node.Elements("world"))
            {
                var world = new Dictionary<int, List<WarpList>>();
                var npcs = n.Elements("npc");

                foreach (var npc in npcs)
                {
                    var list = new List<WarpList>();
                    var warpList = npc.Elements("warp_list");

                    foreach (var w in warpList)
                    {
                        list.Add(new WarpList(int.Parse(w.Attribute("order").Value),
                                              int.Parse(w.Attribute("type").Value),
                                              int.Parse(w.Attribute("target_id").Value),
                                              int.Parse(w.Attribute("level").Value),
                                              int.Parse(w.Attribute("Fee").Value),
                                              (w.Attribute("warp_item").Value != "") ? int.Parse(w.Attribute("warp_item").Value.Split(':')[0]) : 0,
                                              (w.Attribute("warp_item").Value != "") ? int.Parse(w.Attribute("warp_item").Value.Split(':')[1]) : 0,
                                              (w.Attribute("quest_id").Value != "") ? int.Parse(w.Attribute("quest_id").Value) : 0,
                                              int.Parse(w.Attribute("gps_view").Value) == 1));
                    }

                    world.Add(int.Parse(npc.FirstAttribute.Value), list);
                }

                warpNPCs.Add(int.Parse(n.FirstAttribute.Value), world);
            }

            xml = XDocument.Load("data/MapData.xml");
            node = xml.Root;
            var mI = 1;

            foreach (var n in node.Elements("map"))
            {
                var id = int.Parse(n.Attribute("id").Value);
                var type = int.Parse(n.Attribute("type").Value);
                var threadmap = n.Attribute("thread").Value;

                
                var mobsdata = LoadMobsData(id);
                var mobs = LoadMobs(id, mobsdata);
                var terrain = LoadTerrain(threadmap);

                var map = new Map(id, mobs, mobsdata, warps, terrain, (deadReturn.ContainsKey(mI) ? deadReturn[mI][0] : 1), (deadReturn.ContainsKey(mI) ? deadReturn[mI][1] : 1), (warpNPCs.ContainsKey(mI) ? warpNPCs[mI] : null));

                maps.Add(mI, map);
                mI++;
            }

            return maps;
        }

       

        List<MobData> LoadMobsData(int map)
        {
            var data = XDocument.Load("data/MobData.xml");
            var node2 = data.Root;
            var mobsdata = new List<MobData>();

            foreach (var n in node2.Elements("mob"))
            {
                var I = int.Parse(n.Attribute("id").Value);
                var Level = int.Parse(n.Attribute("level").Value);
                var Exp = int.Parse(n.Attribute("exp").Value);
                var HP = int.Parse(n.Attribute("hp").Value);
                var Defense = int.Parse(n.Attribute("defense").Value);
                var AtkRate = int.Parse(n.Attribute("atkrate").Value);
                var DefRate = int.Parse(n.Attribute("defrate").Value);
                var Hpregen = int.Parse(n.Attribute("hpregen").Value);
                var Respawn = int.Parse(n.Attribute("respawn").Value);
                var PatkMin = int.Parse(n.Attribute("patkmin").Value);
                var PatkMax = int.Parse(n.Attribute("patkmax").Value);

                mobsdata.Add(new MobData(I, Level, Exp, HP, Defense, AtkRate, DefRate, Hpregen, Respawn, PatkMin, PatkMax));
            }

            return mobsdata;
        }

        List<MobEntity> LoadMobs(int map, List<MobData> mobsdata)
        {
            var list = XDocument.Load("data/MobList.xml");
            var node = list.Root;
            var mobs = new List<MobEntity>();

            //var mobsdata = new List<MobData>();

            foreach (var n in node.Elements("World"))
            {
                var id = int.Parse(n.Attribute("id").Value);

                if (map != id)
                    continue;

                var mI = (uint)0;

                foreach (var _n in n.Element("Mobs").Elements("mob"))
                {
                    var Id = UInt16.Parse(_n.Attribute("id").Value);
                    var PosX = UInt16.Parse(_n.Attribute("x").Value);
                    var PosY = UInt16.Parse(_n.Attribute("y").Value);


                    var md = mobsdata[Id];

                    mobs.Add(new MobEntity(mI, Id, PosX, PosY, PosX, PosY, md.Lev, md.HP, md.HP, md.Respawn));
                    mI++;
                }
            }

            return mobs;
        }

        

    Tile[] LoadTerrain(string thread)
        {
            string file = "data/tmap/" + thread;

            if (!File.Exists(file))
                return null;

            BinaryReader b;
            //int ProcessNum;
            int TileNum = 0;
            Tile[] tile = null;

            try
            {
                b = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read));

                b.ReadInt32(); //ProcessNum = b.ReadInt32();
                TileNum = b.ReadInt32();

                tile = new Tile[TileNum];

                for (int i = 0; i < TileNum; i++)
                {
                    tile[i] = new Tile();
                    b.ReadInt32(); //tile[i].iProcessIdx = b.ReadInt32();
                    b.ReadInt32(); //tile[i].iTileIdx = b.ReadInt32();
                    b.ReadInt32(); //tile[i].iPosX = b.ReadInt32();
                    b.ReadInt32(); //tile[i].iPosY = b.ReadInt32();
                    b.ReadUInt32(); //tile[i].bIsEdge = b.ReadUInt32();
                    b.ReadUInt32(); //tile[i].unk = b.ReadUInt32();

                    for (int j = 0; j < 9; j++)
                    {
                        //tile[i].data[j] = new TileData();
                        b.ReadUInt32(); //tile[i].data[j].tile_count = b.ReadUInt32();

                        for (int k = 0; k < 9; k++)
                        {
                            b.ReadUInt32(); //tile[i].data[j].tile_id[k] = b.ReadUInt32();
                        }

                        b.ReadBytes(152 * 4); //Buffer.BlockCopy(b.ReadBytes(152 * 4), 0, tile[i].data[j].unk, 0, 152);
                    }

                    var a = new uint[256];

                    for (int j = 0; j < 256; j++)
                    {
                        a[j] = b.ReadUInt32();
                    }

                    tile[i].pixels = a;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message,"[Channel::MapLoader::LoadTerrain()]");
            }

            return tile;
        }
    }
}
