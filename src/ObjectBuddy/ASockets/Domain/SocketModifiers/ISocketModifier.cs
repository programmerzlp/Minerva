using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AwesomeSockets
{
    public interface ISocketModifier
    {
        ISocket Apply(ISocket socket);
    }
}
