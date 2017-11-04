using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mendz.Matrix.Compressed
{
    public sealed partial class CVS<T>
    {
        /// <summary>
        /// Transpose the CVS instance in place.
        /// </summary>
        public void Transpose()
        {
            (int rows, int columns) size = (Size.columns, Size.rows);
            Parallel.ForEach(LinearIndex, (lis) =>
            {
                for (int i = 0; i < lis.Count; i++)
                {
                    lis[i] = MatrixCoordinates.TransposeLinearIndex(Size, lis[i], LinearIndexMode).linearIndex;
                }
                lis.Sort();
            });
            Size = size;
        }

        /// <summary>
        /// Transpose the matrix to a new CVS instance.
        /// </summary>
        /// <param name="cvs">The matrix as CVS.</param>
        /// <returns>The transposed matrix as CVS.</returns>
        public CVS<T> TransposeToNewCVS()
        {
            (List<T> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size) = this;
            List<T> newValue = new List<T>();
            List<List<int>> newLinearIndex = new List<List<int>>();
            List<int> lis;
            for (int i = 0; i < value.Count; i++)
            {
                newValue.Add(value[i]);
                lis = new List<int>();
                foreach (var li in linearIndex[i])
                {
                    lis.Add(MatrixCoordinates.TransposeLinearIndex(size, li, linearIndexMode).linearIndex);
                }
                lis.Sort();
                newLinearIndex.Add(lis);
            }
            return new CVS<T>(newValue, newLinearIndex, linearIndexMode, (size.columns, size.rows));
        }

        /// <summary>
        /// Performs matrix addition.
        /// </summary>
        /// <typeparam name="A">The type of entry values to add.</typeparam>
        /// <typeparam name="S">The type of sum.</typeparam>
        /// <param name="cvs1">The CVS instance.</param>
        /// <param name="cvs2">The CVS instance to add.</param>
        /// <returns>The new CVS instance.</returns>
        public CVS<S> MatrixSum<A, S>(CVS<A> cvs2) => MatrixAddOrSubtract<A, S>(this, cvs2, MDAS.Add);

        /// <summary>
        /// Performs matrix subtraction.
        /// </summary>
        /// <typeparam name="A">The type of entry values to subtract.</typeparam>
        /// <typeparam name="S">The type of difference.</typeparam>
        /// <param name="cvs1">The CVS instance.</param>
        /// <param name="cvs2">The CVS instance to subtact.</param>
        /// <returns>The new CVS instance.</returns>
        public CVS<D> MatrixDifference<A, D>(CVS<A> cvs2) => MatrixAddOrSubtract<A, D>(this, cvs2, MDAS.Subtract);

        /// <summary>
        /// Performs matrix addition or subtraction.
        /// </summary>
        /// <typeparam name="A">The type of entry values to add or subtract.</typeparam>
        /// <typeparam name="R">The type of result.</typeparam>
        /// <param name="cvs1">The CVS instance.</param>
        /// <param name="cvs2">The CVS instance to add or subtract.</param>
        /// <param name="mdas">The operation to perform.</param>
        /// <returns>The new CVS instance.</returns>
        private static CVS<R> MatrixAddOrSubtract<A, R>(CVS<T> cvs1, CVS<A> cvs2, MDAS mdas)
        {
            if (!cvs1.Size.Equals(cvs2.Size))
            {
                throw new InvalidOperationException("Matrices must have the same size.");
            }
            var r = new LinearIndexKeyedSparseMatrix<R>(cvs1.Size, cvs1.LinearIndexMode);
            dynamic value;
            Parallel.For(0, cvs1.Value.Count, (i) =>
            {
                value = cvs1.Value[i];
                Parallel.ForEach(cvs1.LinearIndex[i], (li) =>
                {
                    r.SetEntry(li, value);
                });
            });
            Parallel.For(0, cvs2.Value.Count, (i) =>
            {
                value = cvs2.Value[i];
                Parallel.ForEach(cvs2.LinearIndex[i], (li) =>
                {
                    if (r.ContainsKey(li))
                    {
                        if (mdas == MDAS.Subtract)
                        {
                            value = r.GetEntry(li) - value;
                        }
                        else
                        {
                            value = r.GetEntry(li) + value;
                        }
                    }
                    if (value == default(R))
                    {
                        r.TryRemove(li, out R v);
                    }
                    else
                    {
                        r.SetEntry(li, value);
                    }
                });
            });
            return r.ToCVS(r.LinearIndexMode);
        }

        /// <summary>
        /// Returns the matrix-vector scalar product.
        /// </summary>
        /// <typeparam name="S">The type of scalar value.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>The matrix-vector scalar product.</returns>
        public CVS<P> MatrixScalarProduct<S, P>(S scalar)
        {
            (List<T> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size) = this;
            List<P> newValue = new List<P>();
            List<List<int>> newLinearIndex = new List<List<int>>();
            if (!scalar.Equals(default))
            {
                for (int i = 0; i < value.Count; i++)
                {
                    newValue.Add(value[i] * (dynamic)scalar);
                    newLinearIndex.Add(new List<int>(linearIndex[i]));
                }
            }
            return new CVS<P>(newValue, newLinearIndex, linearIndexMode, size);
        }

        /// <summary>
        /// Performs in place CVS matrix-scalar multiplication.
        /// </summary>
        /// <param name="scalar">The scalar value.</param>
        public void MatrixScalarProductInPlace(T scalar)
        {
            if (scalar.Equals(default))
            {
                Value.Clear();
                LinearIndex.Clear();
            }
            else
            {
                for (int i = 0; i < Value.Count; i++)
                {
                    Value[i] *= (dynamic)scalar;
                }
            }
        }

        /// <summary>
        /// Returns the matrix-vector multiplication product.
        /// </summary>
        /// <typeparam name="V">The type of vector values.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="vector">The vector.</param>
        /// <returns>The matrix-vector multiplication product.</returns>
        public IList<P> MatrixVectorProduct<V, P>(IList<V> vector)
        {
            (List<T> value, List<List<int>> linearIndex, MatrixLinearIndexMode linearIndexMode, (int rows, int columns) size) = this;
            P[] product = new P[size.rows];
            dynamic entry;
            for (int i = 0; i < value.Count; i++)
            {
                entry = value[i];
                foreach (var li in linearIndex[i])
                {
                    (int row, int column) = MatrixCoordinates.ToCoordinates(size, li, linearIndexMode);
                    product[row] += entry * vector[column];
                }
            }
            return product;
        }

        /// <summary>
        /// Returns the matrix product.
        /// </summary>
        /// <typeparam name="M">The type of entry values to multiply.</typeparam>
        /// <typeparam name="P">The type of product values.</typeparam>
        /// <param name="cvs2">The CVS instance to multiply by.</param>
        /// <returns>The matrix product.</returns>
        public CVS<P> MatrixProduct<M, P>(CVS<M> cvs2)
        {
            CVS<T> cvs1 = this;
            if (cvs1.Size.rows != cvs2.Size.columns)
            {
                throw new InvalidOperationException("Input matrix number of columns must match matrix number of rows.");
            }
            var r = new LinearIndexKeyedSparseMatrix<P>((cvs1.Size.rows, cvs2.Size.columns), cvs1.LinearIndexMode);
            Parallel.For(0, cvs1.Value.Count, (i) =>
            {
                T v1 = cvs1.Value[i];
                Parallel.ForEach(cvs1.LinearIndex[i], (li1) =>
                {
                    (int row, int column) c1 = MatrixCoordinates.ToCoordinates(cvs1.Size, li1, cvs1.LinearIndexMode);
                    Parallel.For(0, cvs2.Value.Count, (j) =>
                    {
                        M v2 = cvs2.Value[j];
                        Parallel.ForEach(cvs2.LinearIndex[j], (li2) =>
                        {
                            (int row, int column) coordinates = (c1.row, MatrixCoordinates.ToCoordinates(cvs2.Size, li2, cvs2.LinearIndexMode).column);
                            int key = MatrixCoordinates.ToLinearIndex(r.Size, coordinates, r.LinearIndexMode);
                            P value = r[coordinates.row, coordinates.column] + (v1 * (dynamic)v2);
                            if (value.Equals(default))
                            {
                                r.TryRemove(key, out P v);
                            }
                            else
                            {
                                r.SetEntry(key, value);
                            }
                        });
                    });
                });
            });
            return r.ToCVS();
        }
    }
}
