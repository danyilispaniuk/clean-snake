using System;

namespace clean_snake
{
    internal class Lemon : Food
    {
        public Lemon(Point pos) : base(pos, ConsoleColor.Yellow, 1, 0, 10) { }

        public override void apply(Game game, DateTime nowUtc)
        {
            base.apply(game, nowUtc);
            game.snake.Shrink(growSegments);
            game.snakeOverrideColor = ConsoleColor.DarkYellow;
            game.snakeColorUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.showEffect($"LEMON: {growSegments} LEN ({effectLongevity}s)", ConsoleColor.DarkYellow, nowUtc);
        }
    }
}
