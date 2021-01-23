using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel.Libs
{
    public class ClassForDeleteSelectedComfirmed
    {
        public string Title { get; set; }
        public int Id { get; set; }

        public ClassForDeleteSelectedComfirmed()
        {

        }

        public ClassForDeleteSelectedComfirmed(string Title, int Id)
        {
            this.Title = Title;
            this.Id = Id;
        }
    }
}
