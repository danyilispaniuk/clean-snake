using System;

namespace clean_snake
{
    public class Window
    {
        public int windowHeight { get; set; }
        public int windowWidth { get; set; }
        public ThemeColorScheme theme { get; set; }

        public Window()
        {
            windowHeight = 400;
            windowWidth = 600;
            theme = ThemeColorScheme.GetThemeById(6); // Default Black theme
        }

        public Window(int windowHeight, int windowWidth, int themeId)
        {
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            theme = ThemeColorScheme.GetThemeById(themeId);
        }

        public void Apply()
        {
            Console.WindowHeight = windowHeight;
            Console.WindowWidth = windowWidth;
            Console.BackgroundColor = theme.BackgroundColor;
            Console.Clear();
        }

        public static int SetWindowSize(string type)
        {
            bool isInputValid = false;
            int size = 25; // Default size

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

        public static int SetTheme()
        {
            bool isInputValid = false;
            int themeNumber = 6; // Default black theme

            while (!isInputValid)
            {
                try
                {
                    Console.WriteLine("Choose a background color:");
                    Console.WriteLine("1. Red\t\t2. Blue\t\t3. Green\n4-9. Black");
                    themeNumber = int.Parse(Console.ReadLine());

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
