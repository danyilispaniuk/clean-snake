using System;
using System.Collections.Generic;
using System.Text;
namespace clean_snake
{
    internal readonly struct Point
    {
        public int x { get; }
        public int y { get; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
