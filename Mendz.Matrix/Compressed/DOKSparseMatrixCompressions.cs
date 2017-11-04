namespace Mendz.Matrix.Compressed
{
    /// <summary>
    /// Provides extension methods for compression of DOKSparseMatrixBase.
    /// </summary>
    public static class DOKSparseMatrixCompressions
    {
        /// <summary>
        /// Compressed Value Storage (CVS) performs a basic lossless compression such that the
        /// distinct entry values are aligned with their respective collection of linear indexes.
        /// </summary>
        /// <typeparam name="K">The type of keys.</typeparam>
        /// <typeparam name="T">The type of entry values.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix to compress.</param>
        /// <param name="linearIndexMode">The MatrixLinearIndexMode to apply to the coordinates.</param>
        /// <returns>The compressed value storage (CVS) representation of the sparse matrix.</returns>
        public static CVS<T> ToCVS<K, T>(this DOKSparseMatrixBase<K, T> sparseMatrix, MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder)
        {
            CVS<T> cvs = new CVS<T>(linearIndexMode, sparseMatrix.Size);
            cvs.Compress(sparseMatrix);
            return cvs;
        }

        /// <summary>
        /// Returns the compressed row storage (CRS) representation of the sparse matrix.
        /// </summary>
        /// <typeparam name="K">The type of keys.</typeparam>
        /// <typeparam name="T">The type of entry values.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix to compress.</param>
        /// <returns>The compressed row storage (CRS) representation of the sparse matrix.</returns>
        public static CRS<T> ToCRS<K, T>(this DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            CRS<T> crs = new CRS<T>(sparseMatrix.Size);
            crs.Compress(sparseMatrix);
            return crs;
        }

        /// <summary>
        /// Returns the compressed column storage (CCS) representation of the sparse matrix.
        /// </summary>
        /// <typeparam name="K">The type of keys.</typeparam>
        /// <typeparam name="T">The type of entry values.</typeparam>
        /// <param name="sparseMatrix">The sparse matrix to compress.</param>
        /// <returns>The compressed column storage (CCS) representation of the sparse matrix.</returns>
        public static CCS<T> ToCCS<K, T>(this DOKSparseMatrixBase<K, T> sparseMatrix)
        {
            CCS<T> ccs = new CCS<T>(sparseMatrix.Size);
            ccs.Compress(sparseMatrix);
            return ccs;
        }
    }
}
