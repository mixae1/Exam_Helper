using System;
using System.Collections.Generic;

namespace Exam_Helper
{
    public partial class ATest
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int TypeId { get; set; }
        public string Serviceinfo { get; set; }

        public virtual Pack Object { get; set; }
        public virtual Question ObjectNavigation { get; set; }
        public virtual Tests Type { get; set; }
    }
}
