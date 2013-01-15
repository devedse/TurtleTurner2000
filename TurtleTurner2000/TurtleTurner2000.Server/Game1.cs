#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
using DeveConnecteuze.Network;
using TurtleTurner2000.SharedEnums;
using System.Text;
using System.Diagnostics;
#endregion

namespace TurtleTurner2000.Server
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Texture2D pixelTexture;
        public Texture2D tileTexture;
        public Texture2D skwirtleTexture;

        SpriteFont font;

        //public List<ScreenClientje> screenClientjes = new List<ScreenClientje>();

        public MouseState currentMouseState = Mouse.GetState();
        public MouseState previousMouseState = Mouse.GetState();
        public KeyboardState currentKeyboardState = Keyboard.GetState();
        public KeyboardState previousKeyboardState = Keyboard.GetState();

        public int scale = 10; //10x zo klein

        private List<List<String>> map;

        private DeveServer deveServer;
        private Dictionary<DeveConnection, Clientje> allClientjes = new Dictionary<DeveConnection, Clientje>();
        private Dictionary<DeveConnection, ControlClientje> controlClientjes = new Dictionary<DeveConnection, ControlClientje>();
        private Dictionary<DeveConnection, ScreenClientje> screenClientjes = new Dictionary<DeveConnection, ScreenClientje>();

        private Random random = new Random();

        public Rectangle totSize;

        public int tileSize = 64;

        Stopwatch fpsMeterStopwatch = new Stopwatch();

        public Game1()
            : base()
        {
            fpsMeterStopwatch.Start();

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            deveServer = new DeveServer(1337);
            deveServer.Start();

            this.graphics.PreferredBackBufferHeight = 1080;
            this.graphics.PreferredBackBufferWidth = 1920;
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
            LoadMap();
        }

        public void LoadMap()
        {
            map = new List<List<string>>();
            using (TextReader reader = new StreamReader("map.txt"))
            {
                String line;

                while ((line = reader.ReadLine()) != null)
                {
                    List<String> xlist = new List<string>();
                    map.Add(xlist);
                    foreach (Char c in line)
                    {
                        xlist.Add(c.ToString());
                    }
                }
            }

            totSize = new Rectangle(0, 0, map[0].Count * tileSize, map.Count * tileSize);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = Content.Load<Texture2D>("pixel");
            tileTexture = Content.Load<Texture2D>("tile");
            skwirtleTexture = Content.Load<Texture2D>("skwirtle");

            font = Content.Load<SpriteFont>("SpriteFont1");
            //rectSprite = new RectangleSprite(new Rectangle(100, 100, 200, 100), 10, pixelTexture);
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
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateNetwork(gameTime);


            foreach (var screenClientje in screenClientjes.Values)
            {
                screenClientje.Update(gameTime);
            }

            foreach (var controlClientje in controlClientjes.Values)
            {
                controlClientje.Update(gameTime);
            }


            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        private void UpdateNetwork(GameTime gameTime)
        {
            DeveIncomingMessage inc;
            while ((inc = deveServer.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case DeveMessageType.KeepAlive:
                        break;
                    case DeveMessageType.Data:
                        HandleDataMessage(inc);

                        break;
                    case DeveMessageType.StatusChanged:
                        byte newStatus = inc.ReadByte();
                        NetworkStatus ns = (NetworkStatus)newStatus;
                        switch (ns)
                        {
                            case NetworkStatus.Connected:

                                DebugMSG("Er connect iets :O");
                                break;
                            case NetworkStatus.Disconnected:

                                if (controlClientjes.ContainsKey(inc.Sender))
                                {
                                    ControlClientje controlClientje = controlClientjes[inc.Sender];
                                    DeveOutgoingMessage outje = new DeveOutgoingMessage();
                                    outje.WriteInt32((int)ServerSendMessageType.RemovePlayer);
                                    outje.WriteString(controlClientje.guid);
                                    SendToScreens(outje);

                                }

                                RemoveFromAllClientLists(inc.Sender);


                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void HandleDataMessage(DeveIncomingMessage inc)
        {

            ServerReceiveMessageType messageType = (ServerReceiveMessageType)inc.ReadInt32();


            switch (messageType)
            {
                case ServerReceiveMessageType.LoginMessageScreenClient:
                    {
                        DebugMSG("Screen client connected");

                        int xscr = random.Next(100, 1000);
                        int yscr = random.Next(100, 1000);
                        int widthscr = inc.ReadInt32();
                        int heightscr = inc.ReadInt32();

                        ScreenClientje screenClientje = new ScreenClientje(new Rectangle(xscr, yscr, widthscr, heightscr), this, inc.Sender);
                        screenClientjes.Add(inc.Sender, screenClientje);
                        allClientjes.Add(inc.Sender, screenClientje);

                        DeveOutgoingMessage outje = new DeveOutgoingMessage();
                        outje.WriteInt32((int)ServerSendMessageType.MapString);
                        outje.WriteInt32(map[0].Count);
                        outje.WriteInt32(map.Count);
                        foreach (var mapline in map)
                        {
                            StringBuilder build = new StringBuilder();

                            foreach (String str in mapline)
                            {
                                build.Append(str);
                            }

                            outje.WriteString(build.ToString());
                        }

                        screenClientje.deveConnection.Send(outje);

                        foreach (var controlClientje in controlClientjes.Values)
                        {
                            outje = new DeveOutgoingMessage();
                            outje.WriteInt32((int)ServerSendMessageType.SpawnNewPlayer); //Add beestje bij alle screens
                            outje.WriteString(controlClientje.guid);

                            screenClientje.deveConnection.Send(outje);

                            outje = new DeveOutgoingMessage();
                            outje.WriteInt32((int)ServerSendMessageType.SetPlayerLocation);
                            outje.WriteString(controlClientje.guid);
                            outje.WriteInt32((int)controlClientje.posx);
                            outje.WriteInt32((int)controlClientje.posy);
                        }
                    }
                    break;
                case ServerReceiveMessageType.LoginMessageControlClient:
                    {
                        DebugMSG("Het is een Android :)");

                        ControlClientje controlClientje = new ControlClientje(inc.Sender, this);
                        controlClientjes.Add(inc.Sender, controlClientje);
                        allClientjes.Add(inc.Sender, controlClientje);

                        DeveOutgoingMessage outje = new DeveOutgoingMessage();
                        outje.WriteInt32((int)ServerSendMessageType.SpawnNewPlayer); //Add beestje bij alle screens
                        outje.WriteString(controlClientje.guid);

                        SendToScreens(outje);
                    }
                    break;
                case ServerReceiveMessageType.NewButtonState:
                    {
                        ControlClientje curControlClient = controlClientjes[inc.Sender];

                        String direction = inc.ReadString();
                        String onOrOffString = inc.ReadString();
                        Boolean onOrOff = onOrOffString == "True";


                        DebugMSG("Got message with: " + direction + ": " + onOrOff);

                        if (direction == "left")
                        {
                            curControlClient.LeftArrow = onOrOff;
                        }
                        else if (direction == "right")
                        {
                            curControlClient.RightArrow = onOrOff;
                        }
                        else if (direction == "up")
                        {
                            curControlClient.UpArrown = onOrOff;
                        }
                        else if (direction == "down")
                        {
                            curControlClient.DownArrow = onOrOff;
                        }

                        //DebugMSG("X: " + curControlClient.posx + " Y: " + curControlClient.posy);

                        //DeveOutgoingMessage outje = new DeveOutgoingMessage();
                        //outje.WriteInt32((int)ServerSendMessageType.SetPlayerLocation);
                        //outje.WriteString(curControlClient.guid);
                        //outje.WriteInt32((int)curControlClient.posx);
                        //outje.WriteInt32((int)curControlClient.posy);

                        //SendToScreens(outje);
                        break;
                    }
                default:
                    DebugMSG("Uknown message type");
                    break;
            }

        }

        public void SendToScreens(DeveOutgoingMessage outje)
        {
            foreach (var screenClientje in screenClientjes.Values)
            {
                screenClientje.deveConnection.Send(outje);
            }
        }

        public void RemoveFromAllClientLists(DeveConnection deveConnection)
        {
            if (allClientjes.ContainsKey(deveConnection))
            {
                allClientjes.Remove(deveConnection);
            }
            if (controlClientjes.ContainsKey(deveConnection))
            {
                controlClientjes.Remove(deveConnection);
            }
            if (screenClientjes.ContainsKey(deveConnection))
            {
                screenClientjes.Remove(deveConnection);
            }
        }

        public void DebugMSG(String str)
        {
            Console.WriteLine(str);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();


            foreach (var screenClientje in screenClientjes.Values)
            {
                screenClientje.Draw(gameTime, spriteBatch);
            }


            for (int y = 0; y < map.Count; y++)
            {
                List<String> xlist = map[y];
                for (int x = 0; x < xlist.Count; x++)
                {
                    String cur = xlist[x];
                    if (cur == "1")
                    {
                        Rectangle rect = new Rectangle(x * tileSize / scale, y * tileSize / scale, tileSize / scale, tileSize / scale);
                        spriteBatch.Draw(tileTexture, rect, Color.White);
                    }
                }
            }

            foreach (ControlClientje controlClientje in controlClientjes.Values)
            {
                spriteBatch.Draw(skwirtleTexture, new Rectangle((int)(controlClientje.posx) / scale - skwirtleTexture.Width / scale / 2, (int)(controlClientje.posy) / scale - skwirtleTexture.Height / scale / 2, skwirtleTexture.Width / scale, skwirtleTexture.Height / scale), Color.White);
            }



            spriteBatch.DrawString(font, Math.Round(1.0 / fpsMeterStopwatch.Elapsed.TotalSeconds, 2).ToString(), new Vector2(10, 10), Color.White);
            fpsMeterStopwatch.Restart();


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
