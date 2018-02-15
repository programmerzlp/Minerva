/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    http://board.thedivinityproject.com
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
using System.Collections.Generic;

using DItem = Minerva.Structures.Database.Item;
using CItem = Minerva.Structures.Client.Item;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {

        public static void CashNPC(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {

            builder.New(0xA05);
            {
                builder += (ushort)0;
                builder += 1;

                builder += (ushort)0;   //ID
                builder += (ulong)1;    //itemidx
                builder += (uint)0;     //tag
                builder += (uint)0;
                builder += (uint)0;     //duration
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (uint)5357;
                builder += (uint)123;   //price
                builder += (uint)0;
                builder += (ushort)0;
                builder += (ushort)2;
                builder += (ushort)0;

            }

            client.Send(builder, "CashNPC");
        }
    }
}
