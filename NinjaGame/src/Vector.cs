using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    class Vector
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public Vector()
        {
            X = 0;
            Y = 0;
        }
        public Vector(float startX, float startY)
        {
            X = startX;
            Y = startY;
        }

        public Vector Multiply(float scale)
        {
            return new Vector(X * scale, Y * scale);
        }
    }
}