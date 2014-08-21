using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRakeCore
{
    public sealed class WordCooccurrenceMatrix
    {
        //private SparseMatrix<int> _matrix;
        private RowOrientedSparseMatrix<int> _matrix;
        private int _n = 0;
        private string[] _lookupTable = null;

        public int this[int row, int col]
        {
            get
            {
                return _matrix[row, col];
            }
            set
            {
                _matrix[row, col] = value;
            }
        }

        private WordCooccurrenceMatrix() { }

        public WordCooccurrenceMatrix(SortedSet<string> words)
        {
            _lookupTable = words.ToArray();
            //Create a square (n x n) matrix with the same rows and columns.
            _n = _lookupTable.Length;
            //_matrix = new SparseMatrix<int>(_n, _n);
            _matrix = new RowOrientedSparseMatrix<int>(_n, _n);
        }

        public int IndexOf(string word)
        {
            return Array.BinarySearch<string>(_lookupTable, word);
        }

        public void CompileOccurrences(string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                CompileOccurrences(phrase);
            }
        }

        public void CompileOccurrences(string phrase)
        {
            string[] words = phrase.Split(' ');
            for (int r = 0; r < words.Length; r++) //Iterate the rows for each word
            {
                int rowIndex = IndexOf(words[r]);
                for (int c = 0; c < words.Length; c++) //Iterate the columns for each word
                {
                    int colIndex = IndexOf(words[c]);
                    IncrementCounterCell(rowIndex, colIndex);
                }
            }
        }

        public void IncrementCounterCell(int row, int col)
        {
            _matrix[row, col] += 1;
        }

        private SortedList<string, WordScore> ComputeLeagueTable()
        {
            SortedList<string, WordScore> leagueTable = new SortedList<string, WordScore>();

            //foreach (string s in _lookupTable)
            //{
            //    int degree = 0;
            //    int frequency = 0;
            //    int rowIndex = IndexOf(s);
            //    for (int c = 0; c < _n; c++) //Examine every column in the row
            //    {
            //        int entry = _matrix[rowIndex, c];
            //        if (entry != default(int) && entry > 0)
            //        {
            //            frequency += 1;
            //            degree += entry;
            //        }
            //    }
            //    leagueTable.Add(s, new WordScore(degree, frequency));
            //}
            foreach (string s in _lookupTable)
            {
                int degree = 0;
                int frequency = 0;
                int rowIndex = IndexOf(s);

                int[] entries = _matrix.Row(rowIndex);
                foreach (int entry in entries)
                {
                    if (entry != default(int) && entry > 0)
                    {
                        frequency += 1;
                        degree += entry;
                    }
                }               
                leagueTable.Add(s, new WordScore(degree, frequency));
            }

            return leagueTable;
        }

        public static SortedList<string, double> AggregateLeagueTable(SortedList<string, WordScore> leagueTable, string[] phrases)
        {
            SortedList<string, double> agg = new SortedList<string, double>();
            foreach (string phrase in phrases.Distinct())
            {
                string[] words = phrase.Split(' ');
                double ratio = 0;
                foreach (string word in words)
                {
                    ratio += leagueTable[word].Ratio;
                }
                try
                {
                    agg.Add(phrase, ratio);
                }
                catch (ArgumentException)
                {
                    //You would think that the .Distinct() call would prevent duplicate keys in the dict, but the strangest bug I've ever seen
                    //occurs when we extract the text from http://federalreserve.gov/pubs/lockins/default.htm and run it through here.
                    //See KeywordExtractorTests.FindKeyPhrases_DuplicateKeyException().
                }
            }

            return agg;
        }


        public SortedList<string, WordScore> LeagueTable
        {
            get
            {
                return ComputeLeagueTable();
            }
        }
    }
}
