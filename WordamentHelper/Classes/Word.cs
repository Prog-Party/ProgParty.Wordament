using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WordamentHelper.Classes
{
    public class Word
    {
        public String FullWord { get; set; }
        public List<Point> Path { get; set; }

        public override string ToString()
        {
            String pathAsStr = String.Join(",", Path.Select(p => String.Format("[{0},{1}]", p.X, p.Y)));

            return String.Format("{0}: {1}", FullWord, pathAsStr);
        }
    }
}
