using DeveConnecteuze.Network;
using FarseerPhysics.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TurtleTurner2000.SharedEnums;

namespace TurtleTurner2000.Server
{
    public class ScreenClientje : Clientje
    {
        public Rectangle rect;
        public Rectangle rectToDraw;
        private RectangleSprite rectSprite;

        private Game1 game;

        public Boolean dragging = false;

        public ScreenClientje(Rectangle rect, Game1 game, DeveConnection deveConnection)
            : base(deveConnection)
        {
            this.rect = rect;
            this.game = game;
            rectSprite = new RectangleSprite(rect, 5, game);

            SendNewPos();
        }

        public void Update(GameTime gameTime)
        {
            Vector2 adjustedMousePos = game.Camera.ConvertScreenToWorld(new Vector2(game.currentMouseState.X, game.currentMouseState.Y)) * 100;
            Vector2 oldAdjustedMousePos = game.Camera.ConvertScreenToWorld(new Vector2(game.previousMouseState.X, game.previousMouseState.Y)) * 100;
            Rectangle mousePos = new Rectangle((int)adjustedMousePos.X, (int)adjustedMousePos.Y, 1, 1);
            Vector2 rectMove = Vector2.Zero;
            //Console.WriteLine(adjustedMousePos.X + " - " + rect.X + " --- " + adjustedMousePos.Y + " - " + rect.Y);
            if (game.currentMouseState.LeftButton == ButtonState.Pressed && game.previousMouseState.LeftButton == ButtonState.Released && rect.Intersects(mousePos))
            {
                dragging = true;
            }
            if (game.currentMouseState.LeftButton == ButtonState.Released && game.previousMouseState.LeftButton == ButtonState.Pressed)
            {
                dragging = false;
            }
            if (dragging)
            {
                rectMove = adjustedMousePos - oldAdjustedMousePos;
                SendNewPos();
            }
            this.rect.X += (int)rectMove.X;
            this.rect.Y += (int)rectMove.Y;


            //if (dragging)
            //{
            //    Vector2 mouseVect = game.Camera.ConvertScreenToWorld(new Vector2(game.currentMouseState.X - game.previousMouseState.X, game.currentMouseState.Y - game.previousMouseState.Y));
            //    this.rect.X += (int)mouseVect.X;
            //    this.rect.Y += (int)mouseVect.Y;

            //    SendNewPos();
            //}

            //if (game.currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            //{
            //    dragging = false;
            //}

            //if (rect.Intersects(mousePos) && game.currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && game.previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            //{
            //    dragging = true;
            //}

            rectToDraw = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            rectSprite.rect = rectToDraw;
            rectSprite.Update(gameTime);

        }

        public void SendNewPos()
        {
            DeveOutgoingMessage outje = new DeveOutgoingMessage();
            outje.WriteInt32((int)ServerSendMessageType.SetScreenSize);
            outje.WriteInt32(rect.X);
            outje.WriteInt32(rect.Y);
            outje.WriteInt32(rect.Width);
            outje.WriteInt32(rect.Height);

            outje.WriteInt32(game.totSize.X);
            outje.WriteInt32(game.totSize.Y);
            outje.WriteInt32(game.totSize.Width);
            outje.WriteInt32(game.totSize.Height);
            deveConnection.Send(outje);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D pixel = game.pixelTexture;

            if (dragging)
            {
                spriteBatch.Draw(pixel, rectToDraw, Color.Green);
            }
            else
            {
                spriteBatch.Draw(pixel, rectToDraw, Color.Blue);

            }

            rectSprite.Draw(gameTime, spriteBatch);

        }
    }
}
