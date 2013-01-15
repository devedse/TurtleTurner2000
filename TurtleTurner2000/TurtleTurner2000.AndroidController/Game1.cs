using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using DeveConnecteuze.Network;
using System.Net;
using System.IO;
using System;
using Microsoft.Xna.Framework.Input.Touch;
using TurtleTurner2000.SharedEnums;

namespace TurtleTurner2000AndroidController
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        private List<Buttontje> buttontjes = new List<Buttontje>();
        private Texture2D pixelTexture;

        private DeveClient deveClient;

        private TouchCollection currentTouchCollection;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

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

            deveClient = new DeveClient(ip, 1337);
            deveClient.Start();

            DeveOutgoingMessage outje = new DeveOutgoingMessage();
            outje.WriteInt32((int)ServerReceiveMessageType.LoginMessageControlClient); //Join message
            //outje.WriteInt32(1); //Android
            deveClient.Send(outje);

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            this.currentTouchCollection = TouchPanel.GetState();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("spriteFont1");
            pixelTexture = Content.Load<Texture2D>("white");


            buttontjes.Add(new Buttontje(deveClient, "left", new Rectangle(0, Window.Height / 2, Window.Width / 3, Window.Height / 2), Color.Yellow, pixelTexture, font));
            buttontjes.Add(new Buttontje(deveClient, "right", new Rectangle(2 * Window.Width / 3, Window.Height / 2, Window.Width / 3, Window.Height / 2), Color.Green, pixelTexture, font));
            buttontjes.Add(new Buttontje(deveClient, "down", new Rectangle(1 * Window.Width / 3, Window.Height / 2, Window.Width / 3, Window.Height / 2), Color.Violet, pixelTexture, font));
            buttontjes.Add(new Buttontje(deveClient, "up", new Rectangle(Window.Width / 2 - (Window.Width / 3 / 2), 0, Window.Width / 3, Window.Height / 2), Color.DarkBlue, pixelTexture, font));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentTouchCollection = TouchPanel.GetState();




            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                deveClient.Stop();
                Exit();
            }

            foreach (var buttontje in buttontjes)
            {
                buttontje.Update(currentTouchCollection);
            }




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Hello from MonoGame!", new Vector2(16, 16), Color.White);

            foreach (var buttontje in buttontjes)
            {
                buttontje.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
