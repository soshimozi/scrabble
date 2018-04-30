using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrabble
{
    class Dictionary
    {
        private readonly List<string> _words = new List<string>();
        public readonly DefaultDictionary<string, List<string>> LettersMap = new DefaultDictionary<string, List<string>>();
        public readonly DefaultDictionary<string, List<string>> LettersMapOneBlank = new DefaultDictionary<string, List<string>>();
        public readonly DefaultDictionary<string, List<string>> LettersMapTwoBlanks = new DefaultDictionary<string, List<string>>();

        private readonly HashSet<string> _wordSet = new HashSet<string>();

        public Dictionary()
        {

        }

        public bool HasWord(string word)
        {
            return _wordSet.Contains(word);
        }

        public static Dictionary Load(string filename)
        {
            var dictionary = new Dictionary();

            var whole_file = System.IO.File.ReadAllText(filename);

            var words = whole_file.Split();

            words = dictionary.RemoveUnsuitableWords(words, 15);

            dictionary.SetWords(words);

            return dictionary;
        }

        private void SetWords(string[] words)
        {
            _words.AddRange(words);
            GenerateLetterMaps();
        }

        private void GenerateLetterMaps()
        {
            var word_count = _words.Count;
            var last_percent = 0;
            var i = 0;

            foreach (var word in _words)
            {
                var letters = new string(word.Distinct().Select(c => Char.ToUpper(c)).ToArray());
                LettersMap[letters].Add(word);

                foreach (var subword in letters.RemoveOneLetter())
                {
                    LettersMapOneBlank[subword].Add(word);
                }

                foreach (var subword in letters.RemoveTwoLetters())
                {
                    LettersMapTwoBlanks[subword].Add(word);
                }

                //# Show progress information.
                var percent = (int)(i * 100 / word_count);
                if (percent / 10 != last_percent / 10)
                {
                    Console.WriteLine($"    {percent}%");
                    last_percent = percent;
                }

                i++;
            }
        }

        private string[] RemoveUnsuitableWords(string[] words, int size)
        {
            return words.Where(w => w.Length < size && !w.Contains("-")).ToArray();
        }
    }
}
