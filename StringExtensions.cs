using System.Collections.Generic;
namespace Scrabble
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

        /// <summary>
        /// Returns a sequence of words from "word" with each letter missing
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static IEnumerable<string> RemoveOneLetter(this string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                yield return string.Concat(word.Substring(0, i), word.Substring(i + 1));
            }
        }

        /// <summary>
        /// returns a sequence of words for "word" with paris of letters missing.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static IEnumerable<string> RemoveTwoLetters(this string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                var first_part = word.Substring(0, i);

                for (int j = i + 1; j < word.Length; j++)
                {
                    yield return first_part + word.Substring(i + 1, j - i - 1) + word.Substring(j + 1);
                }
            }
        }

    }
}