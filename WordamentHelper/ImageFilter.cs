using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Tesseract;
using WordamentHelper.Helper;
using WordamentHelper.WordFiles;

namespace WordamentHelper
{
    class ImageFilter
    {
        private int _totalOffsetX;
        private Bitmap GetPlayingfield(Bitmap screenShot)
        {
            Point leftTopPoint = GetLeftTopPoint(screenShot);
            Point rightBottomPoint = GetRightBottomPoint(screenShot);

            int roiWidth = rightBottomPoint.X - leftTopPoint.X;
            int roiHeight = rightBottomPoint.Y - leftTopPoint.Y;

            Rectangle roiRectangle = new Rectangle(leftTopPoint, new Size(roiWidth, roiHeight));

            var playingField = screenShot.Clone(roiRectangle, screenShot.PixelFormat);

            SetLetterLocation(leftTopPoint, roiHeight);

            return playingField;
        }

        private Point GetLeftTopPoint(Bitmap screenShot)
        {
            for (int x = 0; x < screenShot.Width; x++)
            {
                for (int y = 0; y < screenShot.Height; y++)
                {
                    var colorBeige = Color.FromArgb(222, 218, 209);
                    if (screenShot.GetPixel(x, y) == colorBeige)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(0, 0);
        }

        private Point GetRightBottomPoint(Bitmap screenShot)
        {
            for (int y = screenShot.Height-1; y > 0; y--)
            {
                for (int x = screenShot.Width-1; x > 0; x--)
                {
                    var colorBeige = Color.FromArgb(222, 218, 209);
                    if (screenShot.GetPixel(x, y) == colorBeige)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(0, 0);
        }

        private void SetLetterLocation(Point initLocation, int roiHeight)
        {
            int resOfBlock = roiHeight * 124 / 550;
            int resOfBetween = roiHeight * 18 / 550;

            int localOffset = resOfBlock/2;


            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Wordament.LetterLocationOnscreen[x, y].X = _totalOffsetX + initLocation.X + localOffset + x * (resOfBlock + resOfBetween);
                    Wordament.LetterLocationOnscreen[x, y].Y = initLocation.Y + localOffset + y * (resOfBlock + resOfBetween);
                }
            }
        }

        private Bitmap []ConstructGroups(Bitmap extractLetters)
        {
            Bitmap[] extractedLetterGroup = new Bitmap[4];

            int heightOfBlock = extractLetters.Height * 124 / 550;
            int heightOfBetween = extractLetters.Height * 18 / 550;

            for (int i = 0; i < 4; i++)
            {
                int startPointY = i*(heightOfBlock + heightOfBetween);
                int marginTop = Convert.ToInt32(Math.Round(heightOfBlock/3.6));
                startPointY += marginTop;

                int height = heightOfBlock - marginTop;
                if ((height + startPointY) > extractLetters.Height)
                    height = extractLetters.Height - startPointY;

                var extractedLetterGroupRow = extractLetters.Clone(new Rectangle(new Point(0, startPointY), new Size(extractLetters.Width, height)), PixelFormat.DontCare);

                extractedLetterGroupRow.Save(Program.ScreenshotDir + "WordamentGroup" + i + ".png");

                extractedLetterGroup[i] = extractedLetterGroupRow;
            }

            return extractedLetterGroup;
        }

        private Bitmap[] SetThreshold(Bitmap[] letterGroup)
        {
            int i = 0;
            foreach (Bitmap bitmap in letterGroup)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        if (bitmap.GetPixel(x, y).ToArgb() > (-16777216*0.65))
                        {
                            bitmap.SetPixel(x, y, Color.Black);
                        }
                        else
                        {
                            bitmap.SetPixel(x, y, Color.White);
                        }
                    }
                }

                bitmap.Save(Program.ScreenshotDir + "WordamentGroup" + i + "_Black.png");
                i++;
            }
            return letterGroup;
        }

