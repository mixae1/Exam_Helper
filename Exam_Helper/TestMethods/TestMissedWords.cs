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
        const string alphabet= "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσΤτΥυΦφΧχΨψΩω";
        const string rus = "аи";
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
            List<string> stringUnioner = new List<string>();
            stringUnioner.Add("");
            int curr_word = 0;
            while(curr_word < words.Length)
            {
                if (!answers.ContainsKey(curr_word))
                {
                    stringUnioner[stringUnioner.Count - 1] += words[curr_word] + " ";
                }
                else
                {
                    if (stringUnioner[stringUnioner.Count - 1] != "")
                    {
                        stringUnioner[stringUnioner.Count - 1] = "<span class=\"h5\">" + stringUnioner[stringUnioner.Count - 1] + "</span>";
                        stringUnioner.Add("<input class=\"editablesection test\" maxlength=\"" + answers[curr_word].Length + "\" style=\"width: " + answers[curr_word].Length*10 + "px;\"/>");
                    }
                    else
                    {
                        stringUnioner[stringUnioner.Count - 1] = "<input size=\"5\" class=\"editablesection test\" maxlength=\"" + answers[curr_word].Length + "\" style=\"width: " + answers[curr_word].Length * 10 + "px;\" />";
                    }
                    if (curr_word != words.Length - 1) stringUnioner.Add("");
                    else return stringUnioner.ToArray();
                }
                curr_word++;
            }
            stringUnioner[stringUnioner.Count - 1] = "<span class=\"h5\"> " + stringUnioner[stringUnioner.Count - 1] + "</span>";
            return stringUnioner.ToArray();
            //return Words.Select((x, i) => answers.ContainsKey(i) ?
            //"<input size=\"5\" class=\"test\" />" :
            //"<span class=\"h5\">" + words[i] + "</span>").ToArray();
        }

        public TestMissedWords(string Thereom, string Instruction = "33;true")
        {
            if (string.IsNullOrEmpty(Thereom))
            {
                isPossible = false;
                return;
            }

            if (string.IsNullOrEmpty(Instruction)) Instruction = "50;false";
            var instructions = new MissedWordsInstruction(Instruction);

            percent = instructions.percent;

            percent /= 200; // 2 - так как мы же не хотим все слова делать полями

            
            string rep = "$1";
            Thereom = Regex.Replace(Thereom, @"(,|\.|:|\?|\&|!|\(|\)|\{|\}|\-|=|<|>|\r\n)", " "+rep+" ").Trim();
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

        bool isPunct(string x)
        {
            return char.IsPunctuation(x, x.Length - 1) && x!="=";
        }

        int GreekChars(string x)
        {
            return x.Count(x => alphabet.Contains(x));
        }

        bool ValidSingleChar(string x)
        {
            return x.Length==1 && !(x.Count(x => rus.Contains(x)) == 1 || GreekChars(x) == 1);
        }

        bool BR(string x)
        {
            return x == "\r\n";
        }

        bool NotValid(string x)
        {
            return isPunct(x) || GreekChars(x)>1  || ValidSingleChar(x) || BR(x);
        }

        //получаем строку из которой выкидываем слова 
        private bool CreateTest()
        {
            List<(int, string)> temp = new List<(int, string)>();

            for(int i = 0; i<words.Length; i++)
            {
                if (NotValid(words[i])) continue;
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

            if (missedwords == temp.Count)
                return false;
            

            while (answers.Count != missedwords)
            {
                int t = r.Next() % temp.Count;
                if (!answers.ContainsKey(temp[t].Item1) && !answers.ContainsKey(temp[t].Item1-1) && !answers.ContainsKey(temp[t].Item1+1))
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
                percent = PERCENT;
        }
    }

}

