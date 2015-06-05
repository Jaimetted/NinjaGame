using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NinjaGame
{
    class Player
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector Direction { get; set; }
        public Player()
        {
            X = 0;
            Y = 0;
            Direction = new Vector(0,1);
        }

        public void Move(float moveX,float moveY)
        {
            X += moveX;
            Y += moveY;
        }
        
    }
}
