using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrabble
{
    class Position
    {

        public Position(int row , int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; private set; }
        public int Col { get; private set; }
    }
}
