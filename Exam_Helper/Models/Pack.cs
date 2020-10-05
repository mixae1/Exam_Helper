using System;
using System.Collections.Generic;

namespace Exam_Helper
{
    public partial class Pack
    {
        public Pack()
        {
            ATest = new HashSet<ATest>();
        }

        public int Id { get; set; }
        public string QuestionSet { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TagsId { get; set; }

        public virtual ICollection<ATest> ATest { get; set; }
    }
}
