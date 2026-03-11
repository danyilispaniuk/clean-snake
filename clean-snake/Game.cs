using System;
using System.Threading;

namespace clean_snake
{
    internal sealed class Game
    {
        public enum gameStatus { Started, Finished, Won, Lost }

        public Window window;
        private readonly int baseTickMs;
        private readonly Random rng = new Random();

        public gameStatus status { get; private set; } = gameStatus.Started;
        public int score { get; private set; }

        public readonly Playfield playfield;  
        private readonly Snake snake;
        private readonly ConsoleRenderer renderer;
        private Food food;

        
        private DateTime speedEffectUntilUtc = DateTime.MinValue;
        private double speedMultiplier = 1.0;
        private DateTime snakeColorUntilUtc = DateTime.MinValue;
        private ConsoleColor snakeOverrideColor = ConsoleColor.Red;
        private DateTime flashBackgroundUntilUtc = DateTime.MinValue;
        private string lastEffectText = "";
        private ConsoleColor lastEffectColor = ConsoleColor.White;
        private DateTime lastEffectUntilUtc = DateTime.MinValue;

        public Game(int baseTickMs, Window window)
        {
            this.baseTickMs = baseTickMs;
            this.window = window;

            playfield = new Playfield();
            renderer = new ConsoleRenderer(window);
            snake = new Snake(this);

            food = SpawnFood();
        }

        public void Run()
        {
            var lastTick = DateTime.UtcNow;

            while (status == gameStatus.Started)
            {
               
                if (playfield.UpdateSize())
                {
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

                renderer.Draw(
                    now, playfield.screenWidth, playfield.screenHeight,
                    playfield.left, playfield.top, playfield.right, playfield.bottom,
                    score, snake, food,
                    GetSnakeColor(now), IsBackgroundFlashing(now),
                    RemainingSeconds(now, speedEffectUntilUtc),
                    RemainingSeconds(now, snakeColorUntilUtc),
                    RemainingSeconds(now, flashBackgroundUntilUtc),
                    lastEffectText, lastEffectColor, lastEffectUntilUtc
                );
            }

            renderer.DrawGameOver(playfield.screenWidth, playfield.top, playfield.bottom, score);
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

            if (!playfield.IsInside(nextHead) || snake.Occupies(nextHead))
            {
                status = gameStatus.Lost;
                return;
            }

            bool ate = (nextHead.x == food.pos.x && nextHead.y == food.pos.y);
            int growBy = ate ? food.growSegments : 0;

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

        private Food SpawnFood()
        {
            var pos = playfield.RandomFreeCell(snake);
            return GenerateRandomFood(pos);
        }

        private Food EnsureFoodInsideAndNotOnSnake(Food current)
        {
            if (!playfield.IsInside(current.pos) || snake.Occupies(current.pos))
            {
                var pos = playfield.RandomFreeCell(snake);
                return GenerateFoodByType(current, pos);
            }
            return current;
        }

        private Food GenerateRandomFood(Point pos)
        {
            int r = rng.Next(0, 100);
            if (r < 55) return new Apple(pos);
            if (r < 70) return new Chili(pos);
            if (r < 85) return new Mushroom(pos);
            if (r < 95) return new Lemon(pos);
            return new FlashBerry(pos);
        }

        private Food GenerateFoodByType(Food currentFood, Point newPos)
        {
            if (currentFood is Apple) return new Apple(newPos);
            if (currentFood is Chili) return new Chili(newPos);
            if (currentFood is Mushroom) return new Mushroom(newPos);
            if (currentFood is Lemon) return new Lemon(newPos);
            if (currentFood is FlashBerry) return new FlashBerry(newPos);
            return new Apple(newPos);
        }

        private void ApplyFoodEffect(Food f, DateTime nowUtc)
        {
            score = Math.Max(0, score + f.scoreDelta);

            if (f is Apple)
            {
                ShowEffect($"+{Math.Max(0, f.scoreDelta)} score, +{f.growSegments} len", ConsoleColor.Cyan, nowUtc);
                return;
            }

            if (f is Chili)
            {
                speedMultiplier = 0.60;
                speedEffectUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                snakeOverrideColor = ConsoleColor.Yellow;
                snakeColorUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                ShowEffect($"CHILI: SPEED UP ({f.effectLongevity}s)", ConsoleColor.Yellow, nowUtc);
                return;
            }

            if (f is Mushroom)
            {
                speedMultiplier = 1.80;
                speedEffectUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                snakeOverrideColor = ConsoleColor.Magenta;
                snakeColorUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                ShowEffect($"MUSHROOM: SLOW ({f.effectLongevity}s)", ConsoleColor.Magenta, nowUtc);
                return;
            }

            if (f is Lemon)
            {
                snake.Shrink(f.growSegments);
                snakeOverrideColor = ConsoleColor.DarkYellow;
                snakeColorUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                ShowEffect($"LEMON: {f.growSegments} LEN ({f.effectLongevity}s)", ConsoleColor.DarkYellow, nowUtc);
                return;
            }

            if (f is FlashBerry)
            {
                flashBackgroundUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                snakeOverrideColor = ConsoleColor.Cyan;
                snakeColorUntilUtc = nowUtc.AddSeconds(f.effectLongevity);
                ShowEffect($"FLASH BERRY: BACKGROUND FLASH ({f.effectLongevity}s)", ConsoleColor.Cyan, nowUtc, 3);
                return;
            }
        }

        private int GetCurrentTickMs(DateTime nowUtc)
        {
            if (nowUtc > speedEffectUntilUtc) speedMultiplier = 1.0;
            double clamped = Math.Max(0.35, Math.Min(3.0, speedMultiplier));
            return (int)Math.Max(30, baseTickMs * clamped);
        }

        private ConsoleColor GetSnakeColor(DateTime nowUtc) => nowUtc > snakeColorUntilUtc ? window.colorScheme.DefaultSnakeColor : snakeOverrideColor;
        private bool IsBackgroundFlashing(DateTime nowUtc) => nowUtc <= flashBackgroundUntilUtc;
        private static int RemainingSeconds(DateTime nowUtc, DateTime untilUtc)
        {
            var ms = (untilUtc - nowUtc).TotalMilliseconds;
            return ms <= 0 ? 0 : (int)Math.Ceiling(ms / 1000.0);
        }

        private void ShowEffect(string text, ConsoleColor color, DateTime nowUtc, int secondsVisible = 2)
        {
            lastEffectText = text;
            lastEffectColor = color;
            lastEffectUntilUtc = nowUtc.AddSeconds(secondsVisible);
        }
    }
}