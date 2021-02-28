using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Exam_Helper.TestMethods
{
    public class TestTheWrongText
    {
        private IEnumerable<string> parts;
        private IEnumerable<int> adjectives; //their indecies in the parts
        private float percent;
        const string alphabet = "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσΤτΥυΦφΧχΨψΩω";
 
        public TestTheWrongText(string Text, string Instruction = "")
        {
            //text -> parts[]
            parts = Regex.Replace(Text, @"(,|\.|:|\?|\&|!|\(|\)|\{|\}|\-|=|<|>|\r\n)", " " + "$1" + " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim());

            //parts[] -> adjs[]
            adjectives = parts.Where((x, ind) => isAdjective(x)).Select((x, ind)=>ind);
        }

        void CreateTest()
        {

        }

        bool isAdjective(string s)
        {
            if (s.Length >= 3 && Regex.IsMatch(s, @"((ая)|ое|на|о|ых)\b"))
                return true;

            return false;
        }
    }
}
