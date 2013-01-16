#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Collision;
#endregion

namespace TestTietje
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private World world;
        public Camera2D Camera;
        private Body rectangle;
        private Body circle;
        private RevoluteJoint joint;
        private Vector2 hit = Vector2.Zero;
        private bool onGround;

        private List<Body> tiles;
        private Texture2D rectangleTex;
        private Texture2D circleTex;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.ToggleFullScreen();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            world = new World(new Vector2(0f, 20.0f));
            Camera = new Camera2D(GraphicsDevice);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(63f);
            tiles = new List<Body>();

            // Create the character rec body part
            rectangle = BodyFactory.CreateRectangle(world, 1f, 1f, 1f, Vector2.Zero);
            rectangle.BodyType = BodyType.Dynamic;
            rectangle.Restitution = 0.0f;
            rectangle.Friction = 0.0f;
            rectangle.FixedRotation = true;

            // Create the character circle body part
            circle = BodyFactory.CreateCircle(world, 0.5f, 1f, new Vector2(0.0f, 0.5f));
            circle.BodyType = BodyType.Dynamic;
            circle.Restitution = 0f;
            circle.Friction = 5.0f;

            //Connect the two bodies
            joint = JointFactory.CreateRevoluteJoint(world, rectangle, circle, Vector2.Zero);
            joint.MotorEnabled = true;

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    Body tile = BodyFactory.CreateRectangle(world, 1f, 1f, 1f, ConvertUnits.ToSimUnits(new Vector2(i * 64, (j * 64) + 64)));
                    tile.BodyType = BodyType.Static;
                    tile.FixedRotation = true;
                    tile.Friction = 0.5f;
                    tile.Restitution = 0.0f;
                    if (j != 9)
                        tile.BodyType = BodyType.Static;
                    tiles.Add(tile);
                }
            }

            rectangle.FixedRotation = true;

            rectangle.BodyType = BodyType.Dynamic;

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

            rectangleTex = Content.Load<Texture2D>("1");
            circleTex = Content.Load<Texture2D>("2");
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

            //rectangle.Position = new Vector2(ConvertUnits.ToSimUnits(Mouse.GetState().X), ConvertUnits.ToSimUnits(Mouse.GetState().Y));

            Vector2 p1 = circle.Position + new Vector2(0.0f, 0.4f);
            Vector2 p2 = circle.Position + new Vector2(0.0f, 0.55f);

            onGround = false;
            world.RayCast((fixture, v1, v2, fraction) =>
            {
                onGround = true;
                return -1;
            }, p1, p2);

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.A))
            {
                rectangle.ApplyForce(new Vector2(-80f, 0f));
            }
            if (ks.IsKeyDown(Keys.D))
            {
                rectangle.ApplyForce(new Vector2(80f, 0f));
            }
            if (ks.IsKeyDown(Keys.W) && onGround)
            {
                rectangle.ApplyLinearImpulse(new Vector2(0f, -20f));
            }


            float xVelocity = MathHelper.Clamp(rectangle.LinearVelocity.X, -10, 10);
            float yVelocity = MathHelper.Clamp(rectangle.LinearVelocity.Y, -50, 80);

            if (onGround && !(ks.IsKeyDown(Keys.A) || ks.IsKeyDown(Keys.D)))
            {
                xVelocity *= 0.7f;
            }

            rectangle.LinearVelocity = new Vector2(xVelocity, yVelocity);


            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            HandleCamera(gameTime, ks);
            Camera.Update(gameTime);
            //Camera.ResetCamera();
            //Camera.MoveCamera(circle.Position);
            //Camera.Update(gameTime);


            base.Update(gameTime);
        }

        private void HandleCamera(GameTime gameTime, KeyboardState ks)
        {
            Vector2 camMove = Vector2.Zero;

            if (ks.IsKeyDown(Keys.Up))
            {
                camMove.Y -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                camMove.Y += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (ks.IsKeyDown(Keys.Left))
            {
                camMove.X -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                camMove.X += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (ks.IsKeyDown(Keys.PageUp))
            {
                Camera.Zoom += 50f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            }
            if (ks.IsKeyDown(Keys.PageDown))
            {
                Camera.Zoom -= 50f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            }
            if (camMove != Vector2.Zero)
            {
                Camera.MoveCamera(camMove);
            }
            if (ks.IsKeyDown(Keys.Home))
            {
                rectangle.Position = Vector2.Zero;
                //circle.Position = Vector2.Zero + new Vector2(0.0f, 0.5f);
                Camera.ResetCamera();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            spriteBatch.Draw(rectangleTex, ConvertUnits.ToDisplayUnits(rectangle.Position), Color.White);
            spriteBatch.Draw(circleTex, ConvertUnits.ToDisplayUnits(circle.Position + new Vector2(0.5f, 0.5f)), new Rectangle(0, 0, 64, 64),
                            Color.White, circle.Rotation, ConvertUnits.ToDisplayUnits(new Vector2(0.5f, 0.5f)), 1.0f, SpriteEffects.None, 1.0f);
            foreach (Body tile in tiles)
            {
                spriteBatch.Draw(rectangleTex, ConvertUnits.ToDisplayUnits(tile.Position), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
