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
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Packet = System.Tuple<byte[], int, string, bool>;

#endregion

namespace Minerva
{
    public class ClientHandler
    {
        const uint READ_BLOCKED_CODE = 0x80131620;

        TcpClient client;
        Cryption crypt;
        PacketHandler packets;
        EventHandler events;

        NetworkStream stream;

        BlockingCollection<Packet> send;
        CancellationTokenSource cancel;

        public int AccountID { get; set; }
        public ConcurrentDictionary<string, object> Metadata { get; set; }
        public EndPoint RemoteEndPoint { get { return client.Client.RemoteEndPoint; } }

        public ClientHandler(TcpClient client, PacketHandler packets, EventHandler events)
        {
            this.client = client;
            this.packets = packets;
            this.events = events;

            send = new BlockingCollection<Packet>();
            cancel = new CancellationTokenSource();

            Metadata = new ConcurrentDictionary<string, object>();
            crypt = new Cryption();
            stream = client.GetStream();
        }

        ~ClientHandler()
        {
            stream.Dispose();
            client = null;
            crypt = null;
        }

        public void Start()
        {
            var t = new Thread(Run);
            t.Start();

            var t2 = new Thread(RunSend);
            t2.Start();
        }

        void Run()
        {
            byte[] data = new byte[0x1000];
            int i = 0;
            int read;

            while (true)
            {
                try
                {
                    read = stream.Read(data, i, 0x1000);
                }
                catch (System.IO.IOException ioe)
                {
                    uint err_code = (uint)ioe.HResult;

                    // if it was thrown due closed stream, it's okay
                    // otherwise, we need to log this exception
                    if (err_code != READ_BLOCKED_CODE)
                        Log.Error(ioe.Message);

                    break;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    break;
                }

                if (read == 0)
                    break;

                var packet = new byte[read];
                Array.ConstrainedCopy(data, 0, packet, 0, read);
                i = packets.Queue(packet, this, events);
            }

            cancel.Cancel();

            if (client.Connected)
            {
                events.ClientDisconnected(this, this);
                client.Close();
            }
        }

        void RunSend()
        {
            while (true)
            {
                try
                {
                    Packet packet;
                    send.TryTake(out packet, -1, cancel.Token);

                    Send(packet.Item1, packet.Item2, packet.Item3, packet.Item4);
                }
                catch (OperationCanceledException)
                { break; }
            }
        }

        private void Send(byte[] packet, int size, string name, bool suppress)
        {
            try
            {
                var opcode = (ushort)packet[4];
                opcode += (ushort)(packet[5] << 8);

                byte[] enc = crypt.Encrypt(packet, size);
                stream.Write(enc, 0, size);

                if (!suppress)
                    events.SentPacket(this, new PacketEventArgs(RemoteEndPoint.ToString(), name, opcode, size));
            }
            catch (Exception e)
            { Log.Error(e.Message); }
        }

        public void Send(PacketBuilder packet, string name, bool suppress = false)
        {
            var size = packet.Size;
            var data = packet.Data;

            send.Add(new Packet(data, size, name, suppress));
        }

        public void Send(ref byte[] packet, string name, bool suppress = false)
        {
            var size = packet.Length;

            send.Add(new Packet(packet, size, name, suppress));
        }

        public void Disconnect()
        {
            events.ClientDisconnected(this, this);
            stream.Close();
            client.Close();
        }

        public void GenerateKeychain(uint key, int position, int size)
        {
            crypt.GenerateKeychain(key, position, size);
        }

        public uint GetKeyFromKeychain(int index)
        {
            return BitConverter.ToUInt32(crypt.Keychain, index);
        }

        public void ChangeKey(uint key, uint step)
        {
            crypt.ChangeKeychain(key, step);
        }

        public void Decrypt(ref byte[] packet, int index, int size)
        {
            crypt.Decrypt(ref packet, index, size);
        }

        public int GetPacketSize(ref byte[] packet, int index)
        {
            return crypt.GetPacketSize(ref packet, index);
        }

        public PacketBuilder CreatePacket(string name, params object[] args)
        {
            return packets.CreatePacket(name, args);
        }
    }
}