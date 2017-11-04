namespace Mendz.Matrix
{
    /// <summary>
    /// Represents a DOK sparse matrix with linear index keys.
    /// </summary>
    /// <typeparam name="T">The type of entries.</typeparam>
    public class LinearIndexKeyedSparseMatrix<T> : DOKSparseMatrixBase<int, T>
    {
        /// <summary>
        /// Gets or sets the matrix entry value.
        /// </summary>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <returns>The entry value.</returns>
        public override T this[int row, int column]
        {
            get => this[MatrixCoordinates.ToLinearIndex(Size, row, column, LinearIndexMode)];
            set => this[MatrixCoordinates.ToLinearIndex(Size, row, column, LinearIndexMode)] = value;
        }

        /// <summary>
        /// Creates a sparse matrix.
        /// </summary>
        /// <param name="size">The size of the matrix</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <param name="defaultValue">The default entry value.</param>
        /// <param name="diagonal">The default diagonal entry value.</param>
        public LinearIndexKeyedSparseMatrix((int rows, int columns) size,
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder,
            T defaultValue = default, T diagonal = default)
            : base(size, (s, key) =>
            {
                (int row, int column) coordinates = MatrixCoordinates.ToCoordinates(s, key);
                MatrixCoordinates.CheckCoordinates(s, coordinates);
                return coordinates;
            }, linearIndexMode, defaultValue, diagonal)
        {
            LinearIndexMode = linearIndexMode;
        }
    }
}
