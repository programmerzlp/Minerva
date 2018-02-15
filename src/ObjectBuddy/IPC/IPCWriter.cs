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

#endregion

namespace Minerva
{
    public class IPCWriter
    {
        AwesomeSockets.Buffer packet;
        public IPC Opcode { get; private set; }
        public int Size { get { return packet.GetSize(); } }
         
        public IPCWriter(IPC opcode)
        {
            Opcode = opcode;
            packet = AwesomeSockets.Buffer.New();
            AwesomeSockets.Buffer.ClearBuffer(packet);
            AwesomeSockets.Buffer.Add(packet, (byte)opcode);
        }

        public AwesomeSockets.Buffer GetRawPacket()
        {
            return packet;
        }

        public void Write(bool value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(byte value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(sbyte value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(ushort value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(short value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(uint value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(int value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(ulong value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(long value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(string value) { AwesomeSockets.Buffer.Add(packet, value); }
        public void Write(byte[] value) { AwesomeSockets.Buffer.Add(packet, value); }
    }
}
