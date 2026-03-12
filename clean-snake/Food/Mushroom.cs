using System;

namespace clean_snake
{
    internal class Mushroom : Food
    {
        public Mushroom(Point pos) : base(pos, ConsoleColor.Magenta, 1, 1, 10) { }

        public override void apply(Game game, DateTime nowUtc)
        {
            base.apply(game, nowUtc);
            game.speedMultiplier = 1.80;
            game.speedEffectUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.snakeOverrideColor = ConsoleColor.Magenta;
            game.snakeColorUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.showEffect($"MUSHROOM: SLOW ({effectLongevity}s)", ConsoleColor.Magenta, nowUtc);
        }
    }
}
