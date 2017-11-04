using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mendz.Matrix.Compressed
{
    /// <summary>
    /// Represents a matrix in Compressed Value Storage (CVS) format.
    /// </summary>
    /// <typeparam name="T">The type of entry values.</typeparam>
    public sealed partial class CVS<T>
    {
        /// <summary>
        /// The list A[NDNZ] of distinct non-zero values.
        /// </summary>
        public List<T> Value { get; private set; } = new List<T>();

        /// <summary>
        /// The list of lists LA[NDNZ][Ng] of A[i]'s linear indexes.
        /// </summary>
        public List<List<int>> LinearIndex { get; private set; } = new List<List<int>>();

        /// <summary>
        /// The linear indexing mode.
        /// </summary>
        public MatrixLinearIndexMode LinearIndexMode { get; private set; } = MatrixLinearIndexMode.RowMajorOrder;

        /// <summary>
        /// The size of the matrix.
        /// </summary>
        public (int rows, int columns) Size { get; private set; }

        /// <summary>
        /// Creates an empty CVS instance representing a matrix with all values equal to default(T).
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        public CVS((int rows, int columns) size)
            : this(MatrixLinearIndexMode.RowMajorOrder, size)
        {

        }

        /// <summary>
        /// Creates an empty CVS instance representing a matrix with all values equal to default(T).
        /// </summary>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <param name="size">The size of the matrix.</param>
        public CVS(MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size)
        {
            LinearIndexMode = linearIndexMode;
            Size = size;
        }

        /// <summary>
        /// Creates a CVS instance.
        /// </summary>
        /// <param name="value">The list of values.</param>
        /// <param name="linearIndex">The list of lists of linear indexes.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <param name="size">The size of the matrix.</param>
        public CVS(List<T> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size)
        {
            Value = value;
            LinearIndex = linearIndex;
            LinearIndexMode = linearIndexMode;
            Size = size;
        }

        /// <summary>
        /// Compresses a sparse matrix.
        /// </summary>
        /// <typeparam name="K">The type of keys.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix.</param>
        public void Compress<K>(DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            List<T> value = new List<T>();
            List<List<int>> linearIndex = new List<List<int>>();
            Size = sparseMatrix.Size;
            bool isLinearIndexed = sparseMatrix.IsLinearIndexed;
            if (isLinearIndexed)
            {
                LinearIndexMode = sparseMatrix.LinearIndexMode;
            }
            foreach (var item in sparseMatrix)
            {
                T v = item.Value;
                if (!value.Contains(v))
                {
                    value.Add(v);
                }
                int i = value.IndexOf(v);
                int count = linearIndex.Count;
                if (count == 0 || i > count - 1)
                {
                    linearIndex.Add(new List<int>());
                }
                if (isLinearIndexed)
                {
                    linearIndex[i].Add(sparseMatrix.GetKeyAsLinearIndex(item.Key));
                }
                else
                {
                    linearIndex[i].Add(MatrixCoordinates.ToLinearIndex(Size, sparseMatrix.GetKeyAsCoordinates(item.Key), LinearIndexMode));
                }
            }
            foreach (var lis in linearIndex)
            {
                lis.Sort();
            }
            Value = value;
            LinearIndex = linearIndex;
        }

        /// <summary>
        /// Decompress a CVS instance to a sparse matrix instance.
        /// </summary>
        /// <typeparam name="K">The type of keys.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix.</param>
        public void Decompress<K>(DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            if (!sparseMatrix.Size.Equals(Size))
            {
                throw new InvalidOperationException("Sparse matrix size is invalid.");
            }
            if (sparseMatrix.LinearIndexMode != LinearIndexMode)
            {
                throw new InvalidOperationException("Sparse matrix linear index mode is invalid.");
            }
            bool isLinearIndexed = sparseMatrix.IsLinearIndexed;
            dynamic key;
            Parallel.For(0, Value.Count, (i) =>
            {
                T v = Value[i];
                Parallel.ForEach(LinearIndex[i], (li) =>
                {
                    if (isLinearIndexed)
                    {
                        key = li;
                    }
                    else
                    {
                        key = MatrixCoordinates.ToCoordinates(Size, li, LinearIndexMode);
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
            (List<T> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size) = this;
            T[,] denseMatrix = Matrix<T>.Create(size, defaultValue, diagonal);
            for (int i = 0; i < value.Count; i++)
            {
                T v = value[i];
                foreach (var li in linearIndex[i])
                {
                    (int row, int column) = MatrixCoordinates.ToCoordinates(size, li, linearIndexMode);
                    denseMatrix[row, column] = v;
                }
            }
            return denseMatrix;
        }

        /// <summary>
        /// Set the CVS instance to the provided linear index mode.
        /// Alters the CVS instance only if the linear index mode passed is different.
        /// </summary>
        /// <param name="linearIndexMode">The new linear index mode.</param>
        public void SetLinearIndexMode(MatrixLinearIndexMode linearIndexMode)
        {
            if (linearIndexMode != LinearIndexMode)
            {
                Parallel.ForEach(LinearIndex, (lis) =>
                {
                    for (int i = 0; i < lis.Count; i++)
                    {
                        lis[i] = MatrixCoordinates.ToLinearIndex(Size, MatrixCoordinates.ToCoordinates(Size, lis[i], LinearIndexMode), linearIndexMode);
                    }
                    lis.Sort();
                });
                LinearIndexMode = linearIndexMode;
            }
        }

        public void Deconstruct(out List<T> value, out List<List<int>> linearIndex, out MatrixLinearIndexMode linearIndexMode, out (int rows, int columns) size)
        {
            value = Value;
            linearIndex = LinearIndex;
            linearIndexMode = LinearIndexMode;
            size = Size;
        }
    }
}
