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
    partial class PacketProtocol
    {
        public static void QueryCashItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {

            builder.New(0x1A2);
            {

                Character character = client.Metadata["fullchar"] as Character;
                var cid = character.id;
                var server = (int)client.Metadata["server"];
                var syncServer = client.Metadata["syncServer"] as SyncReceiver;

                var CashItens = CharacterManagement.GetCashItem(syncServer, server, cid/8);

                
                /*
                builder += (int)2;          //item qnt

                builder += (int)1;          //item cashid
                builder += (int)163865;         //item idx
                builder += (byte)0xF4;
                builder += (byte)0x00;
                builder += (byte)0x00;
                builder += (byte)0x00;
                //builder += (int)0x000000E4;          //item opt extreme
                builder += (int)536871167;  //item opt old
                builder += (int)0;
                builder += (byte)31;         //duration

                builder += (int)2;          //item cashid
                builder += (int)24;         //item idx
                builder += (int)4;          //item opt extreme
                builder += (int)536871167;  //item opt old
                builder += (int)0;
                builder += (byte)17;        //duration
                */
         

                
                builder += CashItens.Length;

                foreach (var i in CashItens)
                {
                    
                    builder += (int)i.ID;
                    builder += (int)i.itemid;
                    builder += i.itemopt;
                    builder += (int)i.itemopt2;
                    builder += (byte)i.duration;
       

                }

                /*
                builder += (int)2; //item qnt

                builder += (int)1; //item cashid
                builder += (int)25; //item idx
                builder += (int)536880405; //item opt
                builder += (byte)0; //duration

                builder += (int)2; //item cashid
                builder += (int)25; //item idx
                builder += (int)0; //item opt
                builder += (byte)0; //duration*/

            }

            client.Send(builder, "QueryCashItem");
        }
    }
}