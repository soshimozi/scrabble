using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    public class Scrabble
    {
        private readonly Tuple<int, int> _across = new Tuple<int, int>(1, 0);
        private readonly Tuple<int, int> _down = new Tuple<int, int>(0, 1);

        private HashSet<string> _words, _prefixes;

        private readonly List<string> _letters =
            new List<string>("A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(",".ToCharArray()));

        // anchor that can be any letter
        private readonly Anchor _any;

        private readonly Dictionary<char, int> _points = new Dictionary<char, int>()
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

        public Scrabble(HashSet<string> words, HashSet<string> prefixes)
        {
            _any = new Anchor(_letters);
            _words = words;
            _prefixes = prefixes;
        }

        protected void Initialize()
        {
            var prefixAndWords = ReadWordList("words4k.txt");
            _words = prefixAndWords.Item1;
            _prefixes = prefixAndWords.Item2;
        }

        public HashSet<string> FindWords(string letters, string prefix = "", HashSet<string> results = null)
        {
            if (results == null)
                results = new HashSet<string>();

            if (prefix == null)
                prefix = "";

            if (_words.Contains(prefix))
                results.Add(prefix);
            if (_prefixes.Contains(prefix))
                foreach (var l in letters)
                    FindWords(letters.RemoveFirst(l), prefix + l, results);

            return results;
        }

        public HashSet<string> WordPlays(string hand, string boardLetters)
        {
            var results = new HashSet<string>();
            foreach (var pre in FindPrefixes(hand))
            {
                foreach (var l in boardLetters)
                {
                    AddSuffixesOld(Removed(hand, pre), pre + l, results);
                }
            }

            return results;
        }

        public HashSet<string> LongestWords(string hand, string boardLetters)
        {
            return new HashSet<string>(WordPlays(hand, boardLetters).OrderByDescending((s1) => s1));
        }


        //private HashSet<string> ExtendPrefix(string pre, string letters, HashSet<string> results)
        //{
        //    if (_words.Contains(pre))
        //        results.Add(pre);
        //    if (_prefixes.Contains(pre))
        //        foreach (var l in letters)
        //            ExtendPrefix(pre + l, letters.RemoveFirst(l), results);

        //    return results;
        //}

        private string Removed(string letters, string remove)
        {
            foreach (var l in remove)
            {
                var index = letters.IndexOf(l);
                if (index != -1)
                {
                    letters = letters.Remove(index, 1);
                }

            }

            return letters;
        }

        public HashSet<string> FindPrefixes(string hand, string pre = "", HashSet<string> results = null)
        {
            if (results == null)
                results = new HashSet<string>();

            if (pre == null)
                pre = "";

            if (_words.Contains(pre) || _prefixes.Contains(pre))
                results.Add(pre);

            if (_prefixes.Contains(pre))
                foreach (var l in hand)
                    FindPrefixes(hand.RemoveFirst(l), pre + l, results);

            return results;
        }

        public IList<List<Letter>> Transpose(IList<List<Letter>> matrix)
        {
            var map = new List<List<Letter>>();
            for (var col = 0; col < matrix[0].Count; col++ )
            {
                map.Add(new List<Letter>());
            }

                //map.Add(new List<Letter>());
                for (var i = 0; i < matrix[0].Count; i++)
                {
                    for (var j = 0; j < matrix.Count; j++)
                    {
                        map[i].Add(matrix[j][i]);
                    }
                }

            return map;
        }

        public bool IsLetter(Letter sq)
        {
            return _letters.Contains(sq.Value);
        }

        public bool IsEmpty(Letter sq)
        {
            //"Is this an empty square (no letters, but a valid position on board)."
            return sq.Value == "." || sq.Value == "*" || sq.IsAnchor;
        }

//        public HashSet<string> Prefixes(string word)
//        {
        //    return word.Prefixes;
        //    var result = new HashSet<string>();
        //    for (var i = 0; i < word.Length; i++)
        //    {
        //        result.Add(word.Substring(0, i));
        //    }

        //    return result;
//        }

        public Tuple<HashSet<string>, HashSet<string>> ReadWordList(string filename)
        {
            var wordset =
                new HashSet<string>(
                    new StreamReader(File.OpenRead("words4k.txt")).ReadToEnd().ToUpper().Split("\r\n".ToCharArray()).
                        Where(s => !string.IsNullOrEmpty(s)));

            var prefixes = wordset.SelectMany((w) => { return w.Prefixes(); });

            var prefixset = new HashSet<string>();
            return new Tuple<HashSet<string>, HashSet<string>>(wordset, prefixset);
        }


        public HashSet<string> AddSuffixesOld(string hand, string pre, HashSet<string> results)
        {
            //"""Return the set of words that can be formed by extending pre with letters
            //in hand."""
            if (_words.Contains(pre))
                results.Add(pre);

            if (_prefixes.Contains(pre))
                foreach (var l in hand)
                {
                    AddSuffixesOld(hand.RemoveFirst(l), pre + l, results);
                }

            return results;
        }

        public int WordScore(string hand)
        {
            return hand.Sum(l => _points[l]);
        }

        public HashSet<string> TopN(string hand, string boardLetters, int n = 10)
        {
            var localwords = WordPlays(hand, boardLetters);
            return new HashSet<string>(localwords.OrderByDescending(WordScore).Take(n));
        }

        public void SetAnchors(List<Letter> row, int rowIndex, List<List<Letter>> board)
        {
            // anchors are empty squares with a neighboring letter.  Some are restricted
            // by cross-words to be only a subset of letters.
            for (var colIndex = 1; colIndex < row.Count - 1; colIndex++)
            {
                var neighborListTuple = Neighbors(board, colIndex, rowIndex);
                var neighborList =
                    new List<Letter>(new []
                                         {
                                             neighborListTuple.Item1, neighborListTuple.Item2, neighborListTuple.Item3,
                                             neighborListTuple.Item4
                                         });

                if (row[colIndex].Value != "*" && (!IsEmpty(row[colIndex]) || !neighborList.Any(IsLetter))) continue;
                if(IsLetter(neighborListTuple.Item1) || IsLetter(neighborListTuple.Item2))
                {
                    // crossword
                    var crossword = FindCrossword(board, colIndex, rowIndex);
                    row[colIndex] = new Letter(new Anchor(_letters.Where(l => _words.Contains(crossword.Item2.Replace(".", l)))));
                }
                else
                {
                    row[colIndex] = new Letter(_any);
                }
            }

        }

        public Tuple<int, string> FindCrossword(IList<List<Letter>> board, int colIndex, int rowIndex)
        {
            var sq = board[rowIndex][colIndex];

            var w = IsLetter(sq) ? sq : new Letter(".");

            var j2 = rowIndex;
            for(; j2 >= 0; j2--)
            {
                var sq2 = board[j2 - 1][colIndex];
                if (IsLetter(sq2))
                    w = new Letter(sq2.Value + w.Value);
                else
                    break;
            }

            for(var j3 = rowIndex+1; j3 < board.Count; j3++)
            {
                var sq3 = board[j3][colIndex];
                if (IsLetter(sq3))
                    w = new Letter(w.Value + sq3.Value);
                else
                    break;
            }

            return new Tuple<int, string>(j2, w.Value);

        }

        public Tuple<Letter, Letter, Letter, Letter> Neighbors(List<List<Letter>> board, int col, int row)
        {
            return new Tuple<Letter, Letter, Letter, Letter>(board[row - 1][col], board[row + 1][col],
                                                             board[row][col + 1], board[row][col - 1]);
        }


    }

    public class Letter
    {
        public Letter(Anchor anchor)
        {
            Anchor = anchor;
            Value = null;
        }

        public Letter(String value)
        {
            Anchor = null;
            Value = value;
        }

        public bool IsAnchor
        {
            get { return Anchor != null;  }
        }

        public Anchor Anchor { get; private set; }

        public string Value { get; private set; }

    }

    public class Anchor : HashSet<string>
    {
        public Anchor(IEnumerable<string> data) : base(data)
        {
        }
    }

}
