using System;
using System.Collections.Generic;

namespace Exam_Helper
{
    public partial class Tests
    {
        public Tests()
        {
            ATest = new HashSet<ATest>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<ATest> ATest { get; set; }
    }
}
