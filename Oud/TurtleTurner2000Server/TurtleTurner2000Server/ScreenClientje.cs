using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleTurner2000Server
{
    class ScreenClientje : Clientje
    {
        public int numberOfScreen;

        public ScreenClientje(DeveConnection deveConnection, int numberOfScreen)
            : base(deveConnection)
        {
            this.numberOfScreen = numberOfScreen;
        }
    }
}
