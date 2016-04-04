using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WunderBraSolver
{
    /// <summary>
    /// struct for letter index
    /// </summary>
    public struct LetterIndex
    {
        public LetterIndex(int i, int j, int k)
        {
            I = i;
            J = j;
            K = k;

            _compare = i + j * 10 + k * 100;
        }
        public readonly int I;
        public readonly int J;
        public readonly int K;

        readonly int _compare;

        public static bool operator ==(LetterIndex c1, LetterIndex c2) => c1._compare == c2._compare;

        public static bool operator !=(LetterIndex c1, LetterIndex c2) => c1._compare != c2._compare;

        public override bool Equals(object obj)
        {
            var comp = (LetterIndex)obj;
            return _compare == comp._compare;
        }

        public override int GetHashCode() => _compare;

        public override string ToString() => $"{I}, {J}, {K}";
    }

    /// <summary>
    /// Solve wunderdog cube
    /// </summary>
    public class Solver
    {
        // words
        IReadOnlyList<string> _words = new List<string>();
        // found words
        List<string> _found = new List<string>();
        // wunder cube letters
        static char[,,] _letters;

        // cube side length
        const int Lenght = 4;
        static int MaxDegreeOfParallelism = 1;

        public Solver(IReadOnlyList<string> words)
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount;

            _words = words;
            InitWordCube();
        }

        /// <summary>
        /// Solve cube
        /// </summary>
        /// <returns>word count</returns>
        public int Solve()
        {
            FindWords();
            return _found.Count;
        }

        // find all words in cube
        void FindWords()
        {
            Parallel.ForEach(_words, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, (word) =>
            {
                // find starting positions
                var starts = FindStarts(word[0]);

                var found = false;
                if (starts.Count() > 0 && word.Length == 1)
                {
                    found = true;
                    _found.Add(word);
                    return;
                }
                foreach (var start in starts)
                {
                    var usedLetters = new HashSet<LetterIndex>();
                    // find subsequent letters
                    if (FindWord(start, word, 1, usedLetters))
                    {
                        if (!_found.Contains(word))
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (found) _found.Add(word);
            }
            );
        }

        // find word in wunder cube
        static bool FindWord(LetterIndex start, string word, int currentIndex, HashSet<LetterIndex> usedLetters)
        {
            usedLetters.Add(start);
            HashSet<LetterIndex> clone;
            var positions = FindNeighbors(start, word[currentIndex], usedLetters);
            var ok = false;
            if (positions.Count() > 0)
            {
                if (currentIndex == word.Length - 1)
                {
                    return true;
                }
                currentIndex++;
                clone = usedLetters;
                foreach (var pos in positions)
                {
                    // because hashset constructor takes time, it's faster to clone it only when needed
                    if (positions.Count > 1)
                    {
                        clone = new HashSet<LetterIndex>(usedLetters);
                    }
                    ok = FindWord(pos, word, currentIndex, clone);
                    if (ok)
                    {
                        break;
                    }
                }
            }
            return ok;
        }

        // find all starting positions
        static IReadOnlyList<LetterIndex> FindStarts(char letter)
        {
            var starts = new List<LetterIndex>();
            for (var i = 0; i < Lenght; i++)
            {
                for (var j = 0; j < Lenght; j++)
                {
                    for (var k = 0; k < Lenght; k++)
                    {
                        if (_letters[i, j, k] == letter)
                        {
                            starts.Add(new LetterIndex(i, j, k));
                        }
                    }
                }
            }
            return starts;
        }

        // find all neighbors that contains letter
        static IReadOnlyList<LetterIndex> FindNeighbors(LetterIndex pos, char letter, HashSet<LetterIndex> usedLetters)
        {
            var ret = new List<LetterIndex>();
            for (var i = Math.Max(0, pos.I - 1); i <= Math.Min(Lenght - 1, pos.I + 1); i++)
            {
                for (var j = Math.Max(0, pos.J - 1); j <= Math.Min(Lenght - 1, pos.J + 1); j++)
                {
                    for (var k = Math.Max(0, pos.K - 1); k <= Math.Min(Lenght - 1, pos.K + 1); k++)
                    {
                        var li = new LetterIndex(i, j, k);
                        if (ContainsLetter(li, letter) && !usedLetters.Contains(li))
                        {
                            ret.Add(li);
                        }
                    }
                }
            }
            return ret;
        }

        // check if position contains letter
        static bool ContainsLetter(LetterIndex pos, char letter) => (_letters[pos.I, pos.J, pos.K] == letter);

        // init letters to
        static void InitWordCube()
        {
            _letters = new char[,,]
             {
                {
                    {'A','J','F','E'},
                    {'A','P','U','W'},
                    {'O','G','M','R'},
                    {'M','N','X','K'}
                },
                {
                    {'D','N','S','I'},
                    {'F','O','D','S'},
                    {'J','E','G','I'},
                    {'W','K','P','R'}
                },
                {
                    {'E','Q','M','F'},
                    {'R','K','I','D'},
                    {'D','M','I','R'},
                    {'E','O','S','D'}
                    },
                {
                    {'R','T','S','L'},
                    {'D','K','P','I'},
                    {'S','P','O','I'},
                    {'J','Q','D','T'}
                }
             };
        }
    }
}