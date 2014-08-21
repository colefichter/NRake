using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NRakeCore;

namespace UnitTestProject1
{
    [TestClass]
    public class RowOrientedSparseMatrixTests
    {
        protected static readonly int WIDTH = 4;
        protected static readonly int HEIGHT = 4;

        protected RowOrientedSparseMatrix<char> matrix = new RowOrientedSparseMatrix<char>(4, 4);

        [TestInitialize]
        public void Setup()
        {
            /* Construct a test matrix (that is not at all sparse) containing characters like:
             * 
             *       0 1 2 3
             *      --------
             *   0 | A B C D
             *   1 | E F G H
             *   2 | I J   L   <-- Note empty cell where 'K' should be...
             *   3 | M N O P
             * 
             */
            matrix = new RowOrientedSparseMatrix<char>(WIDTH, HEIGHT);
            int firstChar = 65;
            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    if (i == 2 && j == 2) { continue; } //Leave empty cell where 'K' should be
                    matrix[i, j] = Convert.ToChar(firstChar + (HEIGHT * i) + j);
                }
            }
        }

        [TestMethod]
        public void Indexers()
        {
            //Arrange

            Assert.AreEqual('A', matrix[0, 0]);
            Assert.AreEqual('J', matrix[2, 1]);
            Assert.AreEqual('H', matrix[1, 3]);
            Assert.AreEqual('P', matrix[3, 3]);

            //Act
            matrix[0, 0] = 'W';
            matrix[2, 1] = 'X';
            matrix[1, 3] = 'Y';
            matrix[3, 3] = 'Z';
            
            //Assert
            Assert.AreEqual('W', matrix[0, 0]);
            Assert.AreEqual('X', matrix[2, 1]);
            Assert.AreEqual('Y', matrix[1, 3]);
            Assert.AreEqual('Z', matrix[3, 3]);
        }

        //[TestMethod]
        //public void IsCellEmpty()
        //{
        //    //Arrange

        //    //Act

        //    //Assert
        //    Assert.IsTrue(matrix.IsCellEmpty(2, 2));
        //    Assert.IsFalse(matrix.IsCellEmpty(1, 0));
        //    Assert.IsFalse(matrix.IsCellEmpty(1, 1));
        //    Assert.IsFalse(matrix.IsCellEmpty(1, 2));
        //    Assert.IsFalse(matrix.IsCellEmpty(1, 3));
        //    Assert.IsFalse(matrix.IsCellEmpty(3, 1));
        //    Assert.IsFalse(matrix.IsCellEmpty(3, 2));
        //    Assert.IsFalse(matrix.IsCellEmpty(3, 3));
        //}
    }
}
