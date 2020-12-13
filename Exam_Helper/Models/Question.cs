using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper
{
    public partial class Question
    {
        public Question()
        {
            ATest = new HashSet<ATest>();
        }

        public int Id { get; set; }
        [Display(Name = "Определение")]
        public string Definition { get; set; }
        [Display(Name = "Наименование")]
        public string Title { get; set; }
        [Display(Name = "Доказательство")]
        public string Proof { get; set; }
        [Display(Name = "Автор")]
        public string Author { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Дата обновления")]
        public DateTime UpdateDate { get; set; }
        [Display(Name = "Теги")]
        public string TagIds { get; set; }
        [Display(Name = "Приватность")]
        public bool IsPrivate { get; set; }
        public virtual ICollection<ATest> ATest { get; set; }
    }
}
