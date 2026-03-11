using System;

namespace clean_snake
{
    public class ThemeColor
    {
        public ConsoleColor Color { get; }

        public ThemeColor(ConsoleColor color)
        {
            this.Color = color;
        }

        public static implicit operator ConsoleColor(ThemeColor themeColor) => themeColor.Color;
    }
}
