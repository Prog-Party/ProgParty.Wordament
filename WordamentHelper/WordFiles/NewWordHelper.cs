using System;
using System.Collections.Generic;
using System.IO;

namespace WordamentHelper.WordFiles
{
    class NewWordHelper
    {
        public static void SaveCommonWords(List<String> newCommonWords)
        {
            String fullName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            using (StreamWriter sw = File.AppendText(fullName + "/WordFiles/WordsCommon.txt"))
            {
                foreach (String newCommonWord in newCommonWords)
                {
                    sw.WriteLine(newCommonWord);
                }
            }
        }

        public static void SaveObscureWords(List<String> newObscureWords)
        {
            String fullName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            using (StreamWriter sw = File.AppendText(fullName + "/WordFiles/WordsObscure.txt"))
            {
                foreach (String newObscureWord in newObscureWords)
                {
                    sw.WriteLine(newObscureWord);
                }
            }
        }
    }
}
