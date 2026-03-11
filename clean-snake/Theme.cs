using System;

namespace clean_snake
{
    public class Theme
    {
        public BackgroundColor BackgroundColor { get; }
        public DefaultSnakeColor DefaultSnakeColor { get; }
        public UiColor UiColor { get; }
        public WallColor WallColor { get; }
        public UiAccentColor UiAccentColor { get; }
        public UiInactiveColor UiInactiveColor { get; }

        public Theme(BackgroundColor background, DefaultSnakeColor snake, UiColor uiColor, WallColor wallColor, UiAccentColor uiAccentColor, UiInactiveColor uiInactiveColor)
        {
            BackgroundColor = background;
            DefaultSnakeColor = snake;
            UiColor = uiColor;
            WallColor = wallColor;
            UiAccentColor = uiAccentColor;
            UiInactiveColor = uiInactiveColor;
        }

        public static Theme GetThemeById(int id)
        {
            return id switch
            {
                1 => new Theme(new BackgroundColor(ConsoleColor.DarkRed), new DefaultSnakeColor(ConsoleColor.White), new UiColor(ConsoleColor.Gray), new WallColor(ConsoleColor.White), new UiAccentColor(ConsoleColor.Yellow), new UiInactiveColor(ConsoleColor.DarkRed)),
                2 => new Theme(new BackgroundColor(ConsoleColor.DarkBlue), new DefaultSnakeColor(ConsoleColor.Cyan), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.Cyan), new UiAccentColor(ConsoleColor.Yellow), new UiInactiveColor(ConsoleColor.DarkBlue)),
                3 => new Theme(new BackgroundColor(ConsoleColor.DarkGreen), new DefaultSnakeColor(ConsoleColor.Yellow), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.Yellow), new UiAccentColor(ConsoleColor.Cyan), new UiInactiveColor(ConsoleColor.DarkGreen)),
                4 => new Theme(new BackgroundColor(ConsoleColor.DarkYellow), new DefaultSnakeColor(ConsoleColor.Black), new UiColor(ConsoleColor.Black), new WallColor(ConsoleColor.Black), new UiAccentColor(ConsoleColor.DarkBlue), new UiInactiveColor(ConsoleColor.DarkYellow)),
                5 => new Theme(new BackgroundColor(ConsoleColor.DarkMagenta), new DefaultSnakeColor(ConsoleColor.White), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.White), new UiAccentColor(ConsoleColor.Yellow), new UiInactiveColor(ConsoleColor.DarkMagenta)),
                6 => new Theme(new BackgroundColor(ConsoleColor.Black), new DefaultSnakeColor(ConsoleColor.Green), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.Gray), new UiAccentColor(ConsoleColor.Cyan), new UiInactiveColor(ConsoleColor.Black)),
                7 => new Theme(new BackgroundColor(ConsoleColor.White), new DefaultSnakeColor(ConsoleColor.DarkBlue), new UiColor(ConsoleColor.Black), new WallColor(ConsoleColor.DarkGray), new UiAccentColor(ConsoleColor.DarkBlue), new UiInactiveColor(ConsoleColor.White)),
                8 => new Theme(new BackgroundColor(ConsoleColor.DarkGray), new DefaultSnakeColor(ConsoleColor.Magenta), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.White), new UiAccentColor(ConsoleColor.Cyan), new UiInactiveColor(ConsoleColor.DarkGray)),
                9 => new Theme(new BackgroundColor(ConsoleColor.Gray), new DefaultSnakeColor(ConsoleColor.Black), new UiColor(ConsoleColor.Black), new WallColor(ConsoleColor.Black), new UiAccentColor(ConsoleColor.DarkBlue), new UiInactiveColor(ConsoleColor.Gray)),
                _ => new Theme(new BackgroundColor(ConsoleColor.Black), new DefaultSnakeColor(ConsoleColor.Red), new UiColor(ConsoleColor.White), new WallColor(ConsoleColor.Gray), new UiAccentColor(ConsoleColor.Cyan), new UiInactiveColor(ConsoleColor.Black)) // Default
            };
        }
    }
}
