using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mendz.Matrix.Compressed
{
    /// <summary>
    /// Represents a matrix in Compressed Column Storage (CCS) format.
    /// </summary>
    /// <typeparam name="T">The type of entry values.</typeparam>
    public sealed partial class CCS<T>
    {
        /// <summary>
        /// The list A[NNZ] of non-zero values.
        /// </summary>
        public List<T> Value { get; private set; } = new List<T>();

        /// <summary>
        /// The list IA[Ni+1] of column pointers, such that A[k]'s are grouped between IA[i] and IA[i + 1] - 1.
        /// </summary>
        public List<int> ColumnPointer { get; private set; } = new List<int>();

        /// <summary>
        /// The list JA[NNZ] of row indexes, such that JA[i] = A[i]'s row index in the matrix.
        /// </summary>
        public List<int> RowIndex { get; private set; } = new List<int>();

        /// <summary>
        /// The size of the matrix.
        /// </summary>
        public (int rows, int columns) Size { get; private set; }

        /// <summary>
        /// Creates an empty CCS instance representing a matrix with all values equal to default(T).
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        public CCS((int rows, int columns) size) => Size = size;

        /// <summary>
        /// Creates  a CCS instance.
        /// </summary>
        /// <param name="value">The list of values.</param>
        /// <param name="columnPointer">The column pointer.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="size">The size of the matrix.</param>
        public CCS(List<T> value, List<int> columnPointer, List<int> rowIndex, (int rows, int columns) size)
        {
            Value = value;
            ColumnPointer = columnPointer;
            RowIndex = rowIndex;
            Size = size;
        }

        /// <summary>
        /// Compresses a sparse matrix.
        /// </summary>
        /// <typeparam name="K">The type of keys.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix to compress.</param>
        public void Compress<K>(DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            List<T> value = new List<T>();
            List<int> columnPointer = new List<int>();
            List<int> rowIndex = new List<int>();
            (int rows, int columns) size = sparseMatrix.Size;
            var ccs = from items in sparseMatrix
                      orderby sparseMatrix.GetKeyAsLinearIndex(items.Key, MatrixLinearIndexMode.ColumnMajorOrder)
                      select (sparseMatrix.GetKeyAsCoordinates(items.Key), items.Value);
            int c = -1;
            int cp = 0;
            foreach (var item in ccs)
            {
                (int row, int column) = item.Item1;
                if (column > c)
                {
                    // Fill-in for columns of 0's...
                    for (int i = column; i > c; i--)
                    {
                        columnPointer.Add(cp);
                    }
                    c = column;
                }
                value.Add(item.Value);
                rowIndex.Add(row);
                cp++;
            }
            if (cp != value.Count)
            {
                throw new DataMisalignedException("JA[N] != NNZ");
            }
            if (size.columns - 1 > c)
            {
                // Fill-in for columns of 0's...
                for (int i = size.columns - 1; i > c; i--)
                {
                    columnPointer.Add(cp);
                }
            }
            columnPointer.Add(cp);
            Value = value;
            ColumnPointer = columnPointer;
            RowIndex = rowIndex;
            Size = size;
        }

        /// <summary>
        /// Decompress to a sparse matrix instance.
        /// </summary>
        /// <typeparam name="T">The type of entry values.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix instance to decompress to.</param>
        public void Decompress<K>(DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            if (!sparseMatrix.Size.Equals(Size))
            {
                throw new InvalidOperationException("Sparse matrix size is invalid.");
            }
            int columns = Size.columns;
            bool isLinearIndexed = sparseMatrix.IsLinearIndexed;
            T v;
            int r, c;
            dynamic key;
            Parallel.For(0, columns, (j) =>
            {
                c = j;
                Parallel.For(ColumnPointer[j], ColumnPointer[j + 1], (i) =>
                {
                    v = Value[i];
                    r = RowIndex[i];
                    if (isLinearIndexed)
                    {
                        key = MatrixCoordinates.ToLinearIndex(Size, (r, c), sparseMatrix.LinearIndexMode);
                    }
                    else
                    {
                        key = (r, c);
                    }
                    sparseMatrix.SetEntry(key, v);
                });
            });
        }

        /// <summary>
        /// Decompress to a dense matrix instance.
        /// </summary>
        /// <param name="defaultValue">The default entry value.</param>
        /// <param name="diagonal">The default diagonal entry value.</param>
        /// <returns>The dense matrix instance.</returns>
        public T[,] Decompress(T defaultValue = default, T diagonal = default)
        {
            (List<T> value, List<int> columnPointer, List<int> rowIndex, (int rows, int columns) size) = this;
            T[,] denseMatrix;
            denseMatrix = Matrix<T>.Create(size, defaultValue, diagonal);
            for (int j = 0; j < size.columns; j++)
            {
                for (int i = columnPointer[j]; i < columnPointer[j + 1]; i++)
                {
                    denseMatrix[rowIndex[i], j] = value[i];
                }
            }
            return denseMatrix;
        }

        public void Deconstruct(out List<T> value, out List<int> columnPointer, out List<int> rowIndex, out (int rows, int columns) size)
        {
            value = Value;
            columnPointer = ColumnPointer;
            rowIndex = RowIndex;
            size = Size;
        }
    }
}
