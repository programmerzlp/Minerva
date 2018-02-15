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
        public static void AddUser(SyncReceiver sync, ulong key, int id)
        {
            var packet = sync.CreateIPC(IPC.AddUser);
            packet.Write(key);
            packet.Write(id);

            sync.Send(packet);
        }

        public static int GetUser(SyncReceiver sync, ulong key)
        {
            var id = -1;
            var packet = sync.CreateIPC(IPC.GetUser);
            packet.Write(key);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return id;

            id = recv.ReadInt32();
            

            return id;
        }

        public static void RegisterChannel(SyncReceiver sync, int server, int channel, int type, uint ip, int port, int maxPlayers)
        {
            var packet = sync.CreateIPC(IPC.RegisterChannel);
            packet.Write((byte)server);
            packet.Write((byte)channel);
            packet.Write(type);
            packet.Write(ip);
            packet.Write((ushort)port);
            packet.Write((ushort)maxPlayers);

            sync.Send(packet);
        }

        public static bool VerifyPassword(SyncReceiver sync, int id, string pass)
        {
            var packet = sync.CreateIPC(IPC.VerifyPassword);
            packet.Write(id);
            packet.Write(pass);
            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return false;

            var tmp = recv.ReadBoolean();

            return tmp;
        }

        public static void UpdateOnline(SyncReceiver sync, int id, bool online)
        {
            var packet = sync.CreateIPC(IPC.UpdateOnline);
            packet.Write(id);
            packet.Write(online);

            sync.Send(packet);
        }
    }
}
