using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class TestParent
    {
       public TestParent() { }

        ///<summary>
        /// настройки метода тестирования для кнопки : пройти еше раз 
        /// </summary>
        public string TestInstructions { get; set; }
        ///флаг ,который определяет, метод запущен самостоятельно или в составе мульти 
      public bool isMulti { get; set; }

        ///<summary>
        ///имя контроллера , который инициировал многоразовое тестирование 
        ///</summary>
        public string ControllerName { get; set; }

       /// <summary>
       /// строка ,отвечающая за кол-во правильных и неправильных вопросов пользователя 
       /// </summary>
        public string UserAnswers { get; set; }

        ///<summary>
        ///имя контроллера библиотеки,из которого пришли 
        ///</summary>
        public string ReturnControllerName { get; set; }
    }
}
