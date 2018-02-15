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
    public class EventHandler
    {
        public delegate void IPCReceivedPacketHandler(object sender, IPCPacketEventArgs e);
        public delegate void IPCSentPacketHandler(object sender, IPCPacketEventArgs e);

        public delegate void ReceivedPacketHandler(object sender, PacketEventArgs e);
        public delegate void SentPacketHandler(object sender, PacketEventArgs e);
        public delegate void ErrorHandler(object sender, string message);

        public delegate void ClientConnectHandler(object sender, ConnectEventArgs e);
        public delegate void ClientDisconnectHandler(object sender, ClientHandler client);
        public delegate void LoginHandler(object sender, LoginEventArgs e);
        public delegate void VersionMismatchHandler(object sender, VersionCheckEventArgs e);

        public delegate void SayLocalHandler(object sender, int id, string name, string message);
        public delegate void SayChannelHandler(object sender, int id, string name, string message);
        public delegate void WarpHandler(object sender, ClientHandler client, int map, int x, int y);

        public event IPCReceivedPacketHandler OnIPCReceivePacket;
        public event IPCSentPacketHandler OnIPCSendPacket;

        public event ReceivedPacketHandler OnReceivePacket;
        public event SentPacketHandler OnSendPacket;
        public event ErrorHandler OnError;

        public event ClientConnectHandler OnClientConnect;
        public event ClientDisconnectHandler OnClientDisconnect;
        public event LoginHandler OnFailedLogin;
        public event LoginHandler OnSuccessfulLogin;
        public event VersionMismatchHandler OnVersionMismatch;

        public event SayLocalHandler OnSayLocal;
        public event SayChannelHandler OnSayChannel;
        public event WarpHandler OnWarp;

        public void IPCReceivedPacket(object sender, IPCPacketEventArgs e)
        { if (OnIPCReceivePacket != null) OnIPCReceivePacket(sender, e); }

        public void IPCSentPacket(object sender, IPCPacketEventArgs e)
        { if (OnIPCSendPacket != null) OnIPCSendPacket(sender, e); }

        public void ReceivedPacket(object sender, PacketEventArgs e)
            { if (OnReceivePacket != null) OnReceivePacket(sender, e); }

        public void SentPacket(object sender, PacketEventArgs e)
            { if (OnSendPacket != null) OnSendPacket(sender, e); }

        public void Error(object sender, string message)
            { if (OnError != null) OnError(sender, message); }

        public void ClientConnected(object sender, ConnectEventArgs e)
            { if (OnClientConnect != null) OnClientConnect(sender, e); }

        public void ClientDisconnected(object sender, ClientHandler client)
            { if (OnClientDisconnect != null) OnClientDisconnect(sender, client); }

        public void FailedLogin(object sender, LoginEventArgs e)
            { if (OnFailedLogin != null) OnFailedLogin(sender, e); }

        public void SuccessfulLogin(object sender, LoginEventArgs e)
            { if (OnSuccessfulLogin != null) OnSuccessfulLogin(sender, e); }

        public void VersionMismatch(object sender, VersionCheckEventArgs e)
            { if (OnVersionMismatch != null) OnVersionMismatch(sender, e); }

        public void SaidLocal(object sender, int id, string name, string message)
            { if (OnSayLocal != null) OnSayLocal(sender, id, name, message); }

        public void SaidChannel(object sender, int id, string name, string message)
            { if (OnSayChannel != null) OnSayChannel(sender, id, name, message); }

        public void Warped(object sender, ClientHandler client, int map, int x, int y)
            { if (OnWarp != null) OnWarp(sender, client, map, x, y); }
    }
}