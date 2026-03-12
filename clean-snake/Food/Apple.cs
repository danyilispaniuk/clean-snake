using System;

namespace clean_snake
{
    internal class Apple : Food
    {
        public Apple(Point pos) : base(pos, ConsoleColor.Green, 1, 1, 0) { }

        public override void apply(Game game, DateTime nowUtc)
        {
            base.apply(game, nowUtc);
            game.showEffect($"+{Math.Max(0, scoreDelta)} score, +{growSegments} len", ConsoleColor.Cyan, nowUtc);
        }
    }
}
