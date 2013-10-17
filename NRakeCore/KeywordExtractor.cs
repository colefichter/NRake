using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NRakeCore
{
    public class KeywordExtractor
    {
        IStopWordFilter _stopWords;
        Regex _reSplit = new Regex(@"[a-zA-Z']+");

        public KeywordExtractor()
        {
            _stopWords = new BasicStopWordFilter();
        }

        public KeywordExtractor(IStopWordFilter filter)
        {
            _stopWords = filter;
        }

        public string[] FindKeyWords(string inputText)
        {
            return new string[] { };
        }

        public string[] Tokenize(string inputText)
        {
            List<string> tokens = new List<string>();
            foreach (Match m in _reSplit.Matches(inputText))
            {
                if (!string.IsNullOrEmpty(m.Value))
                {
                    tokens.Add(m.Value.Trim().ToLower());
                }
            }

            return tokens.ToArray();
        }

        public string[] ToPhrases(string[] tokens)
        {
            List<string> phrases = new List<string>();

            string current = string.Empty;
            foreach (string t in tokens)
            {
                if (_stopWords.IsStopWord(t))
                {
                    if (current.Length > 0)
                    {
                        phrases.Add(current);
                        current = string.Empty;
                    }
                }
                else
                {
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
    }
}