        public char[] RetrieveCharacters()
        {
            DateTime now = DateTime.Now;

            Bitmap imageFile;
            if(Program.UseCaptureScreenshot)
                imageFile = new Bitmap(ScreenCaptureHelper.CaptureDesktop());
            else
                imageFile = GetImageByFile();

            Bitmap roi = GetPlayingfield(imageFile);

            Bitmap[] constructGroups = ConstructGroups(roi);

            Bitmap[] defineBackground = SetThreshold(constructGroups);

            List<char> chars = new List<char>();

            foreach (Bitmap bitmap in defineBackground)
            {
                chars.AddRange(RetrieveCharsByOcr(bitmap, Program.TessDataDir));
            }

            var charArray = chars.ToArray();

            double amountmiliseconds1 = (DateTime.Now - now).TotalMilliseconds;
            Console.WriteLine("Time retrieving chardata from image: " + amountmiliseconds1);

            return charArray;
        }

        private Bitmap GetImageByFile()
        {
            String fileName = Program.FileName;

            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            return new Bitmap(fileName);
        }

        private char[] RetrieveCharsByOcr(Bitmap data, string tessDataDir)
        {
            //            ocr.SetVariable("tessedit_char_blacklist", "0123456789"); // no digit
            //            ocr.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyz"); // yes letters
            //            ocr.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            TesseractEngine engine = new TesseractEngine(tessDataDir, "eng", EngineMode.Default);
            engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            engine.SetVariable("tessedit_char_blacklist", "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=\\|/?"); // no digit
            Page page = engine.Process(data, PageSegMode.SingleWord);

            //Console.WriteLine("{0} : {1}", page.GetMeanConfidence(), page.GetText());

            return page.GetText().Replace("\n", "").ToLower().ToCharArray();
        }

        public void GetNewWords()
        {
            Bitmap imageFile;
            if (Program.UseCaptureScreenshot)
                imageFile = new Bitmap(ScreenCaptureHelper.CaptureDesktop());
            else
                imageFile = GetImageByFile();


//            imageFile = new Bitmap("../../../data/results.png");

            List<string> newCommonWords = GetNewCommonWords(imageFile);
            NewWordHelper.SaveCommonWords(newCommonWords);

            List<string> newObscureWords = GetNewObscureWords(imageFile);
            NewWordHelper.SaveObscureWords(newObscureWords);
        }

        private List<String> GetNewCommonWords(Bitmap imageFile)
        {
            Bitmap extractMissedCommonWordsInImage = ExtractMissedCommonWordsInImage(imageFile);

            TesseractEngine engine = new TesseractEngine(Program.TessDataDir, "eng", EngineMode.Default);
            engine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyz");
            engine.SetVariable("tessedit_char_blacklist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=\\|/?"); // no digit
            Page page = engine.Process(extractMissedCommonWordsInImage, PageSegMode.SingleBlock);

            String text = page.GetText();

            return text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private List<String> GetNewObscureWords(Bitmap imageFile)
        {
            Bitmap extractMissedObscureWordsInImage = ExtractMissedObscureWordsInImage(imageFile);
            
            TesseractEngine engine = new TesseractEngine(Program.TessDataDir, "eng", EngineMode.Default);
            engine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyz");
            engine.SetVariable("tessedit_char_blacklist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=\\|/?"); // no digit
            Page page = engine.Process(extractMissedObscureWordsInImage, PageSegMode.SingleBlock);

            String text = page.GetText();
            
            return text.Split(new []{"\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private Bitmap ExtractMissedCommonWordsInImage(Bitmap imageFile)
        {
            //TODO the coords need te be calculated
            Rectangle mask = new Rectangle(1320,414,144,522);
            
            return imageFile.Clone(mask, PixelFormat.DontCare);
        }

        private Bitmap ExtractMissedObscureWordsInImage(Bitmap imageFile)
        {
            //TODO the coords need te be calculated
            Rectangle mask = new Rectangle(1544,414,144,522);
            
            return imageFile.Clone(mask, PixelFormat.DontCare);
        }
    }
}
