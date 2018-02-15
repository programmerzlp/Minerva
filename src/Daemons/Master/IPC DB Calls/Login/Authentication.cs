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
    partial class IPCProtocol
    {
        void FetchAccount(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var name = data.ReadString();
            var pass = data.ReadString();

            var logindb = sync.GetLoginDBHandler();
            var packet = new IPCWriter(IPC.FetchAccount);

            if (!logindb.FetchAccount(name))
            {
                packet.Write(0x00);
                packet.Write((byte)0x03);
                packet.Write(false);
            }
            else
            {
                logindb.ReadRow();
                var id = (logindb["id"] as int?).Value;
                var _pass = logindb["password"].ToString();
                var auth = (logindb["auth"] as byte?).Value;
                var online = Convert.ToBoolean((logindb["online"]));

                if (pass != _pass)
                {
                    packet.Write(0x00);
                    packet.Write((byte)0x04);
                    packet.Write(false);
                }
                else
                {
                    packet.Write(id);
                    packet.Write(auth);
                    packet.Write(online);
                }
                
            }

            receiver.Send(packet);
        }

        void UpdateIPDate(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var id = data.ReadInt32();
            var ip = data.ReadString();
            var date = DateTime.FromBinary(data.ReadInt64());
            var logindb = sync.GetLoginDBHandler();

            logindb.UpdateIPDate(id, ip, date);
        }

        void UpdateOnline(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var id = data.ReadInt32();
            var online = data.ReadBoolean();
            var logindb = sync.GetLoginDBHandler();

            logindb.UpdateOnline(id, online);
        }

        void VerifyPassword(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var id = data.ReadInt32();
            var pass = data.ReadString();

            var logindb = sync.GetLoginDBHandler();
            var packet = new IPCWriter(IPC.VerifyPassword);
            var status = logindb.VerifyPassword(id, pass);

            packet.Write(status);

            receiver.Send(packet);
        }
    }
}
