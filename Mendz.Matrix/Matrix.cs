using System.Collections.Generic;

namespace Mendz.Matrix
{
    /// <summary>
    /// Provides methods to create and initialize a matrix,
    /// which is basically T[,] -- a two-dimensional array of type T.
    /// </summary>
    /// <typeparam name="T">The type of the elements/entries.</typeparam>
    public static class Matrix<T>
    {
        /// <summary>
        /// Creates and initializes a matrix.
        /// </summary>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the main diagonal.</param>
        /// <returns>Returns the matrix.</returns>
        public static T[,] Create(int rows, int columns, 
            T entry = default, T diagonal = default)
        {
            T[,] matrix = new T[rows, columns];
            Initialize(matrix, rows, columns, entry, diagonal, false);
            return matrix;
        }

        /// <summary>
        /// Creates and initializes a matrix.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the main diagonal.</param>
        /// <returns>Returns the matrix.</returns>
        public static T[,] Create((int rows, int columns) size, 
            T entry = default, T diagonal = default) => Create(size.rows, size.columns, entry, diagonal);

        /// <summary>
        /// Initializes a matrix.
        /// </summary>
        /// <param name="matrix">The matrix to initialize.</param>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the main diagonal.</param>
        /// <param name="force">To force initialization even if entry or diagonal is default(T). Default is true.</param>
        /// <remarks>
        /// The entries are initialized only when the entry is not equal to default(T).
        /// The diagonal values are initialized only when the diagonal is not equal to default(T).
        /// </remarks>
        private static void Initialize(T[,] matrix, int rows, int columns, 
            T entry = default, T diagonal = default, bool force = true)
        {
            bool isSetEntry = !EqualityComparer<T>.Default.Equals(entry, default);
            bool isSetDiagonal = !EqualityComparer<T>.Default.Equals(diagonal, default);
            if ((!isSetEntry && isSetDiagonal) || force)
            {
                for (int i = 0; i < rows; i++)
                {
                    matrix[i, i] = diagonal;
                }
            }
            else if (isSetEntry || isSetDiagonal || force)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (i == j)
                        {
                            matrix[i, j] = diagonal;
                        }
                        else
                        {
                            matrix[i, j] = entry;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a matrix.
        /// </summary>
        /// <param name="matrix">The matrix to initialize.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the main diagonal.</param>
        /// <param name="force">To force initialization even if entry or diagonal is default(T). Default is true.</param>
        public static void Initialize(T[,] matrix, 
            T entry = default, T diagonal = default, bool force = true) => Initialize(matrix, matrix.GetLength(0), matrix.GetLength(1), entry, diagonal, force);
    }
}
