using System;
using System.Collections.Generic;
using System.Text;
namespace clean_snake
{

    internal sealed class Snake
    {
        private readonly Queue<Point> body = new Queue<Point>();
        private readonly Game game;

        public Direction Direction { get; private set; } = Direction.Right;
        public int Length => body.Count;

        public Snake(Game game)
        {
            this.game = game;

            int startX = Console.WindowWidth / 2;
            int startY = Math.Max(6, Console.WindowHeight / 2);
            var start = game.ClampToPlayableArea(new Point(startX, startY));
            body.Enqueue(start);
        }

        public void ClampInside()
        {
            var arr = body.ToArray();
            body.Clear();
            foreach (var p in arr)
                body.Enqueue(game.ClampToPlayableArea(p));
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
