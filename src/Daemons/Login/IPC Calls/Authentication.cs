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
    class Authentication
    {
        public static AccountData FetchAccount(SyncReceiver sync, string name, string pass)
        {
            var packet = sync.CreateIPC(IPC.FetchAccount);
            packet.Write(name);
            packet.Write(pass);
            sync.Send(packet);

            var recv = sync.ReadIPC();

            AccountData data = new AccountData();
            data.id = -1;
            data.name = name;
            data.status = AuthStatus.Unverified;
            data.online = false;

            if (recv == null)
                return data;

            data.id = recv.ReadInt32();                     // we should change to uint; and use "id = 0" for invalid/non exist account
            data.status = (AuthStatus)recv.ReadByte();
            data.online = recv.ReadBoolean();

            return data;
        }

        public static void UpdateIPDate(SyncReceiver sync, int id, string ip, DateTime date)
        {
            var packet = sync.CreateIPC(IPC.UpdateIPDate);
            packet.Write(id);   // change to uint..?
            packet.Write(ip);   // ip should be uint
            packet.Write(date.ToBinary());

            sync.Send(packet);
        }

        public static void UpdateOnline(SyncReceiver sync, int id, bool online)
        {
            var packet = sync.CreateIPC(IPC.UpdateOnline);
            packet.Write(id);       // change to uint..?
            packet.Write(online);   

            sync.Send(packet);
        }

        public static void AddUser(SyncReceiver sync, ulong key, int id)
        {
            var packet = sync.CreateIPC(IPC.AddUser);
            packet.Write(key);
            packet.Write(id);   // change to uint..?

            sync.Send(packet);
        }

        public static int GetUser(SyncReceiver sync, ulong key)
        {
            // change to uint..?

            int id = -1;
            var packet = sync.CreateIPC(IPC.GetUser);
            packet.Write(key);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return id;

            id = recv.ReadInt32();

            return id;
        }
    }

    class AccountData
    {
        public int id               { get; set; }
        public string name          { get; set; }
        public AuthStatus status    { get; set; }
        public bool online          { get; set; }
    }
}
