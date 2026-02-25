using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using clean_snake;
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
}
