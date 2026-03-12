namespace clean_snake
{

    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            const int basicTickMs = 100;

            int screenwidth = Window.SetWindowSize("width");
            Console.WriteLine(value: $"Width is set to {screenwidth}.");

            int screenheight = Window.SetWindowSize("height");
            Console.WriteLine(value: $"Height is set to {screenheight}.");

            int backgroundColor = Window.SetColorScheme();

            Window gameWindow = new Window(screenheight, screenwidth, backgroundColor);
            gameWindow.Apply();

            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            var game = new Game(basicTickMs, gameWindow);
            game.Run();

            Console.SetCursorPosition(2, Console.WindowHeight - 1);
            Console.CursorVisible = true;
            Console.ReadKey(true);
        }
    }
}
