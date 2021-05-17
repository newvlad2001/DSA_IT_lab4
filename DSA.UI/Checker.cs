using System.Numerics;
using Helper;

namespace DSA.UI
{
    public static class Checker
    {
        public static bool CheckQ(string qStr)
        {
            return HelperFunctions.FermaPrimeTest(BigInteger.Parse(qStr));
        }
        
        public static bool CheckP(string qStr, string pStr)
        {
            BigInteger p = BigInteger.Parse(pStr);
            if (!HelperFunctions.FermaPrimeTest(p))
            {
                return false;
            }

            BigInteger q = BigInteger.Parse(qStr);
            return (p - 1) % q == 0;
        }

        public static bool CheckH(string qStr, string pStr, string hStr)
        {
            BigInteger h = BigInteger.Parse(hStr);
            BigInteger p = BigInteger.Parse(pStr);
            if (h < 1 || h > (p - 1))
            {
                return false;
            }

            BigInteger q = BigInteger.Parse(qStr);
            BigInteger g = HelperFunctions.FastExp(h, (p - 1) / q, p);
            if (g < 1)
            {
                return false;
            }

            return true;
        }

        public static bool CheckIsInInterval(string leftPart, string rightPart, string valueStr)
        {
            BigInteger value = BigInteger.Parse(valueStr);
            BigInteger right = BigInteger.Parse(rightPart);
            BigInteger left = BigInteger.Parse(leftPart);
           
            if (value < left || value > right)
            {
                return false;
            }

            return true;
        }
    }
}
