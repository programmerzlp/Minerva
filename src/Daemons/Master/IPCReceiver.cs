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

using AwesomeSockets;
using System;
using System.Threading;

#endregion

namespace Minerva
{
    class IPCReceiver
    {
        ISocket socket;
        Thread rThread;
        AwesomeSockets.Buffer recvBuffer;
        IPCProtocol ipc;

        byte serverId;
        byte channelId;

        public IPCReceiver(ISocket socket, IPCProtocol ipc)
        {
            serverId = 0;
            channelId = 0;
            this.socket = socket;
            this.ipc = ipc;
            recvBuffer = AwesomeSockets.Buffer.New();
            rThread = new Thread(ReceiveThread);
            rThread.Start();
        }

        public void Send(IPCWriter writer)
        {
            var packet = writer.GetRawPacket();
            AwesomeSockets.Buffer.FinalizeBuffer(packet);
            SendIPC(packet);
        }

        bool SendIPC(AwesomeSockets.Buffer packet)
        {
            try
            {
                AweSock.SendMessage(socket, packet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void ReceiveThread()
        {
            while (true)
            {
                AwesomeSockets.Buffer.ClearBuffer(recvBuffer);
                try
                {
                    AweSock.ReceiveMessage(socket, recvBuffer);
                    var reader = new IPCReader(recvBuffer);
                    var packet = reader.Opcode;

                    ipc.HandlePacket(this, reader, packet);
                }
                catch (Exception)
                {
                    if (serverId > 0 && channelId > 0) 
                        ipc.RemoveChannel(serverId, channelId); // let's remove channel from list

                    rThread.Abort();
                }
            }
        }

        public void SetServerInfo(byte serverId, byte channelId)
        {
            this.serverId = serverId;
            this.channelId = channelId;
        }
    }
}
