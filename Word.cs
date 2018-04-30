using System;
using System.Collections.Generic;

namespace Scrabble
{
    public class Word
    {
        public Word(IEnumerable<string> anchor)
        {
            Anchor = new List<String>(anchor);
            Value = null;
        }

        public Word(String value)
        {
            Anchor = null;
            Value = value;
        }

        public bool IsAnchor
        {
            get { return Anchor != null;  }
        }

        public List<string> Anchor { get; private set; }

        public string Value { get; private set; }

    }

}
