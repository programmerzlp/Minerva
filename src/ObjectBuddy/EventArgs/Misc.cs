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
    public class PacketEventArgs : EventArgs
    {
        private string _ip;
        private string _name;
        private ushort _opcode;
        private int _length;
        private DateTime _time;

        public string IP { get { return _ip; } }
        public string Name { get { return _name; } }
        public ushort Opcode { get { return _opcode; } }
        public int Length { get { return _length; } }
        public DateTime Time { get { return _time; } }

        public PacketEventArgs(string ip, string name, ushort opcode, int length)
        {
            _ip = ip;
            _name = name;
            _opcode = opcode;
            _length = length;
            _time = DateTime.Now;
        }
    }
}