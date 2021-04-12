using System.Text;

namespace XGS.James.Tool
{
    public class TextEncodeHelper
    {
        static TextEncodeHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static byte[] String2ByteArray(string str)
        {
            return Encoding.GetEncoding("GB2312").GetBytes(str);
        }

        public static string ByteArray2String(byte[] arr)
        {
            return Encoding.GetEncoding(Encoding.UTF8.CodePage).GetString(arr);
        }
    }
}
