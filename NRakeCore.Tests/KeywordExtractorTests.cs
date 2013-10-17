using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NRakeCore;

namespace UnitTestProject1
{
    [TestClass]
    public class KeywordExtractorTests
    {
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

        [TestMethod]
        public void FindKeyWords()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string[] expected = Sample1ExpectedOutput;

            //Act
            var res = extractor.FindKeyWords(this.Sample1);

            //Assert
            Assert.AreEqual(expected.Length, res.Length);
        }

        [TestMethod]
        public void Tokenize1()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string[] expected = new string[] { };

            //Act
            var res = extractor.Tokenize("The quick, brown fox jumps over the lazy dog. Yes he does!");

            //Assert
            Assert.AreEqual(12, res.Length);
            Assert.AreEqual("the", res[0]);
            Assert.AreEqual("fox", res[3]);
            Assert.AreEqual("does", res[11]);
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
    }
}
