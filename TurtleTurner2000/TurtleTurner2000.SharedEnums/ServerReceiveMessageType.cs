using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurtleTurner2000.SharedEnums
{
    public enum ServerReceiveMessageType
    {
        //0 - 100 are messages from a screen client
        //The rest are messages from a controller

        LoginMessageScreenClient = 0, //int screenWidth, int screenHeight
        LoginMessageControlClient = 1, //Lalala



        NewButtonState = 100 //string direction, boolean on or off
    }
}
