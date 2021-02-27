using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Exam_Helper.ViewsModel
{
    public class TestChoiceViewModel
    {
        public TestChoiceViewModel()
        {
            
        }

        public string [] TestMethodsNames { get; set; }
        public int[] TestsMethodsIds { get; set; }

        public string ServiceInfo { get; set; }

        public string MultiTestingInfoTests { get; set; }

        [Required(ErrorMessage = "Выберите один из вариантов")]
        [Range(1,100000,ErrorMessage ="Выберите один из вариантов")]
        public int SelectedId { get; set; }
    }





}
