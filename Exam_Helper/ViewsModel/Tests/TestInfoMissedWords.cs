using Exam_Helper.ViewsModel.Tests;

namespace Exam_Helper.ViewsModel
{
    public class TestInfoMissedWords:TestParent
    {   
        public string[] Teorem { get; set; }

        public string[] Answers { get; set; }

        public bool IsSuccessed { get; set; }

        public TestInfoMissedWords():base()
        {

        }

        public void Foo()
        {
            throw new System.NotImplementedException();
        }
    }
    
}
