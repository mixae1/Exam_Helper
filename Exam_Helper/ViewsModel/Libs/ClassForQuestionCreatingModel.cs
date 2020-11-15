using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper.ViewsModel
{
    public class ClassForQuestionCreatingModel
    {   
       
       // public Question question { get; set;
       
       public string Proof { get; set; }
       
        [Required]
        [StringLength(200,MinimumLength =10)]
        public string Definition { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 10)]
        public string Title { get; set; }


        public List<TagForQuestionCreatingModel> tags { get; set; }

        
    }

    public class TagForQuestionCreatingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
