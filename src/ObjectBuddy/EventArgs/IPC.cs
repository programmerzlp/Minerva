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
    public class IPCPacketEventArgs : EventArgs
    {
        private IPC _opcode;
        private int _length;
        private DateTime _time;

        public IPC Opcode { get { return _opcode; } }
        public int Length { get { return _length; } }
        public DateTime Time { get { return _time; } }

        public IPCPacketEventArgs(IPC opcode, int length)
        {
            _opcode = opcode;
            _length = length;
            _time = DateTime.Now;
        }
    }
}