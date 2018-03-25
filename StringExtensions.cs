using System.Collections.Generic;
namespace ConsoleApplication2
{
    public static class StringExtensions
    {
        public static string RemoveFirst(this string str, char what)
        {
            var index = str.IndexOf(what);
            return index != -1 ? str.Remove(index, 1) : str;
        }

        public static HashSet<string> Prefixes(this string word)
        {
            var result = new HashSet<string>();
            for (var i = 0; i < word.Length; i++)
            {
                result.Add(word.Substring(0, i));
            }

            return result;
        }
   
    }
}