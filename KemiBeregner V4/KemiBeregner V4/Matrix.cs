using System;
using System.IO;
using System.Numerics;
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
        public int yLength { get { return matrix.GetLength(0); } }

        /// <summary>
        /// The length of the matrix in the x-axis
        /// </summary>
        public int xLength { get { return matrix.GetLength(1); } }

        /// <summary>
        /// The determinant of the matrix
        /// </summary>
        public T determinant { get { return FindDeterminant(this); } }

        /// <summary>
        /// The rank of the matrix
        /// </summary>
        public int rank { get { return FindRank(this); } }

        /// <summary>
        /// The nullity of this matrix
        /// </summary>
        public int nullity { get { return xLength - rank; } }

        /// <summary>
        /// The inverse of the matrix
        /// </summary>
        public Matrix<T> inverse { get { return FindInverse(this); } }

        /// <summary>
        /// The reduced echelon form of the matrix
        /// </summary>
        public Matrix<T> reducedEchelonForm { get { return FindReducedRowEchelonForm(this); } }

        /// <summary>
        /// Gets an element of the matrix
        /// </summary>
        /// <param name="indexX">The x-value of the value</param>
        /// <param name="indexY">The y-value of the value</param>
        public T this[int indexX, int indexY] { get { return matrix[indexY, indexX]; } set { matrix[indexY, indexX] = value; } }
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
        /// <param name="initXLength">The length in the x-axis</param>
        /// <param name="initYLength">The length in the y-axis</param>
        public Matrix(int initXLength, int initYLength)
        {
            MakeNewMatrix(initXLength, initYLength);
        }
        #endregion

        #region ultility functions

        /// <summary>
        /// Makes a new matrix and saves it 
        /// </summary>
        /// <param name="initXLength">The length in the x-axis</param>
        /// <param name="initYLength">The length in the y-axis</param>
        private void MakeNewMatrix(int initXLength, int initYLength)
        {
            T[,] newMatrix = new T[initYLength,initXLength];
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
        /// Writes the matrix out
        /// </summary>
        /// <param name="val">The string the matrix should the written to</param>
        public string WriteMatrix()
        {
            string result = "";
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
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
            T[,] result = new T[yLength, xLength];
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
                {
                    result[i, j] = matrix[j, i];
                }
            }
            matrix = result;
        }

        // TODO: ADD ZERO-MATRIX



        /// <summary>
        /// Reduces the matrix to a 1D array
        /// </summary>
        /// <returns>The matrix as an array</returns>
        public T[] ReduceToArray()
        {
            T[] result = new T[yLength * xLength];

            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
                {
                    result[i * xLength + j] = this[j, i];
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
            Matrix<T> row = new Matrix<T>(xLength, 1);
            for(int i = 0;  i < xLength; i++) { row[i,0] = this[i,index]; }
            return row;
        }

        /// <summary>
        /// Gets a colum with a ceritan index
        /// </summary>
        /// <param name="index">The index of the colum</param>
        /// <returns>A n(y) by 1(x) matrix</returns>
        public Matrix<T> GetColum(int index)
        {
            Matrix<T> colum = new Matrix<T>(1, yLength);
            for(int i = 0; i < yLength; i++) { colum[0,i] = this[index, i]; }
            return colum;
        }

        /// <summary>
        /// Removes a row from the matrix
        /// </summary>
        /// <param name="index">The index of the row that should be removed</param>
        public void RemoveRow(int index)
        {
            T[,] result = new T[yLength - 1, xLength];
            for (int i = 0; i < yLength; i++)
            {
                if (i == index) { continue; }
                for (int j = 0; j < xLength; j++)
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
            T[,] result = new T[yLength, xLength - 1];
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
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
            T[,] result = new T[yLength + 1, xLength];
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
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
            T[,] result = new T[yLength, xLength + 1];
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
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
            if(row.xLength != xLength) { throw new Exception("The length of the row needs to be the same as the length of the matrix"); }

            for(int i = 0; i < xLength; i++)
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
            if (colum.yLength != yLength) { throw new Exception("The length of the row needs to be the same as the length of the matrix"); }

            for (int i = 0; i < yLength; i++)
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
            if (yDir?(inputMatrix.xLength != xLength):(inputMatrix.yLength != yLength)) { throw new Exception("Non-viable matrix: The matrix must have the same length in the dimention that is not being appended"); }

            if (yDir)
            {
                for(int i = 0; i < inputMatrix.yLength; i++)
                {
                    AddRow(i + yLength, inputMatrix.GetRow(i));
                }
            }
            else
            {
                for(int i = 0; i < inputMatrix.xLength; i++)
                {
                    AddColum(i + xLength, inputMatrix.GetColum(i));
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
            if(subXLength + xPoint > xLength || subYLength + yPoint > yLength) { throw new Exception("Submatrix exeeds bounds of matrix"); }

            Matrix<T> result = new Matrix<T>(subXLength, subYLength);

            for (int i = 0; xPoint + i < xLength && i < subXLength; i++)
            {
                for (int j = 0; yPoint + j < yLength && j < subYLength; j++)
                {
                    result[i, j] = this[xPoint + i, yPoint + j];
                }
            }

            return result;
        }
        #endregion

        #region Arithmatic operators
        public static Matrix<T> operator *(Matrix<T> algMatrix, T val)
        {
            for (int i = 0; i < algMatrix.yLength; i++)
            {
                for (int j = 0; j < algMatrix.xLength; j++)
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

        public static Matrix<T> operator +(Matrix<T> algMatrix, T val)
        {
            for (int i = 0; i < algMatrix.yLength; i++)
            {
                for (int j = 0; j < algMatrix.xLength; i++)
                {
                    algMatrix.matrix[i, j] += val;
                }
            }
            return algMatrix;
        }

        public static Matrix<T> operator -(Matrix<T> algMatrix1, T val)
        {
            return algMatrix1 + (-val);
        }

        public static Matrix<T> operator +(Matrix<T> algMatrix1, Matrix<T> algMatrix2)
        {
            if(algMatrix1.xLength != algMatrix2.xLength || algMatrix1.yLength != algMatrix2.yLength)
            {
                throw new Exception("Both matricies must have the same length in both dimentions");
            }

            for(int i = 0; i < algMatrix1.yLength; i++)
            {
                for(int j = 0; j < algMatrix2.xLength; j++)
                {
                    algMatrix1[j,i] += algMatrix2[j,i];
                }
            }
            return algMatrix1;
        }

        public static Matrix<T> operator -(Matrix<T> algMatrix1, Matrix<T> algMatrix2)
        {
            return algMatrix1 + (algMatrix2 * -T.One);
        }

        // TODO: ADD MATRIXMULTIPLICATION
        #endregion

        #region Matrix-specific attributes
        /// <summary>
        /// Finds the determinant of a matrix
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The determinat as a double</returns>
        public T FindDeterminant(Matrix<T> input)
        {
            if (input.xLength != input.yLength) { throw new Exception("Nonviable matrix: The matrix must have the same length in both directions."); }
            T determinant = T.Zero;
            int length = input.yLength;
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
        public int FindRank(Matrix<T> input)
        {
            // TODO: REDEFINE USING OTHER ALGORITM, MAYBE GAUSS-JORDAN

            int rank = Math.Min(input.xLength, input.yLength);
            T determinant = T.Zero;
            bool breakout = true;
            while (breakout)
            {
                if (rank < 1)
                {
                    return -1;
                }

                for (int i = 0; i < input.yLength - rank + 1; i++)
                {
                    for (int j = 0; j < input.xLength - rank + 1; j++)
                    {
                        Matrix<T> temp = input.SubMatrix(rank, rank, j, i);
                        determinant = temp.determinant;
                        if (determinant == T.Zero)
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
        /// Finds the inverse of a matrix
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>The inverse of the matrix as a matrix</returns>
        public Matrix<T> FindInverse(Matrix<T> matrix)
        {
            T determiant = FindDeterminant(matrix);
            Matrix<T> inverse = new Matrix<T>(matrix.yLength, matrix.xLength);

            for (int i = 0; i < matrix.yLength; i++)
            {
                for (int j = 0; j < matrix.xLength; j++)
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
        /// Finds the reduced row echelon form of a matrix using gaussian elination 
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The reduced row echelon form as a matrix</returns>
        public Matrix<T> FindReducedRowEchelonForm(Matrix<T> input)
        {
            int rows

            /*
            int lead = 0;
            for (int r = 0; r < input.yLength; r++)
            {
                if (input.xLength <= lead) { break; }
                int i = r;
                while (input[lead, i] == T.Zero)
                {
                    i++;
                    if (input.yLength == i)
                    {
                        i = r;
                        lead++;
                        if (input.xLength == lead) { lead--; break; }
                    }
                }

                SwapRows(i, r);

                if (input[lead, r] != T.Zero)
                {
                    input.ReplaceRow(r, input.GetRow(r) / input[lead, r]);
                }

                for (; i < input.yLength; i++)
                {
                    if (r != i)
                    {
                        Matrix<T> tempMatrix1 = input.GetRow(i);
                        Matrix<T> tempMatrix2 = input.GetRow(r) * input[lead, i];
                        input.ReplaceRow(i, tempMatrix1 - tempMatrix2);
                    }
                }
                lead++;
            }

            return input;
            */
            void SwapRows(int rowIndex1, int rowIndex2)
            {
                Matrix<T> firstRow = input.GetRow(rowIndex1);
                Matrix<T> lastRow = input.GetRow(rowIndex2);
                input.ReplaceRow(rowIndex2, firstRow);
                input.ReplaceRow(rowIndex1, lastRow);
            }
        }

        #endregion
    }
}
