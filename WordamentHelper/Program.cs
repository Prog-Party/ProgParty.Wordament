using System;
using System.IO;
using System.Threading;

namespace WordamentHelper
{
    internal class Program
    {
        public const bool IsDennisLaptop = false;
        public const int MaxDepth = 16;
        public const int SleepTimePerWord = 100;
        public const int SleepTimePerLetter = 10;
        public const int StopAfter = 115000;//115000; // stops after 1 minute 50 seconds

        public const bool UseOnlineDictionary = false;
        public const bool UseCaptureScreenshot = true;

        public static string FileName { get; set; }
        public static string TessDataDir { get; set; }
        public static string ScreenshotDir { get; set; }

        private static void Main(string[] args)
        {   
            string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            FileName = solutionDirectory + "\\data\\screen_shot_dennis.png";
            TessDataDir = solutionDirectory + "\\tessdata";
            ScreenshotDir = solutionDirectory + "\\data\\Screenshots\\";

            Wordament wordament = new Wordament();

            //InterceptKeys i = new InterceptKeys();

            //While loop is used for multiple games
            while (true)
            {
                var v = Console.ReadLine();
                if (v == "retard")
                    break;

        //MouseOutput.DoTest(); 
                wordament.ComputeWords();

                Thread.Sleep(10000); // wait 10 sec to be sure that result screen is displayed
                wordament.GetNewWords();
            }
        }
    }
}