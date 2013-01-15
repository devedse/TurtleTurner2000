using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurtleTurner2000.ScreenClient
{
    class Brokemon
    {
        private string id;
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private Vector2 direction;
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float xSpeed = 0f;
        public float ySpeed = 0f;

        private Texture2D[] textures;
        public Texture2D[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }
        private SpriteBatch sb;

        private float aliveTime;
        public float AliveTime
        {
            get { return aliveTime; }
            set { aliveTime = value; }
        }
        private int evolutionLevel;
        public int EvolutionStage
        {
            get { return evolutionLevel; }
            set { evolutionLevel = value; }
        }

        public Brokemon(Vector2 direction, Vector2 position, Texture2D[] textures, SpriteBatch sb, string id)
        {
            this.evolutionLevel = 0;
            this.direction = direction;
            this.position = position;
            this.textures = textures;
            this.sb = sb;
            this.id = id;
        }

        public void Update(GameTime gameTime)
        {
            //Make pokemon evolve beyond a certain time being alive.
            aliveTime += gameTime.ElapsedGameTime.Milliseconds;
            if (aliveTime > 1000 && aliveTime < 2000 && textures[1] != null)
            {
                evolutionLevel = 1;
            }
            if (aliveTime > 2000 && textures[2] != null)
            {
                evolutionLevel = 2;
            }


            position.X += xSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.Y += ySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;


        }

        public void Draw(Rectangle curSize)
        {
            Vector2 newPos = new Vector2(this.Position.X - curSize.X - textures[evolutionLevel].Width / 2, this.Position.Y - curSize.Y - textures[evolutionLevel].Height / 2);

            sb.Draw(textures[evolutionLevel], newPos, Color.White);

            //sb.Draw(textures[evolutionLevel], newPos,
            //                        textures[evolutionLevel].Bounds, Color.White,
            //                        this.Rotation, new Vector2(textures[evolutionLevel].Width / 2, textures[evolutionLevel].Height / 2),
            //                        1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
