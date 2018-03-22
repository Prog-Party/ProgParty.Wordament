using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using WordamentHelper.Classes;

namespace WordamentHelper.Mouse
{
    class MouseOutput
    {
        public static void DoTest()
        {
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            MouseOperations.SetCursorPosition(1000, 500);
            Thread.Sleep(500);
            MouseOperations.SetCursorPosition(900, 400);
            Thread.Sleep(500);
            MouseOperations.SetCursorPosition(800, 500);
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
        }

        public static void SelectWordsInGridByCursor(Stopwatch s, List<Word> wordList)
        {
            while (true)
            {
                foreach (Word word in wordList)
                {
                    if (s.ElapsedMilliseconds > Program.StopAfter)
                        return;

                    SelectWordInGridByCursor(word);
                }
                wordList.Shuffle();
            }
        }

        private static void SelectWordInGridByCursor(Word word)
        {
            Console.WriteLine("busy with word: " + word);
            // set curor begin letter en klik

            SetCursorToPoint(word.Path[0]);

            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);

            for (int i = 1; i < word.Path.Count; i++)
            {
                Console.WriteLine("LETTER: " + word.FullWord[i]);
                //SetCursorToPointTween(word.Path[i - 1], word.Path[i]);
                SetCursorToPoint(word.Path[i]);

                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            }
            
            Thread.Sleep(Program.SleepTimePerLetter / 2);

            // laatste letter klik los
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);

            Thread.Sleep(Program.SleepTimePerWord);
        }

        private static void SetCursorToPointTween(Point pointFrom, Point pointTo)
        {
            Point pointOnScreenFrom = Wordament.LetterLocationOnscreen[pointFrom.Y, pointFrom.X];
            Point pointOnScreenTo = Wordament.LetterLocationOnscreen[pointTo.Y, pointTo.X];
            int amountToRight = (pointOnScreenTo.X - pointOnScreenFrom.X);
            int amountToDown = (pointOnScreenTo.Y - pointOnScreenFrom.Y);


            int max = Math.Abs(amountToRight);
            if (Math.Abs(amountToDown) > max)
                max = Math.Abs(amountToDown);

            int stepSizeX = 0;
            if(amountToRight > 0)
                stepSizeX = 1;
            else if (amountToRight < 0)
                stepSizeX = -1;

            int stepSizeY = 0;
            if (amountToDown > 0)
                stepSizeY = 1;
            else if (amountToDown < 0)
                stepSizeY = -1;

            for (int i = 0; i < max; i++)
            {
                int newX = pointOnScreenFrom.X + i * stepSizeX;
                int newY = pointOnScreenFrom.Y + i * stepSizeY;

                MouseOperations.SetCursorPosition(newX, newY);
                Thread.Sleep(Program.SleepTimePerLetter / max);
            }
        }

        private static void SetCursorToPoint(Point pointInGrid)
        {
            Point pointOnScreen = Wordament.LetterLocationOnscreen[pointInGrid.Y, pointInGrid.X];
            if(Program.IsDennisLaptop)
                MouseOperations.SetCursorPosition(pointOnScreen.X/2, pointOnScreen.Y/2);
            else
            MouseOperations.SetCursorPosition(pointOnScreen.X, pointOnScreen.Y);

            Thread.Sleep(Program.SleepTimePerLetter);
        }
    }
}
