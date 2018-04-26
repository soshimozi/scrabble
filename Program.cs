using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    internal class Program
    {
        public HashSet<string> words, prefixes;

        public Dictionary<char, int> points = new Dictionary<char, int>()
                                                         {
                                                             {'A', 1},
                                                             {'B', 3},
                                                             {'C', 3},
                                                             {'D', 2},
                                                             {'E', 1},
                                                             {'F', 4},
                                                             {'G', 2},
                                                             {'H', 4},
                                                             {'I', 1},
                                                             {'J', 8},
                                                             {'K', 5},
                                                             {'L', 1},
                                                             {'M', 3},
                                                             {'N', 1},
                                                             {'O', 1},
                                                             {'P', 3},
                                                             {'Q', 10},
                                                             {'R', 1},
                                                             {'S', 1},
                                                             {'T', 1},
                                                             {'U', 1},
                                                             {'V', 4},
                                                             {'W', 4},
                                                             {'X', 8},
                                                             {'Y', 4},
                                                             {'Z', 10},
                                                             {'_', 0}
                                                         };

        private static void Main(string[] args)
        {
            //words = new HashSet<string>(new StreamReader(File.OpenRead("words4k.txt")).ReadToEnd().ToUpper().Split("\r\n".ToCharArray()).Where(s => !string.IsNullOrEmpty(s)));

            //var prefixAndWords = ReadWordList("words4k.txt");
            //words = prefixAndWords.Item1;
            //prefixes = prefixAndWords.Item2;

            // test out prefixes
            //var result = Prefixes("WORD");

            //    var prefixAndWords = ReadWordList("words4k.txt");
            //    _words = prefixAndWords.Item1;
            //    _prefixes = prefixAndWords.Item2;


            var p = new Program();
            p.Run(args);
        }

        public void Run(string[] args)
        {
            var wordset =
                new List<string>(
                    new StreamReader(File.OpenRead("words4k.txt")).ReadToEnd().ToUpper().Split("\r\n".ToCharArray()).
                        Where(s => !string.IsNullOrEmpty(s)));

            var prefixes = wordset.SelectMany((w) => 
            {
                return w.Prefixes();
            });

            var scrabble = new Scrabble(wordset, prefixes);

            var test = scrabble.FindWords("AQZOOP");

            var top = scrabble.TopN("AQZOOP", "", 2);

        }

    //    public void Initialize()
    //    {
    //        var prefixAndWords = ReadWordList("words4k.txt");
    //        words = prefixAndWords.Item1;
    //        prefixes = prefixAndWords.Item2;
    //    }

    //    public HashSet<string> FindWords(string letters, string prefix = "", HashSet<string> results = null)
    //    {
    //        if (results == null)
    //            results = new HashSet<string>();

    //        if (prefix == null)
    //            prefix = "";

    //        if (words.Contains(prefix))
    //            results.Add(prefix);
    //        if (prefixes.Contains(prefix))
    //            foreach (var l in letters)
    //                FindWords(letters.RemoveFirst(l), prefix + l, results);

    //        //return ExtendPrefix("", letters, new HashSet<string>());

    //        return results;
    //    }

    //    public HashSet<string> WordPlays(string hand, string boardLetters)
    //    {
    //        var results = new HashSet<string>();
    //        foreach (var pre in FindPrefixes(hand))
    //        {
    //            foreach (var l in boardLetters)
    //            {
    //                add_suffixes_old(Removed(hand, pre), pre + l, results);
    //            }
    //        }

    //        return results;
    //    }

    //    //"Find all word plays from hand that can be made to abut with a letter on board."
    //    //# Find prefix + L + suffix; L from board_letters, rest from hand
    //    //results = set()
    //    //for pre in find_prefixes(hand):
    //    //    for L in board_letters:
    //    //        add_suffixes_old(removed(hand, pre), pre+L, results)
    //    //return results
    //    public HashSet<string> LongestWords(string hand, string board_letters)
    //    {
    //        //"Return all word plays, longest first."
    //        //words = word_plays(hand, board_letters)
    //        //return sorted(words, reverse = True, key = len)
    //        //var words = word_plays(hand, board_letters);
    //        return new HashSet<string>(WordPlays(hand, board_letters).OrderByDescending((s1) => s1));
    //        //return new SortedSet<string>(words, StringComparer.Create()
    //    }


    //    //static HashSet<string> FindWords(string letters, string pre = "", HashSet<string> results = null   )
    //    //{
    //    //        if( results == null )
    //    //            results = new HashSet<string>();


    //    ////Find all words that can be made from the letters in hand.
    //    ////All words start with pre. results, if given, is expected to be a set.
    //    ////
    //    //        if (words.Contains(pre))
    //    //            results.Add(pre);

    //    //        if (prefixes.Contains(pre))
    //    //            foreach (var l in letters)
    //    //                FindWords(letters.RemoveFirst(l), pre + l, results);
    //    //if pre in PREFIXES:
    //    //    for L in letters:
    //    //        find_words(letters.replace(L, '', 1), pre+L, results)
    //    //return results


    //    //     return results;
    //    //}

    //    private HashSet<string> ExtendPrefix(string pre, string letters, HashSet<string> results)
    //    {
    //        if (words.Contains(pre))
    //            results.Add(pre);
    //        if (prefixes.Contains(pre))
    //            foreach (var l in letters)
    //                ExtendPrefix(pre + l, letters.RemoveFirst(l), results);

    //        return results;
    //    }

    //    private string Removed(string letters, string remove)
    //    {
    //        foreach (var l in remove)
    //        {
    //            var index = letters.IndexOf(l);
    //            if (index != -1)
    //            {
    //                letters = letters.Remove(index, 1);
    //            }

    //        }

    //        return letters;
    //    }

    //    public HashSet<string> FindPrefixes(string hand, string pre = "", HashSet<string> results = null)
    //    {
    //        if (results == null)
    //            results = new HashSet<string>();

    //        if (pre == null)
    //            pre = "";

    //        if (words.Contains(pre) || prefixes.Contains(pre))
    //            results.Add(pre);

    //        if (prefixes.Contains(pre))
    //            foreach (var l in hand)
    //                FindPrefixes(hand.RemoveFirst(l), pre + l, results);

    //        return results;
    //    }

    //    public HashSet<string> Prefixes(string word)
    //    {
    //        var result = new HashSet<string>();
    //        for (var i = 0; i < word.Length; i++)
    //        {
    //            result.Add(word.Substring(0, i));
    //        }

    //        return result;
    //    }

    //    public Tuple<HashSet<string>, HashSet<string>> ReadWordList(string filename)
    //    {
    //        var wordset =
    //            new HashSet<string>(
    //                new StreamReader(File.OpenRead("words4k.txt")).ReadToEnd().ToUpper().Split("\r\n".ToCharArray()).
    //                    Where(s => !string.IsNullOrEmpty(s)));
    //        var prefixset = new HashSet<string>(wordset.SelectMany(Prefixes));
    //        return new Tuple<HashSet<string>, HashSet<string>>(wordset, prefixset);
    //    }


    //    public HashSet<string> add_suffixes_old(string hand, string pre, HashSet<string> results)
    //    {
    //        //"""Return the set of words that can be formed by extending pre with letters
    //        //in hand."""
    //        if (words.Contains(pre))
    //            results.Add(pre);

    //        if (prefixes.Contains(pre))
    //            foreach (var l in hand)
    //            {
    //                add_suffixes_old(hand.RemoveFirst(l), pre + l, results);
    //            }

    //        return results;
    //    }

    //    public int WordScore(string hand)
    //    {
    //        return hand.Sum(l => points[l]);
    //    }

    //    public HashSet<string> TopN(string hand, string board_letters, int n = 10)
    //    {
    //        var localwords = WordPlays(hand, board_letters);
    //        return new HashSet<string>(localwords.OrderByDescending(WordScore).Take(n));
    //    }
    }
}
