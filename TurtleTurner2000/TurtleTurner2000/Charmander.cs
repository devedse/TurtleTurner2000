using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TurtleTurner2000
{
    class Charmander
    {
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

        public Charmander(Vector2 direction, Vector2 position)
        {
            this.direction = direction;
            this.position = position;
        }

        public void Update(GameTime gameTime)
        {
            position += direction * 10;
            if (direction.X != 0)
                rotation += 0.2f * direction.X / Math.Abs(direction.X);
            if (rotation > 360.0f)
                rotation = 0.0f;
        }
    }
}
