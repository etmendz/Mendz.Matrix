using System;
using System.Collections.Concurrent;

namespace Mendz.Matrix
{
    /// <summary>
    /// The base class of DOK sparse matrix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class DOKSparseMatrixBase<K, T> : ConcurrentDictionary<K, T>
    {
        private object o = new object();

        /// <summary>
        /// Gets the coordinates checker.
        /// </summary>
        protected Func<(int rows, int columns), K, (int row, int column)> CoordinatesChecker { get; set; }

        /// <summary>
        /// Gets the size of the matrix.
        /// </summary>
        public (int rows, int columns) Size { get; protected set; }

        /// <summary>
        /// Gets the linear index mode.
        /// </summary>
        public MatrixLinearIndexMode LinearIndexMode { get; protected set; } = MatrixLinearIndexMode.RowMajorOrder;

        /// <summary>
        /// Gets the default entry value.
        /// </summary>
        public T Default { get; protected set; }

        /// <summary>
        /// Gets the default diagonal entry value.
        /// </summary>
        public T Diagonal { get; protected set; }

        /// <summary>
        /// Gets an indicator if the sparse matrix is linear indexed.
        /// </summary>
        public bool IsLinearIndexed
        {
            get
            {
                return (typeof(K) == typeof(int));
            }
        }

        /// <summary>
        /// Gets or sets the matrix entry value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The entry value.</returns>
        public new T this[K key]
        {
            get
            {
                lock (o)
                {
                    (int row, int column) coordinates = CoordinatesChecker(Size, key);
                    if (ContainsKey(key))
                    {
                        return base[key];
                    }
                    else
                    {
                        return (coordinates.row == coordinates.column) ? Diagonal : Default;
                    }
                }
            }
            set => SetEntry(key, value);
        }

        /// <summary>
        /// Gets or sets the matrix entry value.
        /// </summary>
        /// <param name="row">The row coordinate.</param>
        /// <param name="column">The column coordinate.</param>
        /// <returns>The entry value.</returns>
        public abstract T this[int row, int column] { get; set; }

        /// <summary>
        /// Creates a sparse matrix.
        /// </summary>
        /// <param name="size">The size of the matrix</param>
        /// <param name="coordinatesChecker">The coordinates checker.</param>
        /// <param name="defaultValue">The default entry value.</param>
        /// <param name="diagonal">The default diagonal entry value.</param>
        protected DOKSparseMatrixBase((int rows, int columns) size,
            Func<(int rows, int columns), K, (int row, int column)> coordinatesChecker,
            MatrixLinearIndexMode linearIndexMode = MatrixLinearIndexMode.RowMajorOrder,
            T defaultValue = default, T diagonal = default)
        {
            Size = size;
            LinearIndexMode = linearIndexMode;
            CoordinatesChecker = coordinatesChecker;
            Default = defaultValue;
            Diagonal = diagonal;
        }

        /// <summary>
        /// Gets an entry in the matrix.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The entry value.</returns>
        public T GetEntry(K key)
        {
            return this[key];
        }

        /// <summary>
        /// Sets an entry in the matrix.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The entrys value.</param>
        public void SetEntry(K key, T value)
        {
            AddOrUpdate(key, value, (k, v) => value);
        }

        /// <summary>
        /// Adds a key/value pair if the key does not already exist, 
        /// or updates a key/value pair if the key already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="addValue">The entry value to add.</param>
        /// <param name="updateValueFactory">The update entry value.</param>
        /// <returns>The new value for the key.</returns>
        public new T AddOrUpdate(K key, T addValue, Func<K, T, T> updateValueFactory)
        {
            lock (o)
            {
                CoordinatesChecker(Size, key);
                return base.AddOrUpdate(key, addValue, updateValueFactory);
            }
        }

        /// <summary>
        /// Adds a key/value pair if the key does not already exist, 
        /// or updates a key/value pair if the key already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="addValueFactory">The entry value to add.</param>
        /// <param name="updateValueFactory">The update entry value.</param>
        /// <returns>The new value for the key.</returns>
        public new T AddOrUpdate(K key, Func<K, T> addValueFactory, Func<K, T, T> updateValueFactory)
        {
            lock (o)
            {
                CoordinatesChecker(Size, key);
                return AddOrUpdate(key, addValueFactory, updateValueFactory);
            }
        }

        /// <summary>
        /// Adds a matrix entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The entry value.</param>
        /// <returns>True if successful. Otherwise, false.</returns>
        public new bool TryAdd(K key, T value)
        {
            lock (o)
            {
                CoordinatesChecker(Size, key);
                return base.TryAdd(key, value);
            }
        }
    }
}
