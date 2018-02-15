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
using System.Security.Cryptography;
using System.IO;

#endregion

namespace Minerva
{
    class RSA
    {
        public ushort KeyLength { get; private set; }
        public byte[] PublicKey { get; private set; }
        RSACryptoServiceProvider rsaProvider;

        public RSA(ushort keyLength = 2048)
        {
            KeyLength = keyLength;

            // let's generate key pair
            GenerateKeyPair();
        }

        public byte[] Decrypt(byte[] encrypted)
        {
            try
            {
                byte[] decrypted = rsaProvider.Decrypt(encrypted, true);
                return decrypted;
            }
            catch (ArgumentNullException ae)
            {
                Log.Error(ae.Message);
                return null;
            }
            catch (CryptographicException ce)
            {
                Log.Error(ce.Message);
                return null;
            }
        }

        void GenerateKeyPair()
        {
            try
            {
                rsaProvider = new RSACryptoServiceProvider(KeyLength, new CspParameters());
                var parameters = rsaProvider.ExportParameters(true);

                PreparePublicKey();
            }
            catch (ArgumentOutOfRangeException ae)
            {
                Log.Error(ae.Message);
            }
            catch (CryptographicException ce)
            {
                Log.Error(ce.Message);
            }
        }

        void PreparePublicKey()
        {
            var parameters = rsaProvider.ExportParameters(false);
            using (var bitStringStream = new MemoryStream())
            {
                var bitStringWriter = new BinaryWriter(bitStringStream);
                bitStringWriter.Write((byte)0x30); // sequence
                using (var paramsStream = new MemoryStream())
                {
                    var paramsWriter = new BinaryWriter(paramsStream);
                    EncodeIntegerBigEndian(paramsWriter, parameters.Modulus);   // modulus
                    EncodeIntegerBigEndian(paramsWriter, parameters.Exponent);  // exponent
                    int paramsLength = (int)paramsStream.Length;
                    EncodeLength(bitStringWriter, paramsLength);
                    bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                }

                PublicKey = new byte[bitStringStream.Length];
                Array.Copy(bitStringStream.GetBuffer(), PublicKey, bitStringStream.Length);
            }
        }

        void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "Length must be non-negative");

            if (length < 0x80)
                stream.Write((byte)length); // short form
            else
            {
                // long form
                int temp = length;
                int bytesRequired = 0;

                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }

                stream.Write((byte)(bytesRequired | 0x80));

                for (int i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xFF));
                }
            }
        }

        void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // integer
            int prefixZeros = 0;

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }

            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7F)
                {
                    // add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                    EncodeLength(stream, value.Length - prefixZeros);

                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }
    }
}
