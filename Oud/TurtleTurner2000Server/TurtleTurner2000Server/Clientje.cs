﻿using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleTurner2000Server
{
    class Clientje
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
