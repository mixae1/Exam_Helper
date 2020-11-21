using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper.ViewsModel
{
    public class ClassForQuestionCreatingModel
    {   
       
       public string Proof { get; set; }
       
        [Required]
        [StringLength(1000,MinimumLength =10)]
        public string Definition { get; set; }

        [Required]
        public string Title { get; set; }

        public int Id { get;set; }

        public List<TagForQuestionCreatingModel> tags { get; set; }

        
    }

    public class TagForQuestionCreatingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
