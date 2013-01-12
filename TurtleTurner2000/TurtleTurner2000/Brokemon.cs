using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TurtleTurner2000
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
        float rotation = 0.0f;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
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
            if (aliveTime > 1000 && aliveTime < 2000 && textures[1] !=null)
            {
                evolutionLevel = 1;
            }
            if (aliveTime > 2000 && textures[2] != null)
            {
                evolutionLevel = 2;
            }

            position += direction * 10;
            if (direction.X != 0)
                rotation += 0.05f * direction.X / Math.Abs(direction.X);
            if (rotation > 360.0f)
                rotation = 0.0f;
        }

        public void Draw(Rectangle curSize)
        {
            Vector2 newPos = new Vector2(this.Position.X - curSize.X, this.Position.Y - curSize.Y);
            sb.Draw(textures[evolutionLevel], newPos,
                                    textures[evolutionLevel].Bounds, Color.White,
                                    this.Rotation, new Vector2(textures[evolutionLevel].Width / 2, textures[evolutionLevel].Height / 2),
                                    1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
