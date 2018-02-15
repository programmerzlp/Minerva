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

using System.Collections.Generic;

#endregion

namespace Minerva
{
    partial class SyncHandler
    {
        SortedDictionary<byte, SortedDictionary<byte, ChannelData>> channels;

        public void SetChannelsData(SortedDictionary<byte, SortedDictionary<byte, ChannelData>> channels)
        {
            this.channels = channels;
        }

        public bool ServerExists(byte serverId)
        {
            lock (channels)
            {
                return channels.ContainsKey(serverId);
            }
        }

        public void AddServer(byte serverId)
        {
            lock (channels)
            {
                channels.Add(serverId, new SortedDictionary<byte, ChannelData>());
            }
        }

        public bool ServerHasChannel(byte serverId, byte channelId)
        {
            lock (channels)
            {
                return channels[serverId].ContainsKey(channelId);
            }
        }

        public void AddServerChannel(byte serverId, byte channelId, ChannelData channelData)
        {
            lock (channels)
            {
                channels[serverId].Add(channelId, channelData);
                Log.Notice(string.Format("Channel registered: {0}, {1}", serverId, channelId));
            }
        }

        public SortedDictionary<byte, SortedDictionary<byte, ChannelData>> GetAllChannels()
        {
            lock (channels)
            {
                return channels;
            }
        }

        public void RemoveChannel(byte serverId, byte channelId)
        {
            lock (channels)
            {
                if (!channels.ContainsKey(serverId))
                    return;

                if (!channels[serverId].ContainsKey(channelId))
                    return;

                channels[serverId].Remove(channelId);

                if (channels[serverId].Count == 0)
                    channels.Remove(serverId);
            }
        }
    }
}