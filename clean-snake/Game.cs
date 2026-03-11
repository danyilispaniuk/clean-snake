using System;
using System.Collections.Generic;
using System.Text;

namespace clean_snake
{
    internal sealed class Game
    {
        public enum gameStatus { Started, Finished, Won, Lost }

        public Window window;

        private int screenWidth;
        private int screenHeight;

        private int playLeft;
        private int playTop;
        private int playRight;
        private int playBottom;

        private int fieldWidth;
        private int fieldHeight;

        private readonly int baseTickMs;
        private readonly Random rng = new Random();

        public gameStatus status { get; private set; } = gameStatus.Started;
        public int score { get; private set; }

        private readonly Snake snake;
        private Food food;

        private DateTime speedEffectUntilUtc = DateTime.MinValue;
        private double speedMultiplier = 1.0;

        private DateTime snakeColorUntilUtc = DateTime.MinValue;
        private ConsoleColor snakeOverrideColor = ConsoleColor.Red;

        private DateTime flashBackgroundUntilUtc = DateTime.MinValue;
        private const int flashPeriodMs = 150;

        private string lastEffectText = "";
        private ConsoleColor lastEffectColor = ConsoleColor.White;
        private DateTime lastEffectUntilUtc = DateTime.MinValue;

        public Game(int baseTickMs, Window window)
        {
            this.baseTickMs = baseTickMs;

            ReadConsoleSize();
            DefinePlayfield();
            this.window = window;
            snake = new Snake(this);
            food = SpawnFood();
        }

        public void Run()
        {
            var lastTick = DateTime.UtcNow;

            while (status == gameStatus.Started)
            {
                if (Console.WindowWidth != screenWidth || Console.WindowHeight != screenHeight)
                {
                    ReadConsoleSize();
                    DefinePlayfield();
                    snake.ClampInside();
                    food = EnsureFoodInsideAndNotOnSnake(food);
                }

                var now = DateTime.UtcNow;
                int tickMs = GetCurrentTickMs(now);

                if ((now - lastTick).TotalMilliseconds < tickMs)
                {
                    Thread.Sleep(1);
                    continue;
                }

                lastTick = now;

                HandleInput();
                Step(now);
                Draw(now);
            }

            DrawGameOver();
        }

        private void ReadConsoleSize()
        {
            screenWidth = Console.WindowWidth;
            screenHeight = Console.WindowHeight;
        }

        private void DefinePlayfield()
        {
            playLeft = 0;
            playTop = 4;
            playRight = Math.Max(10, screenWidth - 1);
            playBottom = Math.Max(8, screenHeight - 2);

            fieldWidth = playRight - playLeft + 1;
            fieldHeight = playBottom - playTop + 1;

            if (fieldWidth < 10) { playRight = playLeft + 9; fieldWidth = 10; }
            if (fieldHeight < 6) { playBottom = playTop + 5; fieldHeight = 6; }
        }

        public bool IsInsidePlayableArea(Point p)
        {
            return p.X > playLeft && p.X < playRight && p.Y > playTop && p.Y < playBottom;
        }

        public Point ClampToPlayableArea(Point p)
        {
            int x = Math.Max(playLeft + 1, Math.Min(playRight - 1, p.X));
            int y = Math.Max(playTop + 1, Math.Min(playBottom - 1, p.Y));
            return new Point(x, y);
        }

        public Point RandomFreeCell()
        {
            int maxAttempts = 5000;

            for (int i = 0; i < maxAttempts; i++)
            {
                int x = rng.Next(playLeft + 1, playRight);
                int y = rng.Next(playTop + 1, playBottom);
                var p = new Point(x, y);

                if (!snake.Occupies(p))
                    return p;
            }

            for (int y = playTop + 1; y <= playBottom - 1; y++)
            {
                for (int x = playLeft + 1; x <= playRight - 1; x++)
                {
                    var p = new Point(x, y);
                    if (!snake.Occupies(p))
                        return p;
                }
            }

            return new Point(playLeft + 1, playTop + 1);
        }

        private int GetCurrentTickMs(DateTime nowUtc)
        {
            if (nowUtc > speedEffectUntilUtc)
                speedMultiplier = 1.0;

            double clamped = Math.Max(0.35, Math.Min(3.0, speedMultiplier));
            return (int)Math.Max(30, baseTickMs * clamped);
        }

        private ConsoleColor GetSnakeColor(DateTime nowUtc)
        {
            if (nowUtc > snakeColorUntilUtc)
                return window.theme.DefaultSnakeColor;

            return snakeOverrideColor;
        }

