using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.TestMethods
{
    public class TestPuzzle
    {
        public string Thereom { get; set; }
        private string[] words;
        private int words_in_block = 3;
        private int blocks_amount;
        private int[] right_index_order;
        string[] test_strings;
        private float percent;
        private bool isPossible;

        private const float PERCENT = 33f;

        private string[] blocks;

        public bool IsSuccessed
        {
            get
            {
                return isPossible;
            }
        }
        public string[] Words
        {
            get
            {
                return words;
            }
        }
        public int[] RightIndexes
        {
            get
            {
                return right_index_order;
            }
        }
        public string[] TestStrings
        {
            get
            {
                return test_strings;
            }
        }

        public TestPuzzle(string Thereom, string Instruction = "33")
        {
            if (!float.TryParse(Instruction, out percent)) percent = PERCENT;

            if (string.IsNullOrEmpty(Thereom))
                throw new Exception("incorrect string");
            if (percent < 1 || percent > 100)
                throw new Exception("incorrect percent");

            percent /= 100;

            words = Thereom.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            blocks_amount = words.Length / words_in_block + (words.Length % words_in_block == 0 ? 0 : 1);

            isPossible = CreateTest();
        }

        private bool CreateTest()
        {
            string[] blocks = new string[blocks_amount];
            int i = 0;

            int curr_words_amount = 0;
            foreach (var word in words)
            {
                if (curr_words_amount == words_in_block)
                {
                    ++i;
                    curr_words_amount = 0;
                }
                blocks[i] = blocks[i] + word + " ";
                ++curr_words_amount;
            }
            Thereom = Thereom + " ";

            Random rnd = new Random();
            right_index_order = new int[blocks_amount];
            test_strings = new string[blocks_amount];
            for (int j = 0; j < blocks_amount; ++j)
            {
                right_index_order[j] = -1;
                test_strings[j] = null;
            }
            for (int j = 0; j < blocks_amount; ++j)
            {
                int rm = rnd.Next() % blocks_amount;
                while (test_strings[rm] != null) rm = rnd.Next() % blocks_amount;
                right_index_order[j] = rm;
                test_strings[rm] = blocks[j];
            }

            return true;
        }
    }
}
