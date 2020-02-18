using System;
using System.Collections.Generic;
using System.Text;

namespace MMOLoginServer.LoginServerLogic
{
    public class BasicFunctions
    {
        public string GenerateSalt(int lengthOfSalt)
        {
            string salt = "";
            for (int i = 0; i < lengthOfSalt; i++)
            {
                salt += new Random().Next(0, 300);
            }
            return salt;
        }

        public byte[] ConcatByteArrays(params byte[][] arrays)
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
    }
}
