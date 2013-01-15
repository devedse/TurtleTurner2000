using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurtleTurner2000.SharedEnums
{
    public enum ServerSendMessageType
    {
        //Base stuff
        SetScreenSize = 0, //int x, int y, int width, int height, followed by x, y, width, height for the total screen
        MapString = 1, //int width, int height, for each xrow a String, so keep doing Readline untill height

        //Spawn stuff
        SpawnNewPlayer = 100, //String id
        RemovePlayer = 101, //String id
        SetPlayerLocation = 102 //String id, int locationx, int locationy

    }
}
