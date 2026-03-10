using clean_snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
namespace Snake
{

    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            int screenwidth = 0;
            int screenheight = 0;
            int backgroundColor = 6; 


            screenwidth = Window.SetWindowSize("width");
            Console.WriteLine(value: $"Width set to {screenwidth}.");

            screenheight = Window.SetWindowSize("height");
            Console.WriteLine(value: $"height set to {screenheight}.");

            //Console.WriteLine("Choose a background color:");
            //Console.WriteLine("1. Red\t\t2. Blue\t\t3. Green\n4. Yellow\t5. Magenta\t6. Black\n7. White\t8. Dark grey\t9. Grey");
            //int backgroundColor = int.Parse(Console.ReadLine());
            backgroundColor = Window.SetTheme();

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
