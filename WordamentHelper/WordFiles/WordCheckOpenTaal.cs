using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordamentHelper.WordFiles
{
    public class WordCheckOpenTaal
    {
        public static Dictionary<String, List<String>> AllWords = new Dictionary<string, List<string>>( );

        public List<String> CheckWordOnline(String currentWord)
        {
            if (AllWords.Count == 0)
                ReadAllWords();//takes 160 ms

            String firstThreeChars = currentWord.Substring(0, 3);

            if (AllWords.ContainsKey(firstThreeChars))
                return AllWords[firstThreeChars];

            return new List<string>();
        }

        private void ReadAllWords()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            AddByTxtFile(projectDirectory + "/WordFiles/OpenTaal-210G-basis-gekeurd.txt");
            AddByTxtFile(projectDirectory + "/WordFiles/OpenTaal-210G-basis-ongekeurd.txt");
            AddByTxtFile(projectDirectory + "/WordFiles/OpenTaal-210G-flexievormen.txt");
            AddByTxtFile(projectDirectory + "/WordFiles/OpenTaal-210G-verwarrend.txt");

            AddByTxtFile(projectDirectory + "/WordFiles/WordsCommon.txt");
            AddByTxtFile(projectDirectory + "/WordFiles/WordsObscure.txt");
        }

        private void AddByTxtFile(string filename)
        {
            String file;
            using (StreamReader sr = new StreamReader(filename))
            {
                file = sr.ReadToEnd();
            }
            IEnumerable<String> words = file.Replace("\n", "|").Split('|');
            foreach (string word in words)
            {
                if (word.Length < 3)
                    continue;

                String w = word.ToLower();
                
                String firstThreeChars = w.Substring(0, 3);
                if (!AllWords.ContainsKey(firstThreeChars))
                    AllWords.Add(firstThreeChars, new List<string>());

                AllWords[firstThreeChars].Add(w);
            }
        }
    }
}
