

namespace Exam_Helper.ViewsModel
{
    public class TestInfoMissedWords:TestParent
    {   
        public string Title { get; set; }

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
