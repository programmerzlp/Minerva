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
using System.Linq;

#endregion

namespace Minerva
{
    public class IPCReader
    {
        AwesomeSockets.Buffer packet;
        public IPC Opcode
        {
            get
            {
                if (packet == null)
                    return IPC.None;

                return (IPC)AwesomeSockets.Buffer.Get<byte>(packet);
            }
        }
        public int Size { get { return packet.GetSize(); } }

        public IPCReader(AwesomeSockets.Buffer packet)
        {
            this.packet = packet;
        }

        public bool ReadBoolean() { return AwesomeSockets.Buffer.Get<bool>(packet); }
        public byte ReadByte() { return AwesomeSockets.Buffer.Get<byte>(packet); }
        public sbyte ReadSByte() { return AwesomeSockets.Buffer.Get<sbyte>(packet); }
        public ushort ReadUInt16() { return AwesomeSockets.Buffer.Get<ushort>(packet); }
        public short ReadInt16() { return AwesomeSockets.Buffer.Get<short>(packet); }
        public uint ReadUInt32() { return AwesomeSockets.Buffer.Get<uint>(packet); }
        public int ReadInt32() { return AwesomeSockets.Buffer.Get<int>(packet); }
        public ulong ReadUInt64() { return AwesomeSockets.Buffer.Get<ulong>(packet); }
        public long ReadInt64() { return AwesomeSockets.Buffer.Get<long>(packet); }
        public string ReadString() { return AwesomeSockets.Buffer.Get<string>(packet); }
        public byte[] ReadBytes(int length) { return AwesomeSockets.Buffer.GetBytes(packet,length); }
    }
}
