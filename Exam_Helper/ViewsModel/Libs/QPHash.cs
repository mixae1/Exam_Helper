using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel
{
    public class QPHash
    {
        public static readonly string source = "ljngfeb";
        public enum Type
        {
            Question,
            Pack
        }
        public readonly int number;
        public readonly Type type;
        public QPHash(string hash)
        {
            if (hash.Length != 7) throw new Exception("wrong hash");

            hash = hash.ToLower();
            int coef = hash[6] - source[6];
            if (coef < 1 || coef > 10) throw new Exception("wrong hash");

            if ((hash[0] - 'l') / coef == 1) type = Type.Question;
            else if ((hash[0] - 'l') / coef == 2) type = Type.Pack;
            else throw new Exception("wrong hash");

            number = 0;
            int pow = 10000;
            for (int i = 1; i < 6; i++)
            {
                number += pow * ((hash[i] - source[i]) / coef); //даже если неправильный хэш - он сработает
                pow /= 10;
            }
        }

        private static string rec(int i, int coef, int number)
        {
            if (i == 1) return "" + (char)(source[i] + coef * (number % 10));
            else
            {
                return rec(i - 1, coef, number / 10) + (char)(source[i] + coef * (number % 10));
            }
        }

        public static string CreateHash(Type type, int number)
        {
            //Random rand = new Random();
            //int coef = rand.Next(1, 10);
            int coef = 1;
            string res = "";

            if (type == Type.Question) res += (char)(source[0] + coef);
            else if (type == Type.Pack) res += (char)(source[0] + 2 * coef);

            res += rec(5, coef, number);

            res += (char)(source[6] + coef);

            return res;
        }
    }
}
/*
mjnhflc
mjngkmc
 */
