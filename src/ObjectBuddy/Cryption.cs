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
    public class Cryption
    {
        uint[] masks = { 0x00000000, 0x000000FF, 0x0000FFFF, 0x00FFFFFF };

        public byte[] Keychain;

        public uint HeaderXor;
        public uint Step;
        public uint Mul;

        bool first = true;

        public Cryption()
        {
            Keychain = new byte[0x20000];
            GenerateKeychain(0x8F54C37B, 0, 0x4000);

            HeaderXor = 0xB43CC06E;
            Step = 0;
            Mul = 1;
        }

        public byte[] Encrypt(byte[] packet, int size)
        {
            byte[] pck = new byte[size];
            Array.Copy(packet, pck, size);

            unsafe
            {
                fixed (byte* ppacket = pck, pkeychain = Keychain)
                {
                    *((uint*)ppacket) ^= 0x7AB38CF1;
                    var token = *((uint*)ppacket);
                    token &= 0x3FFF;
                    token *= 4;
                    token = *((uint*)&pkeychain[token]);

                    int i, t = (size - 4) / 4;    // Process in blocks of 32 bits (4 bytes)

                    for (i = 4; t > 0; i += 4, t--)
                    {
                        var t1 = *((uint*)&ppacket[i]);
                        t1 ^= token;
                        *((uint*)&ppacket[i]) = t1;

                        t1 &= 0x3FFF;
                        t1 *= 4;
                        token = *((uint*)&pkeychain[t1]);
                    }

                    token &= masks[((size - 4) & 3)];
                    *((uint*)&ppacket[i]) ^= token;
                }
            }

            return pck;
        }

        public void Decrypt(ref byte[] packet, int index, int size)
        {
            var header = (uint)GetPacketSize(ref packet, index);
            header <<= 16;
            header += 0xB7E2;
            
            if (first)
                first = false;

            unsafe
            {
                fixed (byte* ppacket = &packet[index], pkeychain = Keychain)
                {
                    var token = *((uint*)ppacket);
                    token &= 0x3FFF;
                    token *= Mul;
                    token *= 4;
                    token = *((uint*)&pkeychain[token]);
                    *((uint*)ppacket) = header;

                    int i, t = (size - 8) / 4;    // Process in blocks of 32 bits (4 bytes)

                    for (i = 8; t > 0; i +=4, t--)
                    {
                        var t1 = *((uint*)&ppacket[i]);
                        token ^= t1;
                        *((uint*)&ppacket[i]) = token;

                        t1 &= 0x3FFF;
                        t1 *= Mul;
                        t1 *= 4;
                        token = *((uint*)&pkeychain[t1]);
                    }

                    token &= masks[((size - 8) & 3)];
                    *((uint*)&ppacket[i]) ^= token;
                    
                    *((uint*)&ppacket[4]) = 0;

                    Step += 1;
                    Step &= 0x3FFF;
                    HeaderXor = *((uint*)&pkeychain[Step * Mul * 4]);
                }
            }
        }

        public void GenerateKeychain(uint key, int position, int size)
        {
            uint ret2;
            uint ret3;
            uint ret4;

            for (int i = position; i < position + size; i++)
            {
                ret4 = key * 0x2F6B6F5;
                ret4 += 0x14698B7;
                ret2 = ret4;
                ret4 >>= 0x10;
                ret4 *= 0x27F41C3;
                ret4 += 0x0B327BD;
                ret4 >>= 0x10;

                ret3 = ret2 * 0x2F6B6F5;
                ret3 += 0x14698B7;
                key = ret3;
                ret3 >>= 0x10;
                ret3 *= 0x27F41C3;
                ret3 += 0x0B327BD;
                ret3 &= 0xFFFF0000;

                ret4 |= ret3;

                unsafe
                {
                    fixed (byte* pkeychain = Keychain)
                        *((uint*)&pkeychain[i * 4]) = ret4;
                }
            }
        }

        public void ChangeKeychain(uint key, uint step)
        {
            Mul = 2;
            Step = step - 1;

            if ((int)Step < 0)
                Step = (uint)((int)Step + 0x4000);

            GenerateKeychain(key, 0x4000, 0x4000);

            unsafe
            {
                fixed (byte* pkeychain = Keychain)
                    HeaderXor = *((uint*)&pkeychain[Step * Mul * 4]);
            }
        }

        public int GetPacketSize(ref byte[] packet, int index)
        {
            unsafe
            {
                uint header;

                fixed (byte* ppacket = packet)
                    header = *((uint*)&ppacket[index]);

                if (first)
                    return 0x0E;

                header ^= HeaderXor;
                header >>= 16;

                return (int)header;
            }
        }
    }
}