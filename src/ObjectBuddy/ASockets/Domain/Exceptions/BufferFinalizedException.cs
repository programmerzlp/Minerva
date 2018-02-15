using System;

namespace AwesomeSockets
{
    public class BufferFinalizedException : Exception
    {
        public BufferFinalizedException(string message) : base(message)
        {
            //here just for chained constructor
        }

    }
}
