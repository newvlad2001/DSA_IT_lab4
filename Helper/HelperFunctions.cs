using System;
using System.Numerics;

namespace Helper
{
    public static class HelperFunctions
    {
        public static bool FermaPrimeTest(BigInteger x)
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                BigInteger a = rand.Next() % (x - 2) + 2;
                
                if (FastExp(a, x - 1, x) != 1)
                {
                    return false;
                }
            }

            return true;
        }

        public static BigInteger FastExp(BigInteger number, BigInteger power, BigInteger mod)
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