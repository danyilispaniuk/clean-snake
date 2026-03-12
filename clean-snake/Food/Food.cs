using System;

namespace clean_snake
{
    internal abstract class Food
    {
        public Point pos { get; set; }
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

        public virtual void apply(Game game, DateTime nowUtc)
        {
            game.score = Math.Max(0, game.score + scoreDelta);
        }

        public static Food createRandom(Playfield playfield, Snake snake)
        {
            var pos = playfield.RandomFreeCell(snake);
            var rng = new Random();
            int r = rng.Next(0, 100);

            if (r < 55) return new Apple(pos);
            if (r < 70) return new Chilli(pos);
            if (r < 85) return new Mushroom(pos);
            if (r < 95) return new Lemon(pos);
            return new FlashBerry(pos);
        }

        public static Food ensureInside(Food current, Playfield playfield, Snake snake)
        {
            if (!playfield.IsInside(current.pos) || snake.Occupies(current.pos))
            {
                var newPos = playfield.RandomFreeCell(snake);
                return createByType(current, newPos);
            }
            return current;
        }

        private static Food createByType(Food current, Point pos)
        {
            return current switch
            {
                Apple => new Apple(pos),
                Chilli => new Chilli(pos),
                Mushroom => new Mushroom(pos),
                Lemon => new Lemon(pos),
                FlashBerry => new FlashBerry(pos),
                _ => new Apple(pos)
            };
        }
    }
}
