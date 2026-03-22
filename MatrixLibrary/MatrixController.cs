using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace MatrixLibrary
{
    public static class MatrixController
    {
        public static void Show<T>(Matrix<T> matrix) where T : INumber<T>
        {
            Console.Write(matrix.ToString());
        }

        public static void FillFromFile<T>(Matrix<T> matrix, string filePath, CultureInfo? culture = null) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix, $"Parameter {nameof(matrix)} can't be null.");

            string[] lines = File.ReadAllLines(filePath).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            if (lines.Length == 0)
                throw new InvalidDataException($"File {Path.GetFileName(filePath)} is empty.");

            culture ??= CultureInfo.InvariantCulture;

            int rowsCount = lines.Length;


            for (int i = 0; i < rowsCount; i++)
            {
                string[] values = lines[i].Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                int columnsCount = values.Length;

                for (int j = 0; j < columnsCount; j++)
                {
                    matrix[i, j] = ParseToNumber<T>(values[j], culture);
                }
            }
        }

        private static T ParseToNumber<T>(string value, CultureInfo culture) where T : INumber<T>
        {
            return T.Parse(value, NumberStyles.Any, culture);
        }

        public static void FillFromArray<T>(Matrix<T> matrix, T[,] array) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix, $"Parameter {nameof(matrix)} can't be null.");

            if (matrix.RowsCount != array.GetLength(0) || matrix.ColumnsCount != array.GetLength(1))
                throw new InvalidOperationException();

            int rowsCount = matrix.RowsCount;
            int columnsCount = matrix.ColumnsCount;

            for(int i = 0; i<rowsCount; i++)
            {
                for (int j = 0;j < columnsCount; j++)
                {
                    matrix[i,j] = array[i,j];
                }
            }
        }

        public static void FillFromArray<T>(Matrix<T> matrix, T[] array) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix, $"Parameter {nameof(matrix)} can't be null.");

            if (array.Length != matrix.RowsCount*matrix.ColumnsCount)
                throw new ArgumentException($"Array length {array.Length} does not match matrix size" +
                    $" {matrix.RowsCount}x{matrix.ColumnsCount} = {matrix.RowsCount * matrix.ColumnsCount}.");

            int rowsCount = matrix.RowsCount;
            int columnsCount = matrix.ColumnsCount;

            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    matrix[i,j] = array[i*columnsCount + j];
                }
            }
        }

        public static Matrix<T> Sum<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix1, $"Parameter {nameof(matrix1)} can't be null.");
            ArgumentNullException.ThrowIfNull(matrix2, $"Parameter {nameof(matrix2)} can't be null.");

            if (matrix1.RowsCount != matrix2.RowsCount || matrix1.ColumnsCount != matrix2.ColumnsCount)
                throw new InvalidOperationException();

            int rowsCount = matrix1.RowsCount;
            int columnsCount = matrix1.ColumnsCount;
            Matrix<T> resultMatrix = new Matrix<T>(rowsCount,columnsCount);

            for(int i = 0; i< rowsCount; i++)
            {
                for(int j = 0; j < columnsCount; j++)
                {
                    resultMatrix[i,j] = matrix1[i,j]+matrix2[i,j];
                }
            }

            return resultMatrix;
        }

        public static Matrix<T> Difference<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T: INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix1, $"Parameter {nameof(matrix1)} can't be null.");
            ArgumentNullException.ThrowIfNull(matrix2, $"Parameter {nameof(matrix2)} can't be null.");

            if (matrix1.RowsCount != matrix2.RowsCount || matrix1.ColumnsCount != matrix2.ColumnsCount)
                throw new InvalidOperationException();

            int rowsCount = matrix1.RowsCount;
            int columnsCount = matrix1.ColumnsCount;
            Matrix<T> resultMatrix = new Matrix<T>(rowsCount, columnsCount);

            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    resultMatrix[i, j] = matrix1[i, j] - matrix2[i, j];
                }
            }

            return resultMatrix;
        }

        public static Matrix<T> Transposition<T>(Matrix<T> matrix) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix, $"Parameter {nameof(matrix)} can't be null.");

            if (matrix.IsSquare)
                return TranspositionSquareMatrix<T>(matrix);
            return TranspositionRectangleMatrix<T>(matrix);
        }

        private static Matrix<T> TranspositionSquareMatrix<T>(Matrix<T> matrix) where T : INumber<T>
        {
            int size = matrix.RowsCount;

            Matrix<T> resultMatrix = new Matrix<T>(size);


            for (int i = 0; i < size; i++)
            {
                resultMatrix[i,i] = matrix[i,i];
                for (int j = i+1; j < size; j++)
                {
                    resultMatrix[i, j] = matrix[j, i];
                    resultMatrix[j, i] = matrix[i, j];
                }
            }

            return resultMatrix;
        }

        private static Matrix<T> TranspositionRectangleMatrix<T>(Matrix<T> matrix) where T : INumber<T>
        {
            int rowsCount = matrix.ColumnsCount;
            int columnsCount = matrix.RowsCount;

            Matrix<T> resultMatrix = new Matrix<T>(rowsCount, columnsCount);

            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    resultMatrix[i, j] = matrix[j, i];
                }
            }

            return resultMatrix;
        }

        public static Matrix<T> Multiply<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T: INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix1, $"Parameter {nameof(matrix1)} can't be null.");
            ArgumentNullException.ThrowIfNull(matrix2, $"Parameter {nameof(matrix2)} can't be null.");

            if (matrix1.ColumnsCount != matrix2.RowsCount)
                throw new InvalidOperationException($"The number of columns in the first matrix ({matrix1.ColumnsCount})" +
                    $" must be equal to the number of rows in the second matrix ({matrix2.RowsCount}).");

            int rowsCount = matrix1.RowsCount;
            int columnsCount = matrix2.ColumnsCount;
            int commonDimension = matrix1.ColumnsCount;

            Matrix <T> resultMatrix = new Matrix<T>(rowsCount,columnsCount);

            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    for (int k = 0; k < commonDimension; k++)
                    {
                        resultMatrix[i,j] += matrix1[i,k] * matrix2[k,j];
                    }
                }
            }
            return resultMatrix;
        }

        public static Matrix<T> Exponentiation<T>(Matrix<T> matrix, int degree) where T: INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix, $"Parameter {nameof(matrix)} can't be null.");

            if (matrix.ColumnsCount != matrix.RowsCount)
                throw new InvalidOperationException($"The number of columns in the first matrix must be equal to the number of rows in the second matrix.");

            if (degree < 0)
                throw new InvalidOperationException("The degree must be a positive number.");
                return ExponentionPositiveDegree(matrix, degree);
            
        }

        private static Matrix<T> ExponentionPositiveDegree<T>(Matrix<T> matrix, int degree) where T : INumber<T>
        {
            if (degree == 1)
                return matrix.Copy();

            int size = matrix.RowsCount;

            if (degree == 0)
                return GetIdentityMatrix<T>(size);

            Matrix<T> resultMatrix = matrix.Copy();

            for (int i = 1; i < degree; i++)
            {
                resultMatrix = Multiply(resultMatrix, matrix);
            }

            return resultMatrix;
        }

        private static Matrix<T> GetIdentityMatrix<T>(int size) where T : INumber<T>
        {
            Matrix<T> identityMatrix = new Matrix<T>(size);

            for (int i = 0; i < size; i++)
                identityMatrix[i, i] = T.One;

            return identityMatrix;
        }

        public static T GetDeterminant<T>(Matrix<T> matrix) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(matrix, $"Parameter {nameof(matrix)} can't be null.");

            if (!matrix.IsSquare)
                throw new InvalidOperationException("The matrix must be square.");

            return GetDeterminantRecursive(matrix, matrix.RowsCount);
        }

        private static T GetDeterminantRecursive<T>(Matrix<T> matrix, int size) where T : INumber<T>
        {
            if (size == 1)
                return matrix[0, 0];

            if (size == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            T determinant = T.Zero;

            for (int i = 0; i < size; i++)
            {
                var tempMatrix = GetMinor(matrix, 0, i, size);

                T sign = (i % 2 == 0) ? T.One : -T.One;

                determinant += sign * matrix[0, i] * GetDeterminantRecursive(tempMatrix, size - 1);
            }
            return determinant;
        }

        private static Matrix<T> GetMinor<T>(Matrix<T> matrix, int row, int column, int size) where T : INumber<T>
        {
            var result = new Matrix<T>(size - 1, size - 1);

            int currentRow = 0;
            int currentColumn = 0;

            for (int i = 0; i < size; i++)
            {
                if (i == row)
                    continue;
                currentColumn = 0;
                for (int j = 0; j < size; j++)
                {
                    if (j == column)
                        continue;
                    result[currentRow, currentColumn] = matrix[i, j];
                    currentColumn++;
                }
                currentRow++;
            }
            return result;
        }
    }
}
