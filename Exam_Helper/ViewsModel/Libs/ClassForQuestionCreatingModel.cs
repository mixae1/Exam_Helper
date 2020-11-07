using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class ClassForQuestionCreatingModel
    {
        public Question question { get; set; }
        public List<TagForQuestionCreatingModel> tags { get; set; }

        
    }

    public class TagForQuestionCreatingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
