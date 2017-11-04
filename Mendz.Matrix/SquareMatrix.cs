using System.Collections.Generic;

namespace Mendz.Matrix
{
    /// <summary>
    /// Provides methods to create and initialize a square matrix,
    /// which is basically T[,] -- a two-dimensional array of type T.
    /// </summary>
    /// <typeparam name="T">The type of the elements/entries.</typeparam>
    public static class SquareMatrix<T>
    {
        /// <summary>
        /// Creates and initializes a square matrix.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the main diagonal.</param>
        /// <returns>Returns the matrix.</returns>
        public static T[,] Create(int order, 
            T entry = default, T diagonal = default) => Matrix<T>.Create(order, order, entry, diagonal);

        /// <summary>
        /// Creates and initializes an upper triangular matrix.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the diagonal.</param>
        /// <returns>Returns the square matrix.</returns>
        public static T[,] CreateUpperTriangular(int order, T entry, T diagonal = default)
        {
            T[,] matrix = new T[order, order];
            InitializeUpperTriangular(matrix, entry, diagonal, false);
            return matrix;
        }

        /// <summary>
        /// Creates and initializes a lower triangular matrix.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the diagonal.</param>
        /// <returns>Returns the square matrix.</returns>
        public static T[,] CreateLowerTriangular(int order, T entry, T diagonal = default)
        {
            T[,] matrix = new T[order, order];
            InitializeLowerTriangular(matrix, entry, diagonal, false);
            return matrix;
        }

        /// <summary>
        /// Initializes a square matrix.
        /// </summary>
        /// <param name="matrix">The square matrix to initialize.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the main diagonal.</param>
        /// <param name="force">To force initialization even if entry or diagonal is default(T). Default is true.</param>
        public static void Initialize(T[,] matrix, 
            T entry = default, T diagonal = default, bool force = true) => Matrix<T>.Initialize(matrix, entry, diagonal, force);

        /// <summary>
        /// Initializes an upper triangular matrix.
        /// </summary>
        /// <param name="matrix">The square matrix to initialize.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the diagonal.</param>
        public static void InitializeUpperTriangular(T[,] matrix, T entry, T diagonal = default, bool force = true)
        {
            bool isSetEntry = !EqualityComparer<T>.Default.Equals(entry, default);
            bool isSetDiagonal = !EqualityComparer<T>.Default.Equals(diagonal, default);
            if (isSetEntry || isSetDiagonal || force)
            {
                int order = matrix.GetLength(0);
                for (int i = 0; i < order; i++)
                {
                    for (int j = i; j < order; j++)
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
        /// Initializes a lower triangular matrix.
        /// </summary>
        /// <param name="matrix">The square matrix to initialize.</param>
        /// <param name="entry">The default entry values.</param>
        /// <param name="diagonal">The default values of the diagonal.</param>
        public static void InitializeLowerTriangular(T[,] matrix, T entry, T diagonal = default, bool force = true)
        {
            bool isSetEntry = !EqualityComparer<T>.Default.Equals(entry, default);
            bool isSetDiagonal = !EqualityComparer<T>.Default.Equals(diagonal, default);
            if (isSetEntry || isSetDiagonal || force)
            {
                int order = matrix.GetLength(0);
                for (int i = 0; i < order; i++)
                {
                    for (int j = 0; j < order; j++)
                    {
                        if (i == j)
                        {
                            matrix[i, j] = diagonal;
                            break;
                        }
                        else
                        {
                            matrix[i, j] = entry;
                        }
                    }
                }
            }
        }
    }
}
