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
using System.Linq;
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
        Configuration conf;

        HashSet<ClientHandler> clients;

        PacketHandler packets;
        EventHandler events;
        ScriptHandler scripts;

        int ticks = Environment.TickCount;
        int count = 1;

        public Server()
        {
            Console.Title = "Minerva Login Server";
            Console.CursorVisible = false;

            int start = Environment.TickCount;

            Util.Info.PrintLogo();
            Console.WriteLine();
            Util.Info.PrintInfo();
            Console.WriteLine();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Log.Start("Login"); // start logging service

            clients = new HashSet<ClientHandler>();

            Log.Message("Reading configuration...", Log.DefaultFG);
            conf = new Configuration();

            Log.Message("Registering events...", Log.DefaultFG);
            events = new EventHandler();

            events.OnClientDisconnect += (sender, client) => 
            {
                Log.Notice("Client {0} disconnected from Login Server", client.RemoteEndPoint);

                // temporary disabled
                //if (client.AccountID > 0 && syncServer != null)
                //    Authentication.UpdateOnline(syncServer, client.AccountID, false);

                clients.Remove(client);
            };

            events.OnError += (sender, message) => Log.Error(message);
            events.OnReceivePacket += (sender, e) => Log.Received(e.Name, e.Opcode, e.Length);
            events.OnSendPacket += (sender, e) => Log.Sent(e.Name, e.Opcode, e.Length);

            events.OnIPCReceivePacket += (sender, e) => Log.IPC_Received(e.Opcode, e.Length);
            events.OnIPCSendPacket += (sender, e) => Log.IPC_Sent(e.Opcode, e.Length);

            Log.Message("Compiling and registering scripts...", Log.DefaultFG);
            scripts = new ScriptHandler();
            scripts.Concatenate("Events", new string[] { "mscorlib" });
            scripts.Run("Events");
            scripts.CreateInstance("Events");
            scripts.Invoke("_init_", events);

            Log.Message("Registering packets...", Log.DefaultFG);
            packets = new PacketHandler("login", new PacketProtocol().GetType(), events);

            Log.Level = conf.LogLevel;

            try
            {
                listener = new TcpListener(conf.ListenIp, conf.ListenPort);
                thread = new Thread(Listen);
                thread.Start();
                                                           // fixme
                syncServer = new SyncReceiver(conf.MasterIp.ToString(), conf.MasterPort, events);

                Log.Notice("Debugging mode: {0}", conf.Debug ? "on" : "off");
                Log.Notice("Whitelist: {0}", conf.WhiteList ? "on" : "off");
                Log.Notice("Minerva started in: {0} seconds", (Environment.TickCount - start) / 1000.0f);
            }
            catch (Exception e) 
            {
                Log.FatalError(e.Message);
                #if DEBUG
                throw e;
                #endif
            }
        }

        void Listen()
        {
            listener.Start();
            Log.Notice("Login Server listening for clients on {0}", listener.LocalEndpoint);

            while (true)
            {
                // waits for a client to connect to the server
                var client = listener.AcceptTcpClient();
                var endPoint = client.Client.RemoteEndPoint as IPEndPoint;

                if (conf.WhiteList)
                {
                    string ip = endPoint.Address.ToString();

                    if (!conf.WhiteIps.Contains(ip))
                    {
                        Log.Warning("Client {0} is trying to connect. Disconnecting client...", endPoint);
                        client.Close();

                        continue;
                    }
                }

                Log.Notice("Client {0} connected to Login Server", endPoint);

                if (clients.Count >= conf.MaxUsers)
                {
                    Log.Warning("Server is full: {0} clients connected. Disconnecting client...", clients.Count);

                    client.Close();
                    continue;
                }

                var timestamp = Environment.TickCount - ticks;
                var key = ((ulong)count << 32) + (ulong)timestamp;
                var c = new ClientHandler(client, packets, events);

                c.Metadata["timestamp"] = (uint)timestamp;
                c.Metadata["count"] = (ushort)count++;
                c.Metadata["magic"] = key;
                c.Metadata["conf"] = conf;
                c.Metadata["syncServer"] = syncServer;
                c.Start();

                clients.Add(c);

                /* temporary disabled
                // i'm not sure how this works
                // client just connected(not logged in yet)
                // it won't represent a real client account id(c.AccountID)
                Authentication.UpdateOnline(syncServer, c.AccountID, true);*/
            }
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.FatalError("Fatal exception!");

            var ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            Log.FatalError(ex.Message);
            Log.FatalError("\n" + ex.StackTrace);

            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}