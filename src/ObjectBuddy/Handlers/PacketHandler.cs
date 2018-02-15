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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Packet = System.Tuple<Minerva.ClientHandler, byte[], int, int>;
using PacketInfo = System.Tuple<string, Minerva.PacketMethod>;

#endregion

namespace Minerva
{
    public class PacketHandler
    {
        public Dictionary<ushort, PacketInfo> _methods;
        public Dictionary<string, PacketConstructor> _constructors;

        BlockingCollection<Packet> packets;
        ConcurrentBag<PacketBuilder> builders;
        ConcurrentBag<PacketReader> readers;

        EventHandler events;

        public PacketHandler(string server, Type type, EventHandler events)
        {
            this.events = events;

            _methods = new Dictionary<ushort, PacketInfo>();
            _constructors = new Dictionary<string, PacketConstructor>();
            packets = new BlockingCollection<Packet>();
            builders = new ConcurrentBag<PacketBuilder>();
            readers = new ConcurrentBag<PacketReader>();

            type.InvokeMember("Initialise", BindingFlags.Default | BindingFlags.InvokeMethod, null, null, new object[] { _methods, _constructors });

            var t = new Thread(Run);
            t.Start();
        }

        void Run()
        {
            while (true)
            {
                var packet = packets.Take();

                ThreadPool.QueueUserWorkItem(ThreadProc, packet);
            }
        }

        public int Queue(byte[] packet, ClientHandler client, EventHandler events)
        {
            int i = 0;

            while (true)
            {
                if(client==null || client.RemoteEndPoint==null) break;

                int size = (packet.Length - i >= 4) ? client.GetPacketSize(ref packet, i) : 4;

                if (size > packet.Length - i)
                {
                    Log.Warning(string.Format("Found partial packet of length {0}!  Reported length should be {1}.  Storing...", packet.Length - i, size));
                    return i;
                }

                if (size != packet.Length - i)
                    Log.Warning(string.Format("Merged packet found! Size of: First = {0}, Remaining = {1}", size, packet.Length - i - size));

                client.Decrypt(ref packet, i, size);

                packets.Add(new Packet(client, packet, i, size));

                i += size;

                if (i == packet.Length)
                    break;
            }

            return 0;
        }

        void Parse(PacketReader reader, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            if (client != null || client.RemoteEndPoint != null)
            {
                ushort opcode = reader.Opcode;

                if (_methods.ContainsKey(opcode))
                {
                    reader.index = 10;
                        events.ReceivedPacket(this, new PacketEventArgs(client.RemoteEndPoint.ToString(), _methods[opcode].Item1, opcode, reader.Size));
                        _methods[opcode].Item2(reader, builder, client, events);
                }
                else
                    events.Error(this, string.Format("Unknown packet ({0}) from {1}", opcode, client.RemoteEndPoint));
            }
        }

        public PacketBuilder CreatePacket(string name, object[] args)
        {
            var result = _constructors[name](args);

            return result;
        }

        void ThreadProc(Object stateInfo)
        {
            var packet = (Packet)stateInfo;
            var client = packet.Item1;
            var data = packet.Item2;
            var index = packet.Item3;
            var size = packet.Item4;

            PacketBuilder builder;
            PacketReader reader;

            if (client !=null || ((ClientHandler)client).RemoteEndPoint != null)
            {
                var btaken = builders.TryTake(out builder);
                var rtaken = readers.TryTake(out reader);

                if (!btaken)
                    builder = new PacketBuilder();

                if (!rtaken)
                    reader = new PacketReader();

                reader.New(ref data, index, size);

                Parse(reader, builder, client, events);

                builders.Add(builder);
                readers.Add(reader);
            }
        }
    }

    public delegate void PacketMethod(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events);
    public delegate PacketBuilder PacketConstructor(object[] args);
}