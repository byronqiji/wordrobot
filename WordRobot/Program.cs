using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordRobot
{
    class Program
    {
        private static List<string> lineList = new List<string>();

        private static Dictionary<string, int> wordMap = new Dictionary<string, int>();

        private static Regex reg = new Regex(@"^[A-Za-z0-9]+$");

        static void Main(string[] args)
        {
            FixData();
            SplitWords();
            Console.WriteLine("Over");
        }

        private static void FixData()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Data\\Data.txt";

            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Data\\FixData.txt", false);

            using (StreamReader sr = new StreamReader(path))
            {
                string line = string.Empty;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.Trim().StartsWith("【") || line.StartsWith(" 2017-") || string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    lineList.Add(line.Replace(".", "").Replace("。", "").Replace(",", "").Replace("，", "").Replace("？", "").Replace("/", ""));

                    sw.WriteLine(line);
                }
            }

            sw.Close();
        }

        private static void SplitWords()
        {
            for (int i = 0; i < lineList.Count - 1; ++i)
            {
                for (int j = i + 1; j < lineList.Count; ++j)
                {
                    FindWord(lineList[i], lineList[j]);
                }
            }

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Data\\Words.txt", false))
            {
                foreach (KeyValuePair<string, int> item in wordMap)
                {
                    sw.WriteLine(string.Format("{0}\t{1}", item.Key, item.Value));
                }
            }
        }

        private static void FindWord(string s1, string s2)
        {
            string word = string.Empty;
            string findWord = string.Empty;
            int x = 0;
            for (int i = 0; i < s2.Length; ++i)
            {
                for (int k = 1; k <= s2.Length - i; ++k)
                {
                    word = s2.Substring(i, k).Trim();
                    if (word.Length >= 2 && s1.Contains(word) && !reg.IsMatch(word))
                    {
                        ++x;
                        findWord = word;
                    }
                    else
                    {
                        if (x > 0 && !string.IsNullOrWhiteSpace(findWord))
                        {
                            if (!wordMap.ContainsKey(findWord))
                            {
                                wordMap.Add(findWord, 0);
                            }

                            wordMap[findWord]++;

                            i += findWord.Length;
                            k = 1;

                            x = 0;
                        }
                    }
                }
            }
        }
    }
}
