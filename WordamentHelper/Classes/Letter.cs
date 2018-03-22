using System;
using System.Collections.Generic;
using System.Drawing;

namespace WordamentHelper.Classes
{
    public class Letter
    {
        private List<Letter> _neighborLetters = new List<Letter>();
        public Point LocationInGrid = new Point();
        public bool IsUsed = false;
        public String Text = String.Empty;

        public Letter(String text, Point locationInGrid)
        {
            Text = text;
            LocationInGrid = locationInGrid;
        }

        public Letter(Letter letter, Point locationInGrid, bool isUsed) : this(letter.Text, locationInGrid) 
        {
            IsUsed = isUsed;
        }

        public List<Letter> NeighborLetters
        {
            get { return _neighborLetters; }
            set { _neighborLetters = value; }

        }

        public void SetNeighbor(Letter[,] letterGrid)
        {
            IEnumerable<Letter> neighbors = GetNeighbors(letterGrid);
            NeighborLetters.AddRange(neighbors);
        }
        public List<Letter> GetNeighbors(Letter[,] grid)
        {
            var neighbors = new List<Letter>();
            int curX = LocationInGrid.X;
            int curY = LocationInGrid.Y;

            for (int yCount = -1; yCount <= 1; yCount++)
            {
                for (int xCount = -1; xCount <= 1; xCount++)
                {
                    int newX = curX + xCount;
                    int newY = curY + yCount;

                    // boundery and self check 
                    if ((newY < 0) || (newX < 0) || (newY > 3) || (newX > 3) ||
                        (((yCount == 0) && (xCount == 0))))
                        continue;

                    Letter letterToReturn = grid[newX, newY];
                    if (letterToReturn.IsUsed)
                        continue;

                    neighbors.Add(letterToReturn);
                }
            }

            return neighbors;
        }

        public override string ToString()
        {
            return String.Format("[{1},{2}] Letter {0}", Text, LocationInGrid.X, LocationInGrid.Y);
        }
    }
}
