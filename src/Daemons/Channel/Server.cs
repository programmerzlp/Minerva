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
using System.Net.Sockets;
using System.Threading;

#endregion

namespace Minerva
{

    class Server
    {
        TcpListener listener;
        Thread thread;

        SyncReceiver syncServer;

        Dictionary<ulong, ClientHandler> clients;
        Dictionary<int, Map> maps;
        Dictionary<int, SkillData> SkillData;

        PacketHandler packets;
        EventHandler events;
        //ScriptHandler scripts;       

        int ticks = Environment.TickCount;
        int count = 1;

        int server, channel;

        MapLoader mapLoader;
        SkillLoader skillLoader;

        public Server(int server, int channel)
        {
            this.server = server;
            this.channel = channel;

            Console.Title = "Minerva Channel Server";
            Console.CursorVisible = false;

            int start = Environment.TickCount;

            Util.Info.PrintLogo();
            Console.WriteLine();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Configuration.Load(string.Format("Channel_{0}_{1}", server, channel));
            Log.Start(string.Format("Channel_{0}_{1}", server, channel),Configuration.channelLog,Configuration.channelLogLvl);    // Start logging service

            clients = new Dictionary<ulong, ClientHandler>();
            events = new EventHandler();

            events.OnClientDisconnect += (sender, client) => {
                if (client.RemoteEndPoint!=null) { Log.Notice("Client {0} disconnected from Channel Server", client.RemoteEndPoint);
                    if (client.AccountID> 0 && syncServer != null) { Authentication.UpdateOnline(syncServer, client.AccountID, false); } clients.Remove((ulong)client.Metadata["magic"]); } };

            events.OnError += (sender, message) => { Log.Error(message, "[Channel::Server::"+ sender.GetType()+"()]"); };
            events.OnReceivePacket += (sender, e) => { Log.Received(e.Name, e.Opcode, e.Length); };
            events.OnSendPacket += (sender, e) => { Log.Sent(e.Name, e.Opcode, e.Length); };
            events.OnWarp += (sender, client, map, x, y) => { client.Metadata["map"] = maps[map]; maps[map].MoveClient(client, x / 16, y / 16); maps[map].UpdateCells(client); };

            /*Console.WriteLine("Compiling and registering scripts...");
            scripts = new ScriptHandler();
            scripts.Concatenate("Events", new string[] { "mscorlib" });
            scripts.Run("Events");
            scripts.CreateInstance("Events");
            dynamic result = scripts.Invoke("_init_", events);*/

            try
            {
                Log.Message("Reading configuration...", Log.DefaultFG);

                mapLoader = new MapLoader();
                maps = mapLoader.LoadMaps();

                skillLoader = new SkillLoader();
                SkillData = skillLoader.LoadSkills();

                Log.Message("Registering packets...", Log.DefaultFG);
                packets = new PacketHandler("world", new PacketProtocol().GetType(), events);

                var aa = Configuration.channelIp;
                var address = BitConverter.ToUInt32(aa.GetAddressBytes(), 0);
                var port = Configuration.channelPort;

                listener = new TcpListener(aa, port);
                thread = new Thread(Listen);
                thread.Start();
                
                syncServer = new SyncReceiver(Configuration.masterIP, Configuration.masterPort, events);
                syncServer.OnSyncSuccess += (sender, e) => 
                {
                    var type = Configuration.channelType;
                    var maxPlayers = Configuration.maxPlayers;
                    Authentication.RegisterChannel(syncServer, server, channel, type, address, port, maxPlayers);
                };

                Log.Notice("Minerva started in: {0} seconds", (Environment.TickCount - start) / 1000.0f);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, "[Channel::"+e.Source+"::"+e.TargetSite+"()]");
                #if DEBUG
                throw e;
                #endif
            }
        }

        void Listen()
        {
            listener.Start();

            Log.Notice("Channel Server listening for clients on {0}", listener.LocalEndpoint);

            while (true)
            {
                // blocks until a client has connected to the server
                TcpClient client = this.listener.AcceptTcpClient();

                Log.Notice("Client {0} connected to Channel Server", client.Client.RemoteEndPoint);

                int timestamp = Environment.TickCount - ticks;
                ulong key = ((ulong)count << 32) + (ulong)timestamp;
                var c = new ClientHandler(client, packets, events);
                c.Metadata["timestamp"] = (uint)timestamp;
                c.Metadata["count"] = (ushort)count++;
                c.Metadata["magic"] = key;
                c.Metadata["clients"] = clients;
                c.Metadata["server"] = server;
                c.Metadata["channel"] = channel;
                c.Metadata["syncServer"] = syncServer;
                c.Metadata["chServer"] = this;
                c.Start();

                
                if(clients.Count>0) {
                    ClientHandler[] clientarr = new ClientHandler[clients.Values.Count];
                    clients.Values.CopyTo(clientarr,0);
                    ClientHandler d = clientarr[clientarr.Length-1];
                    var oldVal = d.RemoteEndPoint as System.Net.IPEndPoint;
                    var newVal = c.RemoteEndPoint as System.Net.IPEndPoint;
                    if (!oldVal.Address.Equals(newVal.Address)) {
                        clients.Add(key, c);
                    }
                }else
                {
                    clients.Add(key,c);
                }
            }
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Fatal exception!");
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            Log.Error(ex.Message, "[Channel::"+ex.Source+"::" + ex.TargetSite + "()]");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}