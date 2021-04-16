using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Exam_Helper.TestMethods
{
    public class TestTheWrongText
    {
        public string htmlText;
        public bool IsSuccessed;

        private string[] htmlParts;
        private List<string> parts;
        private int number_of_parts;

        private float percent;
        //static readonly string alphabet = "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσΤτΥυΦφΧχΨψΩω";
        
        private static readonly HttpClient client = new HttpClient();
        private static Random rand = new Random();
        
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

        private bool CustomContains(List<int> list, int ind)
        {
            foreach(int el in list)
            {
                if (parts[el] == parts[ind]) return true;
            }
            return false;
        }

        public TestTheWrongText(string Text, string Instruction)
        {
            if (string.IsNullOrEmpty(Instruction)) Instruction = "50;true;false;false;false";
            var instructions = new WrongWordsInstruction(Instruction);

            //text -> parts[]
            parts = Regex.Replace(Text, @"(,|\.|:|\?|\&|!|\(|\)|\{|\}|\-|=|<|>|\r\n)", " " + "$1" + " ").Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            number_of_parts = parts.Count;
            htmlParts = new string[number_of_parts];

            IsSuccessed = CreateTest(instructions);
        }

        private bool CreateTest(WrongWordsInstruction instructions)
        {
            List<int> positions_of_adjs_in_parts; //Содержит позиции прилагательных в parts
            int number_of_adjs;

            //change adjs
            {
                int number_of_chosen_adjs = 0; // Кол-во выбранных прилагательных для замены

                //получаем индексы всех предположительных прилагательных
                positions_of_adjs_in_parts = parts.Select((x, ind) => (x, ind)).Where(x => isAdjective(x.x)).Select(x => x.ind).ToList();
                number_of_adjs = positions_of_adjs_in_parts.Count;

                //перемешиваю индексы
                Shuffle(positions_of_adjs_in_parts);


                if (instructions.isEndingsHided)
                {
                    //собираем список прилагательных
                    List<int> buf = new List<int>(4);
                    for(int i = 0; i < number_of_adjs && buf.Count < 4; ++i)
                    {
                        if(!CustomContains(buf, positions_of_adjs_in_parts[i]))
                        {
                            buf.Add(positions_of_adjs_in_parts[i]);
                        }
                    }

                    buf.Add(buf[0]); //для сдвига
                    for(int i = 1; i < buf.Count; i++)
                    {
                        //очень плохой код, но что поделать
                        htmlParts[buf[i - 1]] = (instructions.isEndingsHided ? cutEnding(parts[buf[i]]) : parts[buf[i]]) + "</label>";
                        if (Char.IsUpper(parts[buf[i - 1]][0]) != Char.IsUpper(parts[buf[i]][0]))
                        {
                            if(Char.IsUpper(parts[buf[i - 1]][0]))
                            {
                                htmlParts[buf[i - 1]] = Char.ToUpper(htmlParts[buf[i - 1]][0]) + htmlParts[buf[i - 1]].Substring(1);
                            }
                            else
                            {
                                htmlParts[buf[i - 1]] = Char.ToLower(htmlParts[buf[i - 1]][0]) + htmlParts[buf[i - 1]].Substring(1);
                            }
                        }
                        htmlParts[buf[i - 1]] = "<label id=\"wr\">" + htmlParts[buf[i - 1]];
                    }
                }
                else
                {
                    //...
                }

            }

            if (instructions.isEndingsHided)
            {
                //for(int i = 0; i < number_of_adjs; i++)
                //{
                //    parts[positions_of_adjs_in_parts[i]] = cutEnding(parts[positions_of_adjs_in_parts[i]]);
                //}

                for (int i = 0; i < number_of_parts; i++)
                {
                    parts[i] = cutEnding(parts[i]);
                }
            }

            if (instructions.isDigit)
            {

            }

            if (instructions.isSignes)
            {

            }

            if (instructions.isLatin)
            {

            }

            createHtmlText();

            return true;
        }

        private static async Task<string> rustxt(string word)
        {
            var values = new Dictionary<string, string>
            {
                { "text", word },
                { "method", "search" },
                { "utm", "" },
                { "returnSrcSearch", "1" }

            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://rustxt.ru/chast-rechi", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return Regex.Match(responseString, "<h1 class=\"text-center\">" + word + @" - (\w+)</h1>").Groups[1].Value;
        }

        private static async Task<string> bestlang(string word)
        {
            var values = new Dictionary<string, string>
            {
                { "word", word },
                { "check", "" }

            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://best-language.ru/tools/chast-rechi/#text", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return Regex.Match(responseString, "<h1 class=\"text-center\">" + word + @" - (\w+)</h1>").Groups[1].Value;
        }

        private static bool isAdjective(string s)
        {
            if (s.Length >= 5 && Regex.IsMatch(s, @"(ие|ых|ой|ий|ый|ая|ое|ее|ье|ья|ые|ьи|ого|его|ей|ых|их|ому|ему|ым|им|ую|ью|ыми|ими|ем|ом)\b"))
                return true;//ие
            return false;
        }

        
        private void createHtmlText()
        {
            for(int i = 0; i < parts.Count; i++)
            {
                if (string.IsNullOrEmpty(htmlParts[i])) htmlParts[i] = parts[i];
            }
            htmlText = string.Join(' ', htmlParts);
        }

        private static string cutEnding(string adj)
        {
            if (Regex.IsMatch(adj, @"(ого|его|ому|ему|ыми|ими|ьей|ьих|ьим|ьем)\b"))
            {
                return adj.Substring(0, adj.Length - 3) + "…";
            }
            else if (Regex.IsMatch(adj, @"(ой|ий|ый|ая|ое|ее|ье|ья|ые|ьи|ей|ых|их|ым|им|ую|ью|ем|ом|ие)\b"))
            {
                return adj.Substring(0, adj.Length - 2) + "…";
            }
            else return adj;
        }

        /*
        private static bool isAdjectiveDoubleCheck(string s)
        {
            if (s.Length >= 5 && Regex.IsMatch(s, @"(ой|ий|ый|ая|ое|ее|ье|ья|ые|ьи|ого|его|ей|ых|их|ому|ему|ым|им|ую|ью|ыми|ими|ем|ом|ие)\b"))
            {
                return true;
            }
            return false;
        }

        private static Tuple<string, string> swapEndings(string adj1, string adj2)
        {
            Ending end1 = getEnding(adj1);
            Ending end2 = getEnding(adj2);

            return new Tuple<string, string>(end1.word + end1.GetLike(end2), end2.word + end2.GetLike(end1));
        }

        private static Ending getEnding(string adj)
        {
            if (Regex.IsMatch(adj, @"(ьего|ьему|ьими|ого|его|ому|ему|ыми|ими)\b"))
            {
                return new Ending(adj.Substring(0, adj.Length - 4), adj.Substring(adj.Length - 4));
            }
            else if (Regex.IsMatch(adj, @"(ьей|ьих|ьим|ьем|ой|ий|ый|ая|ое|ее|ье|ья|ые|ьи|ей|ых|их|ым|им|ую|ью|ем|ом)\b"))
            {
                return new Ending(adj.Substring(0, adj.Length - 3), adj.Substring(adj.Length - 3));
            }
            else throw new Exception(":23");
        }

        public class Ending
        {
            private enum Found
            {
                None,
                Твёрдый,
                Мягкий,
                Шипящий,
                ГКХ,
                Ь
            }

            public string word { get; }
            private char consonant;
            Found found;
            private string ending;
            private bool isPlural;//?
            private bool isStressed;//?
            private string lastSigns;
            private char vowel;

            private void defineFound()
            {

            }

            public Ending(string word, string ending_plus)
            {
                this.word = word;
                found = Found.None;

                consonant = ending_plus.First();

                if (consonant == 'г' || consonant == 'к' || consonant == 'х') found = Found.ГКХ;
                if (consonant == 'ч' || consonant == 'ш' || consonant == 'щ') found = Found.Шипящий;
                if (consonant == 'ь') found = Found.Ь;

                ending = ending_plus.Substring(1);
                lastSigns = ending.Substring(ending.Length == 1 ? 0 : 1);
                if (ending.Length != 0) vowel = ending[0];

                if (ending == "ой") isStressed = true;
                if (ending == "ый" || ending == "ий") isStressed = false;

                switch (vowel)
                {
                    case 'и':
                        if (found == Found.None) found = Found.Мягкий;
                        break;
                    case 'ы':
                        if (found == Found.None) found = Found.Твёрдый;
                        break;
                    default:
                        //if (found == Found.None) found = Found.Твёрдый;
                        break;
                }
            }

            char altVowel(char vowel)
            {
                switch (vowel)
                {
                    case 'и':
                        switch (found)
                        {
                            case Found.None:
                                return vowel;
                            case Found.Твёрдый:
                                return 'ы';
                            case Found.Мягкий:
                                return vowel;
                            case Found.Шипящий:
                                return vowel;
                            case Found.ГКХ:
                                return vowel;
                            case Found.Ь:
                                return vowel;
                        }
                        break;
                    case 'ы':
                        switch (found)
                        {
                            case Found.None:
                                return vowel;
                            case Found.Твёрдый:
                                return vowel;
                            case Found.Мягкий:
                                return 'и';
                            case Found.Шипящий:
                                return 'и';
                            case Found.ГКХ:
                                return 'и';
                            case Found.Ь:
                                return 'и';
                        }
                        break;
                    case 'о':
                        switch (found)
                        {
                            case Found.None:
                                return vowel;
                            case Found.Твёрдый:
                                return vowel;
                            case Found.Мягкий:
                                return 'е';
                            case Found.Шипящий:
                                return 'е';
                            case Found.ГКХ:
                                return vowel;
                            case Found.Ь:
                                return vowel;
                        }
                        break;
                    case 'е':
                        switch (found)
                        {
                            case Found.None:
                                return vowel;
                            case Found.Твёрдый:
                                return 'о';
                            case Found.Мягкий:
                                return vowel;
                            case Found.Шипящий:
                                return vowel;
                            case Found.ГКХ:
                                return 'о';
                            case Found.Ь:
                                return vowel;
                        }
                        break;
                    case 'а':
                        return vowel;
                    case 'я':
                        return vowel;
                    case 'ю':
                        return vowel;
                    case 'у':
                        return vowel;

                    default:
                        break;
                }
                throw new Exception(":24");
            }

            public string GetLike(Ending another)
            {
                return consonant + altVowel(another.vowel).ToString() + another.lastSigns;
            }

        }
        */

        class WrongWordsInstruction
        {
            public float percent;
            public bool isEndingsHided;
            public bool isDigit;
            public bool isSignes;
            public bool isLatin;

            private const float PERCENT = 50f;
            private const bool IS_ENDINGS_HIDED = false;
            private const bool IS_DIGIT = false;
            private const bool IS_SIGNES = false;
            private const bool IS_LATIN = false;


            public WrongWordsInstruction(string instruction)
            {
                string[] instructions = instruction.Split(";");

                if (!float.TryParse(instructions[0], out percent)) percent = PERCENT;

                if (!bool.TryParse(instructions[1], out isEndingsHided)) isEndingsHided = IS_ENDINGS_HIDED;
                
                if (!bool.TryParse(instructions[2], out isDigit)) isDigit = IS_DIGIT;

                if (!bool.TryParse(instructions[3], out isSignes)) isSignes = IS_SIGNES;
                
                if (!bool.TryParse(instructions[4], out isLatin)) isLatin = IS_LATIN;

                if (percent < 1 || percent > 100)
                    //throw new Exception("incorrect percent");
                    percent = PERCENT;
            }
        }

    }
}
