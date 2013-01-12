using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeveConnecteuze.Network
{
    public enum DeveMessageTypeStreamSocket : byte
    {
        KeepAlive = 0,
        Data = 1,
        StatusChanged = 2
    }

    public enum NetworkStatusStreamSocket : byte
    {
        Connected = 0,
        Disconnected = 1
    }
}