        private bool IsBackgroundFlashing(DateTime nowUtc) => nowUtc <= flashBackgroundUntilUtc;

        private static int RemainingSeconds(DateTime nowUtc, DateTime untilUtc)
        {
            var ms = (untilUtc - nowUtc).TotalMilliseconds;
            if (ms <= 0) return 0;
            return (int)Math.Ceiling(ms / 1000.0);
        }

        private void ShowEffect(string text, ConsoleColor color, DateTime nowUtc, int secondsVisible = 2)
        {
            lastEffectText = text;
            lastEffectColor = color;
            lastEffectUntilUtc = nowUtc.AddSeconds(secondsVisible);
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            Direction? desired = null;

            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                desired = key switch
                {
                    ConsoleKey.UpArrow => Direction.Up,
                    ConsoleKey.DownArrow => Direction.Down,
                    ConsoleKey.LeftArrow => Direction.Left,
                    ConsoleKey.RightArrow => Direction.Right,
                    _ => desired
                };
            }

            if (desired.HasValue)
                snake.TrySetDirection(desired.Value);
        }

        private void Step(DateTime nowUtc)
        {
            var nextHead = snake.PeekNextHead();

            if (!IsInsidePlayableArea(nextHead))
            {
                status = gameStatus.Lost;
                return;
            }

            if (snake.Occupies(nextHead))
            {
                status = gameStatus.Lost;
                return;
            }

            bool ate = (nextHead.X == food.Pos.X && nextHead.Y == food.Pos.Y);
            int growBy = ate ? food.GrowSegments : 0;

            snake.Move(growBy);

            if (ate)
            {
                ApplyFoodEffect(food, nowUtc);
                for (int x = 0; x < 5; x++)
                {
                    food = SpawnFood();
                }
            }
        }

        private void ApplyFoodEffect(Food f, DateTime nowUtc)
        {
            score = Math.Max(0, score + f.ScoreDelta);

            if (f.Type == FoodType.Apple)
            {
                ShowEffect($"+{Math.Max(0, f.ScoreDelta)} score, +{f.GrowSegments} len", ConsoleColor.Cyan, nowUtc);
                return;
            }

            if (f.Type == FoodType.Chili)
            {
                speedMultiplier = 0.60;
                speedEffectUntilUtc = nowUtc.AddSeconds(10);

                snakeOverrideColor = ConsoleColor.Yellow;
                snakeColorUntilUtc = nowUtc.AddSeconds(10);

                ShowEffect("CHILI: SPEED UP (10s)", ConsoleColor.Yellow, nowUtc);
                return;
            }

            if (f.Type == FoodType.Mushroom)
            {
                speedMultiplier = 1.80;
                speedEffectUntilUtc = nowUtc.AddSeconds(10);

                snakeOverrideColor = ConsoleColor.Magenta;
                snakeColorUntilUtc = nowUtc.AddSeconds(10);

                ShowEffect("MUSHROOM: SLOW (10s)", ConsoleColor.Magenta, nowUtc);
                return;
            }

            if (f.Type == FoodType.Lemon)
            {
                snake.Shrink(2);

                snakeOverrideColor = ConsoleColor.DarkYellow;
                snakeColorUntilUtc = nowUtc.AddSeconds(10);

                ShowEffect("LEMON: -2 LEN (10s color)", ConsoleColor.DarkYellow, nowUtc);
                return;
            }

            if (f.Type == FoodType.FlashBerry)
            {
                flashBackgroundUntilUtc = nowUtc.AddSeconds(30);

                snakeOverrideColor = ConsoleColor.Cyan;
                snakeColorUntilUtc = nowUtc.AddSeconds(30);

                ShowEffect("FLASH BERRY: BACKGROUND FLASH (30s)", ConsoleColor.Cyan, nowUtc, 3);
                return;
            }
        }

        private Food SpawnFood()
        {
            FoodType type = RollFoodType();
            var pos = RandomFreeCell();
            return Food.Create(type, pos);
        }

        private Food EnsureFoodInsideAndNotOnSnake(Food current)
        {
            if (!IsInsidePlayableArea(current.Pos) || snake.Occupies(current.Pos))
            {
                var pos = RandomFreeCell();
                return Food.Create(current.Type, pos);
            }
            return current;
        }

