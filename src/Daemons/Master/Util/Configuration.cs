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
using System.Net;

#endregion

namespace Minerva
{
    // Config-loading class
    public class Configuration
    {
        public static IPAddress masterIP;
        public static int masterPort;
        public static bool masterLog;
        public static int masterLogLvl;
        public static string loginDB;
        public static string loginDBIP;
        public static string loginDBUser;
        public static string loginDBPass;
        public static string loginDBType;

        public static Dictionary<int, string> serverDBs;
        public static Dictionary<int, string> serverDBIPs;
        public static Dictionary<int, string> serverDBUsers;
        public static Dictionary<int, string> serverDBPasses;
        public static Dictionary<int, string> serverDBTypes;

        public static void Load()
        {
            var conf = new IniReader("conf/Master.ini");

            masterLog = Convert.ToBoolean(conf.GetValue("debug", "use", 0));
            masterLogLvl = conf.GetValue("debug", "level", 0);

            masterIP = IPAddress.Parse(conf.GetValue("listen", "ip", "127.0.0.1"));
            masterPort = conf.GetValue("listen", "port", 9001);

            loginDBType = conf.GetValue("logindb", "type", "");
            loginDB = conf.GetValue("logindb", "name", "");
            loginDBIP = conf.GetValue("logindb", "ip", "");
            loginDBUser = conf.GetValue("logindb", "user", "");
            loginDBPass = conf.GetValue("logindb", "password", "");

            serverDBs = new Dictionary<int, string>();
            serverDBIPs = new Dictionary<int, string>();
            serverDBUsers = new Dictionary<int, string>();
            serverDBPasses = new Dictionary<int, string>();
            serverDBTypes = new Dictionary<int, string>();
        }

        public static void LoadMasterServer(int server)
        {
            var conf = new IniReader("conf/Master.ini");
            var section = string.Format("server{0}db", server);

            serverDBTypes.Add(server, conf.GetValue(section, "type", ""));
            serverDBs.Add(server, conf.GetValue(section, "name", ""));
            serverDBIPs.Add(server, conf.GetValue(section, "ip", ""));
            serverDBUsers.Add(server, conf.GetValue(section, "user", ""));
            serverDBPasses.Add(server, conf.GetValue(section, "password", ""));
        }
    }
}