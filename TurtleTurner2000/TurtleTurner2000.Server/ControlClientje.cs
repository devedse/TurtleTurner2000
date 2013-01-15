using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurtleTurner2000.Server
{
    class ControlClientje : Clientje
    {
        public int posx = 0;
        public int posy = 0;

        public ControlClientje(DeveConnection deveConnection)
            : base(deveConnection)
        {

        }


    }
}
