using System.Numerics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MatrixLibrary
{
    public class Matrix<T> : IEquatable<Matrix<T>> where T: INumber<T>
    {
        private int _rowsCount;
        private int _columnsCount;

        public int RowsCount
        {
            get => _rowsCount;

            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException($"the number of rows {value} must be a positive number.");
                _rowsCount = value;
            }
        }

        public int ColumnsCount
        {
            get => _columnsCount;

            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException($"the number of columns {value} must be a positive number.");
                _columnsCount = value;
            }
        }

        public bool IsSquare => RowsCount == ColumnsCount;

        private T[,] Values { get; set; }

        public Matrix(int rowsCount, int columnsCount)
        {
            RowsCount = rowsCount;
            ColumnsCount = columnsCount;
            Values = new T[RowsCount, ColumnsCount];
        }

        public Matrix(int size)
        {
            RowsCount = size;
            ColumnsCount= size;
            Values = new T[RowsCount, ColumnsCount];
        }

        public Matrix(T[,] array) : this(array.GetLength(0),array.GetLength(1))
        {
            ArgumentNullException.ThrowIfNull($"Parameter {nameof(array)} can't be null.");

            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    Values[i,j] = array[i,j];
                }
            }
        }

        public Matrix(T[] array, int rows, int columns)
        {
            ArgumentNullException.ThrowIfNull($"Parameter {nameof(array)} can't be null.");

            if (array.Length != rows * columns)
                throw new ArgumentException($"Array length {array.Length} does not match matrix size {rows}x{columns} = {rows * columns}.");

            RowsCount = rows;
            ColumnsCount = columns;
            Values = new T[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                   Values[i, j] = array[i * columns + j];
                }
            }
        }

        public T this[int row, int column]
        {
            get
            {
                if (row < 0 || column < 0 ||  row >= RowsCount ||  column >= ColumnsCount)
                    throw new ArgumentOutOfRangeException(nameof(row));
                return Values[row, column];
            }

            set
            {
                if (row < 0 || column < 0 || row >= RowsCount || column >= ColumnsCount)
                    throw new ArgumentOutOfRangeException(nameof(row));
                Values[row, column] = value;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Matrix<T> other && Equals(other);
        }

        public bool Equals(Matrix<T>? other)
        {
            if (other is null) return false;
            if (RowsCount != other.RowsCount || ColumnsCount != other.ColumnsCount)
                return false;

            for (int i = 0; i < RowsCount; i++)
                for (int j = 0; j < ColumnsCount; j++)
                    if (Values[i, j] != other.Values[i, j])
                        return false;

            return true;
        }

        public Matrix<T> Copy()
        {
            var result = new Matrix<T>(RowsCount, ColumnsCount);

            for (int i = 0; i < RowsCount; i++)
                for (int j = 0; j < ColumnsCount; j++)
                    result[i, j] = Values[i, j];

            return result;
        }

        public override string ToString()
        {
            StringBuilder stringMatrix = new StringBuilder() ;

            for(int i = 0; i < RowsCount; i++)
            {
                for(int j = 0; j < ColumnsCount; j++)
                {
                    stringMatrix.Append(Values[i,j]?.ToString() ?? "null");
                    stringMatrix.Append('\t');
                }
                stringMatrix.AppendLine();
            }

            return stringMatrix.ToString();
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(RowsCount);
            hash.Add(ColumnsCount);

            for (int i = 0; i < RowsCount; i++)
                for (int j = 0; j < ColumnsCount; j++)
                    hash.Add(Values[i, j]);

            return hash.ToHashCode();
        }
    }
}
