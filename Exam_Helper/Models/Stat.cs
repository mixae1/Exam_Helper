using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.Models
{
    public partial class Stat
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public int QuestionId { get; set; }

        public string ServiceInfo { get; set; }

    }
}
