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

        public static IEnumerable<string> Prefixes(this string word)
        {
            for (var i = 0; i < word.Length; i++)
            {
                yield return word.Substring(0, i);
            }
        }
   
    }
}