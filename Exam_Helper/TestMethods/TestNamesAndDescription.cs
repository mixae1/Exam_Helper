using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Exam_Helper.TestMethods
{
    public class HelpStruct
    {
        public string def { get; set; }
        public string title { get; set; }
        public HelpStruct(string d, string t)
        {
            def = d;
            title = t;
        }
    }

    public class TestNamesAndDescription
    {
        public List<HelpStruct> Names { get; set; }

        private List<string> finalNames;
        private string description;
        private int answerId;
        private float percent;
        private const float PERCENT = 66f;
        private int countOfNames;

        public List<string> FinalNames
        {
            get
            {
                return finalNames;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public int AnswerId
        {
            get
            {
                return answerId;
            }
        }

        public int CountOfNames
        {
            get
            {
                return countOfNames;
            }
        }

        public TestNamesAndDescription(List<HelpStruct> Names, string Instruction)
        {
            if (!float.TryParse(Instruction, out percent)) percent = PERCENT;

            if (percent < 1 || percent > 100)
                throw new Exception("incorrect percent");

            percent /= 100;

            Random r = new Random((int)DateTime.Now.Ticks);

            countOfNames = (int)(Names.Count * percent);

            if (countOfNames < 1)
            {
                countOfNames = 1;
            }

            while (countOfNames < Names.Count)
            {
                int step = r.Next(Names.Count);
                Names.RemoveAt(step);
            }

            answerId = r.Next(Names.Count);

            finalNames = Names.ConvertAll(x => x.title);

            description = Names[answerId].def;
        }
    }
}
