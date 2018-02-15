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
    public partial class PipeServer
    {
        Dictionary<ulong, int> users;
        SortedDictionary<byte, SortedDictionary<byte, ChannelData>> channels;
        
        DatabaseHandler logindb;
        Dictionary<int, DatabaseHandler> serverdbs;

        Dictionary<int, List<int>> initialCharStats;
        Dictionary<int, List<Tuple<byte[], ushort, byte>>> initialCharItems;
        Dictionary<int, List<int[]>> initialCharSkills;
        Dictionary<int, List<int[]>> initialCharQuickslots;

        SyncHandler syncHandler;
        IPCProtocol ipcProtocol;

        public PipeServer(Dictionary<int, List<int>> stats, Dictionary<int, List<Tuple<byte[], ushort, byte>>> items, Dictionary<int, List<int[]>> skills, Dictionary<int, List<int[]>> quickslots)
        {
            initialCharStats = stats;
            initialCharItems = items;
            initialCharSkills = skills;
            initialCharQuickslots = quickslots;

            if (initialCharItems.ContainsKey(-1))
            {
                foreach (var i in initialCharItems)
                    if (i.Key != -1)
                        i.Value.AddRange(initialCharItems[-1]);

                initialCharItems.Remove(-1);
            }

            if (initialCharSkills.ContainsKey(-1))
            {
                foreach (var i in initialCharSkills)
                    if (i.Key != -1)
                        i.Value.AddRange(initialCharSkills[-1]);

                initialCharSkills.Remove(-1);
            }

            if (initialCharQuickslots.ContainsKey(-1))
            {
                foreach (var i in initialCharQuickslots)
                    if (i.Key != -1)
                        i.Value.AddRange(initialCharQuickslots[-1]);

                initialCharQuickslots.Remove(-1);
            }
        }

        public void RunPipe()
        {
            users = new Dictionary<ulong, int>();
            channels = new SortedDictionary<byte, SortedDictionary<byte, ChannelData>>();

            Log.Message("Connecting to Login Database...", Log.DefaultFG);
            logindb = new DatabaseHandler(Configuration.loginDBType, Configuration.loginDBIP, Configuration.loginDB, Configuration.loginDBUser, Configuration.loginDBPass);

            serverdbs = new Dictionary<int, DatabaseHandler>();

            syncHandler = new SyncHandler();

            syncHandler.SetLoginDBHandler(logindb);
            syncHandler.SetServerDBHandler(serverdbs);
            syncHandler.SetUsersData(users);
            syncHandler.SetChannelsData(channels);

            syncHandler.SetInitialCharStats(initialCharStats);
            syncHandler.SetInitialCharItems(initialCharItems);
            syncHandler.SetInitialCharSkills(initialCharSkills);
            syncHandler.SetInitialCharQuickSlots(initialCharQuickslots);

            ipcProtocol = new IPCProtocol(Configuration.masterIP.ToString(), 9001, syncHandler);
            ipcProtocol.StartSync();
        }
    }
}