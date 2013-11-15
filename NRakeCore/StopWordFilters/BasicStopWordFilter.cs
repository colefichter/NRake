using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRakeCore.StopWordFilters
{
    /// <summary>
    /// A simple English stop-word filter.
    /// </summary>
    public class BasicStopWordFilter : IStopWordFilter
    {
        public virtual bool IsPunctuation(string word)
        {
            bool isPunctuation = false;

            switch (word)
            {
                case @".":
                case @",":
                case @"!":
                case @"?":
                case @";":
                case @":":
                case @"(":
                case @")":
                case @"[":
                case @"]":
                case @"{":
                case @"}":
                case @"|":
                case @"/":
                case @"\":  
                case @"#":
                case @"%":
                case @"^":
                case @"&":
                case @"*":
                case @"-":
                case @"_":
                case @"+":
                case @"=":
                //case"\r":
                //case "\n":
                //case "\r\n":
                    isPunctuation = true;
                    break;
            }

            return isPunctuation;
        }

        /// <summary>
        /// Assumes that the input word is already trimmed and lowercased.
        ///   Good: "word", "of", "and", "hello"
        ///   Bad:  "Word", " of", "and ", " HeLlO "
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public virtual bool IsStopWord(string word)
        {
            if (word.Length < 3) { return true; }

            bool isStopWord = false;

            //Would it be faster to do a binary search here? I think flow control through a switch is O(1), whereas a binary search is O(log n).
            switch (word)
            {
                case "a":
                case "about":
                case "above":
                case "after":
                case "again":
                case "against":
                case "all":
                case "am":
                case "an":
                case "and":
                case "any":
                case "are":
                case "aren't":
                case "as":
                case "at":
                case "be":
                case "because":
                case "been":
                case "before":
                case "being":
                case "below":
                case "between":
                case "both":
                case "but":
                case "by":
                case "can": //Added by CF
                case "can't":
                case "cannot":
                case "could":
                case "couldn't":
                case "did":
                case "didn't":
                case "do":
                case "does":
                case "doesn't":
                case "doing":
                case "don't":
                case "down":
                case "during":
                case "each":
                case "etc": //Added by CF
                case "few":
                case "for":
                case "from":
                case "further":
                case "had":
                case "hadn't":
                case "has":
                case "hasn't":
                case "have":
                case "haven't":
                case "having":
                case "he":
                case "he'd":
                case "he'll":
                case "he's":
                case "her":
                case "here":
                case "here's":
                case "hers":
                case "herself":
                case "him":
                case "himself":
                case "his":
                case "how":
                case "how's":
                case "i":
                case "i'd":
                case "i'll":
                case "i'm":
                case "i've":
                case "if":
                case "in":
                case "into":
                case "is":
                case "isn't":
                case "it":
                case "it's":
                case "its":
                case "itself":
                case "let's":
                case "me":
                case "more":
                case "most":
                case "mustn't":
                case "my":
                case "myself":
                case "no":
                case "nor":
                case "not":
                case "of":
                case "off":
                case "on":
                case "once":
                case "only":
                case "or":
                case "other":
                case "ought":
                case "our":
                case "ours":
                case "ourselves":
                case "out":
                case "over":
                case "own":
                case "same":
                case "shan't":
                case "she":
                case "she'd":
                case "she'll":
                case "she's":
                case "should":
                case "shouldn't":
                case "so":
                case "some":
                case "such":
                case "than":
                case "that":
                case "that's":
                case "the":
                case "their":
                case "theirs":
                case "them":
                case "themselves":
                case "then":
                case "there":
                case "there's":
                case "these":
                case "they":
                case "they'd":
                case "they'll":
                case "they're":
                case "they've":
                case "this":
                case "those":
                case "through":
                case "to":
                case "too":
                case "under":
                case "until":
                case "up":
                case "use": //Added by CF
                case "used": //Added by CF
                case "very":
                case "was":
                case "wasn't":
                case "we":
                case "we'd":
                case "we'll":
                case "we're":
                case "we've":
                case "were":
                case "weren't":
                case "what":
                case "what's":
                case "when":
                case "when's":
                case "where":
                case "where's":
                case "which":
                case "while":
                case "who":
                case "who's":
                case "whom":
                case "why":
                case "why's":
                case "with":
                case "won't":
                case "would":
                case "wouldn't":
                case "you":
                case "you'd":
                case "you'll":
                case "you're":
                case "you've":
                case "your":
                case "yours":
                case "yourself":
                case "yourselves":
                    isStopWord = true;
                    break;
            }

            return isStopWord;
        }
    }
}
