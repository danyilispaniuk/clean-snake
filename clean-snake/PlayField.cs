using System;

namespace clean_snake
{
    internal class Playfield
    {
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }

        private readonly Random rng = new Random();

        public Playfield()
        {
            UpdateSize();
        }

        public bool UpdateSize()
        {
            if (ScreenWidth == Console.WindowWidth && ScreenHeight == Console.WindowHeight)
                return false;

            ScreenWidth = Console.WindowWidth;
            ScreenHeight = Console.WindowHeight;

            Left = 0;
            Top = 4;
            Right = Math.Max(10, ScreenWidth - 1);
            Bottom = Math.Max(8, ScreenHeight - 2);

            int width = Right - Left + 1;
            int height = Bottom - Top + 1;

            if (width < 10) { Right = Left + 9; }
            if (height < 6) { Bottom = Top + 5; }

            return true;
        }

        public bool IsInside(Point p)
        {
            return p.X > Left && p.X < Right && p.Y > Top && p.Y < Bottom;
        }

        public Point Clamp(Point p)
        {
            int x = Math.Max(Left + 1, Math.Min(Right - 1, p.X));
            int y = Math.Max(Top + 1, Math.Min(Bottom - 1, p.Y));
            return new Point(x, y);
        }

        public Point RandomFreeCell(Snake snake)
        {
            int maxAttempts = 5000;

            for (int i = 0; i < maxAttempts; i++)
            {
                int x = rng.Next(Left + 1, Right);
                int y = rng.Next(Top + 1, Bottom);
                var p = new Point(x, y);

                if (!snake.Occupies(p))
                    return p;
            }

            for (int y = Top + 1; y <= Bottom - 1; y++)
            {
                for (int x = Left + 1; x <= Right - 1; x++)
                {
                    var p = new Point(x, y);
                    if (!snake.Occupies(p))
                        return p;
                }
            }

            return new Point(Left + 1, Top + 1);
        }
    }
}