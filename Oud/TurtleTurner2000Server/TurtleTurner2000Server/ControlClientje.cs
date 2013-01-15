using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleTurner2000Server
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
