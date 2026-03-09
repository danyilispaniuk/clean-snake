using System;
using System.Collections.Generic;
using System.Text;
namespace clean_snake
{
    internal readonly struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
