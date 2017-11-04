using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mendz.Matrix
{
    public abstract partial class DOKSparseMatrixBase<K, T> : ConcurrentDictionary<K, T>
    {
        /// <summary>
        /// Transpose a matrix to a new sparse matrix.
        /// </summary>
        /// <param name="result">The new sparse matrix.</param>
        public void Transpose(DOKSparseMatrixBase<K, T> result)
        {
            if (!result.Size.Equals(Size))
            {
                throw new InvalidOperationException("Result matrix size must match matrix size.");
            }
            bool isLinearIndexed = result.IsLinearIndexed;
            dynamic key;
            Parallel.ForEach(this, (entry) =>
            {
                if (isLinearIndexed)
                {
                    key = MatrixCoordinates.TransposeLinearIndex(result.Size, this.GetKeyAsLinearIndex(entry.Key), result.LinearIndexMode);
                }
                else
                {
                    key = MatrixCoordinates.TransposeCoordinates(this.GetKeyAsCoordinates(entry.Key));
                }
                result.SetEntry(key, entry.Value);
            });
        }

        /// <summary>
        /// Performs matrix addition and saves the results to a new sparse matrix.
        /// </summary>
        /// <typeparam name="A">The type of input entry values.</typeparam>
        /// <typeparam name="S">The type of sum values.</typeparam>
        /// <param name="sparseMatrix2">Tha other sparse matrix.</param>
        /// <param name="result">The new sparse matrix.</param>
        public void MatrixSum<A, S>(DOKSparseMatrixBase<K, A> sparseMatrix2, 
            DOKSparseMatrixBase<K, S> result) => MatrixAddOrSubtract(this, sparseMatrix2, result, MDAS.Add);

        /// <summary>
        /// Performs matrix subtraction and saves the results to a new sparse matrix.
        /// </summary>
        /// <typeparam name="A">The type of input entry values.</typeparam>
        /// <typeparam name="D">The type of difference values.</typeparam>
        /// <param name="sparseMatrix2">Tha other sparse matrix.</param>
        /// <param name="result">The new sparse matrix.</param>
        public void MatrixDifference<A, D>(DOKSparseMatrixBase<K, A> sparseMatrix2, 
            DOKSparseMatrixBase<K, D> result) => MatrixAddOrSubtract(this, sparseMatrix2, result, MDAS.Subtract);

        /// <summary>
        /// Performs matrix addition or subtraction and saves the results to a new sparse matrix.
        /// </summary>
        /// <typeparam name="A">The type of input entry values.</typeparam>
        /// <typeparam name="R">The type of result values.</typeparam>
        /// <param name="sparseMatrix1">The sparse matrix.</param>
        /// <param name="sparseMatrix2">Tha input sparse matrix.</param>
        /// <param name="result">The new sparse matrix.</param>
        /// <param name="mdas">The type of operation, whether to Add or Subtract.</param>
        private static void MatrixAddOrSubtract<A, R>(DOKSparseMatrixBase<K, T> sparseMatrix1, 
            DOKSparseMatrixBase<K, A> sparseMatrix2, 
            DOKSparseMatrixBase<K, R> result, MDAS mdas)
        {
            if (!sparseMatrix1.Size.Equals(sparseMatrix2.Size))
            {
                throw new InvalidOperationException("Matrices must have the same size.");
            }
            if (!result.Size.Equals(sparseMatrix1.Size) || !result.Size.Equals(sparseMatrix2.Size))
            {
                throw new InvalidOperationException("Result matrix size must match matrices size.");
            }
            Parallel.ForEach(sparseMatrix1, (entry) =>
            {
                result.SetEntry(entry.Key, (dynamic)entry.Value);
            });
            Parallel.ForEach(sparseMatrix2, (entry) =>
            {
                var key = entry.Key;
                dynamic value;
                if (result.ContainsKey(key))
                {
                    value = result.GetEntry(key);
                    if (mdas == MDAS.Subtract)
                    {
                        value -= entry.Value;
                    }
                    else
                    {
                        value += entry.Value;
                    }
                    if (value == default(R))
                    {
                        result.TryRemove(key, out R v);
                    }
                    else
                    {
                        result.SetEntry(key, value);
                    }
                }
                else
                {
                    value = entry.Value;
                    result.SetEntry(key, value);
                }
            });
        }

        /// <summary>
        /// Performs a matrix-scalar multiplication to a new sparse matrix.
        /// </summary>
        /// <typeparam name="S">The type of scalar value.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="scalar">The scalar value.</param>
        /// <param name="result">The new sparse matrix.</param>
        public void MatrixScalarProduct<S, P>(S scalar, DOKSparseMatrixBase<K, P> result)
        {
            if (!result.Size.Equals(Size))
            {
                throw new InvalidOperationException("Result matrix size must match matrix size.");
            }
            if (scalar.Equals(default))
            {
                result.Clear();
            }
            else
            {
                Parallel.ForEach(this, (entry) =>
                {
                    result.SetEntry(entry.Key, entry.Value * (dynamic)scalar);
                });
            }
        }

        /// <summary>
        /// Performs a matrix-scalar multiplication in place.
        /// </summary>
        /// <param name="scalar">The scalar value.</param>
        public void MatrixScalarProductInPlace(T scalar) => MatrixScalarProduct(scalar, this);

        /// <summary>
        /// Returns the matrix-vector multiplication product.
        /// </summary>
        /// <typeparam name="V">The type of vector values.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="vector">The vector.</param>
        /// <returns>The matrix-vector multiplication product.</returns>
        public IList<P> MatrixVectorProduct<V, P>(IList<V> vector)
        {
            (int rows, int columns) size = Size;
            int rows = size.rows;
            P[] product = new P[rows];
            bool isLinearIndexed = IsLinearIndexed;
            (int row, int column) coordinates;
            foreach (var item in this)
            {
                if (isLinearIndexed)
                {
                    coordinates = MatrixCoordinates.ToCoordinates(size, this.GetKeyAsLinearIndex(item.Key), LinearIndexMode);
                }
                else
                {
                    coordinates = this.GetKeyAsCoordinates(item.Key);
                }
                product[coordinates.row] += item.Value * (dynamic)vector[coordinates.column];
            }
            return product;
        }

        /// <summary>
        /// Performs matrix multiplication and saves the results to a new sparse matrix.
        /// </summary>
        /// <typeparam name="M">The type of input entry values.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="sparseMatrix2">The input sparse matrix.</param>
        /// <param name="result">The new sparse matrix.</param>
        public void MatrixProduct<M, P>(DOKSparseMatrixBase<K, M> sparseMatrix2, DOKSparseMatrixBase<K, P> result)
        {
            DOKSparseMatrixBase<K, T> sparseMatrix1 = this;
            if (sparseMatrix1.Size.rows != sparseMatrix2.Size.columns)
            {
                throw new InvalidOperationException("Input matrix number of columns must match matrix number of rows.");
            }
            if (result.Size.rows != sparseMatrix1.Size.rows || result.Size.columns != sparseMatrix2.Size.columns)
            {
                throw new InvalidOperationException("Result matrix size m x n must match matrix rows m and input matrix columns n.");
            }
            bool isLinearIndexed = result.IsLinearIndexed;
            Action<(int row, int column), T, (int row, int column), M> multiply = (c1, v1, c2, v2) =>
            {
                (int row, int column) coordinates = (c1.row, c2.column);
                dynamic key;
                if (isLinearIndexed)
                {
                    key = MatrixCoordinates.ToLinearIndex(result.Size, coordinates, result.LinearIndexMode);
                }
                else
                {
                    key = coordinates;
                }
                P value = result[coordinates.row, coordinates.column] + (v1 * (dynamic)v2);
                if (value.Equals(default))
                {
                    result.TryRemove(key, out P v);
                }
                else
                {
                    result.SetEntry(key, value);
                }
            };
            Parallel.ForEach(sparseMatrix1, (entry1) =>
            {
                (int row, int column) c1 = sparseMatrix1.GetKeyAsCoordinates(entry1.Key);
                T v1 = entry1.Value;
                Parallel.ForEach(sparseMatrix2, (entry2) =>
                {
                    multiply(c1, v1, sparseMatrix2.GetKeyAsCoordinates(entry2.Key), entry2.Value);
                });
            });
        }
    }
}
