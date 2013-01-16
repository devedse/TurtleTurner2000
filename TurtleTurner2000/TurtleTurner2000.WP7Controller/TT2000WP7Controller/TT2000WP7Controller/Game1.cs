using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace TT2000WP7Controller
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TouchCollection touchColl;

        //Send thread
        private bool threadIsRunning;
        Socket socket;
        IPEndPoint endpoint;

        //Buttons
        List<Buttontje> buttons = new List<Buttontje>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            //Create socket
            CreateSocket();
        }

        public void CreateSocket()
        {
            IPAddress ipAddress = IPAddress.Parse("192.168.2.15");
            int port = 1337;

            // create endpoint var ipAddress = IPAddress.Parse(host);
            endpoint = new IPEndPoint(ipAddress, port);

            // create event args
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = endpoint;
            args.Completed += new EventHandler<SocketAsyncEventArgs>(args_Completed);
            DeveOutgoingMessage dom = new DeveOutgoingMessage(DeveMessageType.Data);
            dom.WriteInt32(0);
            dom.WriteInt32(1);
            byte[] buffer = dom.GetBytes();
            args.SetBuffer(buffer, 0, buffer.Length);

            // create a new socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;

            // connect socket
            bool completesAsynchronously = socket.ConnectAsync(args);
        }

        void args_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Debug.WriteLine("connecting" + e.SocketError.ToString());
        }

        void argsSend_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Debug.WriteLine("sending" + e.SocketError.ToString());
        }

        public void SendMessage(string direction)
        {
            if (socket.Connected)
            {
                // create event args
                var args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = endpoint;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(argsSend_Completed);
                DeveOutgoingMessage dom = new DeveOutgoingMessage(DeveMessageType.Data);
                dom.WriteInt32(1);
                dom.WriteString(direction);
                byte[] buffer = dom.GetBytes();
                args.SetBuffer(buffer, 0, buffer.Length);

                // connect socket
                bool completesAsynchronously = socket.SendAsync(args);

                // check if the completed event will be raised.
                // if not, invoke the handler manually.
                if (!completesAsynchronously)
                {
                    args_Completed(socket, args);
                }
            }
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

            SpriteFont spriteFont = Content.Load<SpriteFont>("spriteFont1");

            int width = graphics.PreferredBackBufferWidth;
            int height = graphics.PreferredBackBufferHeight;
            buttons.Add(new Buttontje(this, "left", new Rectangle(0, height / 2, width / 3, height / 2), Color.Yellow, spriteFont));
            buttons.Add(new Buttontje(this, "right", new Rectangle(2 * width / 3, height / 2, width / 3, height / 2), Color.Green, spriteFont));
            buttons.Add(new Buttontje(this, "down", new Rectangle(1 * width / 3, height / 2, width / 3, height / 2), Color.Violet, spriteFont));
            buttons.Add(new Buttontje(this, "up", new Rectangle(width / 2 - (width / 3 / 2), 0, width / 3, height / 2), Color.DarkBlue, spriteFont));

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            touchColl = TouchPanel.GetState();
            foreach (Buttontje button in buttons)
            {
                button.Update(touchColl);
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
            foreach (Buttontje button in buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
