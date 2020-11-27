using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class ClassForPublicLibrary
    { 
        public List<Pack> packs { get; set; }
        public List<Tags> tags { get; set; }

        public List<Question> questions { get; set; }
    }

    public class ClassForUserLibrary
    {
        public List<Pack> packs { get; set; }
        public List<Tags> tags { get; set; }

        public IEnumerable<QuestionInfo> questions { get; set; }
    }



    public class QuestionInfo
    {
        public Question question { get; set; }
        public bool IsUser { get; set; }
    }
}
