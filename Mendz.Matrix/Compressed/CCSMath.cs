using System.Collections.Generic;

namespace Mendz.Matrix.Compressed
{
    public sealed partial class CCS<T>
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
            (List<T> value, List<int> columnPointer, List<int> rowIndex, (int rows, int columns) size) = this;
            P[] product = new P[size.rows];
            dynamic v;
            for (int j = 0; j < size.columns; j++)
            {
                v = vector[j];
                for (int i = columnPointer[j]; i < columnPointer[j + 1]; i++)
                {
                    product[rowIndex[i]] += value[i] * v;
                }
            }
            return product;
        }
    }
}
