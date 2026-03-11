using System;

namespace clean_snake
{
    public class ColorScheme
    {
        internal BackgroundColor BackgroundColor { get; }
        internal DefaultSnakeColor DefaultSnakeColor { get; }
        internal UiColor UiColor { get; }
        internal WallColor WallColor { get; }
        internal UiAccentColor UiAccentColor { get; }
        internal UiInactiveColor UiInactiveColor { get; }

        internal ColorScheme(BackgroundColor background, DefaultSnakeColor snake, 
                        UiColor uiColor, WallColor wallColor, UiAccentColor uiAccentColor, 
                        UiInactiveColor uiInactiveColor)
        {
            BackgroundColor = background;
            DefaultSnakeColor = snake;
            UiColor = uiColor;
            WallColor = wallColor;
            UiAccentColor = uiAccentColor;
            UiInactiveColor = uiInactiveColor;
        }

        public static ColorScheme GetColorSchemeById(int id)
        {
            return id switch
            {
                1 => new ColorScheme(
                    new BackgroundColor(ConsoleColor.DarkRed), 
                    new DefaultSnakeColor(ConsoleColor.White), 
                    new UiColor(ConsoleColor.Gray), 
                    new WallColor(ConsoleColor.White), 
                    new UiAccentColor(ConsoleColor.Yellow), 
                    new UiInactiveColor(ConsoleColor.DarkRed)
                    ),
                2 => new ColorScheme(
                    new BackgroundColor(ConsoleColor.DarkBlue), 
                    new DefaultSnakeColor(ConsoleColor.Cyan), 
                    new UiColor(ConsoleColor.White), 
                    new WallColor(ConsoleColor.Cyan), 
                    new UiAccentColor(ConsoleColor.Yellow), 
                    new UiInactiveColor(ConsoleColor.DarkBlue)
                    ),
                3 => new ColorScheme(
                    new BackgroundColor(ConsoleColor.DarkGreen), 
                    new DefaultSnakeColor(ConsoleColor.Yellow), 
                    new UiColor(ConsoleColor.White), 
                    new WallColor(ConsoleColor.Yellow), 
                    new UiAccentColor(ConsoleColor.Cyan), 
                    new UiInactiveColor(ConsoleColor.DarkGreen)
                    ),
                _ => new ColorScheme(new BackgroundColor(ConsoleColor.Black), new DefaultSnakeColor(ConsoleColor.Red), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.Gray), new UiAccentColor(ConsoleColor.Cyan), new UiInactiveColor(ConsoleColor.Black)) // Default
            };
        }
    }
}
