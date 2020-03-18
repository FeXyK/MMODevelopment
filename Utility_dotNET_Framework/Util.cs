using System;
using System.Collections.Generic;
using System.Text;

namespace Utility_dotNET_Framework
{
    public class Util
    {
        public static byte[] GenerateRandomSequence(int length)
        {
            byte[] seq = new byte[length];
            for (int i = 0; i < length; i++)
            {
                new Random().NextBytes(seq);
            }
            return seq;
        }
        public static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            byte[] data;
            int length = 0;
            foreach (var array in arrays)
            {
                length += array.Length;
            }
            data = new byte[length];
            int offset = 0;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, data, offset, array.Length);
                offset += array.Length;
            }
            return data;
        }
        public static bool CompareByteArrays(byte[] left, byte[] right)
        {
            for (int i = 0; i < left.Length && left.Length == right.Length; i++)
            {
                Console.Write(left[i] + "" + right[i] + " ");
                if (left[i] != right[i])
                {
                    return false;
                }
            }
            Console.WriteLine();
            return true;
        }
    }
}
