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
using System.Net;

#endregion

namespace Minerva
{
    // Config-loading class
    public class Configuration
    {

        public static bool channelLog;
        public static int channelLogLvl;

        public static IPAddress channelIp;
        public static int channelPort;
        public static int channelType;
        public static int maxPlayers;

        public static string masterIP;
        public static int masterPort;

        public static void Load(string name)
        {
            var conf = new IniReader(string.Format("conf/{0}.ini", name));

            channelLog = Convert.ToBoolean(conf.GetValue("debug", "use", 0));
            channelLogLvl = conf.GetValue("debug", "level", 0);

            channelIp = IPAddress.Parse(conf.GetValue("listen", "ip", "127.0.0.1"));
            channelPort = conf.GetValue("listen", "port", 0);
            channelType = conf.GetValue("channel", "type", 0);
            maxPlayers = conf.GetValue("channel", "max_players", 0);

            masterIP = conf.GetValue("master", "ip", "localhost");
            masterPort = conf.GetValue("master", "port", 0);
        }
    }
}