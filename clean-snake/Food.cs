using System;
using System.Collections.Generic;
using System.Text;
namespace clean_snake
{
    internal enum FoodType
    {
        Apple,
        Chili,
        Mushroom,
        Lemon,
        FlashBerry
    }

    internal readonly struct Food
    {
        public FoodType Type { get; }
        public Point Pos { get; }
        public ConsoleColor Color { get; }
        public int ScoreDelta { get; }
        public int GrowSegments { get; }

        private Food(FoodType type, Point pos, ConsoleColor color, int scoreDelta, int growSegments)
        {
            Type = type;
            Pos = pos;
            Color = color;
            ScoreDelta = scoreDelta;
            GrowSegments = growSegments;
        }

        public static Food Create(FoodType type, Point pos)
        {
            return type switch
            {
                FoodType.Apple => new Food(type, pos, ConsoleColor.Cyan, 1, 1),
                FoodType.Chili => new Food(type, pos, ConsoleColor.Red, 2, 1),
                FoodType.Mushroom => new Food(type, pos, ConsoleColor.Magenta, 1, 1),
                FoodType.Lemon => new Food(type, pos, ConsoleColor.Yellow, -1, 0),
                FoodType.FlashBerry => new Food(type, pos, ConsoleColor.White, 3, 1),
                _ => new Food(FoodType.Apple, pos, ConsoleColor.Cyan, 1, 1),
            };
        }
    }
}
