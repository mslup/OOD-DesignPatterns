using System.Runtime.CompilerServices;

namespace ProjOb
{
    public static class Utils
    {
        public static string Bold(this string str)
        {
            return "\u001b[1m" + str + "\u001b[0m";
        }
    }

    public static class StringExtension
    {
        public static string TrimWQ(this string str)
        {
            return str.Trim(' ', '\t', '\n', '\r', '\'', '"');
        }
    }


    public static class EnumExtension
    {
        public static bool SafeTryParse<TEnum>(this ref TEnum Field,
            string value) where TEnum : struct
        {
            TEnum oldValue = Field;
            if (!Enum.TryParse(value, out Field))
            {
                Field = oldValue;
                return false;
            }
            return true;
        }
    }
    public static class IntExtension
    {
        public static bool SafeTryParse(this ref int Field,
            string value)
        {
            int oldValue = Field;
            if (!int.TryParse(value, out Field))
            {
                Field = oldValue;
                return false;
            }
            return true;
        }
    }
}