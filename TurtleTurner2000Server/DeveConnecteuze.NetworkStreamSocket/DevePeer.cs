using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeveConnecteuze.Network
{
    public abstract class DevePeerStreamSocket
    {
        protected DeveQueueStreamSocket<DeveIncomingMessageStreamSocket> messages = new DeveQueueStreamSocket<DeveIncomingMessageStreamSocket>(100);

        internal int maxMessageSize = 100000;
        public int MaxMessageSize
        {
            get { return maxMessageSize; }
            set { maxMessageSize = value; }
        }

        public DevePeerStreamSocket()
        {

        }

        public DeveIncomingMessageStreamSocket ReadMessage()
        {
            if (messages.Count == 0)
            {
                return null;
            }
            else
            {
                DeveIncomingMessageStreamSocket retval;
                Boolean didItWork = messages.TryDequeue(out retval);
                if (!didItWork)
                {
                    throw new Exception("Strange error");
                }
                return retval;
            }
        }

        internal void AddDeveIncomingMessage(DeveIncomingMessageStreamSocket devInc)
        {
            messages.Enqueue(devInc);
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
