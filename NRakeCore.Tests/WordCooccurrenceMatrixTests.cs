using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NRakeCore;

namespace UnitTestProject1
{
    [TestClass]
    public class WordCooccurrenceMatrixTests
    {
        protected string Sample1
        {
            get
            {
                return File.ReadAllText("Sample1.txt");
            }
        }

        [TestMethod]
        public void IndexOf()
        {
            //Arrange
            SortedSet<string> words = new SortedSet<string>();
            words.Add("a");
            words.Add("b");
            words.Add("c");
            
            //Act
            WordCooccurrenceMatrix matrix = new WordCooccurrenceMatrix(words);
            
            //Assert
            Assert.AreEqual(0, matrix.IndexOf("a"));
            Assert.AreEqual(1, matrix.IndexOf("b"));
            Assert.AreEqual(2, matrix.IndexOf("c"));
        }

        [TestMethod]
        public void IncrementCounterCell()
        {
            //Arrange
            SortedSet<string> words = new SortedSet<string>();
            words.Add("a");
            words.Add("b");
            words.Add("c");
            WordCooccurrenceMatrix matrix = new WordCooccurrenceMatrix(words);

            //Act
            var res1 = matrix[0, 0];
            matrix.IncrementCounterCell(0, 0);
            var res2 = matrix[0, 0];
            matrix.IncrementCounterCell(0, 0);
            var res3 = matrix[0, 0];

            //Assert
            Assert.AreEqual(default(int), res1);
            Assert.AreEqual(1, res2);
            Assert.AreEqual(2, res3);
        }

        [TestMethod]
        public void CompileOccurrences()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string[] tokens = extractor.Tokenize(this.Sample1);
            string[] phrases = extractor.ToPhrases(tokens);
            WordCooccurrenceMatrix matrix = new WordCooccurrenceMatrix(extractor.UniqueWordIndex);

            //Act
            matrix.CompileOccurrences(phrases);

            //Assert
            Assert.AreEqual(2, matrix[matrix.IndexOf("algorithms"), matrix.IndexOf("algorithms")], "'algorithms' diagonal count");
            Assert.AreEqual(1, matrix[matrix.IndexOf("bounds"), matrix.IndexOf("bounds")], "'bounds' diagonal count");
            Assert.AreEqual(1, matrix[matrix.IndexOf("corresponding"), matrix.IndexOf("algorithms")], "'corresponding'->'algorithms' count");
            Assert.AreEqual(2, matrix[matrix.IndexOf("minimal"), matrix.IndexOf("set")], "'minimal'->'set' count");
            Assert.AreEqual(2, matrix[matrix.IndexOf("set"), matrix.IndexOf("minimal")], "'set'->'minimal' count");
        }

        [TestMethod]
        public void ComputeLeagueTable()
        {
            //Arrange
            KeywordExtractor extractor = new KeywordExtractor();
            string[] tokens = extractor.Tokenize(this.Sample1);
            string[] phrases = extractor.ToPhrases(tokens);
            WordCooccurrenceMatrix matrix = new WordCooccurrenceMatrix(extractor.UniqueWordIndex);

            //Act
            matrix.CompileOccurrences(phrases);
            SortedList<string, WordScore> leagueTable = matrix.LeagueTable;

            //Assert
            Assert.AreEqual(3, leagueTable["algorithms"].Degree, "Degree 1");
            Assert.AreEqual(2, leagueTable["algorithms"].Frequency, "Frequency 1");
            Assert.AreEqual(1.5, leagueTable["algorithms"].Ratio, "Ratio 1");

            Assert.AreEqual(8, leagueTable["minimal"].Degree, "Degree 1");
            Assert.AreEqual(5, leagueTable["minimal"].Frequency, "Frequency 1");
            Assert.AreEqual(1.6, leagueTable["minimal"].Ratio, "Ratio 1");
        }

        [TestMethod]
        public void AggregateLeagueTable()
        {
            //Arrange
            SortedList<string, WordScore> leagueTable = new SortedList<string, WordScore>();
            leagueTable.Add("algorithms", new WordScore(3, 2));
            leagueTable.Add("bounds", new WordScore(2, 1));
            leagueTable.Add("compatibility", new WordScore(2, 2));
            leagueTable.Add("components", new WordScore(1, 1));
            leagueTable.Add("constraints", new WordScore(5, 3));
            string[] phrases = new string[] { "algorithms", "bounds compatibility", "components", "constraints bounds" };

            //Act
            SortedList<string, double> agg = WordCooccurrenceMatrix.AggregateLeagueTable(leagueTable, phrases);
            
            //Assert
            Assert.AreEqual(1.5, agg["algorithms"]);
            Assert.AreEqual(3, agg["bounds compatibility"]);
            Assert.AreEqual(1, agg["components"]);
            Assert.AreEqual(Math.Round((double)11/(double)3, 4), Math.Round(agg["constraints bounds"], 4));
        }
    }
}
