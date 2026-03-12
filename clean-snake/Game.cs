using System;
using System.Threading;

namespace clean_snake
{
    internal sealed class Game
    {
        public enum gameStatus { Started, Finished, Won, Lost }

        public Window window { get; }
        private readonly int baseTickMs;
        private readonly Random rng = new Random();

        public gameStatus status { get; private set; } = gameStatus.Started;
        public int score { get; internal set; }

        public readonly Playfield playfield;
        public readonly Snake snake;
        private readonly ConsoleRenderer renderer;
        private Food food;

        internal double speedMultiplier { get; set; } = 1.0;
        internal DateTime speedEffectUntilUtc { get; set; } = DateTime.MinValue;
        internal ConsoleColor snakeOverrideColor { get; set; } = ConsoleColor.Red;
        internal DateTime snakeColorUntilUtc { get; set; } = DateTime.MinValue;
        internal DateTime flashBackgroundUntilUtc { get; set; } = DateTime.MinValue;

        private string lastEffectText = "";
        private ConsoleColor lastEffectColor = ConsoleColor.White;
        private DateTime lastEffectUntilUtc = DateTime.MinValue;

        public Game(int baseTickMs, Window window)
        {
            this.baseTickMs = baseTickMs;
            this.window = window;

            this.playfield = new Playfield();
            this.renderer = new ConsoleRenderer(window);
            this.snake = new Snake(this);

            this.food = Food.createRandom(playfield, snake);
        }

        public void Run()
        {
            var lastTick = DateTime.UtcNow;

            while (status == gameStatus.Started)
            {
                if (playfield.UpdateSize())
                {
                    snake.ClampInside();
                    food = Food.ensureInside(food, playfield, snake);
                }

                var now = DateTime.UtcNow;
                int tickMs = getCurrentTickMs(now);

                if ((now - lastTick).TotalMilliseconds < tickMs)
                {
                    Thread.Sleep(1);
                    continue;
                }

                lastTick = now;

                handleInput();
                step(now);

                renderer.Draw(
                    now, playfield.screenWidth, playfield.screenHeight,
                    playfield.left, playfield.top, playfield.right, playfield.bottom,
                    score, snake, food,
                    getSnakeColor(now), isBackgroundFlashing(now),
                    remainingSeconds(now, speedEffectUntilUtc),
                    remainingSeconds(now, snakeColorUntilUtc),
                    remainingSeconds(now, flashBackgroundUntilUtc),
                    lastEffectText, lastEffectColor, lastEffectUntilUtc
                );
            }

            renderer.DrawGameOver(playfield.screenWidth, playfield.top, playfield.bottom, score);
        }

        private void handleInput()
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

        private void step(DateTime nowUtc)
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
                food.apply(this, nowUtc);
                food = Food.createRandom(playfield, snake);
            }
        }

        private int getCurrentTickMs(DateTime nowUtc)
        {
            if (nowUtc > speedEffectUntilUtc) speedMultiplier = 1.0;
            double clamped = Math.Max(0.35, Math.Min(3.0, speedMultiplier));
            return (int)Math.Max(30, baseTickMs * clamped);
        }

        private ConsoleColor getSnakeColor(DateTime nowUtc) => 
            nowUtc > snakeColorUntilUtc ? window.colorScheme.defaultSnakeColor : snakeOverrideColor;

        private bool isBackgroundFlashing(DateTime nowUtc) => nowUtc <= flashBackgroundUntilUtc;

        private static int remainingSeconds(DateTime nowUtc, DateTime untilUtc)
        {
            var ms = (untilUtc - nowUtc).TotalMilliseconds;
            return ms <= 0 ? 0 : (int)Math.Ceiling(ms / 1000.0);
        }

        public void showEffect(string text, ConsoleColor color, DateTime nowUtc, int secondsVisible = 2)
        {
            lastEffectText = text;
            lastEffectColor = color;
            lastEffectUntilUtc = nowUtc.AddSeconds(secondsVisible);
        }
    }
}
