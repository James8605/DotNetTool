using System.Text;
using System.Text.RegularExpressions;

namespace DotNetTool
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

        public static int GetChineseCharNumber(string str)
        {
            int count = 0;
            Regex regex = new(@"^[\u4E00-\u9FA5]{0,}$");

            for (int i = 0; i < str.Length; i++)
            {
                if (regex.IsMatch(str[i].ToString()))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
