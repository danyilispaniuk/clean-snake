using System;

namespace clean_snake
{
    internal class FlashBerry : Food
    {
        public FlashBerry(Point pos) : base(pos, ConsoleColor.White, 3, 1, 30) { }

        public override void apply(Game game, DateTime nowUtc)
        {
            base.apply(game, nowUtc);
            game.flashBackgroundUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.snakeOverrideColor = ConsoleColor.Cyan;
            game.snakeColorUntilUtc = nowUtc.AddSeconds(effectLongevity);
            game.showEffect($"FLASH BERRY: BACKGROUND FLASH ({effectLongevity}s)", ConsoleColor.Cyan, nowUtc, 3);
        }
    }
}
