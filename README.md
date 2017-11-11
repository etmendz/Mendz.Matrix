# Mendz.Matrix
Provides a library of APIs for working with dense, (DOK) sparse and compressed (CRS, CCS, CVS) matrices. [Wiki](https://github.com/etmendz/Mendz.Matrix/wiki)
## Implementation
- [Mendz.Graph.Representation](https://www.nuget.org/packages/Mendz.Graph.Representation/)
## Namespaces
### Mendz.Matrix
#### Contents
Name | Description
---- | -----------
Matrix | Provides methods to work on a dense matrix.
SquareMatrix | Provides methods to work on a square matrix.
MatrixCoordinates | Provides methods to work on matrix coordinates.
DOKSparseMatrixBase | The base class of DOK sparse matrix.
CoordinatesKeyedSparseMatrix | Represents a coordinates keyed DOK sparse matrix.
LinearIndexKeyedSparseMatrix | Represents a linear index keyed DOK sparse matrix.
DOKSparseMatrixExtensions | Provides extensions to the DOK sparse matrix type.
MatrixLinearIndexMode | An enumeration of the matrix linear index mode, if row- or column- major order.
MDAS | An enumeration of basic mathematical operations: Multiply, Divide, Add or Subtract.
### Mendz.Matrix.Compressed
#### Contents
Name | Description
---- | -----------
CRS | An implementation of the Compressed Row Storage format, also known as Compressed Sparse Row (CSR) or Yale format.
CCS | An implementation of the Compressed Column Storage format, also known as Compressed Sparse Column (CSC).
CVS | An implementation of the Compressed Value Storage format, a lossless compression of matrices by their entry values.
DOKSparseMatrixCompressions | Provides extensions to the DOK sparse matrix type to compress sparse matrices.
#### CRS and CCS
CRS and CCS are two popular matrix compression formats. Both formats can achieve typical compression ratio of 4:1 and up to 76% space savings.

CRS is a tuple of 4 values:
```C#
(
    List<T> value, 
    List<int> rowPointer, 
    List<int> columnIndex, 
    (int rows, int columns) size
)
```
CCS is a tuple of 4 values:
```C#
(
    List<T> value, 
    List<int> columnPointer, 
    List<int> rowIndex, 
    (int rows, int columns) size
)
```
The implementation of CRS/CCS in Mendz.Matrix.Compressed includes methods to compress, decompress, deconstruct and perform matrix vector multiplication.
#### CVS
CVS applies basic lossless compression techniques to compress a matrix. Essentially, lossless compression exploits redundancy. For matrices with redundant data, like the adjacency matrix, CVS can achieve compression ratios of 13:1 and up to 92% space savings.

CVS is a tuple of 4 values:
```C#
(
    List<int> value, 
    List<List<int>> linearIndex, 
    MatrixLinearIndexMode linearIndexMode,
    (int rows, int columns) size
)
```
The implementation of CVS in Mendz.Matrix.Compressed includes methods to compress, decompress, deconstruct, switch linear index mode, and to perform transpose, matrix addition, matrix substraction, matrix scalar multiplication, matrix multiplication and matrix vector multiplication.
#### DOKSparseMatrixCompressions
By "using Mendz.Matrix.Compressed", extension methods are added to DOKSparseMatrixBase implementations such that the DOK sparse matrix can be compressed:
- ToCRS(),
- ToCCS(), or
- ToCVS()
## NuGet it...
[https://www.nuget.org/packages/Mendz.Matrix/](https://www.nuget.org/packages/Mendz.Matrix/)
