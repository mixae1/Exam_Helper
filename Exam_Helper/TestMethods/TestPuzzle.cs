using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Exam_Helper.TestMethods
{
    public class TestPuzzle
    {
        public string Thereom { get; set; }

        private string[] words;
        private int[] right_index_order;
        string[] test_strings;
        private string[] blocks;

        private int words_in_block;
        private int blocks_amount;
        private float percent;
        private bool isPossible;

        private const float PERCENT = 33f;
        private const int MAX_WORDS_IN_BLOCK = 10;
        private const int MIN_WORDS_IN_BLOCK = 2;
        private const int MIN_BLOCKS = 3;


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
        } //
        public string[] TestStrings
        {
            get
            {
                return test_strings;
            }
        } //

        public TestPuzzle(string Thereom, string Instruction = "33")
        {
            if (!float.TryParse(Instruction, out percent)) percent = PERCENT;

            if (string.IsNullOrEmpty(Thereom))
                throw new Exception("incorrect string");
            if (percent < 1 || percent > 100)
                throw new Exception("incorrect percent");

            this.Thereom = Thereom;
            percent = (101 - percent) / 100;

            //quantity of blocks
            words_in_block = 
                percent * MAX_WORDS_IN_BLOCK < MIN_WORDS_IN_BLOCK ?
                MIN_WORDS_IN_BLOCK :
                (int)(percent * MAX_WORDS_IN_BLOCK);

            words = Thereom.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            blocks_amount = 
                words.Length / words_in_block + 
                (words.Length % words_in_block == 0 ? 0 : 1);

            if (blocks_amount < MIN_BLOCKS) blocks_amount = MIN_BLOCKS;

            right_index_order = new int[blocks_amount];
            test_strings = new string[blocks_amount];

            isPossible = CreateTest();
        }

        private bool CreateTest()
        {
            blocks = new string[blocks_amount];

            if (words.Length < 6) return false;
            
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

            Random rnd = new Random();
            
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
