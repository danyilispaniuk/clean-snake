using System;
using System.Collections.Generic;
using System.Text;
namespace clean_snake
{
        internal abstract class Food
        {
            public Point Pos { get; }
            public ConsoleColor Color { get; }
            public int ScoreDelta { get; }
            public int GrowSegments { get; }

            
            protected Food(Point pos, ConsoleColor color, int scoreDelta, int growSegments)
            {
                Pos = pos;
                Color = color;
                ScoreDelta = scoreDelta;
                GrowSegments = growSegments;
            }
        }

        internal class Apple : Food
        {
            
            public Apple(Point pos) : base(pos, ConsoleColor.Cyan, 1, 1) { }
        }

        internal class Chili : Food
        {
            public Chili(Point pos) : base(pos, ConsoleColor.Red, 2, 1) { }
        }

        internal class Mushroom : Food
        {
            public Mushroom(Point pos) : base(pos, ConsoleColor.Magenta, 1, 1) { }
        }

        internal class Lemon : Food
        {
            public Lemon(Point pos) : base(pos, ConsoleColor.Yellow, -1, 0) { }
        }

        internal class FlashBerry : Food
        {
            public FlashBerry(Point pos) : base(pos, ConsoleColor.White, 3, 1) { }
        }
 


}
