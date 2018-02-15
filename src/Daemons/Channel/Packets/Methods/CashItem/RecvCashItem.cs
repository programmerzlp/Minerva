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
        public static void RecvCashItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {

            builder.New(0x1A3);
            {
                var ID = packet.ReadUShort();
                var unk1 = packet.ReadByte();
                var unk2 = packet.ReadByte();
                var SlotFree = packet.ReadByte();
                var unk4 = packet.ReadByte();



                Character character = client.Metadata["fullchar"] as Character;
                var cid = character.id;
                var server = (int)client.Metadata["server"];
                var syncServer = client.Metadata["syncServer"] as SyncReceiver;
                var slot = character.slot;

                var CashItens = CharacterManagement.GetCashItem(syncServer, server, cid / 8);

                for (int i = 0; i < CashItens.Length; i++ )
                {
                    if (CashItens[i].ID == ID)
                    {

                        builder += (int)ID;
                        builder += (int)CashItens[i].itemid;
                        builder += CashItens[i].itemopt;
                        builder += (int)CashItens[i].itemopt2;
                        builder += (int)SlotFree;
                        builder += (int)0;
                        builder += (short)0;

                        CharacterManagement.SetCashItem(syncServer, server, CashItens[i].ID);
                    }
                    
                }

                //Log.Message(string.Format("'{0}' '{1}' '{2}' '{3}' '{4}'", ID, unk1, unk2, SlotFree, unk4), ConsoleColor.White, "Recv: ");

                /*
                builder += (int)unk0;       //item cashid
                builder += (int)163865;         //item idx
                builder += (int)244;          //item opt extreme
                builder += (int)536871167;  //item opt old
                builder += (int)0;          //item opt
                builder += (byte)unk3;      //Slot Free
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                
                builder += (int)0; 
                builder += (short)0;*/




            }

            client.Send(builder, "RecvCashItem");
        }
    }
}