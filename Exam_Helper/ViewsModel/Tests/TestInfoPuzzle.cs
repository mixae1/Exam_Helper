
namespace Exam_Helper.ViewsModel
{
    public class TestInfoPuzzle:TestParent
    {
        public string Title { get; set; }

        public int[] RightIndexes { get; set; }

        public string[] TestStrings { get; set; }

        public bool IsSuccessed { get; set; }

        public TestInfoPuzzle():base()
        {

        }

        public void Foo()
        {
            throw new System.NotImplementedException();
        }
    }
}
