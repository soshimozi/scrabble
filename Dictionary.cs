using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrabble
{
    class Dictionary
    {
        private List<string> _words = new List<string>();
        private DefaultDictionary<string, List<string>> letters_map = new DefaultDictionary<string, List<string>>();
        private DefaultDictionary<string, List<string>> letters_map_one_blank = new DefaultDictionary<string, List<string>>();
        private DefaultDictionary<string, List<string>> letters_map_two_blanks = new DefaultDictionary<string, List<string>>();
        private HashSet<string> word_set = new HashSet<string>();

        public Dictionary()
        {

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

            foreach(var word in _words)
            {
                var letters = new string(word.Distinct().Select(c => Char.ToUpper(c)).ToArray());
                letters_map[letters].Add(word);

                foreach(var subword in letters.RemoveOneLetter())
                {
                    letters_map_one_blank[subword].Add(word);
                }

                foreach(var subword in letters.RemoveTwoLetters())
                {
                    letters_map_two_blanks[subword].Add(word);
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

        public static Dictionary Load(string filename)
        {
            var dictionary = new Dictionary();

            var whole_file = System.IO.File.ReadAllText(filename);

            var words = whole_file.Split();

            words = dictionary.RemoveUnsuitableWords(words, 15);

            dictionary.SetWords(words);

            return dictionary;
        }

        private string[] RemoveUnsuitableWords(string[] words, int size)
        {
            return words.Where(w => w.Length < size && !w.Contains("-")).ToArray();
        }
    }
}
