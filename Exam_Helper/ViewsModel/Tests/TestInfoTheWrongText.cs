

namespace Exam_Helper.ViewsModel
{
    public class TestInfoTheWrongText : TestParent
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public bool IsSuccessed { get; set; }



        public TestInfoTheWrongText() : base()
        {

        }

        public void Foo()
        {
            throw new System.NotImplementedException();
        }
    }

}