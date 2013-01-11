#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using DeveConnecteuze.Network;
using System.Threading;
#endregion
namespace TurtleTurner2000
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D squirtleTexture;
        List<Squirtle> squirtles;
        Texture2D charmanderTexture;
        List<Charmander> charmanders;

        String octopeusjeToBe = "Skarner skar";

        Rectangle totSize;
        Rectangle curSize;

        DeveClient deveClient;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            //deveClient = new DeveClient("192.168.2.11", 1337);
            deveClient = new DeveClient("10.33.184.220", 1337);
            deveClient.Start();
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            deveClient.Stop();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            squirtles = new List<Squirtle>();
            charmanders = new List<Charmander>();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            squirtleTexture = this.Content.Load<Texture2D>("skwirtle");
            charmanderTexture = this.Content.Load<Texture2D>("charmander2");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //squirtles.Add(new Squirtle(new Vector2(1, 1), Vector2.Zero));

            if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                this.graphics.IsFullScreen = !this.graphics.IsFullScreen;
                this.graphics.ApplyChanges();
                Thread.Sleep(100);
            }

            //Update deveClient
            DeveIncomingMessage inc;
            while ((inc = deveClient.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case DeveMessageType.KeepAlive:
                        break;
                    case DeveMessageType.Data:
                        int messageDinges = inc.ReadInt32();
                        if (messageDinges == 0)
                        {
                            totSize = new Rectangle();
                            curSize = new Rectangle();

                            totSize.X = inc.ReadInt32();
                            totSize.Y = inc.ReadInt32();
                            totSize.Width = inc.ReadInt32();
                            totSize.Height = inc.ReadInt32();

                            curSize.X = inc.ReadInt32();
                            curSize.Y = inc.ReadInt32();
                            curSize.Width = inc.ReadInt32();
                            curSize.Height = inc.ReadInt32();
                        }
                        else if (messageDinges == 1)
                        {
                            int xPos = inc.ReadInt32();
                            int yPos = inc.ReadInt32();
                            int xDir = inc.ReadInt32();
                            int yDir = inc.ReadInt32();
                            squirtles.Add(new Squirtle(new Vector2((float)xDir / 1000.0f, (float)yDir / 1000.0f), new Vector2(xPos, yPos)));
                            charmanders.Add(new Charmander(new Vector2((float)xDir * 2 / 1000.0f, (float)yDir * 2 / 1000.0f), new Vector2(xPos + 50, yPos + 50)));
                        }
                        else if (messageDinges == 2)
                        {
                            foreach (Squirtle squirtle in squirtles)
                            {
                                squirtle.Direction += new Vector2(1, 0);
                                squirtle.Direction.Normalize();
                            }
                            foreach (Charmander charmander in charmanders)
                            {
                                charmander.Direction += new Vector2(2, 0);
                                charmander.Direction.Normalize();
                            }
                        }
                        break;
                    case DeveMessageType.StatusChanged:
                        break;
                    default:
                        break;
                }
            }

            //SquirTOLS
            List<Squirtle> removeSquirtles = new List<Squirtle>();
            foreach (Squirtle squirtle in squirtles)
            {
                squirtle.Update(gameTime);
                if (squirtle.Position.X < totSize.X - squirtleTexture.Width / 2 ||
                    squirtle.Position.X > totSize.X + totSize.Width + squirtleTexture.Width / 2 ||
                    squirtle.Position.Y < totSize.Y - squirtleTexture.Height / 2 ||
                    squirtle.Position.Y > totSize.Y + totSize.Height + squirtleTexture.Height / 2)
                {
                    removeSquirtles.Add(squirtle);
                }
            }
            foreach (Squirtle squirtle in removeSquirtles)
            {
                squirtles.Remove(squirtle);
            }
            removeSquirtles.Clear();

            //Ch0rMovies aka charmenter
            List<Charmander> removeCharmanders = new List<Charmander>();
            foreach (Charmander charmander in charmanders)
            {
                charmander.Update(gameTime);
                if (charmander.Position.X < totSize.X - charmanderTexture.Width / 2 ||
                    charmander.Position.X > totSize.X + totSize.Width + charmanderTexture.Width / 2 ||
                    charmander.Position.Y < totSize.Y - charmanderTexture.Height / 2 ||
                    charmander.Position.Y > totSize.Y + totSize.Height + charmanderTexture.Height / 2)
                {
                    removeCharmanders.Add(charmander);
                }
            }
            foreach (Charmander charmander in removeCharmanders)
            {
                charmanders.Remove(charmander);
            }
            removeCharmanders.Clear();

            base.Update(gameTime);
        }

        //public void SetRandomVectors()
        //{
        //    int edge = random.Next(0, 4);
        //    int randomX = 0;
        //    int randomY = 0;
        //    switch (edge)
        //    {
        //        //top
        //        case 0:
        //            randomX = random.Next(0, graphics.PreferredBackBufferWidth);
        //            randomY -= squirtleTexture.Height / 2;
        //            direction = new Vector2(GetDirectionFloat(), 1);
        //            break;
        //        //right
        //        case 1:
        //            randomX = graphics.PreferredBackBufferWidth;
        //            randomY = random.Next(0, graphics.PreferredBackBufferHeight);
        //            randomX += squirtleTexture.Width / 2;
        //            direction = new Vector2(-1, GetDirectionFloat());
        //            break;
        //        //bot
        //        case 2:
        //            randomX = random.Next(0, graphics.PreferredBackBufferWidth);
        //            randomY = graphics.PreferredBackBufferHeight;
        //            randomY += squirtleTexture.Height / 2;
        //            direction = new Vector2(GetDirectionFloat(), -1);
        //            break;
        //        //left
        //        case 3:
        //            randomY = random.Next(0, graphics.PreferredBackBufferHeight);
        //            randomX -= squirtleTexture.Width / 2;
        //            direction = new Vector2(1, GetDirectionFloat());
        //            break;
        //    }

        //    position = new Vector2(randomX, randomY);
        //}

        //public float GetDirectionFloat()
        //{
        //    return (float)random.Next(-1000, 1001) / 1000;
        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (Squirtle squirtle in squirtles)
            {
                //if (squirtle.Position.X > curSize.X &&
                //    squirtle.Position.X < curSize.X + curSize.Width &&
                //    squirtle.Position.Y > curSize.Y &&
                //    squirtle.Position.Y < curSize.Y + curSize.Height)
                //{
                Vector2 newPos = new Vector2(squirtle.Position.X - curSize.X, squirtle.Position.Y - curSize.Y);
                spriteBatch.Draw(squirtleTexture, newPos,
                                        squirtleTexture.Bounds, Color.White,
                                        squirtle.Rotation, new Vector2(squirtleTexture.Width / 2, squirtleTexture.Height / 2),
                                        1.0f, SpriteEffects.None, 1.0f);
                //}
            }

            foreach (Charmander charmander in charmanders)
            {
                //if (squirtle.Position.X > curSize.X &&
                //    squirtle.Position.X < curSize.X + curSize.Width &&
                //    squirtle.Position.Y > curSize.Y &&
                //    squirtle.Position.Y < curSize.Y + curSize.Height)
                //{
                Vector2 newPos = new Vector2(charmander.Position.X - curSize.X, charmander.Position.Y - curSize.Y);
                spriteBatch.Draw(charmanderTexture, newPos,
                                        charmanderTexture.Bounds, Color.White,
                                        charmander.Rotation, new Vector2(charmanderTexture.Width / 2, charmanderTexture.Height / 2),
                                        1.0f, SpriteEffects.None, 1.0f);
                //}
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
