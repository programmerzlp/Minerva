﻿/*
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

using System.Collections.Generic;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static PacketBuilder PC_ItemDropped(object[] args)
        {
           
            var x = (ushort)args[0];
            var y = (ushort)args[1];
            var id = (ushort)args[2];
            var uid = (ushort)args[3];
            var entity = (ushort)args[4];

            var client = (ClientHandler)args[3];
            Character character = client.Metadata["fullchar"] as Character;

            var p = new PacketBuilder();

            p.New(0x00CC);
            {
                p += (ushort)args[1];
                p += (ushort)args[1];
                p += (ushort)args[2];
                p += (ushort)args[3];
            }

            return p;
        }
    }
}