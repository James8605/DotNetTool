using System;

namespace DotNetTool
{
    public class CRC16Helper
    {
        public static ushort GetCRC(byte[] buffer, int buffer_length)
        {
            byte c, treat, bcrc;
            ushort wcrc = 0;
            int i, j;

            for (i = 0; i < buffer_length; i++)
            {
                c = buffer[i];
                for (j = 0; j < 8; j++)
                {
                    treat = Convert.ToByte(c & 0x80);
                    c <<= 1;
                    bcrc = Convert.ToByte((wcrc >> 8) & 0x80);
                    wcrc <<= 1;
                    if (treat != bcrc)
                    {
                        wcrc ^= 0x1021;
                    }
                }
            }
            return wcrc;
        }
    }
}
