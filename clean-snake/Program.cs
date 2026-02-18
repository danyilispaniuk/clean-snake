// Source - https://codereview.stackexchange.com/q/127515
// Posted by Wagacca, modified by community. See post 'Timeline' for change history
// Retrieved 2026-02-11, License - CC BY-SA 3.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
///█ ■
////https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    class Window
    {
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public int BackgroundColor { get; set; }
        //public int ForegroundColor { get; set; }

        public Window(){
            WindowHeight = 400;
            WindowWidth = 600;
            //ForegroundColor = ((int)Console.ForegroundColor);
            BackgroundColor = ((int)Console.BackgroundColor);
        }

        public Window(int windowHeight, int windowWidth, int backgroundColor, int foregroundColor)
        {
            WindowHeight = windowHeight;
            WindowWidth = windowWidth;
            //ForegroundColor = foregroundColor;
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
            //Console.ForegroundColor = (ConsoleColor)ForegroundColor;
            Console.Clear();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a width of game window:");
            int screenwidth = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter a height of game window:");
            int screenheight = int.Parse(Console.ReadLine());

            Console.WriteLine("Choose a background color:");
            Console.WriteLine("1. Red\t\t2. Blue\t\t3. Green\n4. Yellow\t5. Magenta\t6. Black\n7. White\t8. Dark grey\t9. Grey");
            int backgroundColor = int.Parse(Console.ReadLine());

            // Create and apply the window
            Window gameWindow = new Window(screenheight, screenwidth, backgroundColor, (int)ConsoleColor.White);
            gameWindow.Apply();

            Random randomnummer = new Random();
            int score = 5;
            int gameover = 0;
            Pixel hoofd = new Pixel();
            hoofd.xpos = screenwidth / 2;
            hoofd.ypos = screenheight / 2;
            hoofd.schermkleur = ConsoleColor.Red;
            string movement = "RIGHT";
            List<int> xposlijf = new List<int>();
            List<int> yposlijf = new List<int>();
            int berryx = randomnummer.Next(0, screenwidth);
            int berryy = randomnummer.Next(0, screenheight);
            DateTime tijd = DateTime.Now;
            DateTime tijd2 = DateTime.Now;
            string buttonpressed = "no";
            while (true)
            {
                Console.Clear();
                if (hoofd.xpos == screenwidth - 1 || hoofd.xpos == 0 || hoofd.ypos == screenheight - 1 || hoofd.ypos == 0)
                {
                    gameover = 1;
                }
                for (int i = 0; i < screenwidth; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("■");
                }
                for (int i = 0; i < screenwidth; i++)
                {
                    Console.SetCursorPosition(i, screenheight - 1);
                    Console.Write("■");
                }
                for (int i = 0; i < screenheight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("■");
                }
                for (int i = 0; i < screenheight; i++)
                {
                    Console.SetCursorPosition(screenwidth - 1, i);
                    Console.Write("■");
                }
                Console.ForegroundColor = ConsoleColor.Green;
                if (berryx == hoofd.xpos && berryy == hoofd.ypos)
                {
                    score++;
                    berryx = randomnummer.Next(1, screenwidth - 2);
                    berryy = randomnummer.Next(1, screenheight - 2);
                }
                for (int i = 0; i < xposlijf.Count(); i++)
                {
                    Console.SetCursorPosition(xposlijf[i], yposlijf[i]);
                    Console.Write("■");
                    if (xposlijf[i] == hoofd.xpos && yposlijf[i] == hoofd.ypos)
                    {
                        gameover = 1;
                    }
                }
                if (gameover == 1)
                {
                    break;
                }
                Console.SetCursorPosition(hoofd.xpos, hoofd.ypos);
                Console.ForegroundColor = hoofd.schermkleur;
                Console.Write("■");
                Console.SetCursorPosition(berryx, berryy);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("■");
                tijd = DateTime.Now;
                buttonpressed = "no";
                while (true)
                {
                    tijd2 = DateTime.Now;
                    if (tijd2.Subtract(tijd).TotalMilliseconds > 500) { break; }
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo toets = Console.ReadKey(true);
                        //Console.WriteLine(toets.Key.ToString());
                        if (toets.Key.Equals(ConsoleKey.UpArrow) && movement != "DOWN" && buttonpressed == "no")
                        {
                            movement = "UP";
                            buttonpressed = "yes";
                        }
                        if (toets.Key.Equals(ConsoleKey.DownArrow) && movement != "UP" && buttonpressed == "no")
                        {
                            movement = "DOWN";
                            buttonpressed = "yes";
                        }
                        if (toets.Key.Equals(ConsoleKey.LeftArrow) && movement != "RIGHT" && buttonpressed == "no")
                        {
                            movement = "LEFT";
                            buttonpressed = "yes";
                        }
                        if (toets.Key.Equals(ConsoleKey.RightArrow) && movement != "LEFT" && buttonpressed == "no")
                        {
                            movement = "RIGHT";
                            buttonpressed = "yes";
                        }
                    }
                }
                xposlijf.Add(hoofd.xpos);
                yposlijf.Add(hoofd.ypos);
                switch (movement)
                {
                    case "UP":
                        hoofd.ypos--;
                        break;
                    case "DOWN":
                        hoofd.ypos++;
                        break;
                    case "LEFT":
                        hoofd.xpos--;
                        break;
                    case "RIGHT":
                        hoofd.xpos++;
                        break;
                }
                if (xposlijf.Count() > score)
                {
                    xposlijf.RemoveAt(0);
                    yposlijf.RemoveAt(0);
                }
            }
            Console.SetCursorPosition(screenwidth / 5, screenheight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(screenwidth / 5, screenheight / 2 + 1);
        }
        class Pixel
        {
            public int xpos { get; set; }
            public int ypos { get; set; }
            public ConsoleColor schermkleur { get; set; }
        }
    }
}
//¦