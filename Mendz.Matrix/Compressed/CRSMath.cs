using System.Collections.Generic;

namespace Mendz.Matrix.Compressed
{
    public sealed partial class CRS<T>
    {
        /// <summary>
        /// Returns the matrix-vector multiplication product.
        /// </summary>
        /// <typeparam name="V">The type of vector values.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="vector">The vector.</param>
        /// <returns>The matrix-vector multiplication product.</returns>
        public IList<P> MatrixVectorProduct<V, P>(IList<V> vector)
        {
            (List<T> value, List<int> rowPointer, List<int> columnIndex, (int rows, int columns) size) = this;
            int rows = size.rows;
            P[] product = new P[rows];
            for (int j = 0; j < rows; j++)
            {
                for (int i = rowPointer[j]; i < rowPointer[j + 1]; i++)
                {
                    product[j] += value[i] * (dynamic)vector[columnIndex[i]];
                }
            }
            return product;
        }
    }
}
