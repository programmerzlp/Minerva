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
using System.Threading;
using AwesomeSockets;

#endregion

namespace Minerva
{
    delegate void IPCPacketMethod(IPCReceiver receiver, IPCReader data, SyncHandler sync);

    partial class IPCProtocol
    {
        Dictionary<IPC, IPCPacketMethod> ipcPackets;

        ISocket syncServer;
        Thread syncThread;
        SyncHandler syncHandler;

        string ip;
        int port;

        public IPCProtocol(string ip, int port, SyncHandler sync)
        {
            this.ip = ip;
            this.port = port;
            syncHandler = sync;

            Log.Message("Registering packets...", Log.DefaultFG);
            RegisterPackets();
        }

        public void StartSync()
        {
            syncThread = new Thread(Listen);
            syncThread.Start();
        }

        void RegisterPackets()
        {
            ipcPackets = new Dictionary<IPC, IPCPacketMethod>();

            // shared
            ipcPackets.Add(IPC.HeartBeat, HeartBeat);
            ipcPackets.Add(IPC.AddUser, AddUser);
            ipcPackets.Add(IPC.GetUser, GetUser);

            // login
            ipcPackets.Add(IPC.FetchAccount, FetchAccount);
            ipcPackets.Add(IPC.UpdateIPDate, UpdateIPDate);
            ipcPackets.Add(IPC.UpdateOnline, UpdateOnline);

            // channel
            ipcPackets.Add(IPC.ChannelList, GetChannels);
            ipcPackets.Add(IPC.RegisterChannel, AddChannel);

            // subpass
            ipcPackets.Add(IPC.GetSubPass, GetSubPass);
            ipcPackets.Add(IPC.SetSubPass, SetSubPass);
            ipcPackets.Add(IPC.GetSubPassQuestion, GetSubPassQuestion);
            ipcPackets.Add(IPC.CheckSubPassAnswer, CheckSubPassAnswer);
            ipcPackets.Add(IPC.CheckSubPass, CheckSubPass);
            ipcPackets.Add(IPC.RemoveSubPass, RemoveSubPass);
            ipcPackets.Add(IPC.GetSubPassTime, GetSubPassTime);
            ipcPackets.Add(IPC.SetSubPassTime, SetSubPassTime);

            // character
            ipcPackets.Add(IPC.GetCharacterList, CharacterList);
            ipcPackets.Add(IPC.CreateCharacter, CreateCharacter);
            ipcPackets.Add(IPC.DeleteCharacter, DeleteCharacter);
            ipcPackets.Add(IPC.VerifyPassword, VerifyPassword);
            ipcPackets.Add(IPC.UpdatePosition, UpdateCharacterPosition);
            ipcPackets.Add(IPC.GetFullCharacter, GetFullCharacter);
            ipcPackets.Add(IPC.GetSlotOrder, GetSlotOrder);
            ipcPackets.Add(IPC.SetSlotOrder, SetSlotOrder);
            ipcPackets.Add(IPC.SetQuickSlots, SetQuickSlots);
            ipcPackets.Add(IPC.UpdateStatPoints, UpdateStatPoints);
            ipcPackets.Add(IPC.UpdateSkillPoints, UpdateSkillPoints);
            ipcPackets.Add(IPC.GetCashItemList, GetCashItemList);
            ipcPackets.Add(IPC.SetCashItem, SetCashItem);
        }

        public void HandlePacket(IPCReceiver receiver, IPCReader data, IPC opcode)
        {
            if (ipcPackets.ContainsKey(opcode))
                ipcPackets[opcode].Invoke(receiver, data, syncHandler);
            else
                Log.Error("Unknown IPC received: " + opcode, "[Master::IPCProtocol::HandlePacket()]");
        }

        void Listen()
        {
            syncServer = AweSock.TcpListen(port);
            Thread.Sleep(3000);
            Log.Notice("Master Server is listening on {0}:{1}", ip, port);

            while (true)
            {
                var client = AweSock.TcpAccept(syncServer);
                var receiver = new IPCReceiver(client, this);
            }
        }

        public void RemoveChannel(byte serverId, byte channelId)
        {
            syncHandler.RemoveChannel(serverId, channelId);
        }
    }
}