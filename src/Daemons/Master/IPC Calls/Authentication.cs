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

namespace Minerva
{
    partial class IPCProtocol
    {
        void HeartBeat(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var packet = new IPCWriter(IPC.HeartBeat);
            receiver.Send(packet);
        }

        void AddUser(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var magic = data.ReadUInt64();
            var id = data.ReadInt32();

            if (!sync.UserExists(magic))
                sync.AddUser(magic, id);
        }

        void GetUser(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var magic = data.ReadUInt64();
            var packet = new IPCWriter(IPC.GetUser);

            if (!sync.UserExists(magic))
            {
                packet.Write(-1);
                receiver.Send(packet);
                return;
            }

            var id = sync.GetUser(magic);
            packet.Write(id);
            receiver.Send(packet);
        }
    }
}
