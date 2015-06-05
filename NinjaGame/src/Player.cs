using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    class Player
    {
        public int x { get; set; }
        public int y { get; set; }

        public Player()
        {
            x=y=0;
        }

        public void Move(int moveX,int moveY){
            x += moveX;
            y += moveY;
        }
        
    }
}
