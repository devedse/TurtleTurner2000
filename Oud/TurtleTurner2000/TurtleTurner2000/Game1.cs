﻿#region Using Statements
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
using System.Net;
using System.IO;
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

        Texture2D charmanderTexture;

        Texture2D bulbasaurTexture;
        Texture2D venubronkiTexture;
        Texture2D venubrateuzadroidTexture;

        Texture2D myssignuvskitrabovTexture;

        //List<Brokemon> brokemons;
        Dictionary<string, Brokemon> brokemons2;

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

            String ip = "";
            WebRequest req = WebRequest.Create("https://dl.dropbox.com/u/1814002/TurtleTurner2000/ip.txt");
            WebResponse resp = req.GetResponse();
            using (Stream streampje = resp.GetResponseStream())
            {
                using (TextReader reader = new StreamReader(streampje))
                {
                    ip = reader.ReadLine();
                }
            }
            //deveClient = new DeveClient("localhost", 1337);
            deveClient = new DeveClient(ip, 1337);
            deveClient.Start();

            DeveOutgoingMessage outje = new DeveOutgoingMessage();
            outje.WriteInt32(0);
            outje.WriteInt32(0);
            outje.WriteInt32(graphics.PreferredBackBufferWidth);
            outje.WriteInt32(graphics.PreferredBackBufferHeight);
            deveClient.Send(outje);
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
            //brokemons = new List<Brokemon>();
            brokemons2 = new Dictionary<string, Brokemon>();
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

            bulbasaurTexture = this.Content.Load<Texture2D>("brotasuar");
            venubronkiTexture = this.Content.Load<Texture2D>("venubronki");
            venubrateuzadroidTexture = this.Content.Load<Texture2D>("venubrateuzadroid");

            myssignuvskitrabovTexture = this.Content.Load<Texture2D>("myssignuvskitrabov");
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
                        int messageDinges = inc.ReadInt32(); //Type van de Data message. 0 = your screen position, 1 = monsterspawn, 2 = monster direction gieben

                        string IDString = ""; //ID of the brokemonzo, only used in case 1, 2 and 3, not 0. NOT 0 ;)

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
                        else if (messageDinges == 1) //Spawn a brokejoid
                        {
                            IDString = inc.ReadString(); //ID van het brokemonnetje

                            int xPos = inc.ReadInt32();
                            int yPos = inc.ReadInt32();
                            int xDir = inc.ReadInt32();
                            int yDir = inc.ReadInt32();
                            int brokeSpecies = inc.ReadInt32(); //0 = squirtle, 1 = charmander, 2 = bulbasaur, 10 = missingno

                            Texture2D[] brokemonTexture = new Texture2D[3];
                            //int rInt = 0;
                            //Random r = new Random();
                            //rInt = r.Next(3);

                            switch (brokeSpecies)
                            {
                                case 0:
                                    brokemonTexture[0] = squirtleTexture;
                                    brokemonTexture[1] = squirtleTexture;//replace plx
                                    brokemonTexture[2] = squirtleTexture;//replace plx
                                    break;
                                case 1:
                                    brokemonTexture[0] = charmanderTexture;
                                    brokemonTexture[1] = charmanderTexture;//replace plx
                                    brokemonTexture[2] = charmanderTexture;//replace plx
                                    break;
                                case 2:
                                    brokemonTexture[0] = bulbasaurTexture;
                                    brokemonTexture[1] = venubronkiTexture;
                                    brokemonTexture[2] = venubrateuzadroidTexture;
                                    break;
                                default:
                                    Console.WriteLine("Invalid selection. Please select 0, 1, or 2.");
                                    brokemonTexture[0] = myssignuvskitrabovTexture;
                                    brokemonTexture[1] = myssignuvskitrabovTexture;
                                    brokemonTexture[2] = myssignuvskitrabovTexture;
                                    break;
                            }
                            //if (r.Next(15) == 10)
                            //{
                            //    brokemonTexture[0] = myssignuvskitrabovTexture;
                            //    brokemonTexture[1] = myssignuvskitrabovTexture;
                            //    brokemonTexture[2] = myssignuvskitrabovTexture;
                            //}
                            brokemons2.Add(IDString, new Brokemon(new Vector2((float)xDir * 1f / 1000.0f, (float)yDir * 1f / 1000.0f), new Vector2(xPos, yPos), brokemonTexture, spriteBatch, IDString));
                        }
                        else if (messageDinges == 2) //Delete the brokestamuv
                        {
                            IDString = inc.ReadString(); //ID van het brokemonnetje

                            brokemons2.Remove(IDString);
                        }
                        else if (messageDinges == 3)
                        {
                            IDString = inc.ReadString(); //ID van het brokemonnetje
                            //SetLocation Broketje
                            int xPos = inc.ReadInt32();
                            int yPos = inc.ReadInt32();
                            Brokemon brokemon = brokemons2[IDString];
                            brokemon.Position = new Vector2(xPos, yPos);
                        }
                        break;
                    case DeveMessageType.StatusChanged:
                        break;
                    default:
                        break;
                }
            }

            //Brokemons vanilla set update calls
            List<Brokemon> removeBrokemons = new List<Brokemon>();
            foreach (Brokemon brokemon in brokemons2.Values)
            {
                int el = brokemon.EvolutionStage;
                brokemon.Update(gameTime);

                //Check if brokemon goes out of screen.
                //    if (brokemon.Position.X < totSize.X - brokemon.Textures[el].Width / 2 ||
                //        brokemon.Position.X > totSize.X + totSize.Width + brokemon.Textures[el].Width / 2 ||
                //        brokemon.Position.Y < totSize.Y - brokemon.Textures[el].Height / 2 ||
                //        brokemon.Position.Y > totSize.Y + totSize.Height + brokemon.Textures[el].Height / 2)
                //    {
                //        removeBrokemons.Add(brokemon);
                //    }
                //}
                //foreach (Brokemon brokemon in removeBrokemons)
                //{

                //    brokemons.Remove(brokemon);
                //}
                //removeBrokemons.Clear();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (Brokemon brokemon in brokemons2.Values)
            {
                brokemon.Draw(curSize);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
