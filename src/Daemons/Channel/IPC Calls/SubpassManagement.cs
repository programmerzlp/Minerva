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
    class SubpassManagement
    {
        public static bool GetSubpass(SyncReceiver sync, int id)
        {
            var packet = sync.CreateIPC(IPC.GetSubPass);
            packet.Write(id);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return false;

            var sub = recv.ReadBoolean();

            return sub;
        }

        public static void SetSubPass(SyncReceiver sync, int id, string subpw, int question, string answer)
        {
            var packet = sync.CreateIPC(IPC.SetSubPass);
            packet.Write(id);
            packet.Write(subpw);
            packet.Write((byte)question);
            packet.Write(answer);

            sync.Send(packet);
        }

        public static int GetSubPassQuestion(SyncReceiver sync, int id)
        {
            var packet = sync.CreateIPC(IPC.GetSubPassQuestion);
            packet.Write(id);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return -1;

            var sub = recv.ReadByte();

            return sub;
        }

        public static bool CheckSubPwAnswer(SyncReceiver sync, int id, string answer)
        {
            var packet = sync.CreateIPC(IPC.CheckSubPassAnswer);
            packet.Write(id);
            packet.Write(answer);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return false;

            var sub = recv.ReadBoolean();

            return sub;
        }

        public static bool CheckSubPw(SyncReceiver sync, int id, string pass)
        {
            var packet = sync.CreateIPC(IPC.CheckSubPass);
            packet.Write(id);
            packet.Write(pass);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return false;

            var sub = recv.ReadBoolean();

            return sub;
        }

        public static void RemoveSubPass(SyncReceiver sync, int id)
        {
            var packet = sync.CreateIPC(IPC.RemoveSubPass);
            packet.Write(id);

            sync.Send(packet);
        }

        public static bool GetSubPassTime(SyncReceiver sync, int id)
        {
            var packet = sync.CreateIPC(IPC.GetSubPassTime);
            packet.Write(id);

            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return false;

            var sub = recv.ReadBoolean();

            return sub;
        }

        public static void SetSubPassTime(SyncReceiver sync, int id, int time)
        {
            var packet = sync.CreateIPC(IPC.SetSubPassTime);
            packet.Write(id);
            packet.Write((byte)time);

            sync.Send(packet);
        }
    }
}
