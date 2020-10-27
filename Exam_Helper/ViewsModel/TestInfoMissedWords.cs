using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class TestInfoMissedWords
    {   
        public string[] Teorem { get; set; }
        public string[] Answer { get; set; }
        
        public int[] Check_Answers { get; set; }
        public TestInfoMissedWords()
        {
            Answer = new string[2];
        }
    }
    
}
