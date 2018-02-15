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
using System.Threading;

using AwesomeSockets;

#endregion

namespace Minerva
{
    public class SyncReceiver
    {
        public delegate void SyncRecvHandler(object sender, EventArgs e);

        public event SyncRecvHandler OnSyncSuccess;
        public event SyncRecvHandler OnSyncFailed;

        EventHandler events;

        public void SyncSuccess(object sender, EventArgs e)
        {
            if (OnSyncSuccess != null) OnSyncSuccess(sender, e);
        }

        public void SyncFailed(object sender, EventArgs e)
        {
            if (OnSyncFailed != null) OnSyncFailed(sender, e);
        }

        ISocket syncClient { get; set; }
        Thread syncThread;
        System.Timers.Timer hbTimer;

        string ip;
        int port;
        bool syncRunning = false;

        public SyncReceiver(string ip, int port, EventHandler events = null)
        {
            this.ip = ip;
            this.port = port;
            this.events = events;

            syncThread = new Thread(Listen);
            syncThread.Start();
        }

        void Listen()
        {
            Thread.Sleep(1000);
            Log.Notice("Starting sync connection with Master Server...");
            Thread.Sleep(1000);

            hbTimer = new System.Timers.Timer(1000 * 36); // heartbeat every 30 seconds
            hbTimer.AutoReset = true;
            hbTimer.Elapsed += (sender, e) => { HeartBeat(); };

            while (true)
            {
                if (syncClient == null && !syncRunning)
                {
                    try
                    {
                        syncClient = AweSock.TcpConnect(ip, port);
                        syncRunning = true;
                        hbTimer.Start();
                        Log.Notice("Established sync connection with Master Server!");
                        SyncSuccess(this, new EventArgs());
                    }
                    catch (Exception)
                    {
                        syncRunning = false;
                        Log.Error("Failed to establish sync connection to Master Server!", "[ObjectBuddy::SyncReceiver::Listen()]");
                        SyncFailed(this, new EventArgs());
                    }
                }

                if (syncRunning)
                {
                    if (!syncClient.GetSocket().Connected)
                    {
                        hbTimer.Stop();
                        syncClient = null;
                        syncRunning = false;
                        Log.Error("Lost sync connection to Master Server!", "[ObjectBuddy::SyncReceiver::Listen()]");
                    }                        
                }

                Thread.Sleep(1);
            }
        }

        public IPCWriter CreateIPC(IPC opcode)
        {
            return new IPCWriter(opcode);
        }

        public void Send(IPCWriter writer)
        {
            try {
                var packet = writer.GetRawPacket();
                AwesomeSockets.Buffer.FinalizeBuffer(packet);
                SendIPC(packet);

                var opcode = writer.Opcode;
                int size = writer.Size;
                events.IPCSentPacket(this, new IPCPacketEventArgs(opcode, size));
            }
            catch (Exception ex) {
                Log.Error("Sending IPC Packet failed on packet {0}", writer.GetRawPacket().ToString() , "[ObjectBuddy::SyncReceiver::" + ex.TargetSite + "()]");
            }
        }

        bool SendIPC(AwesomeSockets.Buffer packet)
        {
            try
            {
                AweSock.SendMessage(syncClient, packet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool ReadIPC(AwesomeSockets.Buffer packet)
        {
            try
            {
                AweSock.ReceiveMessage(syncClient, packet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IPCReader ReadIPC()
        {
            AwesomeSockets.Buffer inBuf = AwesomeSockets.Buffer.New();

            if (!ReadIPC(inBuf))
                return null;

            var reader = new IPCReader(inBuf);
            var opcode = reader.Opcode;

            if (opcode == IPC.None)
                return null;

            events.IPCReceivedPacket(this, new IPCPacketEventArgs(opcode, reader.Size));

            return reader;
        }

        void HeartBeat()
        {
            Send(CreateIPC(IPC.HeartBeat));
            ReadIPC();
        }
    }
}
