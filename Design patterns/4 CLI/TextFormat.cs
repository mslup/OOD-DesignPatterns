using System.Runtime.CompilerServices;

namespace ProjOb
{
    public static class TextFormat
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
}