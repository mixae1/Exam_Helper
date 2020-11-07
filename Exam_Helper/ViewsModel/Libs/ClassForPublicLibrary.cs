using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class ClassForPublicLibrary
    {
        public List<Question> questions { get; set; }
        public List<Pack> packs { get; set; }
    }
}
