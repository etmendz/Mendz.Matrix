using System;

namespace Mendz.Matrix
{
    /// <summary>
    /// Provides extension methods to DOKSparseMatrixBase.
    /// </summary>
    public static class DOKSparseMatrixExtensions
    {
        /// <summary>
        /// Gets the key as coordinates.
        /// </summary>
        /// <param name="sparseMatrix">The sparse matrix.</param>
        /// <param name="key">The key.</param>
        /// <returns>The coordinates.</returns>
        public static (int row, int column) GetKeyAsCoordinates<K, T>(this DOKSparseMatrixBase<K, T> sparseMatrix, K key)
        {
            if (sparseMatrix.IsLinearIndexed)
            {
                return MatrixCoordinates.ToCoordinates(sparseMatrix.Size, sparseMatrix.GetKeyAsLinearIndex(key), sparseMatrix.LinearIndexMode);
            }
            else
            {
                return ((int row, int column))Convert.ChangeType(key, typeof((int row, int column)));
            }
        }

        /// <summary>
        /// Gets the key as a linear index.
        /// </summary>
        /// <param name="sparseMatrix">The sparse matrix.</param>
        /// <param name="key">The key.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The linear index.</returns>
        public static int GetKeyAsLinearIndex<K, T>(this DOKSparseMatrixBase<K, T> sparseMatrix, K key, MatrixLinearIndexMode linearIndexMode)
        {
            if (sparseMatrix.IsLinearIndexed)
            {
                return Convert.ToInt32(key);
            }
            else
            {
                return MatrixCoordinates.ToLinearIndex(sparseMatrix.Size, sparseMatrix.GetKeyAsCoordinates(key), linearIndexMode);
            }
        }

        /// <summary>
        /// Gets the key as a linear index.
        /// </summary>
        /// <param name="sparseMatrix">The sparse matrix.</param>
        /// <param name="key">The key.</param>
        /// <returns>The linear index.</returns>
        public static int GetKeyAsLinearIndex<K, T>(this DOKSparseMatrixBase<K, T> sparseMatrix, K key) => sparseMatrix.GetKeyAsLinearIndex(key, sparseMatrix.LinearIndexMode);
    }
}
