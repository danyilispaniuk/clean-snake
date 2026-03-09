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
}
