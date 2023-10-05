using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Numerics.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace LinearAlgebra
{
    [Serializable]
    class Matrix<T> : ICloneable
        where T : INumber<T>
    {
        #region properties
        /// <summary>
        /// The raw matrix with all values
        /// </summary>
        public T[,] matrix;
        
        /// <summary>
        /// The length of the matrix in the y-axis
        /// </summary>
        public int rowNumber { get { return matrix.GetLength(0); } }

        /// <summary>
        /// The length of the matrix in the x-axis
        /// </summary>
        public int columnNumber { get { return matrix.GetLength(1); } }

        /// <summary>
        /// The determinant of the matrix
        /// </summary>
        public T determinant { get { return FindDeterminant(this); } }

        /// <summary>
        /// The rank of the matrix
        /// </summary>
        public int rank { get { return FindRankUsingDeterminant(this); } }

        /// <summary>
        /// The nullity of this matrix
        /// </summary>
        public int nullity { get { return columnNumber - rank; } }

        /// <summary>
        /// Checks if this matrix is invertable
        /// </summary>
        public bool isInvertable { get { return determinant != T.Zero; } }

        /// <summary>
        /// The inverse of the matrix, returns null if the matrix is non-invertable
        /// </summary>
        public Matrix<T>? inverse { get { return isInvertable ? FindInverseUsingGauss(this) : null; } }

        /// <summary>
        /// The reduced echelon form of the matrix
        /// </summary>
        public Matrix<T> reducedEchelonForm { get { return FindReducedRowEchelonForm(this); } }

        /// <summary>
        /// Gets an element of the matrix
        /// </summary>
        /// <param name="ColumnIndex">The x-value of the value</param>
        /// <param name="RowIndex">The y-value of the value</param>
        public T this[int ColumnIndex, int RowIndex] { get { return matrix[RowIndex, ColumnIndex]; } set { matrix[RowIndex, ColumnIndex] = value; } }
        #endregion

        #region constructors
        /// <summary>
        /// Makes a 0 by 0 matrix
        /// </summary>
        public Matrix() { MakeNewMatrix(0, 0); }

        /// <summary>
        /// Initializes the matrix with a 2D array 
        /// </summary>
        /// <param name="inputMatrix">The 2D array</param>
        public Matrix(T[,] inputMatrix) { matrix = inputMatrix; }

        /// <summary>
        /// Initializes the matrix with a jaggered array
        /// </summary>
        /// <param name="inputMatrix">The jaggered array</param>
        public Matrix(T[][] inputMatrix)
        {
            // Checks if the inputMatrix is usable
            for (int i = 1; i < inputMatrix.Length; i++) 
            {
                if(inputMatrix[i - 1].Length != inputMatrix[i].Length)
                {
                    throw new Exception("The arrays contained in the jaggered array must all have the same length");
                }
            }
            
            // Edgecase
            if(inputMatrix.Length == 0) { matrix = new T[0,0]; }

            // Copies the jaggered array
            matrix = new T[inputMatrix.Length, inputMatrix[0].Length];
            for(int i = 0; i < inputMatrix.Length; i++)
            {
                for(int j = 0; j < inputMatrix[0].Length; j++)
                {
                    matrix[i, j] = inputMatrix[i][j];
                }
            }
        }

        /// <summary>
        /// Initializes the matrix using a length in the x- and y-axis
        /// </summary>
        /// <param name="colNum">The length in the x-axis</param>
        /// <param name="rowNum">The length in the y-axis</param>
        public Matrix(int colNum, int rowNum)
        {
            MakeNewMatrix(colNum, rowNum);
        }
        #endregion

        #region ultility functions

        /// <summary>
        /// Makes a new matrix and saves it 
        /// </summary>
        /// <param name="colNum">The length in the x-axis</param>
        /// <param name="rowNum">The length in the y-axis</param>
        private void MakeNewMatrix(int colNum, int rowNum)
        {
            T[,] newMatrix = new T[rowNum,colNum];
            matrix = newMatrix;
        }

        /// <summary>
        /// Makes an identity matrix
        /// </summary>
        /// <param name="length">The length of the matrix in the x- and y-axis</param>
        /// <returns>The identity matrix</returns>
        public static Matrix<T> CreateAnIdentityMatrix(int length)
        {
            Matrix<T> matrix = new Matrix<T>(length, length);
            for (int i = 0; i < length; i++) { matrix[i, i] = T.One; }
            return matrix;
        }

        /// <summary>
        /// Creates a matrix filled with zeroes
        /// </summary>
        /// <param name="rowNum">The number of rows in the matrix</param>
        /// <param name="colNum">The number of columns in the matrix</param>
        /// <returns>The matrix filled with zeros</returns>
        public static Matrix<T> CreateZeroMatrix(int rowNum, int colNum)
        {
            Matrix<T> matrix = new Matrix<T>(colNum, rowNum);
            for(int i = 0; i < matrix.columnNumber; i++)
            {
                for(int j = 0; j < matrix.rowNumber; j++) { matrix[i, j] = T.Zero; }
            }
            return matrix;
        }

        /// <summary>
        /// Creates a matrix filled with ones
        /// </summary>
        /// <param name="rowNum">The number of rows in the matrix</param>
        /// <param name="colNum">The number of columns in the matrix</param>
        /// <returns>The matrix filled with ones</returns>
        public static Matrix<T> CreateOnesMatrix(int rowNum, int colNum)
        {
            Matrix<T> matrix = new Matrix<T>(colNum, rowNum);
            for (int i = 0; i < matrix.columnNumber; i++)
            {
                for (int j = 0; j < matrix.rowNumber; j++) { matrix[i, j] = T.One; }
            }
            return matrix;
        }

        /// <summary>
        /// Converts the matrix into a string
        /// </summary>
        /// <returns>The matrix as a string</returns>
        public string WriteMatrix()
        {
            string result = "";
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    result += $"{matrix[i, j]}\t";
                }
                result += "\n";
            }
            return result;
        }

        /// <summary>
        /// Flips the matrix around its identity (the diagonal)
        /// </summary>
        public void FlipMatrixAroundIdentity()
        {
            T[,] result = new T[rowNumber, columnNumber];
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    result[i, j] = matrix[j, i];
                }
            }
            matrix = result;
        }

        /// <summary>
        /// Reduces the matrix to a 1D array
        /// </summary>
        /// <returns>The matrix as an array</returns>
        public T[] ReduceToArray()
        {
            T[] result = new T[rowNumber * columnNumber];

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    result[i * columnNumber + j] = this[j, i];
                }
            }

            return result;
        }

        /// <summary>
        /// Makes a deep copy of the matrix
        /// </summary>
        /// <returns>A deep copy of this matrix</returns>
        public object Clone()
        {
            return new Matrix<T>((T[,])matrix.Clone());
        }
        #endregion

        #region Read, Write and other manipulations of rows, columns and the matrix
        /// <summary>
        /// Gets a row with a ceritan index
        /// </summary>
        /// <param name="index">The index of the row</param>
        /// <returns>A 1(y) by n(x) matrix</returns>
        public Matrix<T> GetRow(int index)
        {
            Matrix<T> row = new Matrix<T>(columnNumber, 1);
            for(int i = 0;  i < columnNumber; i++) { row[i,0] = this[i,index]; }
            return row;
        }

        /// <summary>
        /// Gets a colum with a ceritan index
        /// </summary>
        /// <param name="index">The index of the colum</param>
        /// <returns>A n(y) by 1(x) matrix</returns>
        public Matrix<T> GetColum(int index)
        {
            Matrix<T> colum = new Matrix<T>(1, rowNumber);
            for(int i = 0; i < rowNumber; i++) { colum[0,i] = this[index, i]; }
            return colum;
        }

        /// <summary>
        /// Removes a row from the matrix
        /// </summary>
        /// <param name="index">The index of the row that should be removed</param>
        public void RemoveRow(int index)
        {
            T[,] result = new T[rowNumber - 1, columnNumber];
            for (int i = 0; i < rowNumber; i++)
            {
                if (i == index) { continue; }
                for (int j = 0; j < columnNumber; j++)
                {
                    result[i - ((i > index) ? 1 : 0), j] = matrix[i, j];
                }
            }
            matrix = result;
        }

        /// <summary>
        /// Removes a colum from the matrix 
        /// </summary>
        /// <param name="index">The index of the colum that needs to be removed</param>
        public void RemoveColum(int index)
        {
            T[,] result = new T[rowNumber, columnNumber - 1];
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    if (j == index) { continue; }
                    result[i, j - ((j > index) ? 1 : 0)] = matrix[i, j];
                }
            }
            matrix = result;
        }

        /// <summary>
        /// Adds an empty row
        /// </summary>
        /// <param name="index">The index where the row needs to be added</param>
        public void AddRow(int index)
        {
            T[,] result = new T[rowNumber + 1, columnNumber];
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    result[i + ((i >= index) ? 1 : 0), j] = matrix[i, j];
                }
            }
            matrix = result;
        }

        /// <summary>
        /// Adds a filled row
        /// </summary>
        /// <param name="index">The index where the row needs to be added</param>
        /// <param name="row">The row that needs to be added</param>
        public void AddRow(int index, Matrix<T> row)
        {
            AddRow(index);
            ReplaceRow(index, row);
        }

        /// <summary>
        /// Adds an empty colum
        /// </summary>
        /// <param name="index">The index where the row needs to be added</param>
        public void AddColum(int index)
        {
            T[,] result = new T[rowNumber, columnNumber + 1];
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    result[i, j + ((j >= index) ? 1 : 0)] = matrix[i, j];
                }
            }
            matrix = result;
        }

        /// <summary>
        /// Adds a filled colum
        /// </summary>
        /// <param name="index">The index where the colum needs to be added</param>
        /// <param name="colum">The colum that needs to be added</param>
        public void AddColum(int index, Matrix<T> colum)
        {
            AddColum(index);
            ReplaceColum(index, colum);
        }

        /// <summary>
        /// Replaces a row with anoter
        /// </summary>
        /// <param name="index">The index of the row that needs to be replaceds</param>
        /// <param name="row">The row that needs to be inserted</param>
        public void ReplaceRow(int index, Matrix<T> row)
        {
            if(row.columnNumber != columnNumber) { throw new Exception("The length of the row needs to be the same as the length of the matrix"); }

            for(int i = 0; i < columnNumber; i++)
            {
                matrix[index, i] = row[i,0];
            }
        }

        /// <summary>
        /// Replaces a colum with another
        /// </summary>
        /// <param name="index">The index of the colum that needs to be replaced</param>
        /// <param name="colum">The colum that is replacing the other (passed as a vertical matrix)</param>
        public void ReplaceColum(int index, Matrix<T> colum)
        {
            if (colum.rowNumber != rowNumber) { throw new Exception("The length of the row needs to be the same as the length of the matrix"); }

            for (int i = 0; i < rowNumber; i++)
            {
                matrix[i, index] = colum[0,i];
            }
        }

        /// <summary>
        /// Appends a matrix to the matrix
        /// </summary>
        /// <param name="inputMatrix">The matrix that should be appended</param>
        /// <param name="yDir">The direction the matrix should be appended in. True is in the y-direction, while false is in the x-direction</param>
        public void AppendMatrix(Matrix<T> inputMatrix, bool yDir = true)
        {
            if (yDir?(inputMatrix.columnNumber != columnNumber):(inputMatrix.rowNumber != rowNumber)) { throw new Exception("Non-viable matrix: The matrix must have the same length in the dimention that is not being appended"); }

            if (yDir)
            {
                int orgRowNumber = rowNumber;
                for (int i = 0; i < inputMatrix.rowNumber; i++)
                {
                    AddRow(i + orgRowNumber, inputMatrix.GetRow(i));
                }
            }
            else
            {
                int orgColumnNuber = columnNumber;
                for (int i = 0; i < inputMatrix.columnNumber; i++)
                {
                    AddColum(i + orgColumnNuber, inputMatrix.GetColum(i));
                }
            }
        }

        /// <summary>
        /// "Cuts" a submatrix out from the matrix
        /// </summary>
        /// <param name="subXLength">The length of the matrix in the x-axis</param>
        /// <param name="subYLength">The length of the matrix in the y-axis</param>
        /// <param name="xPoint">The x-value of the origen of the submatrix</param>
        /// <param name="yPoint">The y-value of the origen of the submatrix</param>
        /// <returns>A submatrix</returns>
        public Matrix<T> SubMatrix(int subXLength, int subYLength, int xPoint = 0, int yPoint = 0)
        {
            if(subXLength + xPoint > columnNumber || subYLength + yPoint > rowNumber) { throw new Exception("Submatrix exeeds bounds of matrix"); }

            Matrix<T> result = new Matrix<T>(subXLength, subYLength);

            for (int i = 0; xPoint + i < columnNumber && i < subXLength; i++)
            {
                for (int j = 0; yPoint + j < rowNumber && j < subYLength; j++)
                {
                    result[i, j] = this[xPoint + i, yPoint + j];
                }
            }

            return result;
        }
        #endregion

        #region elementary row operations
        /// <summary>
        /// Scales a row with the provied scalar-value. This is a type 1 elementary row operation
        /// </summary>
        /// <param name="scalar">The scalar</param>
        /// <param name="rowIndex">The rowindex of the row</param>
        public void ScaleRow(T scalar, int rowIndex)
        {
            for (int i = 0; i < columnNumber; i++)
            {
                matrix[rowIndex, i] *= scalar;
            }
        }

        /// <summary>
        /// Swaps to rows given their indices. This is a type 2 elementary row operation
        /// </summary>
        /// <param name="rowIndex1">The index of the first row</param>
        /// <param name="rowIndex2">The index of the second row</param>
        public void SwapRows(int rowIndex1, int rowIndex2)
        {
            Matrix<T> row1 = GetRow(rowIndex1);
            Matrix<T> row2 = GetRow(rowIndex2);
            ReplaceRow(rowIndex1, row2);
            ReplaceRow(rowIndex2, row1);
        }

        /// <summary>
        /// Makes a copy of a row, scales it, and adds it to another row. This is a type 3 elementary row operation
        /// </summary>
        /// <param name="scalar"> The scalar</param>
        /// <param name="rowIndex1"> The row to be copied and scaled</param>
        /// <param name="rowIndex2"> The row, which is to be added to</param>
        public void ScaleAndAddRow(T scalar, int rowIndex1, int rowIndex2) 
        {
            ReplaceRow(rowIndex2, GetRow(rowIndex2) + GetRow(rowIndex1) * scalar);
        }
        #endregion

        #region Arithmatic operators
        public static Matrix<T> operator *(Matrix<T> algMatrix, T val)
        {
            for (int i = 0; i < algMatrix.rowNumber; i++)
            {
                for (int j = 0; j < algMatrix.columnNumber; j++)
                {
                    algMatrix[j, i] *= val;
                }
            }
            return algMatrix;
        }

        public static Matrix<T> operator /(Matrix<T> algMatrix, T val)
        {
            return algMatrix * (T.One / val);
        }

        public static Matrix<T> operator +(Matrix<T> algMatrix1, Matrix<T> algMatrix2)
        {
            if(algMatrix1.columnNumber != algMatrix2.columnNumber || algMatrix1.rowNumber != algMatrix2.rowNumber)
            {
                throw new Exception("Both matricies must have the same length in both dimentions");
            }

            for(int i = 0; i < algMatrix1.rowNumber; i++)
            {
                for(int j = 0; j < algMatrix2.columnNumber; j++)
                {
                    algMatrix1[j,i] += algMatrix2[j,i];
                }
            }
            return algMatrix1;
        }

        public static Matrix<T> operator -(Matrix<T> algMatrix1, Matrix<T> algMatrix2)
        {
            return algMatrix1 + (-algMatrix2);
        }

        public static Matrix<T> operator -(Matrix<T> algMatrix)
        {
            return algMatrix * -T.One;
        }

        // TODO: ADD MATRIXMULTIPLICATION
        #endregion

        #region Matrix-specific attributes
        /// <summary>
        /// Finds the determinant of a matrix
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The determinat as a double</returns>
        public static T FindDeterminant(Matrix<T> input)
        {
            if (input.columnNumber != input.rowNumber) { throw new Exception("Nonviable matrix: The matrix must have the same length in both directions."); }
            T determinant = T.Zero;
            int length = input.rowNumber;
            if (length == 1) { return input[0, 0]; }

            if (length == 2)
            {
                return input[0, 0] * input[1, 1] - input[0, 1] * input[1, 0];
            }

            for (int i = 0; i < length; i++)
            {
                Matrix<T> subMatrix = (Matrix<T>)input.Clone();
                subMatrix.RemoveRow(0); subMatrix.RemoveColum(i);

                T temp = (i % 2 == 0) ? T.One : -T.One;
                T subDeterminant = temp * FindDeterminant(subMatrix);

                determinant += input[i, 0] * subDeterminant;
            }

            return determinant;
        }
        
        /// <summary>
        /// Finds the rank of a matrix
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The rank of that matrix, -1 if none can be found</returns>
        public static int FindRankUsingDeterminant(Matrix<T> input)
        {
            int rank = Math.Min(input.columnNumber, input.rowNumber);
            T determinant = T.Zero;
            bool breakout = true;
            while (breakout)
            {
                if (rank < 1)
                {
                    return -1;
                }

                for (int i = 0; i < input.rowNumber - rank + 1; i++)
                {
                    for (int j = 0; j < input.columnNumber - rank + 1; j++)
                    {
                        Matrix<T> temp = input.SubMatrix(rank, rank, j, i);
                        determinant = temp.determinant;
                        if (determinant != T.Zero)
                        {
                            breakout = false;
                        }
                    }
                }
                rank--;
            }
            rank++;

            return rank;
        }

        /// <summary>
        /// Finds the inverse of a matrix using it's determinant (assumes the matrix is invertable)
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>The inverse of the matrix as a matrix</returns>
        public static Matrix<T> FindInverse(Matrix<T> matrix)
        {
            T determiant = FindDeterminant(matrix);
            Matrix<T> inverse = new Matrix<T>(matrix.rowNumber, matrix.columnNumber);

            for (int i = 0; i < matrix.rowNumber; i++)
            {
                for (int j = 0; j < matrix.columnNumber; j++)
                {
                    Matrix<T> tempMatrix = (Matrix<T>)matrix.Clone();
                    tempMatrix.RemoveRow(i); tempMatrix.RemoveColum(j);
                    inverse.matrix[i, j] = (((i + j) % 2 == 0)? T.One : -T.One) * FindDeterminant(tempMatrix);
                }
            }

            inverse.FlipMatrixAroundIdentity();
            inverse /= determiant;

            return inverse;
        }

        /// <summary>
        /// Finds the inverse of a matrix using Gauss-Jordan elimination (assumes the matrix is invertable)
        /// </summary>
        /// <param name="input">The matrix that should be inverted</param>
        /// <returns>The inverse of the input matrix</returns>
        public static Matrix<T> FindInverseUsingGauss(Matrix<T> input)
        {
            Matrix<T> inverse = CreateAnIdentityMatrix(input.rowNumber);

            int rowsCompleted = 0;
            int columnsCompleted = 0;
            while (rowsCompleted != input.rowNumber && columnsCompleted != input.columnNumber)
            {
                // Skips all columns not containing a pivot
                if (GenericSum(input.GetColum(columnsCompleted).ReduceToArray()) == T.Zero)
                {
                    columnsCompleted++;
                    continue;
                }

                // Finds the entry with the largest absolute value 
                int index = MaxAbsValIndex(input.GetColum(columnsCompleted).ReduceToArray());

                // Moves the row containing the largest absolute value to the top (type 1 elementary operation)
                if (index != rowsCompleted) 
                { 
                    input.SwapRows(index, rowsCompleted); 
                    inverse.SwapRows(index, rowsCompleted); 
                }

                // Scales the row so the aforementioned value becomes 1 (type 2 elementary operation)
                input.ScaleRow(T.One / input[columnsCompleted, rowsCompleted], rowsCompleted);
                inverse.ScaleRow(T.One / input[columnsCompleted, rowsCompleted], rowsCompleted);

                // Makes sure the other entries in the column is reduced to 0
                for (int i = 0; i < input.rowNumber; i++)
                {
                    if (i == rowsCompleted) { continue; }
                    input.ScaleAndAddRow(-input[columnsCompleted, i], rowsCompleted, i);
                    inverse.ScaleAndAddRow(-input[columnsCompleted, i], rowsCompleted, i);
                }

                rowsCompleted++;
                columnsCompleted++;
            }

            return input;

            // Finds the index containing the maximium absolute entry
            int MaxAbsValIndex(T[] values)
            {
                T max = -T.One;
                int maxIndex = -1;
                for (int i = rowsCompleted; i < values.Length; i++)
                {
                    if (max < T.Abs(values[i])) { max = T.Abs(values[i]); maxIndex = i; }
                }
                return maxIndex;
            }

            // Finds the sum of entries under the finished rows
            T GenericSum(T[] values)
            {
                T sum = T.Zero;
                for (int i = rowsCompleted; i < values.Length; i++)
                {
                    sum += values[i];
                }
                return sum;
            }
        }

        /// <summary>
        /// Finds the reduced row echelon form of a matrix using gaussian elination 
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The reduced row echelon form as a matrix</returns>
        public static Matrix<T> FindReducedRowEchelonForm(Matrix<T> input)
        {
            int rowsCompleted = 0;
            int columnsCompleted = 0;
            while(rowsCompleted != input.rowNumber && columnsCompleted != input.columnNumber)
            {
                // Skips all columns not containing a pivot
                if (GenericSum(input.GetColum(columnsCompleted).ReduceToArray()) == T.Zero)
                {
                    columnsCompleted++;
                    continue;
                }

                // Finds the entry with the largest absolute value 
                int index = MaxAbsValIndex(input.GetColum(columnsCompleted).ReduceToArray());

                // Moves the row containing the largest absolute value to the top (type 1 elementary operation)
                if (index != rowsCompleted) 
                { 
                    input.SwapRows(index, rowsCompleted);
                }

                // Scales the row so the aforementioned value becomes 1 (type 2 elementary operation)
                input.ScaleRow(T.One / input[columnsCompleted, rowsCompleted], rowsCompleted);

                // Makes sure the other entries in the column is reduced to 0
                for (int i = 0; i < input.rowNumber; i++)
                {
                    if (i == rowsCompleted) { continue; }
                    input.ScaleAndAddRow(-input[columnsCompleted, i], rowsCompleted, i);
                }

                rowsCompleted++;
                columnsCompleted++;
            }

            return input;

            // Finds the index containing the maximium absolute entry
            int MaxAbsValIndex(T[] values)
            {
                T max = -T.One;
                int maxIndex = -1;
                for(int i = rowsCompleted; i < values.Length; i++)
                {
                    if(max < T.Abs(values[i])) { max = T.Abs(values[i]); maxIndex = i; }
                }
                return maxIndex;
            } 

            // Finds the sum of entries under the finished rows
            T GenericSum(T[] values) 
            {
                T sum = T.Zero;
                for(int i = rowsCompleted; i < values.Length; i++)
                {
                    sum += values[i];
                }
                return sum;
            }
        }

        #endregion
    }
}
