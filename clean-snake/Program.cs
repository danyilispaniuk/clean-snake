    using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
namespace clean_snake
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

            backgroundColor = Window.SetTheme();

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
