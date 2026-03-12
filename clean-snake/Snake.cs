namespace clean_snake
{
    internal sealed class Snake
    {
        private readonly Queue<Point> body = new Queue<Point>();
        private readonly Game game;

        public Direction direction { get; private set; } = Direction.Right;
        public int length => body.Count;

       
        public IEnumerable<Point> snakeBody => body;

        public Snake(Game game)
        {
            this.game = game;

            int startX = game.playfield.screenWidth / 2;
            int startY = Math.Max(6, game.playfield.screenHeight / 2);

            var start = game.playfield.Clamp(new Point(startX, startY));
            body.Enqueue(start);
        }

        public void ClampInside()
        {
            var arr = body.ToArray();
            body.Clear();
            foreach (var p in arr)
                
                body.Enqueue(game.playfield.Clamp(p));
        }

        public void TrySetDirection(Direction newDir)
        {
            if (IsOpposite(direction, newDir)) return;
            direction = newDir;
        }

        public Point PeekNextHead()
        {
            var head = body.Last();
            return direction switch
            {
                Direction.Up => new Point(head.x, head.y - 1),
                Direction.Down => new Point(head.x, head.y + 1),
                Direction.Left => new Point(head.x - 1, head.y),
                Direction.Right => new Point(head.x + 1, head.y),
                _ => head
            };
        }

        public bool Occupies(Point p) => body.Any(b => b.x == p.x && b.y == p.y);

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
                Console.SetCursorPosition(p.x, p.y);
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