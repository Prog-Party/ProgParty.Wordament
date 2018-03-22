using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WordamentHelper.Classes;
using WordamentHelper.WordFiles;

namespace WordamentHelper
{
    public class WordGetter
    {
        private int _totalRecursive = 0;

        public List<Word> GetWords(Letter[,] grid)
        {
            DateTime now = DateTime.Now;
            var allWords = new List<Word>();
            _totalRecursive = 0;
            foreach (Letter letter in grid)
            {
                String word = "";
                int currentDepth = 0;
                var currentPath = new List<Point>();
                
                List<Word> words = ConstructWord(grid, letter, word, currentDepth, currentPath);
                
                allWords.AddRange(words);   
                }

            //Remove duplicates
            allWords = allWords.GroupBy(p => p.FullWord).Select(g => g.First()).OrderByDescending(w => w.FullWord.Length).ToList();

            double timePassed = (DateTime.Now - now).TotalMilliseconds;

            Console.WriteLine("Time retrieving words:" + timePassed);

            return allWords;
        }

        private List<Word> ConstructWord(Letter[,] grid, Letter currentLetter, String currentWord, int currentDepth, List<Point> currentPath)
        {
            _totalRecursive++;

            var allWords = new List<Word>();
            currentWord += currentLetter.Text;
            currentDepth++;
            currentPath.Add(currentLetter.LocationInGrid);

            if (!WordOrWordPartExists(currentWord))
                return allWords;

            if(WordIsRealWord(currentWord))
                allWords.Add(new Word() { FullWord = currentWord, Path = currentPath});

            if(currentDepth >= Program.MaxDepth)
                return allWords;

            
            Letter[,] newGrid = GenerateNewGrid(grid, currentLetter);

            var neighbors = currentLetter.GetNeighbors(newGrid);
            foreach (Letter neighbor in neighbors)
            {
                List<Point> newPath = currentPath.Select(point => new Point(point.X, point.Y)).ToList();
                allWords.AddRange(ConstructWord(newGrid, neighbor, currentWord, currentDepth, newPath));
            }
            
            return allWords;
        }

        Dictionary<String, List<String>> _realWords = new Dictionary<string, List<string>>();

        /// <summary>
        /// "dus" is a dutch word, so it exists, return true
        /// for example, the word = "mogel"
        ///  "mogel" is not a dutch word, BUT
        ///  "mogelijk" is a dutch word, so it exists, return true
        /// for example, the word = "xals"
        ///  no words start with "xals", return false
        /// </summary>
        /// <param name="currentWord"></param>
        /// <returns></returns>
        private bool WordOrWordPartExists(string currentWord)
        {
            if (currentWord.Length < 3)
                return true;
            if (currentWord.Length == 3)
            {
                if (!_realWords.ContainsKey(currentWord))
                {
                    List<String> realWords;
                    if(Program.UseOnlineDictionary)
                    { 
                        var wordCheckOnline = new WordCheckOnline();
                        realWords = wordCheckOnline.CheckWordOnline(currentWord);
                    }
                    else
                    {
                        var wordCheckOnline = new WordCheckOpenTaal();
                        realWords = wordCheckOnline.CheckWordOnline(currentWord);    
                    }
                    
                    _realWords.Add(currentWord, realWords);
                }
            }

            return WordContainsStartOfWord(currentWord);
        }

        private bool WordContainsStartOfWord(string currentWord)
        {
            if (currentWord.Length < 3)
                return false;

            String firstThreeChars = currentWord.Substring(0, 3);

            if (!_realWords.ContainsKey(firstThreeChars))
                return false;

            foreach (string s in _realWords[firstThreeChars])
            {
                if (s.StartsWith(currentWord))
                    return true;
            }
            return false;
        }

        private bool WordIsRealWord(string currentWord)
        {
            if (currentWord.Length < 3)
                return false;

            String firstThreeChars = currentWord.Substring(0, 3);

            if (!_realWords.ContainsKey(firstThreeChars))
                return false;

            return _realWords[firstThreeChars].Contains(currentWord);
        }

        private Letter[,] GenerateNewGrid(Letter[,] grid, Letter currentLetter)
        {
            var newGrid = new Letter[4,4];
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Letter letter = grid[x, y];
                    bool isUsed = letter.IsUsed;

                    if (currentLetter.LocationInGrid.X == x && currentLetter.LocationInGrid.Y == y)
                        isUsed = true;

                    Letter newLetter = new Letter(letter, letter.LocationInGrid, isUsed);
                    newGrid[x, y] = newLetter;
                }
            }
            return newGrid;
        }
    }
}
