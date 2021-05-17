using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace DSA.Algorithm
{
    public static class Helper
    {
        public static BigInteger FastExp(BigInteger number, BigInteger exp, BigInteger m)
        {
            if (exp < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exp), "Exponent must be greater than - 1");
            }
            
            BigInteger res = 1;
            while (exp != 0)
            {
                while (exp % 2 == 0)
                {
                    exp /= 2;
                    number = (number * number) % m;
                }

                exp--;
                res = (res * number) % m;
            }

            return res;
        }
    }
}
