using DeveConnecteuze.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TurtleTurner2000.SharedEnums;

namespace TurtleTurner2000.Server
{
    class ControlClientje : Clientje
    {
        public float posx = 0;
        public float posy = 0;

        public float xSpeed = 0;
        public float ySpeed = 0;

        private bool dirty = false;

        private bool leftArrow = false;
        public bool LeftArrow
        {
            get { return leftArrow; }
            set { leftArrow = value; dirty = true; }
        }
        private bool rightArrow = false;
        public bool RightArrow
        {
            get { return rightArrow; }
            set { rightArrow = value; dirty = true; }
        }
        private bool upArrown = false;
        public bool UpArrown
        {
            get { return upArrown; }
            set { upArrown = value; dirty = true; }
        }
        private bool downArrow = false;
        public bool DownArrow
        {
            get { return downArrow; }
            set { downArrow = value; dirty = true; }
        }

        private Game1 game;

        public ControlClientje(DeveConnection deveConnection, Game1 game)
            : base(deveConnection)
        {
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            xSpeed = 0f;
            ySpeed = 0f;

            if (leftArrow)
            {
                xSpeed += -300f;
            }
            if (rightArrow)
            {
                xSpeed += 300f;
            }
            if (upArrown)
            {
                ySpeed += -300f;
            }
            if (downArrow)
            {
                ySpeed += 300f;
            }

            posx += xSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            posy += ySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (dirty)
            {


                DeveOutgoingMessage outje = new DeveOutgoingMessage();
                outje.WriteInt32((int)ServerSendMessageType.SetPlayerLocation);
                outje.WriteString(guid);
                outje.WriteInt32((int)posx);
                outje.WriteInt32((int)posy);
                outje.WriteInt32((int)xSpeed);
                outje.WriteInt32((int)ySpeed);

                game.SendToScreens(outje);

                dirty = false;
            }
        }


    }
}
