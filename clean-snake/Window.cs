using System;

namespace clean_snake
{
    public class Window
    {
        public int windowHeight { get; set; }
        public int windowWidth { get; set; }
        public ColorScheme colorScheme { get; set; }
        const int defaultSize = 25;
        const int defultColorScheme = 6;

        public Window()
        {
            windowHeight = 400;
            windowWidth = 600;
            colorScheme = ColorScheme.GetColorSchemeById(defultColorScheme);
        }

        public Window(int windowHeight, int windowWidth, int themeId)
        {
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            colorScheme = ColorScheme.GetColorSchemeById(themeId);
        }

        public void Apply()
        {
            Console.WindowHeight = windowHeight;
            Console.WindowWidth = windowWidth;
            Console.BackgroundColor = colorScheme.BackgroundColor;
            Console.Clear();
        }

        public static int SetWindowSize(string type)
        {
            bool isInputValid = false;
            int size = defaultSize;

            while (!isInputValid)
            {
                try
                {
                    Console.WriteLine("Enter a width of game window (minimum 25):");
                    size = int.Parse(s: Console.ReadLine());

                    if (size < defaultSize)
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

        public static int SetTheme()
        {
            bool isInputValid = false;
            int themeNumber = defultColorScheme;

            while (!isInputValid)
            {
                try
                {
                    Console.WriteLine("Choose a background color:");
                    Console.WriteLine("1. Red\t\t2. Blue\t\t3. Green\n4-9. Black");
                    themeNumber = int.Parse(s: Console.ReadLine());

                    if (themeNumber < 1 || themeNumber > 9)
                    {
                        throw new Exception($"Choose the theme from 1 to 9");
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

            return themeNumber;
        }

    }

}
