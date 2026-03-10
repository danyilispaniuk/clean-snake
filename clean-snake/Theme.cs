using System;

namespace clean_snake
{
    public class Theme
    {
        public ConsoleColor BackgroundColor { get; }
        public ConsoleColor DefaultSnakeColor { get; }
        public ConsoleColor UiColor { get; }
        public ConsoleColor WallColor { get; }
        public ConsoleColor UiAccentColor { get; }
        public ConsoleColor UiInactiveColor { get; }

        public Theme(ConsoleColor background, ConsoleColor snake, ConsoleColor uiColor, ConsoleColor wallColor, ConsoleColor uiAccentColor, ConsoleColor uiInactiveColor)
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
                1 => new Theme(ConsoleColor.DarkRed, ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.DarkGray),
                2 => new Theme(ConsoleColor.DarkBlue, ConsoleColor.Cyan, ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Gray),
                3 => new Theme(ConsoleColor.DarkGreen, ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Gray),
                4 => new Theme(ConsoleColor.DarkYellow, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.DarkBlue, ConsoleColor.DarkGray),
                5 => new Theme(ConsoleColor.DarkMagenta, ConsoleColor.White, ConsoleColor.White, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Gray),
                6 => new Theme(ConsoleColor.Black, ConsoleColor.Green, ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.DarkGray),
                7 => new Theme(ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Black, ConsoleColor.DarkGray, ConsoleColor.DarkBlue, ConsoleColor.Gray),
                8 => new Theme(ConsoleColor.DarkGray, ConsoleColor.Magenta, ConsoleColor.White, ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Gray),
                9 => new Theme(ConsoleColor.Gray, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.DarkBlue, ConsoleColor.DarkGray),
                _ => new Theme(ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.DarkGray) // Default
            };
        }
    }
}
