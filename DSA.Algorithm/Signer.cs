using System.Numerics;
using HashAlgorithms;
using Helper;

namespace DSA.Algorithm
{
    public static class Signer
    {
        public static bool Sign(byte[] initialMsg, BigInteger q, BigInteger p, BigInteger k, 
                                          BigInteger x, BigInteger h, out BigInteger r, out BigInteger s)
        {
            var hash = SHA1.GetHash(initialMsg);
            var g = HelperFunctions.FastExp(h, (p - 1) / q, p);
            r = HelperFunctions.FastExp(g, k, p) % q;
            s = HelperFunctions.FastExp(k, q - 2, q) * (hash + x * r) % q;

            return (r != 0 && s != 0);
        }

        public static bool CheckSign(byte[] msg, BigInteger r, BigInteger s, BigInteger q, BigInteger p,
                                     BigInteger h, BigInteger x, out BigInteger v)
        {
            var w = HelperFunctions.FastExp(s, q - 2, q);
            var hashImage = SHA1.GetHash(msg);
            var u1 = hashImage * w % q;
            var u2 = r * w % q;
            var g = HelperFunctions.FastExp(h, (p - 1) / q, p);
            var y = HelperFunctions.FastExp(g, x, p);
            v = (HelperFunctions.FastExp(g, u1, p) * HelperFunctions.FastExp(y, u2, p) % p) % q;
            
            return v == r;
        }
    }
}
