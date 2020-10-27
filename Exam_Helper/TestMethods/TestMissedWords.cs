using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Exam_Helper.TestMethods
{
    public class TestMissedWords
    {
        //множество индексов слов которые из строки вычленияем 
        private SortedDictionary<int,int> wordsind;
        public string Thereom { get; set; }
        private string[] words;
        //настройка кол-ва слов которые вытаскивать будем
        private int missedwords;

        private string HtmlTagIput = "text";

        //свойство лучше их юзать а не автосвойства 
        public int Missedwords
        {
            get
            {
                return missedwords;
            }
            set
            {
                if (value > 0)
                    missedwords = value;
            }
        }

        public int[] GetAnswersHashCodes() => wordsind.Values.ToArray();
        public TestMissedWords(string Thereom)
        {
            if (string.IsNullOrEmpty(Thereom))
                throw new Exception("incorrect string");
            words = Thereom.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            wordsind = new SortedDictionary<int, int>();
            missedwords = 2;
        }
        /*
        //одиночный тест
        public void Start()
        {
            Console.WriteLine("please enter missing words + \n" + GetTestString());
            string s = Console.ReadLine().ToLower();
            string s1 = Console.ReadLine().ToLower();
            int counter = 0;
            if (wordsind.Contains(s.GetHashCode())) counter++;
            if (wordsind.Contains(s1.GetHashCode())) counter++;
          вернуть статистику    Console.WriteLine($"you ve made {counter}/2");

        }

       */
       
        
        //вспомогательный метод проверки на прилагательное , достаточно просто дописать окончание в регулярку 
        //также может захватывать глаголы , в этом ничего страшного нет, фиксить смысла нет
        //если уж совсем захочется исключить какие-нибудь слова можно просто создать множество этих слов и проверять на принадлженсоть
        bool isPril(string x)
        {
            if (x.Length >= 3 && Regex.IsMatch(x, @"((ая)|ое|на|о|ых)\b"))
                return true;

            return false;
        }

        //получаем строку из которой выкидываем слова 
        public IEnumerable<string> GetTestString()
        {
            var temp = words.Where(x => isPril(x)).ToList();
            if (temp.Count() < missedwords)
                return new string[] { "sorry we cannot make test with this data" };

            Random r = new Random((int)DateTime.Now.Ticks);

            wordsind = new SortedDictionary<int, int>();
            int i = 0;
            while (wordsind.Count != missedwords)
            {
                int t = r.Next() % temp.Count;
                if (!wordsind.ContainsKey(t))
                {
                    wordsind.Add(t,temp[t].ToLower().GetHashCode());
                    i++;
                }
            }
            //если совсем долго будет( что врядли ) можно юзать string builder 
            return words.Select(x => wordsind.ContainsValue(x.GetHashCode()) ? HtmlTagIput : " " + x + " ");
        }

    }

}

