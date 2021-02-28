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

        private List<string> words;             //for [separatingIndex=0]
        private List<string> parts;             //for [separatingIndex=1]
        private List<string> sentantes;         //for [separatingIndex=2]
        private List<string> objs;
        private int[] right_index_order;    //negative indexes for [isSetBlocksByDefault=true], positive ones for dragable blocks
        string[] test_strings;              //done strings for a test with random order
        private string[] blocks;            //done strings for a test without random order

        private int obj_in_block;         //amount words in a block for [isDiffLenghtOfBlocks=false]
        private int blocks_amount;          //amount blocks
        private float percent;              //percent of quantity words in a block
        private bool isDiffLenghtOfBlocks;
        private bool isSetBlocksByDefault;
        private int separatingIndex;
        private bool isPossible;            //boolean for checking test readiness

        private const float PERCENT = 33f;
        private const int MAX_WORDS_IN_BLOCK = 10;
        private const int MIN_WORDS_IN_BLOCK = 2;
        private const int MAX_PARTS_IN_BLOCK = 4;
        private const int MIN_PARTS_IN_BLOCK = 1;
        private const int MAX_SENTS_IN_BLOCK = 2;
        private const int MIN_SENTS_IN_BLOCK = 1;
        private const int MIN_BLOCKS = 3;
        private const int PARAM_SET_BLOCKS = 3;
        private const int MAX_SEQ_BLOCKS = 2;

        private int MAX_OBJ_IN_BLOCK;
        private int MIN_OBJ_IN_BLOCK;

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
                return words.ToArray();
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

        public TestPuzzle(string Thereom, string Instruction = "33;false;false;0;")
        {
            /*
             Instruction:
            [0] - percent                       [1; 100]            by default 33
            [1] - Different lenght of blocks    [true,false]        by default false
            [2] - Set blocks by default         [true,false]        by default false
            [3] - Separating                    [0; 2]              by default 0
                    0 - sepIndulge, 1 - sepByParts, 2 - sepBySentances
             */

            PuzzleInstruction puzzleInstruction = new PuzzleInstruction(Instruction);

            percent = puzzleInstruction.percent;
            isDiffLenghtOfBlocks = puzzleInstruction.isDiffLenghtOfBlocks;
            isSetBlocksByDefault = puzzleInstruction.isSetBlocksByDefault;
            separatingIndex = puzzleInstruction.separatingIndex;

            if (string.IsNullOrEmpty(Thereom))
                throw new Exception("incorrect string");

            this.Thereom = Thereom;
            percent = (101 - percent) / 100;


            words = Thereom.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!(words[words.Count - 1].Last() == '.' || words[words.Count - 1].Last() == '!' || words[words.Count - 1].Last() == '?' || words[words.Count - 1].Last() == ';')) words[words.Count - 1] += '.';
            objs = words;
            MAX_OBJ_IN_BLOCK = MAX_WORDS_IN_BLOCK;
            MIN_OBJ_IN_BLOCK = MIN_WORDS_IN_BLOCK;

            if (separatingIndex > 0)
            {
                parts = new List<string>(words.Count);
                parts.Add("");
                int curr_part = 0; //parts.Count;
                foreach(var word in words)
                {
                    //the last symbol
                    int ls = word.Length - 1;
                    if(word[ls] == ',' || word[ls] == '.' || word[ls] == '!' || word[ls] == ':' || word[ls] == '?' || word[ls] == ';')
                    {
                        parts[curr_part] += word;
                        curr_part++;
                        parts.Add("");
                    }
                    else parts[curr_part] += word + " ";
                }
                parts.RemoveAt(curr_part);
                objs = parts;
                MAX_OBJ_IN_BLOCK = MAX_PARTS_IN_BLOCK;
                MIN_OBJ_IN_BLOCK = MIN_PARTS_IN_BLOCK;
            }

            if (separatingIndex > 1)
            {
                sentantes = new List<string>(parts.Count);
                sentantes.Add("");
                int curr_sent = 0;
                foreach (var part in parts)
                {
                    //the last symbol
                    int ls = part.Length - 1;
                    if (part[ls] == '.' || part[ls] == '!' || part[ls] == '?' || part[ls] == ';')
                    {
                        sentantes[curr_sent] += part;
                        curr_sent++;
                        sentantes.Add("");
                    }
                    else sentantes[curr_sent] += part + " ";
                }
                sentantes.RemoveAt(curr_sent);
                objs = sentantes;
                MAX_OBJ_IN_BLOCK = MAX_SENTS_IN_BLOCK;
                MIN_OBJ_IN_BLOCK = MIN_SENTS_IN_BLOCK;
            }

            isPossible = CreateTest();
        }

        private bool CreateTest()
        {
            //One more ambit                        -editable
            if (objs.Count < MIN_BLOCKS * MIN_OBJ_IN_BLOCK) return false;

            //Counting obj in a block for [isDiffLenghtOfBlocks=false]
            obj_in_block = 
                percent * MAX_OBJ_IN_BLOCK < MIN_OBJ_IN_BLOCK ?
                MIN_OBJ_IN_BLOCK :
                (int)(percent * MAX_OBJ_IN_BLOCK);
            //Counting blocks for [separatingIndex=0]
            blocks_amount =
                objs.Count / obj_in_block;
                //+ (objs.Count % obj_in_block == 0 ? 0 : 1);

            //Checking an ambit for [block_amount]
            if (blocks_amount < MIN_BLOCKS) blocks_amount = MIN_BLOCKS;

            //Creating arrays
            right_index_order = new int[blocks_amount];
            test_strings = new string[blocks_amount];
            blocks = new string[blocks_amount];
            
            Random rnd = new Random();

            //Setting blocks_amount for [isDiffLenghtOfBlocks]
            if (isDiffLenghtOfBlocks)
            {
                //Amount obj in every block
                int[] temp = new int[blocks_amount];
                int amount_used_obj = 0;
                for(int i = 0; i< blocks_amount; i++)
                {
                    //[+1; -1]
                    temp[i] = rnd.Next(Math.Max(obj_in_block - 1, MIN_OBJ_IN_BLOCK), Math.Min(obj_in_block + 2, MAX_OBJ_IN_BLOCK));
                    amount_used_obj += temp[i];
                }
                var diff = objs.Count - amount_used_obj;

                while (diff > 0)
                {
                    temp[rnd.Next() % blocks_amount]++;
                    diff--; 
                }

                while (diff < 0)
                {
                    var buf = rnd.Next() % blocks_amount;
                    if (temp[buf] > MIN_OBJ_IN_BLOCK)
                    {
                        temp[buf]--;
                        diff++;
                    }
                }

                //Filling blocks
                int curr_obj = 0;
                int curr_block = 0;
                foreach(var amount in temp)
                {
                    blocks[curr_block] = "";
                    for (int i = 0; i < amount; i++)
                    {
                        blocks[curr_block] += objs[curr_obj++] + " ";
                    }
                    curr_block++;
                }

            }
            else
            {
                //???
                while(!(obj_in_block * blocks_amount - (obj_in_block - 1) <= objs.Count || obj_in_block * blocks_amount == objs.Count))
                {
                    if (--obj_in_block < MIN_OBJ_IN_BLOCK) return false;
                }

                int i = 0;
                int curr_obj_amount = 0;
                foreach (var obj in objs)
                {
                    if (curr_obj_amount == obj_in_block)
                    {
                        i++;
                        blocks[i] = "";
                        curr_obj_amount = 0;
                    }
                    blocks[i] += obj + " ";
                    curr_obj_amount++;
                }
            }
            
            
            //Randomizing
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

            //Randomizing neg indexes for [isSetBlocksByDefault=true]
            if(isSetBlocksByDefault == true)
            {
                int curr_seq = 0;
                bool[] setByDefault = new bool[blocks_amount];
                for(int i = 0; i < blocks_amount; i++)
                {
                    int buf = rnd.Next() % PARAM_SET_BLOCKS;
                    if (buf == 0 && curr_seq < MAX_SEQ_BLOCKS)
                    {
                        setByDefault[i] = true;
                        curr_seq++;
                    }
                    else
                    {
                        if(curr_seq <= -MAX_SEQ_BLOCKS)
                        {
                            setByDefault[i] = true;
                            curr_seq = 0;
                        }
                        else
                        {
                            if (curr_seq > 0) curr_seq = -1;
                            else curr_seq -= 1;
                        }
                    }
                
                }
                //for example, arr: [2, 3, -4, 0, -1], the first and 4th blocks ll be set by default;
                for(int i = 0; i<blocks_amount; i++)
                {
                    if (setByDefault[right_index_order[i]]) right_index_order[i] *= -1;
                }
            }

            return true;
        }
    }

    class PuzzleInstruction
    {
        /*
             Instruction:
            [0] - percent                       [1; 100]            by default 33
            [1] - Different lenght of blocks    [true,false]        by default false
            [2] - Set blocks by default         [true,false]        by default false
            [3] - Separating                    [0; 2]              by default 0
                    0 - sepIndulge, 1 - sepByParts, 2 - sepBySentances
             */

        public float percent;
        public bool isDiffLenghtOfBlocks;
        public bool isSetBlocksByDefault;
        public int separatingIndex;

        private const float PERCENT = 33f;
        private const bool IS_DIFF_LENGHT_OF_BLOCKS = false;
        private const bool IS_SET_BLOCKS_BY_DEFAULT = false;
        private const int SEPARATING_INDEX = 0;

        public PuzzleInstruction(string instruction)
        {
            if (string.IsNullOrEmpty(instruction)) instruction = "50;false;false;0;";
            string[] instructions = instruction.Split(";");

            if (!float.TryParse(instructions[0], out percent)) percent = PERCENT;

            if (!bool.TryParse(instructions[1], out isDiffLenghtOfBlocks)) isDiffLenghtOfBlocks = IS_DIFF_LENGHT_OF_BLOCKS;

            if (!bool.TryParse(instructions[2], out isSetBlocksByDefault)) isSetBlocksByDefault = IS_SET_BLOCKS_BY_DEFAULT;

            if (!int.TryParse(instructions[3], out separatingIndex)) separatingIndex = SEPARATING_INDEX;

            if (percent < 1 || percent > 100)
                throw new Exception("incorrect percent");

            if (separatingIndex < 0 || separatingIndex > 2)
                throw new Exception("Out of range: separatingIndexs");
        }
    }
}
