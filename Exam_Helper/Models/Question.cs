using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_Helper
{
    public partial class Question
    {
        public Question()
        {
            ATest = new HashSet<ATest>();
        }

        public int Id { get; set; }
        public string Definition { get; set; }
        public string Title { get; set; }
        public string Proof { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TagIds { get; set; }

        public bool IsPrivate { get; set; }
        public virtual ICollection<ATest> ATest { get; set; }
    }
}
