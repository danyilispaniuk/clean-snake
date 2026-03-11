using System;

namespace clean_snake
{
    public class Window
    {
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public Theme Theme { get; set; }

        public Window()
        {
            WindowHeight = 400;
            WindowWidth = 600;
            Theme = Theme.GetThemeById(6); // Default Black theme
        }

        public Window(int windowHeight, int windowWidth, int themeId)
        {
            WindowHeight = windowHeight;
            WindowWidth = windowWidth;
            Theme = Theme.GetThemeById(themeId);
        }

        public void Apply()
        {
            Console.WindowHeight = WindowHeight;
            Console.WindowWidth = WindowWidth;
            Console.BackgroundColor = Theme.BackgroundColor;
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
                    Console.WriteLine("1. Red\t\t2. Blue\t\t3. Green\n4. Yellow\t5. Magenta\t6. Black\n7. White\t8. Dark grey\t9. Grey");
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
