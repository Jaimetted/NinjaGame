﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    class Enemy
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int direction { get; set; }

        public Vector Direction { get; set; }

        public Enemy(float startX, float startY)
        {
            X = startX;
            Y = startY;
        }
        public void Move(float moveX, float moveY)
        {
            X += moveX;
            Y += moveY;
        }
        
    }
}
