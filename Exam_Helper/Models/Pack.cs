using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper
{
    public partial class Pack
    {
        public Pack()
        {
            ATest = new HashSet<ATest>();
        }

        public int Id { get; set; }
        [Display(Name = "Список вопросов")]
        public string QuestionSet { get; set; }
        [Display(Name = "Автор")]
        public string Author { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Последнее обновление")]
        public DateTime UpdateDate { get; set; }
        [Display(Name = "Теги")]
        public string TagsId { get; set; }

        [Display(Name = "Приватность")]
        public bool IsPrivate { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        public virtual ICollection<ATest> ATest { get; set; }
    }
}
