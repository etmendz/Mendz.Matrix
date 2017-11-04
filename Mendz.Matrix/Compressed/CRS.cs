using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mendz.Matrix.Compressed
{
    /// <summary>
    /// Represents a matrix in Compressed Row Storage (CRS) format.
    /// </summary>
    /// <typeparam name="T">The type of entry values.</typeparam>
    public sealed partial class CRS<T>
    {
        /// <summary>
        /// The list A[NNZ] of non-zero values.
        /// </summary>
        public List<T> Value { get; private set; } = new List<T>();

        /// <summary>
        /// The list IA[Ni+1] of row pointers, such that A[k]'s are grouped between IA[i] and IA[i + 1] - 1.
        /// </summary>
        public List<int> RowPointer { get; private set; } = new List<int>();

        /// <summary>
        /// The list JA[NNZ] of column indexes, such that JA[i] = A[i]'s column index in the matrix.
        /// </summary>
        public List<int> ColumnIndex { get; private set; } = new List<int>();

        /// <summary>
        /// The size of the matrix.
        /// </summary>
        public (int rows, int columns) Size { get; private set; }

        /// <summary>
        /// Creates an empty CRS instance representing a matrix with all values equal to default(T).
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        public CRS((int rows, int columns) size) => Size = size;

        /// <summary>
        /// Creates  a CRS instance.
        /// </summary>
        /// <param name="value">The list of values.</param>
        /// <param name="rowPointer">The row pointer.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="size">The size of the matrix.</param>
        public CRS(List<T> value, List<int> rowPointer, List<int> columnIndex, (int rows, int columns) size)
        {
            Value = value;
            RowPointer = rowPointer;
            ColumnIndex = columnIndex;
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
            List<int> rowPointer = new List<int>();
            List<int> columnIndex = new List<int>();
            (int rows, int columns) size = sparseMatrix.Size;
            var crs = from items in sparseMatrix
                      orderby items.Key
                      select (sparseMatrix.GetKeyAsCoordinates(items.Key), items.Value);
            int r = -1;
            int rp = 0;
            foreach (var item in crs)
            {
                (int row, int column) = item.Item1;
                if (row > r)
                {
                    // Fill-in for rows of 0's...
                    for (int i = row; i > r; i--)
                    {
                        rowPointer.Add(rp);
                    }
                    r = row;
                }
                value.Add(item.Value);
                columnIndex.Add(column);
                rp++;
            }
            if (rp != value.Count)
            {
                throw new DataMisalignedException("JA[N] != NNZ");
            }
            if (size.rows - 1 > r)
            {
                // Fill-in for rows of 0's...
                for (int i = size.rows - 1; i > r; i--)
                {
                    rowPointer.Add(rp);
                }
            }
            rowPointer.Add(rp);
            Value = value;
            RowPointer = rowPointer;
            ColumnIndex = columnIndex;
            Size = size;
        }

        /// <summary>
        /// Decompress to a sparse matrix instance.
        /// </summary>
        /// <typeparam name="T">The type of entry values.</typeparam>
        /// <param name="crs">The matrix as CRS.</param>
        /// <param name="sparseMatrix">The sparse matrix instance to decompress to.</param>
        public void Decompress<K>(DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            if (!sparseMatrix.Size.Equals(Size))
            {
                throw new InvalidOperationException("Sparse matrix size is invalid.");
            }
            int rows = Size.rows;
            bool isLinearIndexed = sparseMatrix.IsLinearIndexed;
            T v;
            int r, c;
            dynamic key;
            Parallel.For(0, rows, (j) =>
            {
                r = j;
                Parallel.For(RowPointer[j], RowPointer[j + 1], (i) =>
                {
                    v = Value[i];
                    c = ColumnIndex[i];
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
            (List<T> value, List<int> rowPointer, List<int> columnIndex, (int rows, int columns) size) = this;
            T[,] denseMatrix;
            denseMatrix = Matrix<T>.Create(size, defaultValue, diagonal);
            for (int j = 0; j < size.rows; j++)
            {
                for (int i = rowPointer[j]; i < rowPointer[j + 1]; i++)
                {
                    denseMatrix[j, columnIndex[i]] = value[i];
                }
            }
            return denseMatrix;
        }

        public void Deconstruct(out List<T> value, out List<int> rowPointer, out List<int> columnIndex, out (int rows, int columns) size)
        {
            value = Value;
            rowPointer = RowPointer;
            columnIndex = ColumnIndex;
            size = Size;
        }
    }
}
