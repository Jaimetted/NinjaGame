using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    class Projectile
    {
        public float  X { get; set; }
        public float Y { get; set; }
        public Vector Velocity { get; set; }

        public Projectile(float startX, float startY, Vector velocity)
        {
            X = startX;
            Y = startY;
            Velocity = velocity;

        }
        public void Move()
        {
            X += Velocity.X;
            Y += Velocity.Y;

        }
    }
}
