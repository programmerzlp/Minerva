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
using System.IO;
using System.Net;

#endregion

namespace Minerva
{
    public class Configuration
    {
        // listen
        public IPAddress ListenIp { get; private set; }
        public int ListenPort { get; private set; }

        // server
        public bool Debug { get; private set; }
        public byte LogLevel { get; private set; }
        public ushort MaxUsers { get; private set; }

        // client
        public int ClientVersion { get; private set; }
        public int MagicKey { get; private set; }

        // whitelist
        public bool WhiteList { get; private set; }
        public string[] WhiteIps { get; private set; }

        // url
        public string Cash { get; private set; }
        public string CashCharge { get; private set; }
        public string GuildBoard { get; private set; } // deprecated

        // master
        public IPAddress MasterIp { get; private set; }
        public int MasterPort { get; private set; }

        public Configuration()
        {
            if (!File.Exists("conf/Login.ini"))
                Log.Warning("Configuration file not found! Loading default values...");

            var conf = new IniReader("conf/Login.ini");

            ListenIp = IPAddress.Parse(conf.GetValue("listen", "ip", "127.0.0.1"));
            ListenPort = conf.GetValue("listen", "port", 38101);

            Debug = conf.GetValue("server", "debug", false);
            LogLevel = Byte.Parse(conf.GetValue("server", "log_level", "0"));
            MaxUsers = UInt16.Parse(conf.GetValue("server", "max_users", "64"));

            ClientVersion = conf.GetValue("client", "client_version", 0);
            MagicKey = conf.GetValue("client", "magic_key", 0);

            WhiteList = conf.GetValue("whitelist", "enabled", false);
            var whiteIps = conf.GetValue("whitelist", "whitelist", "");
            WhiteIps = whiteIps.Split(';');

            Cash = conf.GetValue("url", "cash", "http://localhost/cashshop/?v1=");
            CashCharge = conf.GetValue("url", "cash_charge", "http://localhost/cashshop/?v1=");
            GuildBoard = conf.GetValue("url", "guild", "http://127.0.0.1/guild/?EncVal=");  // deprecated

            MasterIp = IPAddress.Parse(conf.GetValue("master", "ip", "127.0.0.1"));
            MasterPort = conf.GetValue("master", "port", 9001);
        }
    }
}