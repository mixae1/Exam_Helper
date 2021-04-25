using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper.ViewsModel
{
    public class ClassForQuestionCreatingModel
    {
        [Display(Name = "Доказательство")]
        public string Proof { get; set; }
       
        [Required(ErrorMessage = "Определение не задано")]
        [Display(Name = "Определение")]
        public string Definition { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Наименование не задано")]
        [MaxLength(100, ErrorMessage = "Максимальная длина наименования - 100 символов")]
        [MinLength(1, ErrorMessage = "Наименование не задано")]
        public string Title { get; set; }

        public int Id { get;set; }

        [Display(Name = "Теги")]
        public TagsForQuestionCreatinModel tags { get; set; }
        
    }

    public class TagsForQuestionCreatinModel
    {
        public List<string> LoadedTags { get; set; }
        public List<string> SelectedTags { get; set; }
        //public List<string> CreatedTags { get; set; }
    }

    public class TagForQuestionCreatingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
