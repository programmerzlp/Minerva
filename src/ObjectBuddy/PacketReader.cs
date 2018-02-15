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
    public class PacketReader : IDisposable
    {
        byte[] _data;
        int _size;
        ushort _opcode;

        public int index;

        public byte this[int index]
            { get { return _data[index]; } }

        public byte[] this[int index, int length]
        {
            get
            {
                var b = new byte[length];
                Array.ConstrainedCopy(_data, index, b, 0, length);
                return b;
            }
        }

        public ushort Opcode
            { get { return _opcode; } }

        public int Size
            { get { return _size; } }

        public PacketReader() { }

        public void New(ref byte[] b, int index, int size)
        {
            _data = b;
            this.index = index;
            _size = size;
            _opcode = _data[8];
            _opcode += (ushort)(_data[9] << 8);

            this.index += 10;
        }

        public void Skip(int bytes)
            { index += bytes; }

        public byte[] ReadBytes(int count)
        {
            var result = new byte[count];

            Array.ConstrainedCopy(_data, index, result, 0, count);

            index += count;

            return result;
        }

        public long ReadLong()
        {
            long result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((long*)pd);
                    index += 8;
                }
            }

            return result;
        }

        public ulong ReadULong()
        {
            ulong result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((ulong*)pd);
                    index += 8;
                }
            }

            return result;
        }

        public int ReadInt()
        {
            int result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((int*)pd);
                    index += 4;
                }
            }

            return result;
        }

        public uint ReadUInt()
        {
            uint result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((uint*)pd);
                    index += 4;
                }
            }

            return result;
        }

        public short ReadShort()
        {
            short result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((short*)pd);
                    index += 2;
                }
            }

            return result;
        }

        public ushort ReadUShort()
        {
            ushort result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((ushort*)pd);
                    index += 2;
                }
            }

            return result;
        }

        public byte ReadByte()
        {
            var result = _data[index];
            index += 1;

            return result;
        }

        public float ReadFloat()
        {
            float result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((float*)pd);
                    index += 4;
                }
            }

            return result;
        }

        public double ReadDouble()
        {
            double result;

            unsafe
            {
                fixed (byte* pdata = _data)
                {
                    byte* pd = pdata;
                    pd += index;

                    result = *((double*)pd);
                    index += 8;
                }
            }

            return result;
        }

        public string ReadString(int length)
        {
            var result = System.Text.Encoding.ASCII.GetString(_data, index, length);

            index += length;

            return result;
        }

        public void Dispose()
        {
            _data = null;
        }
    }
}