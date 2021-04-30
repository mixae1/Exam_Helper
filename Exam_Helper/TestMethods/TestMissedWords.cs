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
        public string htmlText;
        public bool IsSuccessed;

        private string[] htmlParts;
        private List<string> parts;
        private int number_of_parts;

        static readonly string alphabet = "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσΤτΥυΦφΧχΨψΩω";
        static readonly string signes = "∧∨¬→∀∃=><⩽≤⩾≥-+*^∈∉⊆⊂⊇⊃⊊⊋∪⋂∑∏";

        private static Random rand = new Random();

        private SortedDictionary<int, string> answers;

        public string[] Answers
        {
            get
            {
                return answers.Select(x => x.Value).ToArray();

            }
        }

        private void createHtmlText()
        {
            for (int i = 0; i < parts.Count; i++)
            {
                if (string.IsNullOrEmpty(htmlParts[i]))
                    if (parts[i] == "\r\n") htmlParts[i] = parts[i];
                    else htmlParts[i] = "<span class=\"h5\">" + parts[i] + " </span>";
            }
            htmlText = string.Join(null, htmlParts);
        }

        public TestMissedWords(string Text, string Instruction)
        {

            answers = new SortedDictionary<int, string>();
            if (string.IsNullOrEmpty(Text))
            {
                IsSuccessed = false;
                return;
            }

            if (string.IsNullOrEmpty(Instruction)) Instruction = "50;false;false;false";
            var instructions = new MissedWordsInstruction(Instruction);

            //text -> parts[]
            parts = Regex.Replace(Text, @"([,\.:\?\&!\(\)\{\}\-¬→∧∨∀∃=><⩽≤⩾≥\+\*\^∈∉⊆⊂⊇⊃⊊⊋∪⋂∑∏]|\r\n)", " " + "$1" + " ").Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            number_of_parts = parts.Count;
            htmlParts = new string[number_of_parts];

            IsSuccessed = CreateTest(instructions);
        }

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private bool CreateTest(MissedWordsInstruction instructions)
        {
            bool anyChanges = false;

            List<int> positions_of_words_in_parts; //Содержит позиции слов в parts
            int number_of_words;

            if(!instructions.isPrill)
                positions_of_words_in_parts = parts.Select((x, ind) => (x, ind)).Where(x => isWord(x.x)).Select(x => x.ind).ToList();
            else
                positions_of_words_in_parts = parts.Select((x, ind) => (x, ind)).Where(x => isAdjective(x.x)).Select(x => x.ind).ToList();

            // words/adjs
            {
                number_of_words = positions_of_words_in_parts.Count;

                if (number_of_words == 0) goto Label1;
                int percent_number_of_words = Math.Max(Math.Min((int)((number_of_words - 2) * instructions.percent + 2), number_of_words), 2);

                //перемешиваю индексы
                Shuffle(positions_of_words_in_parts);

                for(int i = 0; i < percent_number_of_words; ++i)
                {
                    htmlParts[positions_of_words_in_parts[i]] = 
                        "<input class=\"editablesection\" maxlength=\"" + 
                        parts[positions_of_words_in_parts[i]].Length + 
                        "\" style=\"width: " + 
                        parts[positions_of_words_in_parts[i]].Length * 10 + 
                        "px;\" data-answer=\"" + 
                        parts[positions_of_words_in_parts[i]] + 
                        "\"/> ";
                }

                anyChanges = true;
            }

        Label1:

            if (instructions.isSignes)
            {
                List<int> positions_of_signes_in_parts = parts.Select((x, ind) => (x, ind)).Where(x => isSign(x.x)).Select(x => x.ind).ToList();
                int number_of_signes = positions_of_signes_in_parts.Count;

                if (number_of_signes == 0) goto Label2;

                Shuffle(positions_of_signes_in_parts);

                for (int i = 0; i < Math.Max(number_of_signes * instructions.percent, 1); i++)
                {
                    htmlParts[positions_of_signes_in_parts[i]] =
                        "<input class=\"editablesection\" maxlength=\"1\" style=\"width: 20px;\" data-answer=\"" +
                        parts[positions_of_signes_in_parts[i]] +
                        "\"/> ";
                }

                anyChanges = true;
            }

        Label2:

            if (instructions.isLatin)
            {

                List<int> positions_of_latin_in_parts = parts.Select((x, ind) => (x, ind)).Where(x => isLatin(x.x)).Select(x => x.ind).ToList();
                int number_of_latin = positions_of_latin_in_parts.Count;

                if (number_of_latin < 1) goto Label3;

                Shuffle(positions_of_latin_in_parts);

                for (int i = 0; i < Math.Max(number_of_latin * instructions.percent, 1); i++)
                {
                    htmlParts[positions_of_latin_in_parts[i]] =
                        "<input class=\"editablesection\" maxlength=\"1\" style=\"width: 20px;\" data-answer=\"" +
                        parts[positions_of_latin_in_parts[i]] +
                        "\"/> ";
                }

                anyChanges = true;
            }

        Label3:

            if (!anyChanges) return false;

            createHtmlText();

            return true;
        }


        private static bool isAdjective(string s)
        {
            if (s.Length >= 5 && Regex.IsMatch(s, @"(ие|ых|ой|ий|ый|ая|ое|ее|ье|ья|ые|ьи|ого|его|ей|ых|их|ому|ему|ым|им|ую|ью|ыми|ими|ем|ом)\b"))
                return true;//ие
            return false;
        }

        private static bool isWord(string s)
        {
            if (s.Length >= 3 && !Regex.IsMatch(s, @"[^а-яА-Я]")) return true;
            else return false;
        }

        private static bool isNumber(string s)
        {
            foreach (var c in s) if (!Char.IsDigit(c)) return false;
            return true;
        }

        private static bool isSign(string s)
        {
            return s.Length == 1 && signes.Contains(s);
        }

        private static bool isLatin(string s)
        {
            return s.Length == 1 && alphabet.Contains(s);
        }

    }

    class MissedWordsInstruction
    {
        public float percent;
        public bool isPrill;
        public bool isLatin;
        public bool isSignes;

        private const float PERCENT = 50f;
        private const bool ISPRILL = false;
        private const bool ISLATIN = false;
        private const bool ISSIGNES = false;

        public MissedWordsInstruction(string instruction)
        {
            string[] instructions = instruction.Split(";");

            if (!float.TryParse(instructions[0], out percent)) percent = PERCENT;

            if (percent < 1 || percent > 100)
                percent = PERCENT / 200;
            else
                percent /= 200;

            if (!bool.TryParse(instructions[1], out isPrill)) isPrill = ISPRILL;

            if (!bool.TryParse(instructions[2], out isLatin)) isLatin = ISLATIN;

            if (!bool.TryParse(instructions[3], out isSignes)) isSignes = ISSIGNES;

        }
    }

}

