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
using System.Runtime.InteropServices;

#endregion

namespace Minerva
{
    public static class StructureExtensions
    {
        public static byte[] ToByteArray(this object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }

    public static class ByteArrayExtensions
    {
        public static object ToStructure<T>(this byte[] bytearray)
        {
            object obj = Activator.CreateInstance(typeof(T));
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            Marshal.Copy(bytearray, 0, i, len);
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);

            return obj;
        }

        public static ushort Size(this object value)
            { return (ushort)Marshal.SizeOf(value); }

        public static byte[] Append(this byte[] bytearray, object value)
        {
            var b = new byte[] { };

            if (value is byte[])
                b = (byte[])value;
            else
                b = value.ToByteArray();

            Array.Resize(ref bytearray, bytearray.Length + b.Length);
            b.CopyTo(bytearray, bytearray.Length - b.Length);

            return bytearray;
        }
    }
}