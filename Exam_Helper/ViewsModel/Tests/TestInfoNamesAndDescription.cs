using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class TestInfoNamesAndDescription:TestParent
    {
        public string Description { get; set; }

        public List<string> Names { get; set; }

        public int AnswerID { get; set; }

        public string Instruction { get; set; }

        public TestInfoNamesAndDescription():base()
        {

        }
    }
}
