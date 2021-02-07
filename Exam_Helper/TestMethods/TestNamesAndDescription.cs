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

        public TestNamesAndDescription(List<HelpStruct> Names)
        {
            percent = 1;
            Random r = new Random((int)DateTime.Now.Ticks);

            countOfNames = (int)(Names.Count * percent);

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
