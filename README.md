# Mendz.Matrix
Provides a library of APIs for working with dense, (DOK) sparse and compressed (CRS, CCS, CVS) matrices. [Wiki](https://github.com/etmendz/Mendz.Matrix/wiki)
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
## NuGet it...
[https://www.nuget.org/packages/Mendz.Matrix/](https://www.nuget.org/packages/Mendz.Matrix/)
