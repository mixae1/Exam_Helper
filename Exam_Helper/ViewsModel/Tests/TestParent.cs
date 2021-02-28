using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class TestParent
    {
       public TestParent() { }
        //настройки метода тестирования для кнопки : пройти еше раз 
      public string TestInstructions { get; set; }
        //флаг ,который определяет, метод запущен самостоятельно или в составе мульти 
      public bool isMulti { get; set; }
    }
}
