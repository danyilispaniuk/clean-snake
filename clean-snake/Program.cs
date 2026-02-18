using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var game = new Game(Console.WindowWidth, Console.WindowHeight, tickMs: 120);
            game.Run();

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
        private readonly int tickMs;
        private readonly Random rng = new Random();

        public GameStatus Status { get; private set; } = GameStatus.Started;
        public int Score { get; private set; } = 0;

        private readonly Snake snake;
        private Pixel berry;

        public Game(int screenWidth, int screenHeight, int tickMs)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.tickMs = tickMs;

            snake = new Snake(this, screenWidth, screenHeight);

            berry = SpawnBerry();
        }

        public void Run()
        {
            var lastTick = DateTime.UtcNow;

            while (Status == GameStatus.Started)
            {
                
                var now = DateTime.UtcNow;
                var dt = now - lastTick;
                if (dt.TotalMilliseconds < tickMs)
                {
                    Thread.Sleep(1);
                    continue;
                }
                lastTick = now;

                
                HandleInput();

                
                Step();

                
                Draw();
            }

            DrawGameOver();
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

        private void Step()
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

           
            bool ateBerry = (nextHead.X == berry.X && nextHead.Y == berry.Y);

            snake.Move(grow: ateBerry);

            if (ateBerry)
            {
                Score++;
                berry = SpawnBerry();
            }
        }

        private Pixel SpawnBerry()
        {
            
            while (true)
            {
                int x = rng.Next(1, screenWidth - 1);
                int y = rng.Next(1, screenHeight - 1);
                var p = new Pixel(x, y, ConsoleColor.Cyan);

                if (!snake.Occupies(p))
                    return p;
            }
        }

        private void Draw()
        {
            Console.Clear();

            DrawBorder();

           
            Console.SetCursorPosition(berry.X, berry.Y);
            Console.ForegroundColor = berry.Color;
            Console.Write("■");

            
            snake.Draw();

            
            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score: {Score}");
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
        private readonly Game game;
        private readonly int screenWidth;
        private readonly int screenHeight;

        
        private readonly Queue<Pixel> body = new Queue<Pixel>();

        public Direction Direction { get; private set; } = Direction.Right;

        public Snake(Game game, int screenWidth, int screenHeight)
        {
            this.game = game;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            
            int startX = screenWidth / 2;
            int startY = screenHeight / 2;

            body.Enqueue(new Pixel(startX, startY, ConsoleColor.Red));
        }

        public int Length => body.Count;

        public void TrySetDirection(Direction newDir)
        {
            
            if (IsOpposite(Direction, newDir)) return;
            Direction = newDir;
        }

        public Pixel PeekNextHead()
        {
            var head = body.Last();
            return Direction switch
            {
                Direction.Up => new Pixel(head.X, head.Y - 1, ConsoleColor.Red),
                Direction.Down => new Pixel(head.X, head.Y + 1, ConsoleColor.Red),
                Direction.Left => new Pixel(head.X - 1, head.Y, ConsoleColor.Red),
                Direction.Right => new Pixel(head.X + 1, head.Y, ConsoleColor.Red),
                _ => head
            };
        }

        public bool Occupies(Pixel p) => body.Any(b => b.X == p.X && b.Y == p.Y);

        public void Move(bool grow)
        {
            var nextHead = PeekNextHead();

            

            body.Enqueue(nextHead);

            if (!grow)
                body.Dequeue();
        }

        public void Draw()
        {
            
            var arr = body.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var p = arr[i];
                bool isHead = (i == arr.Length - 1);

                Console.SetCursorPosition(p.X, p.Y);
                Console.ForegroundColor = isHead ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write("■");
            }
        }

        private static bool IsOpposite(Direction a, Direction b) =>
            (a == Direction.Up && b == Direction.Down) ||
            (a == Direction.Down && b == Direction.Up) ||
            (a == Direction.Left && b == Direction.Right) ||
            (a == Direction.Right && b == Direction.Left);
    }

    internal enum Direction
    {
        Up, Down, Left, Right
    }

    internal readonly struct Pixel
    {
        public int X { get; }
        public int Y { get; }
        public ConsoleColor Color { get; }

        public Pixel(int x, int y, ConsoleColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }
}
