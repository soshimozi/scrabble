using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Scrabble
{
    /// <summary>
    /// Represetns a direction (vertical or horizontal)
    /// </summary>
    class Direction
    {
        private readonly int _dRow, _dCol;

        public Direction(int deltaRow, int deltaCol)
        {
            _dRow = deltaRow;
            _dCol = deltaCol;
        }

        /// <summary>
        /// Return a (row,col) pair based on the given pair and a distance and
        /// this direction.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Position Increment(int row, int col, int distance = 1)
        {
            return new Position(row + _dRow * distance, col + _dCol * distance);
        }
        /// <summary>
        /// Return a (row,col) pair based on the given pair and a distance and
        /// the opposite of this direction.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Position Decrement(int row, int col, int distance = 1)
        {
            return new Position(row - _dRow * distance, col - _dCol * distance);
        }

        /// <summary>
        /// Given horizontal, returns vertical and vice versa.
        /// </summary>
        /// <returns></returns>
        public Direction GetPerpendicularDirection()
        {
            return new Direction(_dCol, _dRow);
        }


        /// <summary>
        /// Given a line (in the orthogonal axis) and position (in the axis of this
        /// direction) returns a(row, col) pair for the absolute square position.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public Position GetAbsolutePosition(int pos, int line)
        {
            return new Position(_dRow * pos + _dCol * line, _dCol * pos + _dRow * line);
        }

        /// <summary>
        /// Same as get_absolute_position() but relative to the given position.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="dpos"></param>
        /// <param name="dline"></param>
        /// <returns></returns>
        public Position GetRelativePosition(int row, int col, int dpos, int dline)
        {
            return new Position(row + _dRow * dpos + _dCol * dline, col + _dCol * dpos + _dRow * dline);
        }

        public override string ToString()
        {
            return this == HORIZONTAL ? "H" : "V";
        }

        // constants representing the deltas in the two directions
        public static readonly Direction HORIZONTAL = new Direction(0, 1);
        public static readonly Direction VERTICAL = new Direction(1, 0);

        // all known directions
        public static readonly Direction[] DIRECTIONS = new Direction[2] { HORIZONTAL, VERTICAL };
    }
}

