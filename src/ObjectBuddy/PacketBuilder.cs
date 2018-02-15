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
    public class PacketBuilder : IDisposable
    {
        byte[] _data;
        int _size;

        public byte[] Data
        {
            get
            {
                _data[2] = (byte)_size;
                _data[3] = (byte)(_size >> 8);

                return _data;
            }
        }

        public ushort Opcode
        {
            get
            {
                ushort result = _data[4];
                result += (ushort)(_data[5] << 8);

                return result;
            }
        }

        public int Size
            { get { return _size; } }

        public PacketBuilder() { }    // Just here for sanity's sake, and so we can optimise using the New() method.

        public void New(int opcode, bool init = false)
        {
            _data = new byte[8192];

            if (!init)
            {
                _data[0] = 0xE2;
                _data[1] = 0xB7;

                unchecked
                {
                    _data[4] = (byte)opcode;
                    _data[5] = (byte)(opcode >> 8);
                }

                _size = 6;
            }
            else
            {
                _data[0] = 0xF3;
                _data[1] = 0xC8;

                unchecked
                {
                    _data[6] = (byte)opcode;
                    _data[7] = (byte)(opcode >> 8);
                }

                _size = 8;
            }
        }

        /// <summary>Appends data to the end of the packet.</summary>
        /// <param name="packet">this</param>
        /// <param name="value">The data to be added to the end of the packet.</param>
        /// <returns>this</returns>
        public static PacketBuilder operator +(PacketBuilder packet, byte[] value)
        {
            Array.ConstrainedCopy(value, 0, packet._data, packet._size, value.Length);

            packet._size += value.Length;

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, long value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((long*)pd) = value;
                    packet._size += 8;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, ulong value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((ulong*)pd) = value;
                    packet._size += 8;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, int value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((int*)pd) = value;
                    packet._size += 4;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, uint value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((uint*)pd) = value;
                    packet._size += 4;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, short value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((short*)pd) = value;
                    packet._size += 2;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, ushort value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((ushort*)pd) = value;
                    packet._size += 2;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, byte value)
        {
            packet._data[packet._size] = value;
            packet._size += 1;

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, float value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((float*)pd) = value;
                    packet._size += 4;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, double value)
        {
            unsafe
            {
                fixed (byte* pdata = packet._data)
                {
                    byte* pd = pdata;
                    pd += packet._size;

                    *((double*)pd) = value;
                    packet._size += 8;
                }
            }

            return packet;
        }

        public static PacketBuilder operator +(PacketBuilder packet, string value)
        {
            packet += System.Text.Encoding.ASCII.GetBytes(value);

            return packet;
        }

        public void Dispose()
        {
            _data = null;
        }
    }
}