using System;

namespace clean_snake
{
    public class Window
    {
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public int BackgroundColor { get; set; }

        public Window()
        {
            WindowHeight = 400;
            WindowWidth = 600;
            BackgroundColor = ((int)Console.BackgroundColor);
        }

        public Window(int windowHeight, int windowWidth, int backgroundColor)
        {
            WindowHeight = windowHeight;
            WindowWidth = windowWidth;
            BackgroundColor = backgroundColor;
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
                _ => ConsoleColor.Black // Default
            };
            Console.BackgroundColor = backgroundcolor;
            Console.Clear();
        }

        public static int SetWindowSize(string type)
        {
            bool isInputValid = false;
            int size = 25; //Default size

            while (!isInputValid)
            {
                try
                {
                    Console.WriteLine("Enter a width of game window (minimum 25):");
                    size = int.Parse(Console.ReadLine());

                    if (size < 25)
                    {
                        throw new Exception($"The {type} is too small! Please enter 25 or more.");
                    }

                    isInputValid = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("That's not a number! Please enter digits only.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return size;
        }
    }
}
