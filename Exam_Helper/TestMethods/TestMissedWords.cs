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
        private float percent;
        private bool isPossible;

        private SortedDictionary<int, string> answers;
        private List<Func<string, bool>> funcs;

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

        public TestMissedWords(string Thereom, string Instruction = "33;true")
        {
            var instructions = new MissedWordsInstruction(Instruction);

            percent = instructions.percent;

            if (string.IsNullOrEmpty(Thereom))
                throw new Exception("incorrect string");

            percent /= 200; // 2 - так как мы же не хотим все слова делать полями

            words = Thereom.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            funcs = new List<Func<string, bool>>();
            if(instructions.isPrill) funcs.Add(isPril);
            //...

            isPossible = CreateTest();
        }

        //staff for 
        static bool InvokeMethod(Delegate method, params object[] args)
        {
            return (bool)method.DynamicInvoke(args);
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
                foreach(var func in funcs)
                {
                    if (!InvokeMethod(func, words[i])) goto label1;
                }
                temp.Add((i, words[i].Trim()));
            label1:
                ;
            }

            answers = new SortedDictionary<int, string>();

            if (temp.Count() == 0)
                return false;

            Random r = new Random((int)DateTime.Now.Ticks);

            missedwords = (int)(temp.Count() * percent);
            if (missedwords == 0) missedwords++;
            while (answers.Count != missedwords)
            {
                int t = r.Next() % temp.Count;
                if (!answers.ContainsKey(temp[t].Item1))
                {
                    answers.Add(temp[t].Item1, temp[t].Item2);
                }
            }
            return true;
        }

    }

    class MissedWordsInstruction
    {
        public float percent;
        public bool isPrill;

        private const float PERCENT = 33f;
        private const bool ISPRILL = true;

        public MissedWordsInstruction(string instruction)
        {
            string[] instructions = instruction.Split(";");

            if (!float.TryParse(instructions[0], out percent)) percent = PERCENT;

            if (!bool.TryParse(instructions[1], out isPrill)) isPrill = ISPRILL;

            if (percent < 1 || percent > 100)
                throw new Exception("incorrect percent");
        }
    }

}

