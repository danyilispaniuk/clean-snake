using System;
using System.Collections.Generic;
using System.Text;
namespace clean_snake
{
        internal abstract class Food
        {
            public Point pos { get; }
            public ConsoleColor color { get; }
            public int scoreDelta { get; }
            public int growSegments { get; }
            public int effectLongevity { get; }


        protected Food(Point pos, ConsoleColor color, int scoreDelta, int growSegments, int effectLongevity)
            {
                this.pos = pos;
                this.color = color;
                this.scoreDelta = scoreDelta;
                this.growSegments = growSegments;
                this.effectLongevity = effectLongevity;
            }
        }

        internal class Apple : Food
        {
            
            public Apple(Point pos) : base(pos, ConsoleColor.Green, 1, 1, 0) { }
        }

        internal class Chili : Food
        {
            public Chili(Point pos) : base(pos, ConsoleColor.Red, 2, 1, 10) { }
        }

        internal class Mushroom : Food
        {
            public Mushroom(Point pos) : base(pos, ConsoleColor.Magenta, 1, 1, 10) { }
        }

        internal class Lemon : Food
        {
            public Lemon(Point pos) : base(pos, ConsoleColor.Yellow, 1, 0, 10) { }
        }

        internal class FlashBerry : Food
        {
            public FlashBerry(Point pos) : base(pos, ConsoleColor.White, 3, 1, 30) { }
        }
}
