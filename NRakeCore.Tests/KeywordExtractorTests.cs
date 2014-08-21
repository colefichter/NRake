using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlAgilityPack;

using NRakeCore;
using NRakeCore.StopWordFilters;

namespace UnitTestProject1
{
    [TestClass]
    public class KeywordExtractorTests
    {
        protected string LockInsText
        {
            get
            {
                //This is the boilerpipe extracted content from http://federalreserve.gov/pubs/lockins/default.htm
                return File.ReadAllText("lock-ins.txt");
            }
        }

        protected string Sample1
        {
            get
            {
                return File.ReadAllText("Sample1.txt");
            }
        }
        protected string[] Sample1ExpectedOutput
        {
            get
            {
                return File.ReadAllLines("Sample1_ExpectedOutput.txt").Select(x => x.Trim()).ToArray();
            }
        }

        #region GetBestInstanceForCulture (Note, some of the tests incorporate mixed case, rather than having a specific test for that...)

        [TestMethod]
        public void GetBestInstanceForCulture_Empty()
        {
            //Arrange
            string lang = string.Empty;

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(EnglishSmartStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_Invalid()
        {
            //Arrange
            string lang = "not a real culter, yo!";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(EnglishSmartStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_EN()
        {
            //Arrange
            string lang = @"EN";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(EnglishSmartStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_ENCA()
        {
            //Arrange
            string lang = "en-CA";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(EnglishSmartStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_ENUS()
        {
            //Arrange
            string lang = "en-us";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(EnglishSmartStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_ENGB()
        {
            //Arrange
            string lang = "en-gb";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(EnglishSmartStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_FR()
        {
            //Arrange
            string lang = "fr";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(FrenchStopWordFilter), extractor.StopWordFilter.GetType());
        }

        [TestMethod]
        public void GetBestInstanceForCulture_FRCA()
        {
            //Arrange
            string lang = "fr-ca";

            //Act
            var extractor = KeywordExtractor.GetBestInstanceForCulture(lang);

            //Assert
            Assert.AreEqual(typeof(FrenchStopWordFilter), extractor.StopWordFilter.GetType());
        }

        #endregion

        [TestMethod]
        public void FindKeyPhrases()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            //string[] expected = Sample1ExpectedOutput;

            //Act
            var res = extractor.FindKeyPhrases(this.Sample1);

            //Assert
            Assert.AreEqual(11, res.Length);
            Assert.AreEqual("minimal supporting set", res[0]);
        }

        [TestMethod]
        public void FindKeyPhrases_DuplicateKeyException()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            
            //Act
            var res = extractor.FindKeyPhrases(this.LockInsText);

            //CF Aug 21, 2014: The LockInsText file is weird! The first instance of the word "application" has some wonky encoding that actually
            // ends up as "appli-cation", but it's not displayed consistently in all views.  In WordCooccurrenceMatrix.AggregateLeagueTable()
            // we end up with both variants being put in the array created by .Distinct() but on insertion into the dict, they are treated as
            // an identical key! WTF!
            // This used to throw an unhandled exception (see http://github.dev/yellowpencil/Octave/issues/383) but we've added a try/catch that
            // swallows the exception because it should only occur in really bizarre cases like this one. (Shouldn't be a perf issue either).
            
            //Assert
        }

        [TestMethod]
        public void Tokenize()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string[] expected = new string[] { };

            //Act
            var res = extractor.Tokenize("The quick, brown fox jumps over the lazy dog. Yes he does!");

            //Assert
            Assert.AreEqual(15, res.Length);
            Assert.AreEqual("the", res[0]);
            Assert.AreEqual("quick", res[1]);
            Assert.AreEqual(",", res[2]);
            Assert.AreEqual("brown", res[3]);
            Assert.AreEqual("fox", res[4]);
            Assert.AreEqual("does", res[13]);
            Assert.AreEqual("!", res[14]);
        }

        [TestMethod]
        public void ToPhrases()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string input = "Compatibility of systems of linear constraints over the set of natural numbers.";
            string[] tokens = extractor.Tokenize(input);
            string[] expectedPhrases = new string[] { "compatibility", "systems", "linear constraints", "set", "natural numbers" };

            //Act
            var res = extractor.ToPhrases(tokens);

            //Assert
            Assert.AreEqual(expectedPhrases.Length, res.Length);
            for (int i = 0; i < res.Length; i++)
            {
                Assert.AreEqual(expectedPhrases[i], res[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ScorePhrases_ExpectException()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string[] expectedPhrases = new string[] { "compatibility", "systems", "linear constraints", "set", "natural numbers" };

            //Act
            //This should throw an exception because we did not call ToPhrases() to initialize the index of unique words
            var res = extractor.ScorePhrases(expectedPhrases);

            //Assert
        }

        [TestMethod]
        public void FindKeyPhrases_LargeHtmlFile()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            HtmlDocument doc = new HtmlDocument();
            doc.Load("LargeFile.html");
            var body = doc.DocumentNode.SelectSingleNode("descendant-or-self::*[contains(concat(' ', normalize-space(@class), ' '), ' content ')]");
            RemoveComments(body);
            string text = body.InnerText;

            //Act
            var res = extractor.FindKeyPhrases(text);

            //Assert            
        }

        [TestMethod]
        public void FindKeyPhrases_LargeHtmlFile2()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            HtmlDocument doc = new HtmlDocument();
            doc.Load("LargeFile2.html");
            var body = doc.DocumentNode.SelectSingleNode("descendant-or-self::*[contains(concat(' ', normalize-space(@class), ' '), ' posts ')]");
            RemoveComments(body);
            string text = body.InnerText;

            //Act
            var res = extractor.FindKeyPhrases(text);

            //Assert            
        }

        public static void RemoveComments(HtmlNode node)
        {
            foreach (var n in node.ChildNodes.ToArray())
                RemoveComments(n);
            if (node.NodeType == HtmlNodeType.Comment)
                node.Remove();
        }
    }
}
