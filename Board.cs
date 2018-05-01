using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrabble
{
    class Board
    {
        // Premium cells.
        // http://en.wikipedia.org/wiki/Scrabble#Scoring
        // Legend:
        // T = triple word
        // D = double word
        // t = triple letter
        // d = double letter
        //    . = normal
        // Whitespace is ignored.
        public const string PREMIUM_CELLS = "\n" +
                                    "T. .d. . .T. . .d. .T\n" +
                                    ".D. . .t. . .t. . .D.\n" +
                                    ". .D. . .d.d. . .D. .\n" +
                                    "d. .D. . .d. . .D. .d\n" +
                                    ". . . .D. . . . .D. . . .\n" +
                                    ".t. . .t. . .t. . .t.\n" +
                                    ". .d. . .d.d. . .d. .\n" +
                                    "T. .d. . .D. . .d. .T\n" +
                                    ". .d. . .d.d. . .d. .\n" +
                                    ".t. . .t. . .t. . .t.\n" +
                                    ". . . .D. . . . .D. . . .\n" +
                                    "d. .D. . .d. . .D. .d\n" +
                                    ". .D. . .d.d. . .D. .\n" +
                                    ".D. . .t. . .t. . .D.\n" +
                                    "T. .d. . .T. . .d. .T\n";

        public const int SIZE = 15;

        const int CELL_COUNT = SIZE * SIZE;
        const int MID_ROW = SIZE / 2;
        const int MID_COL = SIZE / 2;

        private readonly char[] _cells = Enumerable.Repeat('\0', CELL_COUNT).ToArray();
        private readonly bool[] _isBlank = Enumerable.Repeat(false, CELL_COUNT).ToArray();

        private const char NONE = '\0';
        public Board()
        {

        }

        /// <summary>
        /// Whether the whole board is empty. We only check the middle cell since the first
        /// word must go through it.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _cells[GetIndex(MID_ROW, MID_COL)] == '\0';
        }

        /// <summary>
        /// Add the given word at the location and direction. If
        /// word_blank_indices is a list, then it tells the indices within "word"
        /// where a blank was used.Returns a list of tuples, one for each letter:
        /// (index in word, row, column, index, character, whether square was
        /// blank).
        /// </summary>
        /// <param name="word"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="direction"></param>
        /// <param name="wordBlankIndices"></param>
        public List<Square> AddWord(string word, int row, int col, Direction direction, int[] wordBlankIndices = null)
        {
            var addedIndices = new List<Square>();

            var word_index = 0;

            foreach(var ch in word.ToList())
            {
                if (!(col < SIZE && row < SIZE))
                {
                    throw new OutsideError();
                }

                var index = GetIndex(row, col);
                if(!(_cells[index] == NONE || _cells[index] == ch))
                {
                    throw new MismatchLetterError();
                }

                addedIndices.Add(new Square { WordIndex = word_index, Row = row, Col = col, Index = index, Letter = ch, IsBlank = (_cells[index] == NONE) });
                _cells[index] = ch;
                if(wordBlankIndices != null && wordBlankIndices.Contains(word_index))
                {
                    _isBlank[index] = true;
                }

                // get new row,col
                var newPosition = direction.Increment(row, col);
                row = newPosition.Row;
                col = newPosition.Col;

                word_index++;
            }

            return addedIndices;
        }

        /// <summary>
        /// Whether a word can fit at the given location with the given rack.
        /// If it can fit, returns a tuple of(number of "rack"'s letters that were
        /// used, list of indices within "word" where a blank was used, and
        /// a list of indices within "rack" that were used). If it cannot fit,
        /// </summary>
        /// <param name="word"></param>
        /// <param name="rack"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="direction"></param>
        /// <param name="wordIndices"></param>
        /// <param name="rackIndices"></param>
        /// <returns></returns>
        public int TryWord(string word, string rack, int row, int col, Direction direction, List<int> wordIndices, List<int> rackIndices)
        {
            var rackUsedCount = 0;

            var word_index = 0;

            foreach (var ch in word.ToList())
            {
                if (col >= SIZE)
                {
                    wordIndices.Clear();
                    rackIndices.Clear();
                    return -1;
                }

                if (row >= SIZE)
                {
                    wordIndices.Clear();
                    rackIndices.Clear();
                    return -1;
                }

                var index = GetIndex(row, col);
                var cell = _cells[index];
                if (cell == NONE)
                {
                    // if the cell is empty, then we must use a letter from the rack.
                    var rack_index = rack.IndexOf(ch);
                    if (rack_index >= 0)
                    {
                        // We have the letter
                        rackUsedCount += 1;

                        // Remove it from the rack
                        rack = rack.Substring(0, rack_index) + "!" + rack.Substring(rack_index + 1);
                        rackIndices.Add(rack_index);

                    } else
                    {
                        // see if we have a blank we can use
                        rack_index = rack.IndexOf(Bag.BLANK);
                        if (rack_index >= 0)
                        {
                            rackUsedCount += 1;

                            // Remove the blank from the rack.
                            rack = rack.Substring(0, rack_index) + "!" + rack.Substring(rack_index + 1);
                            rackIndices.Add(rack_index);
                            wordIndices.Add(rack_index);
                        }
                        else
                        {
                            // no blank for this square
                            wordIndices.Clear();
                            rackIndices.Clear();
                            return -1;
                        }
                    }
                }
                else
                {
                    if (cell != ch)
                    {
                        wordIndices.Clear();
                        rackIndices.Clear();
                        return -1;
                    }
                }

                var newPosition = direction.Increment(row, col);
                row = newPosition.Row;
                col = newPosition.Col;
            }

            return rackUsedCount;
        }


        /// <summary>
        /// Given the row and column of a square (0-based), returns the index into the arrays(also 0 - based).
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static int GetIndex(int row, int col)
        {
            return row * SIZE + col;
        }

        /// <summary>
        /// Return the letter multiplier (e.g, double and triple letter score) for the given square.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetLetterMultiplier(int index)
        {
            var ch = PREMIUM_CELLS[index];
            switch (ch)
            {
                case 'd':
                    return 2;
                case 't':
                    return 3;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Return the word multiplier (e.g, double and triple word score)
        /// for the given square.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetWordMultiplier(int index)
        {
            var ch = PREMIUM_CELLS[index];
            switch (ch)
            {
                case 'D':
                    return 2;

                case 'T':
                    return 3;

                default:
                    return 1;
            }
        }


        /// <summary>
        /// Start at row,col and go in direction and its opposite until we run off the
        /// board or find the last continuous tile.Returns(row, col, length) where length
        /// is the number of letters.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Edge FindEdges(int row, int col, Direction direction)
        {
            // Find start.
            while (true)
            {
                //# Move in reverse until we go too far.
                var position = direction.Decrement(row, col);
                row = position.Row;
                col = position.Col;

                // see if we went too far
                if( row < 0 || row >= SIZE || col < 0 || col >= SIZE || _cells[GetIndex(row, col)] != NONE)
                {
                    // back up one
                    position = direction.Increment(row, col);
                    row = position.Row;
                    col = position.Col;
                    break;
                }
            }
        }
        /*
         

            # See if we went too far.
            if row < 0 or row >= self.SIZE \
                    or col < 0 or col >= self.SIZE \
                    or not self.cells[self.get_index(row, col)]:

                # Back up one.
                row, col = direction.increment(row, col)
                break
        else:
            # Can't get here.
            raise BoardError()

        
        # Go forward until we've gone too far.
        for length in range(1, self.SIZE + 1):
            end_row, end_col = direction.increment(row, col, length)

            # See if we've gone too far.
            if end_row < 0 or end_row >= self.SIZE \
                    or end_col < 0 or end_col >= self.SIZE \
                    or not self.cells[self.get_index(end_row, end_col)]:

                return (row, col, length)

        # Can't get here.
        raise BoardError()         
            
         */

        public class Square
        {
            public int WordIndex { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
            public int Index { get; set; }
            public char Letter { get; set; }
            public bool IsBlank { get; set; }
        }

        public class Edge
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public int Length { get; set; }

        }

    }
}
