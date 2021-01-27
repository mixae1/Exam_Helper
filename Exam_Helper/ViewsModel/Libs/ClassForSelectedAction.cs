using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel.Libs
{
    public class ClassForSelectedComfirmed
    {
        public string Title { get; set; }
        public int Id { get; set; }

        public ClassForSelectedComfirmed()
        {

        }

        public ClassForSelectedComfirmed(string Title, int Id)
        {
            this.Title = Title;
            this.Id = Id;
        }
    }

    public class ClassForChangePrivateSelectedConfirmed
    {
        public List<ClassForSelectedComfirmed> tuples { get; set; }
        public bool publish { get; set; }

        public ClassForChangePrivateSelectedConfirmed(List<ClassForSelectedComfirmed> tuples, bool publish)
        {
            this.tuples = tuples;
            this.publish = publish;
        }
    }
}
