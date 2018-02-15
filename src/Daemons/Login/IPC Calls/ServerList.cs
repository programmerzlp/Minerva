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
    class ServerList
    {
        public static ServerData[] GetChannels(SyncReceiver sync)
        {
            var packet = sync.CreateIPC(IPC.ChannelList);
            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return null;

            int num = recv.ReadInt32();                     // im pretty sure there won't be more than 2 147 483 647 servers and/or channels out there
                                                            // perhaps, change it to byte ..?

            if (num < 0)
                return null;

            var serverList = new ServerData[num];

            for (int i = 0; i < num; i++)
            {
                int serverId = recv.ReadInt32();            // should be replaced with byte
                int channelLength = recv.ReadInt32();       // same thing as with server count variable

                if (serverId < 0)
                    continue;

                serverList[i] = new ServerData(serverId, channelLength);

                for (int j = 0; j < channelLength; j++)
                {
                    int id = recv.ReadInt32();              // should be replaced with byte
                    int type = recv.ReadInt32();            // hmmm... channel type is ulong?
                    uint ip = recv.ReadUInt32();    
                    short port = recv.ReadInt16();          // i think port should be ushort
                    short maxPlayers = recv.ReadInt16();    // should be changed to ushort
                    short curPlayers = recv.ReadInt16();    // should be changed to ushort

                    var chd = new ChannelData(id, type, ip, port, maxPlayers, curPlayers);
                    serverList[i].channels[j] = chd;
                }
            }

            return serverList;
        }
    }
}
