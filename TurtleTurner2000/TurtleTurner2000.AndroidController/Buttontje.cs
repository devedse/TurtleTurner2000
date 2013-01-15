using System;
using System.Collections.Generic;
using System.Text;
using DeveConnecteuze.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using TurtleTurner2000.SharedEnums;

namespace TurtleTurner2000AndroidController
{
    class Buttontje
    {
        private DeveClient deveClient;
        private String stringtosend;
        private Rectangle pos;
        private Texture2D texture;
        private Color c;
        private SpriteFont font;

        private Boolean pressed = false;

        public Buttontje(DeveClient deveClient, String stringtosend, Rectangle pos, Color c, Texture2D texture, SpriteFont font)
        {
            this.deveClient = deveClient;
            this.stringtosend = stringtosend;
            this.pos = pos;
            this.c = c;
            this.texture = texture;
            this.font = font;
        }

        public void Update(TouchCollection currentTouchCollection)
        {
            Boolean previouslyPressed = pressed;

            pressed = false;
            foreach (var v in currentTouchCollection)
            {
                if (v.State == TouchLocationState.Moved || v.State == TouchLocationState.Pressed)
                {
                    Rectangle r = new Rectangle((int)v.Position.X, (int)v.Position.Y, 1, 1);
                    if (r.Intersects(pos))
                    {
                        pressed = true;
                        break;
                    }
                }
            }

            if (previouslyPressed != pressed)
            {
                SendMeClicking();
            }
        }

        public void SendMeClicking()
        {
            DeveOutgoingMessage outje = new DeveOutgoingMessage();
            outje.WriteInt32((int)ServerReceiveMessageType.NewButtonState); //Identifier for command message
            outje.WriteString(stringtosend);
            outje.WriteString(pressed.ToString());
            deveClient.Send(outje);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (pressed)
            {
                int rr = c.R + 50;
                int gg = c.G + 50;
                int bb = c.B + 50;

                if (rr > 255)
                {
                    rr = c.R - 30;
                }
                if (gg > 255)
                {
                    gg = c.G - 30;
                }
                if (bb > 255)
                {
                    bb = c.B - 30;
                }

                Color pressedColor = new Color(rr, gg, bb);
                spriteBatch.Draw(texture, pos, pressedColor);

            }
            else
            {
                spriteBatch.Draw(texture, pos, c);
            }

            spriteBatch.DrawString(font, stringtosend, new Vector2(pos.X + 20, pos.Y + 20), Color.White);
        }
    }
}
