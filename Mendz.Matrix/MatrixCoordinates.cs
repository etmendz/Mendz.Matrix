using System;

namespace Mendz.Matrix
{
    /// <summary>
    /// Provides methods to perform operations on matrix coordinates.
    /// </summary>
    public static class MatrixCoordinates
    {
        /// <summary>
        /// Returns the equivalent linear index of the given coordinates.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The linear index.</returns>
        public static int ToLinearIndex((int rows, int columns) size, int row, int column, MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder)
        {
            if (linearIndexMode == MatrixLinearIndexMode.RowMajorOrder)
            {
                return column + (row * size.columns);
            }
            else // MatrixLinearIndexMode.ColumnMajorOrder
            {
                return row + (column * size.rows);
            }
        }

        /// <summary>
        /// Returns the equivalent linear index of the given coordinates.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="coordinate">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The linear index.</returns>
        public static int ToLinearIndex((int rows, int columns) size, 
            (int row, int column) coordinates, 
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder) => ToLinearIndex(size, coordinates.row, coordinates.column, linearIndexMode);

        /// <summary>
        /// Returns the linear index equivalent of the given coordinates.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The linear index.</returns>
        public static int ToLinearIndex(int order, int row, int column, 
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder) => ToLinearIndex((order, order), row, column, linearIndexMode);

        /// <summary>
        /// Returns the linear index equivalent of the given coordinates.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The linear index.</returns>
        public static int ToLinearIndex(int order, (int row, int column) coordinates, 
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder) => ToLinearIndex((order, order), coordinates.row, coordinates.column, linearIndexMode);

        /// <summary>
        /// Returns the equivalent coordinates of the given linear index.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="linearIndex">The linear index.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The coordinates.</returns>
        public static (int row, int column) ToCoordinates((int rows, int columns) size, int linearIndex, 
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder)
        {
            if (linearIndexMode == MatrixLinearIndexMode.RowMajorOrder)
            {
                return ((int)(linearIndex / size.columns), linearIndex % size.columns);
            }
            else // MatrixLinearIndexMode.ColumnMajorOrder
            {
                return (linearIndex % size.rows, (int)(linearIndex / size.rows));
            }
        }

        /// <summary>
        /// Returns the equivalent coordinates of the given linear index.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="linearIndex">The linear index.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The coordinates.</returns>
        public static (int row, int column) ToCoordinates(int order, int linearIndex, 
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder) => ToCoordinates((order, order), linearIndex);

        /// <summary>
        /// Transpose coordinates.
        /// </summary>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <returns>The transposed coordinates.</returns>
        public static (int row, int column) TransposeCoordinates(int row, int column) => (column, row);
        /// <summary>
        /// Transpose coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>The transposed coordinates.</returns>
        public static (int row, int column) TransposeCoordinates((int row, int column) coordinates) => TransposeCoordinates(coordinates.column, coordinates.row);

        /// <summary>
        /// Transpose a matrix linear index.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="linearIndex">The linear index.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>A tuple of the new size and the transposed linear index.</returns>
        public static ((int rows, int columns) size, int linearIndex) TransposeLinearIndex((int rows, int columns) size, int linearIndex, 
            MatrixLinearIndexMode linearIndexMode)
        {
            (int rows, int columns) s = (size.columns, size.rows);
            int li = ToLinearIndex(s, TransposeCoordinates(ToCoordinates(size, linearIndex, linearIndexMode)), linearIndexMode);
            return (s, li);
        }

        /// <summary>
        /// Transpose a square matrix linear index.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="linearIndex">The linear index.</param>
        /// <param name="linearIndexMode">The linear index mode.</param>
        /// <returns>The transposed linear index.</returns>
        public static int TransposeLinearIndex(int order, int linearIndex, 
            MatrixLinearIndexMode linearIndexMode) => TransposeLinearIndex((order, order), linearIndex, linearIndexMode).linearIndex;

        /// <summary>
        /// Checks coordinates against a matrix dimension/size.
        /// </summary>
        /// <param name="size">The matrix dimension/size.</param>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <param name="suppressException">Flag to suppress excetion or not.</param>
        /// <returns>
        /// If suppressException is true, returns true if the coordinates pass the check. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If suppressException is false, thrown when the row/column coordinate is less than 0; or
        /// greater than or equal to the row/column length.
        /// </exception>
        public static bool CheckCoordinates((int rows, int columns) size, int row, int column, bool suppressException = false)
        {
            bool isOK = true;
            if (isOK && (row < 0 || row >= size.rows))
            {
                if (suppressException)
                {
                    isOK = false;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("row");
                }
            }
            if (isOK && (column < 0 || column >= size.columns))
            {
                if (suppressException)
                {
                    isOK = false;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("column");
                }
            }
            return isOK;
        }

        /// <summary>
        /// Checks coordinates against a matrix dimension/size.
        /// </summary>
        /// <param name="size">The matrix dimension/size.</param>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="suppressException">Flag to suppress excetion or not.</param>
        /// <returns>
        /// If suppressException is true, returns true if the coordinates pass the check. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If suppressException is false, thrown when the row/column coordinate is less than 0; or
        /// greater than or equal to the row/column length.
        /// </exception>
        public static bool CheckCoordinates((int rows, int columns) size, 
            (int row, int column) coordinates, 
            bool suppressException = false) => CheckCoordinates(size, coordinates.row, coordinates.column, suppressException);

        /// <summary>
        /// Checks coordinates against a square matrix.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <param name="suppressException">Flag to suppress excetion or not.</param>
        /// <returns>
        /// If suppressException is true, returns true if the coordinates pass the check. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If suppressException is false, thrown when the row/column coordinate is less than 0; or
        /// greater than or equal to the row/column length.
        /// </exception>
        public static void CheckCoordinates(int order, int row, int column, 
            bool suppressException = false) => CheckCoordinates((order, order), row, column, suppressException);

        /// <summary>
        /// Checks coordinates against a square matrix.
        /// </summary>
        /// <param name="order">The order of the square matrix.</param>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="suppressException">Flag to suppress excetion or not.</param>
        /// <returns>
        /// If suppressException is true, returns true if the coordinates pass the check. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If suppressException is false, thrown when the row/column coordinate is less than 0; or
        /// greater than or equal to the row/column length.
        /// </exception>
        public static void CheckCoordinates(int order, (int row, int column) coordinates, 
            bool suppressException = false) => CheckCoordinates((order, order), coordinates, suppressException);
    }
}
