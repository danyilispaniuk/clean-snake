using System;

namespace clean_snake
{
    internal class Chilli : Food
    {
        public Chilli(Point pos) : base(pos, ConsoleColor.Red, 2, 1, 10) { }

        public override void apply(Game game, DateTime nowUtc)
        {
            base.apply(game, nowUtc);
            game.speedMultiplier = 0.60;
            game.speedEffectUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.snakeOverrideColor = ConsoleColor.Yellow;
            game.snakeColorUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.showEffect($"CHILI: SPEED UP ({effectLongevity}s)", ConsoleColor.Yellow, nowUtc);
        }
    }
}
