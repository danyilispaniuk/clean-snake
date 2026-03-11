using System;

namespace clean_snake
{
    internal class Playfield
    {
        public int screenWidth { get; private set; }
        public int screenHeight { get; private set; }

        public int left { get; private set; }
        public int top { get; private set; }
        public int right { get; private set; }
        public int bottom { get; private set; }

        private readonly Random rng = new Random();

        public Playfield()
        {
            UpdateSize();
        }

        public bool UpdateSize()
        {
            if (screenWidth == Console.WindowWidth && screenHeight == Console.WindowHeight)
                return false;

            screenWidth = Console.WindowWidth;
            screenHeight = Console.WindowHeight;

            left = 0;
            top = 4;
            right = Math.Max(10, screenWidth - 1);
            bottom = Math.Max(8, screenHeight - 2);

            int width = right - left + 1;
            int height = bottom - top + 1;

            if (width < 10) { right = left + 9; }
            if (height < 6) { bottom = top + 5; }

            return true;
        }

        public bool IsInside(Point p)
        {
            return p.x > left && p.x < right && p.y > top && p.y < bottom;
        }

        public Point Clamp(Point p)
        {
            int x = Math.Max(left + 1, Math.Min(right - 1, p.x));
            int y = Math.Max(top + 1, Math.Min(bottom - 1, p.y));
            return new Point(x, y);
        }

        public Point RandomFreeCell(Snake snake)
        {
            int maxAttempts = 5000;

            for (int i = 0; i < maxAttempts; i++)
            {
                int x = rng.Next(left + 1, right);
                int y = rng.Next(top + 1, bottom);
                var p = new Point(x, y);

                if (!snake.Occupies(p))
                    return p;
            }

            for (int y = top + 1; y <= bottom - 1; y++)
            {
                for (int x = left + 1; x <= right - 1; x++)
                {
                    var p = new Point(x, y);
                    if (!snake.Occupies(p))
                        return p;
                }
            }

            return new Point(left + 1, top + 1);
        }
    }
}