using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class ClassForPublicLibrary
    { 
        public List<Pack> packs { get; set; }
        public List<Tags> tags { get; set; }

        public List<Question> questions { get; set; }
    }

    public class ClassForUserLibrary
    {
        public List<Pack> packs { get; set; }
        public List<Tags> tags { get; set; }

        public List<QuestionInfo> questions { get; set; }
    }

    /// <summary>
    /// вспомогательных класс для хранения значения усвоения материала по 3 видам тестирования 
    /// считается : считаем кол-во верных ответов / кол-во всех заданий в методе тестирования ,потом прибавляем к значениям
    /// из бд
    /// </summary>
    public class StatsInfo
    {
        public double MissWords { get; set; }

        public double PuzzleTest { get; set; }

        public double WrongText { get; set; }
    }


    public class QuestionInfo
    {
        public Question question { get; set; }
        public bool IsUser { get; set; }
        public bool IsSearched { get; set; }
        public bool IsSelected { get; set; }

       public StatsInfo stats { get; set; }
    }
}
