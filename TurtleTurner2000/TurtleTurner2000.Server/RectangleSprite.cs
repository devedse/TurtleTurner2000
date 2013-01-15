using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurtleTurner2000.Server
{
    class RectangleSprite
    {
        private Game1 game;
        public Rectangle rect;
        private int width;



        public RectangleSprite(Rectangle rect, int width, Game1 game)
        {
            this.game = game;
            this.rect = rect;
            this.width = width;
        }

        public void Update(GameTime gameTime)
        {
            //var state = Mouse.GetState();

            //var mousepos = new Rectangle(state.X, state.Y, 1, 1);

            //rect.X = mousepos.X;
            //rect.Y = mousepos.Y;

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color alphared = Color.Red;
            //alphared.A = 20;

            Texture2D pixel = game.pixelTexture;

            //spriteBatch.Draw(pixel, rect, Color.Green);
            spriteBatch.Draw(pixel, new Rectangle(rect.X - width, rect.Y, width, rect.Height), alphared); //Links
            spriteBatch.Draw(pixel, new Rectangle(rect.X - width, rect.Y - width, rect.Width + 2 * width, width), alphared); //Top
            spriteBatch.Draw(pixel, new Rectangle(rect.X + rect.Width, rect.Y, width, rect.Height), alphared); //Rechts
            spriteBatch.Draw(pixel, new Rectangle(rect.X - width, rect.Y + rect.Height, rect.Width + 2 * width, width), alphared); //Bottom

        }
    }
}
