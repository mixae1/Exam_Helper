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

        private List<string> parts;
        private List<int> adjectives; //Содержит позиции прилагательных в parts
        private int parts_amount;
        private int adjectives_amount;
        private int amount; // Кол-во выбранный прилагательных для замены

        private Dictionary<int, string> changed_selected_adjectives; //Выбранные прилагательные с изменёнными окончаниями
        private List<int> selected_ids;

        private float percent;
        static readonly string alphabet = "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσΤτΥυΦφΧχΨψΩω";
        private static readonly HttpClient client = new HttpClient();

        public TestTheWrongText(string Text, string Instruction = "")
        {
            //text -> parts[]
            parts = Regex.Replace(Text, @"(,|\.|:|\?|\&|!|\(|\)|\{|\}|\-|=|<|>|\r\n)", " " + "$1" + " ").Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            parts_amount = parts.Count();

            //parts[] -> adjs[]
            adjectives = parts.Select((x, ind) => (x, ind)).Where(x => isAdjective(x.x)).Select(x => x.ind).ToList();
            adjectives_amount = adjectives.Count();

            IsSuccessed = CreateTest();
        }

        public bool CreateTest()
        {
            Random rand = new Random();
            amount = Math.Min(adjectives_amount - adjectives_amount % 2, rand.Next(1, 3) * 2);
            if (amount == 0) return false;

            //позиции выбранных 'прилагательных' в parts
            selected_ids = new List<int>();
            var selected_ids_buf = new List<int>();
            for (int i = 0; i < amount;)
            {
                int temp = rand.Next(0, adjectives_amount);
                if (!selected_ids.Exists(x => x == adjectives[temp]))
                {
                    selected_ids.Add(adjectives[temp]);
                    selected_ids_buf.Add(adjectives[temp]);
                    ++i;
                }
            }
            selected_ids.Sort();

            //выбранные 'прилагательные' в тексте
            List<string> selected_adjectives = selected_ids_buf.Select(x => parts[x]).ToList();

            //выбранные 'прилагательные' в тексте с измененными окончаниями
            changed_selected_adjectives = new Dictionary<int, string>(amount);
            for (int i = 0; i * 2 < amount; ++i)
            {
                var temp = swapEndings(selected_adjectives[i * 2], selected_adjectives[i * 2 + 1]);
                changed_selected_adjectives.Add(selected_ids_buf[i * 2 + 1], temp.Item1);
                changed_selected_adjectives.Add(selected_ids_buf[i * 2], temp.Item2);
            }

            createHtmlText();

            return true;
        }

        private void createHtmlText()
        {
            htmlText = "";
            int lastIdx = 0;

            for (int j = 0; j < selected_ids.Count; j++)
            {
                for (int i = lastIdx; i < selected_ids[j]; i++)
                {
                    htmlText += parts[i] + " ";
                }
                htmlText += "<label id=\"wr\">" + changed_selected_adjectives[selected_ids[j]] + " </label>";
                lastIdx = selected_ids[j] + 1;
            }
            for (int i = lastIdx; i < parts_amount; i++)
            {
                htmlText += parts[i] + " ";
            }
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
            if (s.Length >= 5 && Regex.IsMatch(s, @"(ой|ий|ый|ая|ое|ее|ье|ья|ые|ьи|ого|его|ей|ых|их|ому|ему|ым|им|ую|ью|ыми|ими|ем|ом)\b"))
                return true;//ие
            return false;
        }

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

    }
}
