using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
//using Newtonsoft.Json;
using System.Text.Json;

namespace Exam_Helper.TestMethods
{
    public class TestMissedWords
    {
        public string Thereom { get; set; }
        private string[] words;
        //настройка кол-ва слов которые вытаскивать будем
        private int missedwords;
        private bool isPossible;

        private SortedDictionary<int, string> answers;

        //свойство лучше их юзать а не автосвойства 
        public int AmountOfMissed
        {
            get
            {
                return missedwords;
            }
            
        }
        public bool IsSuccessed
        {
            get
            {
                return isPossible;
            }
        }
        public string[] Words
        {
            get
            {
                return words;
            }
        }
        public string[] Answers
        {
            get
            {
                return answers.Select(x => x.Value).ToArray();

            }
        }

        public string[] GetWordsWithInputs()
        {
            return Words.Select((x, i) => answers.ContainsKey(i) ? " " + Words[i] : Words[i]).ToArray();
        }

        public TestMissedWords(string Thereom, float percent = 0.33f)
        {
            if (string.IsNullOrEmpty(Thereom))
                throw new Exception("incorrect string");
            if (percent < 0 || percent > 1) 
                throw new Exception("incorrect percent");
            
            words = Thereom.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            missedwords = (int)(words.Length * percent / 2);
            if (missedwords == 0) missedwords++;

            CreateTest();
        }       
        
        //вспомогательный метод проверки на прилагательное
        //также может захватывать глаголы
        bool isPril(string x)
        {
            if (x.Length >= 3 && Regex.IsMatch(x, @"((ая)|ое|на|о|ых)\b"))
                return true;

            return false;
        }

        //получаем строку из которой выкидываем слова 
        private bool CreateTest()
        {
            List<(int, string)> temp = new List<(int, string)>();
            for(int i = 0; i<words.Length; i++)
            {
                if (isPril(words[i])) temp.Add((i, words[i]));
            }

            answers = new SortedDictionary<int, string>();

            if (temp.Count() < missedwords)
                return isPossible = false;

            Random r = new Random((int)DateTime.Now.Ticks);

            while (answers.Count != missedwords)
            {
                int t = r.Next() % temp.Count;
                if (!answers.ContainsKey(temp[t].Item1))
                {
                    answers.Add(temp[t].Item1, temp[t].Item2);
                }
            }
            return isPossible = true;
        }

    }

}

