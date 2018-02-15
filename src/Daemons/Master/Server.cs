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

#endregion

namespace Minerva
{
    class Server
    {

        int ticks = Environment.TickCount;

        public Server()
        {
            Console.Title = "Minerva Master Server";
            Console.CursorVisible = false;

            int start = Environment.TickCount;

            Util.Info.PrintLogo();
            Console.WriteLine();
            Util.Info.PrintInfo();
            Console.WriteLine();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Configuration.Load();
            Log.Start("Master",Configuration.masterLog,Configuration.masterLogLvl);

            try
            {
                Log.Message("Reading configuration...", Log.DefaultFG);
                
                Log.Message("Loading initial char data...", Log.DefaultFG);
                var xml = XDocument.Load("data/InitialData.xml");
                var node = xml.Root.Element("char_init");
                var stats = new Dictionary<int, List<int>>();

                foreach (var n in node.Elements("init"))
                {
                    stats.Add(int.Parse(n.Attribute("style").Value), 
                        new List<int>()
                        {
                            int.Parse(n.Attribute("world_id").Value),
                            int.Parse(n.Attribute("x").Value),
                            int.Parse(n.Attribute("y").Value),
                            int.Parse(n.Attribute("suit_1").Value),
                            int.Parse(n.Attribute("glove_2").Value),
                            int.Parse(n.Attribute("boot_3").Value),
                            int.Parse(n.Attribute("rhand_4").Value),
                            (n.Attribute("lhand_5").Value != "") ? int.Parse(n.Attribute("lhand_5").Value) : -1,
                            int.Parse(n.Attribute("hp").Value),
                            int.Parse(n.Attribute("mp").Value)
                        }
                    );
                }

                node = xml.Root.Element("initstat");

                foreach (var n in node.Elements("initstat"))
                {
                    stats[int.Parse(n.Attribute("id").Value)].AddRange(
                        new[] 
                        {
                            int.Parse(n.Attribute("str").Value),
                            int.Parse(n.Attribute("int").Value),
                            int.Parse(n.Attribute("dex").Value)
                        }
                    );
                }

                xml = XDocument.Load("data/new_char.xml");
                node = xml.Root.Element("init");
                var items = new Dictionary<int, List<Tuple<byte[], ushort, byte>>>();
                var skills = new Dictionary<int, List<int[]>>();
                var quickslots = new Dictionary<int, List<int[]>>();

                foreach (var n in node.Elements("item"))
                {
                    var _class = int.Parse(n.Attribute("class").Value);
                    var item = new Item();
                    var amount = 0;
                    var slot = byte.Parse(n.Attribute("slot").Value);

                    item.ID = ushort.Parse(n.Attribute("id").Value);
                    var a = n.Attribute("bonus");
                    if (a != null) item.Bonus = byte.Parse(a.Value);
                    a = n.Attribute("is_bound");
                    if (a != null) item.IsBound = byte.Parse(a.Value);
                    a = n.Attribute("craft_type");
                    if (a != null) item.CraftType = byte.Parse(a.Value);
                    a = n.Attribute("craft_bonus");
                    if (a != null) item.CraftBonus = byte.Parse(a.Value);
                    a = n.Attribute("upgrades");
                    if (a != null) item.Upgrades = byte.Parse(a.Value);
                    a = n.Attribute("upgrade_1");
                    if (a != null) item.Upgrade1 = byte.Parse(a.Value);
                    a = n.Attribute("upgrade_2");
                    if (a != null) item.Upgrade2 = byte.Parse(a.Value);
                    a = n.Attribute("upgrade_3");
                    if (a != null) item.Upgrade3 = byte.Parse(a.Value);
                    a = n.Attribute("upgrade_4");
                    if (a != null) item.Upgrade4 = byte.Parse(a.Value);
                    a = n.Attribute("expiration");
                    if (a != null) item.ExpirationDate = uint.Parse(a.Value);
                    a = n.Attribute("amount");
                    if (a != null) amount = int.Parse(a.Value);

                    if (!items.ContainsKey(_class))
                        items[_class] = new List<Tuple<byte[], ushort, byte>>();

                    items[_class].Add(Tuple.Create(item.ToByteArray(), (ushort)amount, slot));
                }

                foreach (var n in node.Elements("skill"))
                {
                    var _class = int.Parse(n.Attribute("class").Value);
                    var skill = new int[3];

                    skill[0] = int.Parse(n.Attribute("id").Value);
                    skill[1] = int.Parse(n.Attribute("level").Value);
                    skill[2] = int.Parse(n.Attribute("slot").Value);

                    if (!skills.ContainsKey(_class))
                        skills[_class] = new List<int[]>();

                    skills[_class].Add(skill);
                }

                foreach (var n in node.Elements("quickslot"))
                {
                    var _class = int.Parse(n.Attribute("class").Value);
                    var quickslot = new int[2];

                    quickslot[0] = int.Parse(n.Attribute("id").Value);
                    quickslot[1] = int.Parse(n.Attribute("slot").Value);

                    if (!quickslots.ContainsKey(_class))
                        quickslots[_class] = new List<int[]>();

                    quickslots[_class].Add(quickslot);
                }

                Log.Message("Starting Pipe Server...", Log.DefaultFG);
                var pipe = new PipeServer(stats, items, skills, quickslots);
                pipe.RunPipe();

                Log.Notice("Minerva started in: {0} seconds", (Environment.TickCount - start) / 1000.0f);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, "[Master::Server::"+e.TargetSite.Name+"()]");
                #if DEBUG
                throw e;
                #endif
            }
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Fatal exception!");
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            Log.Error(ex.Message,"[Master::"+ex.Source+"::"+ex.TargetSite.Name+"()]");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    struct Item
    {
        public ushort ID;
        public byte Bonus;
        public byte IsBound;
        public byte CraftType;
        public byte CraftBonus;
        public byte Upgrades;
        public byte Upgrade1;
        public byte Upgrade2;
        public byte Upgrade3;
        public byte Upgrade4;
        public uint ExpirationDate;
    }
}