using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{   

    public class QuestionsListForPackCreatingModel
    {
      public  QuestionForPackCreatingModel[] Questions { get; set; }
    }

    public class QuestionForPackCreatingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

       public bool IsSelected { get; set; }
    }

}
