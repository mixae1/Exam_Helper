using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class TestChoiceViewModel
    {
        public TestChoiceViewModel()
        {
            
        }

        public string [] TestMethodsNames { get; set; }
        public int[] TestsMethodsIds { get; set; }
        public int SelectedId { get; set; }
    }





}
