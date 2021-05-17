using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace DSA.UI
{
    public static class Checker
    {
        public static bool CheckP(string qStr, string pStr)
        {
            BigInteger p = BigInteger.Parse(pStr);
            if (!FermaPrimeTest(p))
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
            BigInteger g = FastExp(h, (p - 1) / q, p);
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

        public static bool FermaPrimeTest(BigInteger x)
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                BigInteger a = rand.Next() % (x - 2) + 2;
                
                if (FastExp(a, x - 1, x) != 1)
                {
                    Console.WriteLine($"EXP {a}");
                    return false;
                }
            }

            return true;
        }

        private static BigInteger FastExp(BigInteger number, BigInteger power, BigInteger mod)
        {
            BigInteger x = 1;
            while (power != 0)
            {
                while (power % 2 == 0)
                {
                    power /= 2;
                    number = (number * number) % mod;
                }

                power--;
                x = (x * number) % mod;
            }

            return x;
        }
    }
}
