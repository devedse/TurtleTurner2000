using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurtleTurner2000.Server
{
    public class Clientje
    {
        public DeveConnection deveConnection;
        public String guid;

        public Clientje(DeveConnection deveConnection)
        {
            this.deveConnection = deveConnection;
            guid = Guid.NewGuid().ToString();
        }
    }
}
