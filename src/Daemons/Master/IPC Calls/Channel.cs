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
        void GetChannels(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var channels = sync.GetAllChannels();
            var packet = new IPCWriter(IPC.ChannelList);

            packet.Write(channels.Count);           // server count

            foreach (var server in channels)
            {
                packet.Write((int)server.Key);      // server id
                packet.Write(server.Value.Count);   // channel count

                foreach (var channel in server.Value)
                {
                    packet.Write(channel.Value.id);
                    packet.Write(channel.Value.type);
                    packet.Write((int)channel.Value.ip);
                    packet.Write(channel.Value.port);
                    packet.Write(channel.Value.maxPlayers);
                    packet.Write(channel.Value.curPlayers);
                }
            }

            receiver.Send(packet);
        }

        void AddChannel(IPCReceiver receiver, IPCReader data, SyncHandler sync)
        {
            var serverId = data.ReadByte();
            var channelId = data.ReadByte();
            var type = data.ReadInt32();
            var ip = data.ReadUInt32();
            var port = data.ReadInt16();
            var maxPlayers = data.ReadInt16();

            if (!sync.ServerExists(serverId))
                sync.AddServer(serverId);

            if (sync.ServerHasChannel(serverId, channelId))
                return;

            sync.AddServerChannel(serverId, channelId, new ChannelData(channelId, type, ip, port, maxPlayers, 0));
            receiver.SetServerInfo(serverId, channelId);

            if (!Configuration.serverDBs.ContainsKey(serverId))
            {
                Configuration.LoadMasterServer(serverId);

                Log.Message("Connecting to Database for Server " + serverId.ToString() + "...", Log.DefaultFG);
                var db = new DatabaseHandler(Configuration.serverDBTypes[serverId], Configuration.serverDBIPs[serverId], Configuration.serverDBs[serverId], Configuration.serverDBUsers[serverId], Configuration.serverDBPasses[serverId]);
                sync.AddServerDB(serverId, db);
            }
        }

        /*public bool UpdatePlayerCount(byte server, byte channel, short players)
        {
            lock (channels)
            {
                if (!channels.ContainsKey(server))
                    return false;

                var s = channels[server];

                if (!s.ContainsKey(channel))
                    return false;

                var c = s[channel];
                //s[channel] = new Channel(c.Item1, c.Item2, c.Item3, c.Item4, players);

                Log.Notice(String.Format("Player count updated for channel {0}, {1}", server, channel));

                return true;
            }
        }*/
    }
}
