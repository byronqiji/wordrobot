using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WordRobot
{
    public partial class FormWord : Form
    {
        private static List<string> lineList = new List<string>();

        private static Dictionary<string, int> wordMap = new Dictionary<string, int>();

        private static Regex reg = new Regex(@"^[A-Za-z0-9]+$");

        public FormWord()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = false
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = ofd.FileName;
            }
        }

        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            FixData();
            SplitWords();

            MessageBox.Show("Over");
        }

        private void FixData()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Data";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] tempArr = null;

            using (StreamReader sr = new StreamReader(txtPath.Text))
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Data\\FixData.txt", false))
            {
                //2019-04-09 21:18:20 蒙奇D路飞: 我上次看视频不小瞄到
                string line = string.Empty;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    //if (line.Trim().StartsWith("【") || line.StartsWith(" 2017-") || string.IsNullOrWhiteSpace(line))
                    //{
                    //    continue;
                    //}
                    tempArr = line.Split(':');
                    if (tempArr.Length >= 4)
                    {
                        line = tempArr[3].Trim();
                        if (line.StartsWith("@") && line.Length > 100)
                            continue;

                        lineList.Add(line.Replace(".", "").Replace("。", "").Replace(",", "").Replace("，", "").Replace("？", "").Replace("/", ""));
                        sw.WriteLine(line);
                    }

                }
            }
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
