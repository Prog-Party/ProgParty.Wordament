using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WordamentHelper.Classes;
using WordamentHelper.Helper;
using WordamentHelper.Mouse;
using WordamentHelper.WordFiles;

namespace WordamentHelper
{
    class Wordament
    {
        public static Point[,] LetterLocationOnscreen = new Point[4, 4];

        public void ComputeWords()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            var imageFilter = new ImageFilter();

            char[] chars = imageFilter.RetrieveCharacters();
            Letter[,] letterGrid = GridHelper.ConstructGrid(chars);

            var wordGetter = new WordGetter();
            List<Word> words = wordGetter.GetWords(letterGrid);
            
            MouseOutput.SelectWordsInGridByCursor(s, words);
        }

        public void GetNewWords()
        {
            var imageFilter = new ImageFilter();
            imageFilter.GetNewWords();
        }
    }
}