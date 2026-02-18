using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake
{
    class Window
    {
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public int BackgroundColor { get; set; }

        public Window(){
            WindowHeight = 400;
            WindowWidth = 600;
            BackgroundColor = ((int)Console.BackgroundColor);
        }

        public Window(int windowHeight, int windowWidth, int backgroundColor)
        {
            WindowHeight = windowHeight;
            WindowWidth = windowWidth;
            BackgroundColor =   backgroundColor;
        }

        public void Apply()
        {
            Console.WindowHeight = WindowHeight;
            Console.WindowWidth = WindowWidth;
            // "1. Red\t\t2. Blue\t\t3. Green\n4. Yellow\t5. Magenta\t6. Black\n7. White\t8. Dark grey\t9. Grey"
            ConsoleColor backgroundcolor = BackgroundColor switch
            {
                1 => ConsoleColor.DarkRed,
                2 => ConsoleColor.DarkBlue,
                3 => ConsoleColor.DarkGreen,
                4 => ConsoleColor.DarkYellow,
                5 => ConsoleColor.DarkMagenta,
                6 => ConsoleColor.Black,
                7 => ConsoleColor.White,
                8 => ConsoleColor.DarkGray,
                9 => ConsoleColor.Gray,
                _ => ConsoleColor.Black // default
            };
            Console.BackgroundColor = backgroundcolor;
            Console.Clear();
        }
    }

    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            // Console.WindowHeight = 32;
            // Console.WindowWidth = 50;
            Console.WriteLine("Enter a width of game window:");
            int screenwidth = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter a height of game window:");
            int screenheight = int.Parse(Console.ReadLine());

            Console.WriteLine("Choose a background color:");
            Console.WriteLine("1. Red\t\t2. Blue\t\t3. Green\n4. Yellow\t5. Magenta\t6. Black\n7. White\t8. Dark grey\t9. Grey");
            int backgroundColor = int.Parse(Console.ReadLine());

            // Create and apply the window
            Window gameWindow = new Window(screenheight, screenwidth, backgroundColor);
            gameWindow.Apply();

            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            var game = new Game(baseTickMs: 100, gameWindow);
            game.Run();

            Console.SetCursorPosition(2, Console.WindowHeight - 1);
            Console.CursorVisible = true;
            Console.ReadKey(true);
        }
    }

    internal sealed class Game
    {
        public enum GameStatus { Started, Finished, Won, Lost }

        public Window window;

        private int screenWidth;
        private int screenHeight;

        private int PlayLeft;
        private int PlayTop;
        private int PlayRight;
        private int PlayBottom;

        private int fieldWidth;
        private int fieldHeight;

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

            while (Status == GameStatus.Started)
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
            PlayLeft = 0;
            PlayTop = 4;
            PlayRight = Math.Max(10, screenWidth - 1);
            PlayBottom = Math.Max(8, screenHeight - 2);

            fieldWidth = PlayRight - PlayLeft + 1;
            fieldHeight = PlayBottom - PlayTop + 1;

            if (fieldWidth < 10) { PlayRight = PlayLeft + 9; fieldWidth = 10; }
            if (fieldHeight < 6) { PlayBottom = PlayTop + 5; fieldHeight = 6; }
        }

        public bool IsInsidePlayableArea(Point p)
        {
            return p.X > PlayLeft && p.X < PlayRight && p.Y > PlayTop && p.Y < PlayBottom;
        }

        public Point ClampToPlayableArea(Point p)
        {
            int x = Math.Max(PlayLeft + 1, Math.Min(PlayRight - 1, p.X));
            int y = Math.Max(PlayTop + 1, Math.Min(PlayBottom - 1, p.Y));
            return new Point(x, y);
        }

        public Point RandomFreeCell()
        {
            int maxAttempts = 5000;

            for (int i = 0; i < maxAttempts; i++)
            {
                int x = rng.Next(PlayLeft + 1, PlayRight);
                int y = rng.Next(PlayTop + 1, PlayBottom);
                var p = new Point(x, y);

                if (!snake.Occupies(p))
                    return p;
            }

            for (int y = PlayTop + 1; y <= PlayBottom - 1; y++)
            {
                for (int x = PlayLeft + 1; x <= PlayRight - 1; x++)
                {
                    var p = new Point(x, y);
                    if (!snake.Occupies(p))
                        return p;
                }
            }

            return new Point(PlayLeft + 1, PlayTop + 1);
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
                for(int x = 0; x < 5; x++)
                {
                    food = SpawnFood();
                }
            }
        }

        private void ApplyFoodEffect(Food f, DateTime nowUtc)
        {
            Score = Math.Max(0, Score + f.ScoreDelta);

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
                bool phase = (ms / FlashPeriodMs) % 2 == 0;
                Console.BackgroundColor = phase ? ConsoleColor.DarkBlue : ConsoleColor.Black;
            }
            else
            {

                ConsoleColor backgroundcolor = window.BackgroundColor switch
                {
                    1 => ConsoleColor.DarkRed,
                    2 => ConsoleColor.DarkBlue,
                    3 => ConsoleColor.DarkGreen,
                    4 => ConsoleColor.DarkYellow,
                    5 => ConsoleColor.DarkMagenta,
                    6 => ConsoleColor.Black,
                    7 => ConsoleColor.White,
                    8 => ConsoleColor.DarkGray,
                    9 => ConsoleColor.Gray,
                    _ => ConsoleColor.Black // default
                };
                Console.BackgroundColor = backgroundcolor;
                
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score:{Score}".PadRight(12));

            Console.SetCursorPosition(12, 0);
            Console.Write($"Len:{snake.Length}".PadRight(10));

            Console.SetCursorPosition(2, 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Effects:".PadRight(screenWidth - 4));

            Console.SetCursorPosition(2, 2);
            Console.ForegroundColor = sp > 0 ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
            Console.Write($"Speed:{(sp > 0 ? $"{sp}s" : "off"),-6}");

            Console.SetCursorPosition(13, 2);
            Console.ForegroundColor = col > 0 ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
            Console.Write($"Color:{(col > 0 ? $"{col}s" : "off"),-6}");

            Console.SetCursorPosition(24, 2);
            Console.ForegroundColor = fl > 0 ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
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
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("".PadRight(screenWidth - 4));
            }
        }

        private void DrawUiFrame()
        {
            Console.ForegroundColor = ConsoleColor.Gray;

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
            Console.ForegroundColor = ConsoleColor.Gray;

            for (int x = PlayLeft; x <= PlayRight; x++)
            {
                Console.SetCursorPosition(x, PlayTop);
                Console.Write("■");
                Console.SetCursorPosition(x, PlayBottom);
                Console.Write("■");
            }

            for (int y = PlayTop; y <= PlayBottom; y++)
            {
                Console.SetCursorPosition(PlayLeft, y);
                Console.Write("■");
                Console.SetCursorPosition(PlayRight, y);
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
            int cy = Math.Max(5, (PlayTop + PlayBottom) / 2);

            Console.SetCursorPosition(cx, cy);
            Console.Write($"Game over, Score: {Score}");
            Console.SetCursorPosition(cx, cy + 1);
            Console.Write("Press any key...");
        }
    }

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
