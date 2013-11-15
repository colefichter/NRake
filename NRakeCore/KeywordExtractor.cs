using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using NRakeCore.StopWordFilters;

namespace NRakeCore
{
    public class KeywordExtractor
    {
        IStopWordFilter _stopWords;
        //Regex _reSplit = new Regex(@"[a-zA-Z']+");
        Regex _reSplit = new Regex(@"\s|([_,.;:!?\(\)\[\]\{\}\/\|\\\*\#\%\^\&\-\=\+])"); //This split captures punctuation, but discards spaces.

        SortedSet<string> _uniqueWords = null;

        public SortedSet<string> UniqueWordIndex
        {
            get
            {
                return _uniqueWords;
            }
        }

        public IStopWordFilter StopWordFilter
        {
            get
            {
                return this._stopWords;
            }
        }

        public KeywordExtractor()
        {
            _stopWords = new BasicStopWordFilter();
        }

        public KeywordExtractor(IStopWordFilter filter)
        {
            _stopWords = filter;
        }

        /// <summary>
        /// Returns a KeywordExtractor intialized with the best stop-word filter for a given language/culture string.
        /// The default filter is EnglishSmartStopWordFilter.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static KeywordExtractor GetBestInstanceForCulture(string culture)
        {
            IStopWordFilter filter = new EnglishSmartStopWordFilter();
            if (!string.IsNullOrEmpty(culture))
            {
                culture = culture.Trim().ToLower();
                if (culture.Length > 2)
                {
                    culture = culture.Substring(0, 2);
                }

                if (culture.Length == 2)
                {
                    switch (culture)
                    {
                        case "en":
                            break; //intentionally doing nothing

                        case "fr":
                            filter = new FrenchStopWordFilter();
                            break;

                        default:
                            break; //intentionally doing nothing
                    }
                }
                else
                {
                    //Not a valid culture/language code... ignore.
                }
            }

            return new KeywordExtractor(filter);
        }

        public string[] FindKeyPhrases(string inputText)
        {
            string[] tokens = Tokenize(inputText);
            string[] phrases = ToPhrases(tokens);
            WordCooccurrenceMatrix matrix = new WordCooccurrenceMatrix(this.UniqueWordIndex);
            matrix.CompileOccurrences(phrases);
            SortedList<string, WordScore> leagueTable = matrix.LeagueTable;
            SortedList<string, double> aggregatedLeagueTable = WordCooccurrenceMatrix.AggregateLeagueTable(leagueTable, phrases);

            int count = (int)Math.Ceiling((double)phrases.Length / (double)3); //Take the top 1/3 of the key phrases

            //TODO: make scoring system configurable (for now, just use the Ratio).
            return aggregatedLeagueTable.OrderByDescending(x => x.Value).Take(count).Select(x => x.Key).ToArray();
        }

        public string[] Tokenize(string inputText)
        {
            List<string> tokens = new List<string>();
            foreach (string s in _reSplit.Split(inputText))
            {
                if (!string.IsNullOrEmpty(s))
                {
                    tokens.Add(s.Trim().ToLower());
                }
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// Note: this method has side-effects. In addition to returning the array of phrases, it maintains the internal index of unique words.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public string[] ToPhrases(string[] tokens)
        {
            _uniqueWords = new SortedSet<string>();
            List<string> phrases = new List<string>();

            string current = string.Empty;
            foreach (string t in tokens)
            {
                if (_stopWords.IsPunctuation(t) || _stopWords.IsStopWord(t))
                {
                    //Throw it away!
                    if (current.Length > 0)
                    {
                        phrases.Add(current);
                        current = string.Empty;
                    }
                }
                else
                {
                    _uniqueWords.Add(t);
                    if (current.Length == 0)
                    {
                        current = t;
                    }
                    else
                    {
                        current += " " + t;
                    }
                }
            }

            if (current.Length > 0)
            {
                phrases.Add(current);
            }

            return phrases.ToArray();
        }

        public List<Tuple<string, float>> ScorePhrases(string[] phrases)
        {
            if (_uniqueWords == null)
            {
                throw new ApplicationException("You must call ToPhrases(string[]) before calling ScorePhrases(string[]).");
            }

            return null;

        }
    }
}
