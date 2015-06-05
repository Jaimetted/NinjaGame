using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    class Player
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Player()
        {
            X = 0;
            Y = 0;
        }

        public void Move(int moveX,int moveY)
        {
            X += moveX;
            Y += moveY;
        }
        
    }
}
