using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake
{
    internal static class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;
            Console.WindowWidth = 32;
            Console.WindowHeight = 16;

            var game = new Game(Console.WindowWidth, Console.WindowHeight, 120);
            game.Run();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(2, Console.WindowHeight - 1);
            Console.CursorVisible = true;
            Console.ReadKey(true);
        }
    }

    internal sealed class Game
    {
        public enum GameStatus { Started, Finished, Won, Lost }

        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly int baseTickMs;
        private readonly Random rng = new Random();

        public GameStatus Status { get; private set; } = GameStatus.Started;
        public int Score { get; private set; }

        private readonly Snake snake;
        private Food food;

        private DateTime speedEffectUntilUtc = DateTime.MinValue;
        private double speedMultiplier = 1.0;

        private DateTime snakeColorUntilUtc = DateTime.MinValue;
        private ConsoleColor snakeOverrideColor = ConsoleColor.Red;

        private DateTime flashBackgroundUntilUtc = DateTime.MinValue;

        private const int FlashPeriodMs = 150;

        public Game(int screenWidth, int screenHeight, int baseTickMs)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.baseTickMs = baseTickMs;

            snake = new Snake(screenWidth, screenHeight);
            food = SpawnFood();
        }

        public void Run()
        {
            var lastTick = DateTime.UtcNow;

            while (Status == GameStatus.Started)
            {
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
                return ConsoleColor.Red;

            return snakeOverrideColor;
        }

        private bool IsBackgroundFlashing(DateTime nowUtc) => nowUtc <= flashBackgroundUntilUtc;

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

            if (nextHead.X <= 0 || nextHead.X >= screenWidth - 1 ||
                nextHead.Y <= 0 || nextHead.Y >= screenHeight - 1)
            {
                Status = GameStatus.Lost;
                return;
            }

            if (snake.Occupies(nextHead))
            {
                Status = GameStatus.Lost;
                return;
            }

            bool ate = (nextHead.X == food.Pos.X && nextHead.Y == food.Pos.Y);
            int growBy = ate ? food.GrowSegments : 0;

            snake.Move(growBy);

            if (ate)
            {
                ApplyFoodEffect(food, nowUtc);
                food = SpawnFood();
            }
        }

        private void ApplyFoodEffect(Food f, DateTime nowUtc)
        {
            Score = Math.Max(0, Score + f.ScoreDelta);

            switch (f.Type)
            {
                case FoodType.Chili:
                    speedMultiplier = 0.60;
                    speedEffectUntilUtc = nowUtc.AddSeconds(10);
                    snakeOverrideColor = ConsoleColor.Yellow;
                    snakeColorUntilUtc = nowUtc.AddSeconds(10);
                    break;

                case FoodType.Mushroom:
                    speedMultiplier = 1.80;
                    speedEffectUntilUtc = nowUtc.AddSeconds(10);
                    snakeOverrideColor = ConsoleColor.Magenta;
                    snakeColorUntilUtc = nowUtc.AddSeconds(10);
                    break;

                case FoodType.Lemon:
                    snake.Shrink(2);
                    snakeOverrideColor = ConsoleColor.DarkYellow;
                    snakeColorUntilUtc = nowUtc.AddSeconds(10);
                    break;

                case FoodType.FlashBerry:
                    flashBackgroundUntilUtc = nowUtc.AddSeconds(30);
                    snakeOverrideColor = ConsoleColor.Cyan;
                    snakeColorUntilUtc = nowUtc.AddSeconds(30);
                    break;
            }
        }

        private Food SpawnFood()
        {
            FoodType type = RollFoodType();

            while (true)
            {
                int x = rng.Next(1, screenWidth - 1);
                int y = rng.Next(1, screenHeight - 1);
                var pos = new Point(x, y);

                if (!snake.Occupies(pos))
                    return Food.Create(type, pos);
            }
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
                bool phase = (ms / FlashPeriodMs) % 2 == 0;
                Console.BackgroundColor = phase ? ConsoleColor.DarkBlue : ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
            }

            Console.Clear();
            DrawBorder();

            Console.SetCursorPosition(food.Pos.X, food.Pos.Y);
            Console.ForegroundColor = food.Color;
            Console.Write("■");

            var snakeColor = GetSnakeColor(nowUtc);
            snake.Draw(snakeColor);

            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score: {Score}");

            Console.SetCursorPosition(14, 0);
            Console.Write($"Len: {snake.Length}");

            Console.SetCursorPosition(22, 0);
            Console.Write(IsBackgroundFlashing(nowUtc) ? "FLASH" : "     ");
        }

        private void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("■");
                Console.SetCursorPosition(x, screenHeight - 1);
                Console.Write("■");
            }

            for (int y = 0; y < screenHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("■");
                Console.SetCursorPosition(screenWidth - 1, y);
                Console.Write("■");
            }
        }

        private void DrawGameOver()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            DrawBorder();

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.Write($"Game over, Score: {Score}");
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
            Console.Write("Press any key...");
        }
    }

    internal sealed class Snake
    {
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly Queue<Point> body = new Queue<Point>();

        public Direction Direction { get; private set; } = Direction.Right;
        public int Length => body.Count;

        public Snake(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            int startX = screenWidth / 2;
            int startY = screenHeight / 2;
            body.Enqueue(new Point(startX, startY));
        }

        public void TrySetDirection(Direction newDir)
        {
            if (IsOpposite(Direction, newDir)) return;
            Direction = newDir;
        }

        public Point PeekNextHead()
        {
            var head = body.Last();
            return Direction switch
            {
                Direction.Up => new Point(head.X, head.Y - 1),
                Direction.Down => new Point(head.X, head.Y + 1),
                Direction.Left => new Point(head.X - 1, head.Y),
                Direction.Right => new Point(head.X + 1, head.Y),
                _ => head
            };
        }

        public bool Occupies(Point p) => body.Any(b => b.X == p.X && b.Y == p.Y);

        public void Move(int growBySegments)
        {
            body.Enqueue(PeekNextHead());

            if (growBySegments <= 0)
            {
                body.Dequeue();
                return;
            }

            for (int i = 1; i < growBySegments; i++)
            {
                var tail = body.Peek();
                body.Enqueue(tail);
            }
        }

        public void Shrink(int segments)
        {
            while (segments > 0 && body.Count > 1)
            {
                body.Dequeue();
                segments--;
            }
        }

        public void Draw(ConsoleColor snakeColorOverride)
        {
            foreach (var p in body)
            {
                Console.SetCursorPosition(p.X, p.Y);
                Console.ForegroundColor = snakeColorOverride;
                Console.Write("■");
            }
        }

        private static bool IsOpposite(Direction a, Direction b) =>
            (a == Direction.Up && b == Direction.Down) ||
            (a == Direction.Down && b == Direction.Up) ||
            (a == Direction.Left && b == Direction.Right) ||
            (a == Direction.Right && b == Direction.Left);
    }

    internal enum Direction { Up, Down, Left, Right }

    internal enum FoodType
    {
        Apple,
        Chili,
        Mushroom,
        Lemon,
        FlashBerry
    }

    internal readonly struct Food
    {
        public FoodType Type { get; }
        public Point Pos { get; }
        public ConsoleColor Color { get; }
        public int ScoreDelta { get; }
        public int GrowSegments { get; }

        private Food(FoodType type, Point pos, ConsoleColor color, int scoreDelta, int growSegments)
        {
            Type = type;
            Pos = pos;
            Color = color;
            ScoreDelta = scoreDelta;
            GrowSegments = growSegments;
        }

        public static Food Create(FoodType type, Point pos)
        {
            return type switch
            {
                FoodType.Apple => new Food(type, pos, ConsoleColor.Cyan, 1, 1),
                FoodType.Chili => new Food(type, pos, ConsoleColor.Red, 2, 1),
                FoodType.Mushroom => new Food(type, pos, ConsoleColor.Magenta, 1, 1),
                FoodType.Lemon => new Food(type, pos, ConsoleColor.Yellow, -1, 0),
                FoodType.FlashBerry => new Food(type, pos, ConsoleColor.White, 3, 1),
                _ => new Food(FoodType.Apple, pos, ConsoleColor.Cyan, 1, 1),
            };
        }
    }

    internal readonly struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
