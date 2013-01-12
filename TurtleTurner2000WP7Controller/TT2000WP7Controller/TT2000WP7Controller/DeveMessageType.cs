using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace TT2000WP7Controller
{
    public enum DeveMessageType : byte
    {
        KeepAlive = 0,
        Data = 1,
        StatusChanged = 2
    }

    public enum NetworkStatus : byte
    {
        Connected = 0,
        Disconnected = 1
    }
}
