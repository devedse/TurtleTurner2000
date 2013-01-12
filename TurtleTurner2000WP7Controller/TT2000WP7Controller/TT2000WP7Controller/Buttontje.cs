using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace TT2000WP7Controller
{
    class Buttontje
    {
        private Game1 game;
        private String stringtosend;
        private Rectangle pos;
        private Texture2D texture;
        private Color c;
        private SpriteFont font;

        private Boolean pressed = false;

        public Buttontje(Game1 game, String stringtosend, Rectangle pos, Color c, SpriteFont font)
        {
            this.game = game;
            this.stringtosend = stringtosend;
            this.pos = pos;
            this.c = c;
            Texture2D pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            this.texture = pixel;
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
            game.SendMessage(stringtosend);
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
