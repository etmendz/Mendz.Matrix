namespace Mendz.Matrix
{
    /// <summary>
    /// Represents a DOK sparse matrix with coordinates as keys.
    /// </summary>
    /// <typeparam name="T">The type of entries.</typeparam>
    public class CoordinatesKeyedSparseMatrix<T> : DOKSparseMatrixBase<(int row, int column), T>
    {
        /// <summary>
        /// Gets or sets the matrix entry value.
        /// </summary>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <returns>The entry value.</returns>
        public override T this[int row, int column]
        {
            get => this[(row, column)];
            set => this[(row, column)] = value;
        }

        /// <summary>
        /// Creates a sparse matrix.
        /// </summary>
        /// <param name="size">The size of the matrix</param>
        /// <param name="defaultValue">The default entry value.</param>
        /// <param name="diagonal">The default diagonal entry value.</param>
        public CoordinatesKeyedSparseMatrix((int rows, int columns) size, 
            T defaultValue = default, T diagonal = default)
            : base(size, (s, key) =>
            {
                MatrixCoordinates.CheckCoordinates(s, key);
                return key;
            }, MatrixLinearIndexMode.RowMajorOrder, defaultValue, diagonal)
        {

        }
    }
}
