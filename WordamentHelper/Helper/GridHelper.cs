using System;
using System.Drawing;
using WordamentHelper.Classes;

namespace WordamentHelper.Helper
{
    public class GridHelper
    {
        public static Letter[,] ConstructGrid(Char[] chars)
        {
            var letterGrid = new Letter[4, 4];

            for (int c = 0; c < 4; c++)
            {
                for (int r = 0; r < 4; r++)
                {
                    letterGrid[c, r] = new Letter(chars[c * 4 + r].ToString(), new Point(c, r));
                }
            }
            return letterGrid;
        }
    }
}
