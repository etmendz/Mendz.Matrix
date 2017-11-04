using Mendz.Matrix;
using Mendz.Matrix.Compressed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Mendz.Matrix
{
    [TestClass]
    public class UnitTestCompressedMatrix
    {
        public static CoordinatesKeyedSparseMatrix<int> TestSparseMatrixData()
        {
            CoordinatesKeyedSparseMatrix<int> sm = new CoordinatesKeyedSparseMatrix<int>((5, 5));
            sm.SetEntry((0, 1), 1);
            sm.SetEntry((1, 2), 1);
            sm.SetEntry((2, 3), 1);
            sm.SetEntry((3, 4), 1);
            sm.SetEntry((4, 0), 1);
            /*
             * 0 1 0 0 0
             * 0 0 1 0 0
             * 0 0 0 1 0
             * 0 0 0 0 1
             * 1 0 0 0 0
            */
            return sm;
        }

        public static (CoordinatesKeyedSparseMatrix<int> sm, int[] v, int[] a) TestSparseMatrixVectorMultiplicationData()
        {
            CoordinatesKeyedSparseMatrix<int> sm = TestSparseMatrixData();
            int[] v = new int[] { 2, 0, 5, 0, 1 };
            int[] a = { 0, 5, 0, 1, 2 }; // expected product
            /*
             * 0 1 0 0 0   2   0
             * 0 0 1 0 0   0   5
             * 0 0 0 1 0 x 5 = 0
             * 0 0 0 0 1   0   1
             * 1 0 0 0 0   1   2
            */
            return (sm, v, a);
        }

        [TestMethod]
        public void TestSparseMatrixVectorMultiplication()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, int[] v, int[] a) = TestSparseMatrixVectorMultiplicationData();
            IList<int> p = sm.MatrixVectorProduct<int, int>(v);
            CollectionAssert.AreEqual(a, p.ToArray());
        }

        public (CoordinatesKeyedSparseMatrix<int>, List<int>, List<List<int>>, MatrixLinearIndexMode) TestMatrixToCVSRMOData()
        {
            CoordinatesKeyedSparseMatrix<int> sm = TestSparseMatrixData();
            List<int> v = new List<int> { 1 };
            List<List<int>> li = new List<List<int>>() { new List<int> { 1, 7, 13, 19, 20 } };
            return (sm, v, li, MatrixLinearIndexMode.RowMajorOrder);
        }

        [TestMethod]
        public void TestMatrixToCVSRMO()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, List<int> v, List<List<int>> li, MatrixLinearIndexMode mode) = TestMatrixToCVSRMOData();
            (List<int> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size) = sm.ToCVS(MatrixLinearIndexMode.RowMajorOrder);
            CollectionAssert.AreEqual(v, value);
            CollectionAssert.AreEqual(li[0], linearIndex[0]);
            Assert.AreEqual(mode, linearIndexMode);
            Assert.AreEqual(sm.Size, size);
        }

        public (CoordinatesKeyedSparseMatrix<int>, List<int>, List<List<int>>, MatrixLinearIndexMode) TestMatrixToCVSCMOData()
        {
            CoordinatesKeyedSparseMatrix<int> sm = TestSparseMatrixData();
            List<int> v = new List<int> { 1 };
            List<List<int>> li = new List<List<int>>() { new List<int> { 4, 5, 11, 17, 23 } };
            return (sm, v, li, MatrixLinearIndexMode.ColumnMajorOrder);
        }

        [TestMethod]
        public void TestCVSRMOMatrixVectorMultiplication()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, int[] v, int[] a) = TestSparseMatrixVectorMultiplicationData();
            IList<int> p = sm.ToCVS().MatrixVectorProduct<int, int>(v);
            CollectionAssert.AreEqual(a, p.ToArray());
        }

        [TestMethod]
        public void TestMatrixToCVSCMO()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, List<int> v, List<List<int>> li, MatrixLinearIndexMode mode) = TestMatrixToCVSCMOData();
            (List<int> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size) = sm.ToCVS(MatrixLinearIndexMode.ColumnMajorOrder);
            CollectionAssert.AreEqual(v, value);
            CollectionAssert.AreEqual(li[0], linearIndex[0]);
            Assert.AreEqual(mode, linearIndexMode);
            Assert.AreEqual(sm.Size, size);
        }

        [TestMethod]
        public void TestCVSCMOMatrixVectorMultiplication()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, int[] v, int[] a) = TestSparseMatrixVectorMultiplicationData();
            IList<int> p = sm.ToCVS(MatrixLinearIndexMode.ColumnMajorOrder).MatrixVectorProduct<int, int>(v);
            CollectionAssert.AreEqual(a, p.ToArray());
        }

        public (CoordinatesKeyedSparseMatrix<int>, List<int>, List<int>, List<int>) TestMatrixToCRSData()
        {
            CoordinatesKeyedSparseMatrix<int> sm = TestSparseMatrixData();
            List<int> v = new List<int> { 1, 1, 1, 1, 1 };
            List<int> rp = new List<int> { 0, 1, 2, 3, 4, 5 };
            List<int> ci = new List<int> { 1, 2, 3, 4, 0 };
            return (sm, v, rp, ci);
        }

        [TestMethod]
        public void TestMatrixToCRS()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, List<int> v, List<int> rp, List<int> ci) = TestMatrixToCRSData();
            (List<int> value, List<int> rowPointer, List<int> columnIndex, (int rows, int columns) size) = sm.ToCRS();
            CollectionAssert.AreEqual(v, value);
            CollectionAssert.AreEqual(rp, rowPointer);
            CollectionAssert.AreEqual(ci, columnIndex);
            Assert.AreEqual(sm.Size, size);
        }

        [TestMethod]
        public void TestCRSMatrixVectorMultiplication()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, int[] v, int[] a) = TestSparseMatrixVectorMultiplicationData();
            IList<int> p = sm.ToCRS().MatrixVectorProduct<int, int>(v);
            CollectionAssert.AreEqual(a, p.ToArray());
        }

        public (CoordinatesKeyedSparseMatrix<int>, List<int>, List<int>, List<int>) TestMatrixToCCSData()
        {
            CoordinatesKeyedSparseMatrix<int> sm = TestSparseMatrixData();
            List<int> v = new List<int> { 1, 1, 1, 1, 1 };
            List<int> cp = new List<int> { 0, 1, 2, 3, 4, 5 };
            List<int> ri = new List<int> { 4, 0, 1, 2, 3 };
            return (sm, v, cp, ri);
        }

        [TestMethod]
        public void TestMatrixToCCS()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, List<int> v, List<int> cp, List<int> ri) = TestMatrixToCCSData();
            (List<int> value, List<int> columnPointer, List<int> rowIndex, (int rows, int columns) size) = sm.ToCCS();
            CollectionAssert.AreEqual(v, value);
            CollectionAssert.AreEqual(cp, columnPointer);
            CollectionAssert.AreEqual(ri, rowIndex);
            Assert.AreEqual(sm.Size, size);
        }

        [TestMethod]
        public void TestCCSMatrixVectorMultiplication()
        {
            (CoordinatesKeyedSparseMatrix<int> sm, int[] v, int[] a) = TestSparseMatrixVectorMultiplicationData();
            IList<int> p = sm.ToCCS().MatrixVectorProduct<int, int>(v);
            CollectionAssert.AreEqual(a, p.ToArray());
        }
    }
}
