/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    https://github.com/dignityteam/minerva
    http://www.ragezone.com


    This file is part of Minerva.

    Minerva is free software: you can redistribute it and/or modify
    it under the terms of the GNU Generalpublic static License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Minerva is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Generalpublic static License for more details.

    You should have received a copy of the GNU Generalpublic static License
    along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#endregion

namespace Minerva
{
    public static class Global
    {
        /*
        public static  class Config
        {
            public static  string ConfigPath = "conf/";

            static Dictionary<string, bool> ChannelLog = new Dictionary<string, bool>();
            static Dictionary<string, int> ChannelLogLvl = new Dictionary<string, int>();
            public static Dictionary<string, IPAddress> ChannelIP = new Dictionary<string, IPAddress>();
            static Dictionary<string, int> ChannelPort = new Dictionary<string, int>();
            static Dictionary<string, int> ChannelType = new Dictionary<string, int>();
            static Dictionary<string, int> MaxPlayers = new Dictionary<string, int>();

            public class Channel
            {

                #region Channel Server Region

                public void LoadChannelLog(int server, int channel)
                {
                    IniReader channelconf = new IniReader(string.Format(ConfigPath + "Channel_{0}_{1}.ini", server, channel));

                    if (!ChannelLog.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)) || ChannelLog.Count ==0 )
                        ChannelLog.Add(string.Format("Channel_{0}_{1}", server, channel), Convert.ToBoolean(channelconf.GetValue("debug", "use", 0)));
                    if (!ChannelLogLvl.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)) || ChannelLogLvl.Count == 0)
                        ChannelLogLvl.Add(string.Format("Channel_{0}_{1}", server, channel), channelconf.GetValue("debug", "level", 0));

                    channelconf.Flush();
                }

                public void AddChannel(int server, int channel)
                {
                    IniReader channelconf = new IniReader(string.Format(ConfigPath + "Channel_{0}_{1}.ini", server, channel));

                    if(!ChannelLog.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)) || ChannelLog.Count == 0)
                        ChannelLog.Add(string.Format("Channel_{0}_{1}", server, channel), Convert.ToBoolean(channelconf.GetValue("debug", "use", 0)));
                    if (!ChannelLogLvl.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)) || ChannelLogLvl.Count == 0)
                        ChannelLogLvl.Add(string.Format("Channel_{0}_{1}", server, channel), channelconf.GetValue("debug", "level", 0));                    

                    if(!ChannelIP.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)))
                        ChannelIP.Add(string.Format("Channel_{0}_{1}", server, channel), IPAddress.Parse(channelconf.GetValue("listen", "ip", "127.0.0.1")));
                    if (!ChannelPort.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)))
                        ChannelPort.Add(string.Format("Channel_{0}_{1}", server, channel), channelconf.GetValue("listen", "port", 0));
                    if (!ChannelType.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)))
                        ChannelType.Add(string.Format("Channel_{0}_{1}", server, channel), channelconf.GetValue("listen", "type", -1));
                    if (!MaxPlayers.ContainsKey(string.Format("Channel_{0}_{1}", server, channel)))
                        MaxPlayers.Add(string.Format("Channel_{0}_{1}", server, channel), channelconf.GetValue("listen", "maxplayers", 100));

                    channelconf.Flush();
                }

                public bool getChannelLog(int server, int channel)
                {
                    return ChannelLog[string.Format("Channel_{0}_{1}", server, channel)];
                }

                public int getChannelLogLvl(int server, int channel)
                {
                    return ChannelLogLvl[string.Format("Channel_{0}_{1}", server, channel)];
                }

                public int getChannelPort(int server, int channel)
                {
                    return ChannelPort[string.Format("Channel_{0}_{1}", server, channel)];
                }

                public IPAddress getChannelIp(int server, int channel)
                {
                    return ChannelIP[string.Format("Channel_{0}_{1}", server, channel)];
                }

                public int getChannelType(int server, int channel)
                {
                    return ChannelType[string.Format("Channel_{0}_{1}", server, channel)];
                }

                public int getChannelMaxPlayers(int server, int channel)
                {
                    return MaxPlayers[string.Format("Channel_{0}_{1}", server, channel)];
                }
                #endregion
            }

            public static class Login
            {
                
            }

            public static class Master
            {
                
            }

            public static class Chat
            {
                
            }

            #region Master Server Region
                static IniReader masterconf = new IniReader(ConfigPath+"Master.ini");

                public static IPAddress MasterIP;
                public static int MasterPort;
                public static bool MasterLog;
                public static int MasterLogLvl;
                public static string LoginDB;
                public static string LoginDBIP;
                public static string LoginDBUser;
                public static string LoginDBPass;
                public static string LoginDBType;
                public static Dictionary<int, string> ServerDBs = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBIPs = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBUsers = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBPasses = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBTypes = new Dictionary<int, string>();

                public static  void LoadMasterServer(int server)
                {
                    var section = string.Format("server{0}db", server);

                    ServerDBTypes.Add(server, masterconf.GetValue(section, "type", ""));
                    ServerDBs.Add(server, masterconf.GetValue(section, "name", ""));
                    ServerDBIPs.Add(server, masterconf.GetValue(section, "ip", ""));
                    ServerDBUsers.Add(server, masterconf.GetValue(section, "user", ""));
                    ServerDBPasses.Add(server, masterconf.GetValue(section, "password", ""));
                }
            #endregion

            #region Login Server Region
                public static IPAddress LoginIP;
                public static int LoginPort;
                public static bool LoginLog;
                public static int LoginLogLvl;
                public static int ClientVersion;
                public static int NormalMagicKey;
                public static int IgnoreClientVersion;
                public static string CashURL;
                public static string CashChargeURL;
                public static string GuildURL;
            #endregion


            #region Chat Config
                public static IPAddress ChatIP;
                public static int ChatPort;
                public static bool ChatLog;
                public static int ChatLogLvl;
            #endregion

            static Config() 
            {
                //Master Server
                MasterLog = Convert.ToBoolean(masterconf.GetValue("debug", "use", 0));
                MasterLogLvl = masterconf.GetValue("debug", "level", 0);
                MasterIP = IPAddress.Parse(masterconf.GetValue("listen", "ip", "127.0.0.1"));
                MasterPort = masterconf.GetValue("listen", "port", 9001);
                LoginDBType = masterconf.GetValue("logindb", "type", "");
                LoginDBIP = masterconf.GetValue("logindb", "ip", "");
                LoginDB = masterconf.GetValue("logindb", "name", "");
                LoginDBUser = masterconf.GetValue("logindb", "user", "");
                LoginDBPass = masterconf.GetValue("logindb", "password", "");

                //Login Server
                IniReader loginconf = new IniReader(ConfigPath+"Login.ini");

                LoginLog = Convert.ToBoolean(loginconf.GetValue("debug", "use", 0));
                LoginLogLvl = loginconf.GetValue("debug", "level", 0);
                LoginIP = IPAddress.Parse(loginconf.GetValue("listen", "ip", "127.0.0.1"));
                LoginPort = loginconf.GetValue("listen", "port", 0);
                ClientVersion = loginconf.GetValue("client", "client_version", 0);
                NormalMagicKey = loginconf.GetValue("client", "magic_key", 0);
                IgnoreClientVersion = loginconf.GetValue("client", "ignore_client_version", 0);
                CashURL = loginconf.GetValue("url", "cash", "http://localhost/cashshop/?v1=");
                CashChargeURL = loginconf.GetValue("url", "cash_charge", "http://localhost/cashshop/?v1=");
                GuildURL = loginconf.GetValue("url", "guild", "http://localhost/guild/?EncVal=");

                

            //Chat Server
            IniReader chatconf = new IniReader(ConfigPath+"Chat.ini");

                ChatLog = Convert.ToBoolean(chatconf.GetValue("debug", "use", 0));
                ChatLogLvl = chatconf.GetValue("debug", "level", 0);
                ChatIP = IPAddress.Parse(chatconf.GetValue("listen", "ip", "127.0.0.1"));
                ChatPort = chatconf.GetValue("listen", "port", 0);
            }
        }
        */
    }
}
