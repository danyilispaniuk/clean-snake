using System;

namespace clean_snake
{
    internal class BaseColor
    {
        public ConsoleColor color { get; }

        public BaseColor(ConsoleColor color)
        {
            this.color = color;
        }

        public static implicit operator ConsoleColor(BaseColor baseColor) => baseColor.color;
    }
}
