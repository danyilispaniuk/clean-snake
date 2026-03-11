using System;

namespace clean_snake
{
    internal class ConsoleRenderer
    {
        private readonly Window window;
        const int FlashPeriodsMs = 150;
        public ConsoleRenderer(Window window)
        {
            this.window = window;
        }
        public void Draw(
            DateTime nowUtc, int screenWidth, int screenHeight,
            int playLeft, int playTop, int playRight, int playBottom,
            int score, Snake snake, Food food,
            ConsoleColor snakeColor, bool isFlashing,
            int speedTimer, int colorTimer, int flashTimer,
            string effectText, ConsoleColor effectColor, DateTime effectUntilUtc)
        {


            if (isFlashing)
            {
                long ms = (long)(nowUtc - DateTime.UnixEpoch).TotalMilliseconds;
                bool phase = (ms / FlashPeriodsMs) % 2 == 0; 
                Console.BackgroundColor = phase ? ConsoleColor.DarkBlue : ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = window.Theme.BackgroundColor;
            }

            Console.Clear();
            DrawUiFrame(screenWidth, screenHeight);
            DrawPlayfieldBorder(playLeft, playTop, playRight, playBottom);

            
            Console.SetCursorPosition(food.Pos.X, food.Pos.Y);
            Console.ForegroundColor = food.Color;
            Console.Write("■");

            
            snake.Draw(snakeColor);

            
            DrawHud(screenWidth, score, snake.Length, speedTimer, colorTimer, flashTimer,
                    effectText, effectColor, effectUntilUtc, nowUtc);
        }

        public void DrawGameOver(int screenWidth, int playTop, int playBottom, int score)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            DrawUiFrame(screenWidth, Console.WindowHeight);
            DrawPlayfieldBorder(0, playTop, Math.Max(10, screenWidth - 1), playBottom);

            Console.ForegroundColor = ConsoleColor.White;
            int cx = Math.Max(2, screenWidth / 5);
            int cy = Math.Max(5, (playTop + playBottom) / 2);

            Console.SetCursorPosition(cx, cy);
            Console.Write($"Game over, Score: {score}");
            Console.SetCursorPosition(cx, cy + 1);
            Console.Write("Press any key...");
        }

        private void DrawUiFrame(int screenWidth, int screenHeight)
        {
            Console.ForegroundColor = window.Theme.WallColor;
            int top = 0;
            int bottom = Math.Min(screenHeight - 1, 3);

            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, top); Console.Write("■");
                Console.SetCursorPosition(x, bottom); Console.Write("■");
            }
            for (int y = top; y <= bottom; y++)
            {
                Console.SetCursorPosition(0, y); Console.Write("■");
                Console.SetCursorPosition(screenWidth - 1, y); Console.Write("■");
            }
        }

        private void DrawPlayfieldBorder(int left, int top, int right, int bottom)
        {
            Console.ForegroundColor = window.Theme.WallColor;
            for (int x = left; x <= right; x++)
            {
                Console.SetCursorPosition(x, top); Console.Write("■");
                Console.SetCursorPosition(x, bottom); Console.Write("■");
            }
            for (int y = top; y <= bottom; y++)
            {
                Console.SetCursorPosition(left, y); Console.Write("■");
                Console.SetCursorPosition(right, y); Console.Write("■");
            }
        }

        private void DrawHud(int screenWidth, int score, int length, int sp, int col, int fl,
                             string effectText, ConsoleColor effectColor, DateTime effectUntilUtc, DateTime nowUtc)
        {
            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = window.Theme.UiColor;
            Console.Write($"Score:{score}".PadRight(12));

            Console.SetCursorPosition(12, 0);
            Console.Write($"Len:{length}".PadRight(10));

            Console.SetCursorPosition(2, 1);
            Console.ForegroundColor = window.Theme.UiColor;
            Console.Write("Effects:".PadRight(screenWidth - 4));

            Console.SetCursorPosition(2, 2);
            Console.ForegroundColor = sp > 0 ? window.Theme.UiAccentColor : window.Theme.UiInactiveColor;
            Console.Write($"Speed:{(sp > 0 ? $"{sp}s" : "off"),-6}");

            Console.SetCursorPosition(13, 2);
            Console.ForegroundColor = col > 0 ? window.Theme.UiAccentColor : window.Theme.UiInactiveColor;
            Console.Write($"Color:{(col > 0 ? $"{col}s" : "off"),-6}");

            Console.SetCursorPosition(24, 2);
            Console.ForegroundColor = fl > 0 ? window.Theme.UiAccentColor : window.Theme.UiInactiveColor;
            Console.Write($"Flash:{(fl > 0 ? $"{fl}s" : "off"),-6}");

            Console.SetCursorPosition(2, 3);
            if (nowUtc <= effectUntilUtc && !string.IsNullOrWhiteSpace(effectText))
            {
                Console.ForegroundColor = effectColor;
                Console.Write(effectText.PadRight(screenWidth - 4));
            }
            else
            {
                Console.ForegroundColor = window.Theme.UiInactiveColor;
                Console.Write("".PadRight(screenWidth - 4));
            }
        }
    }
}