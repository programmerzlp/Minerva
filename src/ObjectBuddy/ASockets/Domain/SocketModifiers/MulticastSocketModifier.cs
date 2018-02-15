using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AwesomeSockets
{
    public class MulticastSocketModifier : ISocketModifier
    {
        //These are RFC included values
        private readonly IPAddress multicastIPAddress = IPAddress.Parse("224.168.100.2");
        
        public ISocket Apply(ISocket socket)
        {
            //TODO: Need to check if socket is UDP as Multicast only works with UDP...
            AweSock.SetSockOpt(socket, new Dictionary<SocketOptionName, object>
                {
                    { SocketOptionName.AddMembership, new MulticastOption(multicastIPAddress, IPAddress.Parse("127.0.0.1")) }
                });
            return socket;
        }
    }
}