        private FoodType RollFoodType()
        {
            int r = rng.Next(0, 100);
            if (r < 55) return FoodType.Apple;
            if (r < 70) return FoodType.Chili;
            if (r < 85) return FoodType.Mushroom;
            if (r < 95) return FoodType.Lemon;
            return FoodType.FlashBerry;
        }

        private void Draw(DateTime nowUtc)
        {
            if (IsBackgroundFlashing(nowUtc))
            {
                long ms = (long)(nowUtc - DateTime.UnixEpoch).TotalMilliseconds;
                bool phase = (ms / flashPeriodMs) % 2 == 0;
                Console.BackgroundColor = phase ? ConsoleColor.DarkBlue : ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = window.theme.BackgroundColor;
            }

            Console.Clear();
            DrawUiFrame();
            DrawPlayfieldBorder();

            Console.SetCursorPosition(food.Pos.X, food.Pos.Y);
            Console.ForegroundColor = food.Color;
            Console.Write("■");

            var snakeColor = GetSnakeColor(nowUtc);
            snake.Draw(snakeColor);

            int sp = RemainingSeconds(nowUtc, speedEffectUntilUtc);
            int col = RemainingSeconds(nowUtc, snakeColorUntilUtc);
            int fl = RemainingSeconds(nowUtc, flashBackgroundUntilUtc);

            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = window.theme.UiColor;
            Console.Write($"Score:{score}".PadRight(12));

            Console.SetCursorPosition(12, 0);
            Console.Write($"Len:{snake.Length}".PadRight(10));

            Console.SetCursorPosition(2, 1);
            Console.ForegroundColor = window.theme.UiColor;
            Console.Write("Effects:".PadRight(screenWidth - 4));

            Console.SetCursorPosition(2, 2);
            Console.ForegroundColor = sp > 0 ? window.theme.UiAccentColor : window.theme.UiInactiveColor;
            Console.Write($"Speed:{(sp > 0 ? $"{sp}s" : "off"),-6}");

            Console.SetCursorPosition(13, 2);
            Console.ForegroundColor = col > 0 ? window.theme.UiAccentColor : window.theme.UiInactiveColor;
            Console.Write($"Color:{(col > 0 ? $"{col}s" : "off"),-6}");

            Console.SetCursorPosition(24, 2);
            Console.ForegroundColor = fl > 0 ? window.theme.UiAccentColor : window.theme.UiInactiveColor;
            Console.Write($"Flash:{(fl > 0 ? $"{fl}s" : "off"),-6}");

            if (nowUtc <= lastEffectUntilUtc && !string.IsNullOrWhiteSpace(lastEffectText))
            {
                Console.SetCursorPosition(2, 3);
                Console.ForegroundColor = lastEffectColor;
                Console.Write(lastEffectText.PadRight(screenWidth - 4));
            }
            else
            {
                Console.SetCursorPosition(2, 3);
                Console.ForegroundColor = window.theme.UiInactiveColor;
                Console.Write("".PadRight(screenWidth - 4));
            }
        }

        private void DrawUiFrame()
        {
            Console.ForegroundColor = window.theme.WallColor;

            int top = 0;
            int bottom = Math.Min(screenHeight - 1, 3);

            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, top);
                Console.Write("■");
                Console.SetCursorPosition(x, bottom);
                Console.Write("■");
            }

            for (int y = top; y <= bottom; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("■");
                Console.SetCursorPosition(screenWidth - 1, y);
                Console.Write("■");
            }
        }

        private void DrawPlayfieldBorder()
        {
            Console.ForegroundColor = window.theme.WallColor;

            for (int x = playLeft; x <= playRight; x++)
            {
                Console.SetCursorPosition(x, playTop);
                Console.Write("■");
                Console.SetCursorPosition(x, playBottom);
                Console.Write("■");
            }

            for (int y = playTop; y <= playBottom; y++)
            {
                Console.SetCursorPosition(playLeft, y);
                Console.Write("■");
                Console.SetCursorPosition(playRight, y);
                Console.Write("■");
            }
        }

        private void DrawGameOver()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            DrawUiFrame();
            DrawPlayfieldBorder();

            Console.ForegroundColor = ConsoleColor.White;
            int cx = Math.Max(2, screenWidth / 5);
            int cy = Math.Max(5, (playTop + playBottom) / 2);

            Console.SetCursorPosition(cx, cy);
            Console.Write($"Game over, Score: {score}");
            Console.SetCursorPosition(cx, cy + 1);
            Console.Write("Press any key...");
        }
    }
}
