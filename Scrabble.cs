using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Scrabble
{
    public class Scrabble
    {
        private readonly int[] _across = new int[2] { 1, 0 };
        private readonly int[] _down = new int[2] { 0, 1 };

        private IEnumerable<string> _words, _prefixes;


        private readonly List<string> _letters = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => new string((Char)i, 1)).ToList();

        //private readonly List<string> _letters =
        //    new List<string>("A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(",".ToCharArray()));

        // anchor that can be any letter
        //private readonly IEnumerable<string> _any;

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

        public Scrabble(IEnumerable<string> words, IEnumerable<string> prefixes)
        {
            //_any = new IEnumerable(_letters);
            _words = new List<string>(words);
            _prefixes = new List<string>(prefixes);
        }

        //protected void Initialize()
        //{
        //    var prefixAndWords = ReadWordList("words4k.txt");
        //    _words = prefixAndWords.Item1;
        //    _prefixes = prefixAndWords.Item2;
        //}

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

        public List<string> WordPlays(string hand, string boardLetters)
        {
            var results = new List<string>();
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

        public IList<List<Word>> Transpose(IList<List<Word>> matrix)
        {
            var map = new List<List<Word>>();
            for (var col = 0; col < matrix[0].Count; col++ )
            {
                map.Add(new List<Word>());
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

        public bool IsWord(Word sq)
        {
            return _letters.Contains(sq.Value);
        }

        public bool IsSquareEmpty(Word sq)
        {
            //"Is this an empty square (no letters, but a valid position on board)."
            return sq.Value == "." || sq.Value == "*" || sq.IsAnchor;
        }

        private List<string> AddSuffixesOld(string hand, string pre, List<string> results)
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

        public List<string> TopN(string hand, string boardLetters, int n = 10)
        {
            var localwords = WordPlays(hand, boardLetters);
            return new List<string>(localwords.OrderByDescending(WordScore).Take(n));
        }

        public void SetAnchors(List<Word> row, int rowIndex, ScrabbleBoard board)
        {
            // anchors are empty squares with a neighboring letter.  Some are restricted
            // by cross-words to be only a subset of letters.
            for (var colIndex = 1; colIndex < row.Count - 1; colIndex++)
            {
                var neighborListTuple = Neighbors(board, colIndex, rowIndex);
                var neighborList =
                    new List<Word>(new []
                                         {
                                             neighborListTuple[0], neighborListTuple[1], neighborListTuple[2],
                                             neighborListTuple[3]
                                         });

                if (row[colIndex].Value != "*" && (!IsSquareEmpty(row[colIndex]) || !neighborList.Any(IsWord))) continue;
                if(IsWord(neighborListTuple[0]) || IsWord(neighborListTuple[1]))
                {
                    // crossword
                    var crossword = FindCrossword(board, colIndex, rowIndex);
                    row[colIndex] = new Word(_letters.Where(l => _words.Contains(crossword.Value.Replace(".", l))));
                }
                else
                {
                    row[colIndex] = new Word(_letters);
                }
            }

        }

        public Word FindCrossword(ScrabbleBoard board, int row, int col)
        {
            var foundRow = -1;
            return FindCrossword(board, row, col, ref foundRow);
        }

        public Word FindCrossword(ScrabbleBoard board, int row, int col, ref int foundRow)
        {
            var sq = board[row][col];

            var w = IsWord(sq) ? sq : new Word(".");

            var j2 = row;
            for(; j2 >= 0; j2--)
            {
                var sq2 = board[j2 - 1][col];
                if (IsWord(sq2))
                    w = new Word(sq2.Value + w.Value);
                else
                    break;
            }

            for(var j3 = row + 1; j3 < board.RowCount; j3++)
            {
                var sq3 = board[j3][col];
                if (IsWord(sq3))
                    w = new Word(w.Value + sq3.Value);
                else
                    break;
            }

            foundRow = j2;
            return w;

        }

        public Word[] Neighbors(ScrabbleBoard board, int col, int row)
        {
            return new Word[4] { board[row - 1][col], board[row + 1][col], board[row][col + 1], board[row][col - 1] };
        }


    }

    public class ScrabbleBoard
    {
        private List<List<Word>> _letters = new List<List<Word>>();

        public int RowCount
        {
            get { return _letters.Count; }
        }

        public Word GetAt(int row, int col)
        {
            return _letters[row][col];
        }

        public Word[] this[int rowIndex]
        {
            get { return _letters[rowIndex].ToArray(); }
        }
    }


}
