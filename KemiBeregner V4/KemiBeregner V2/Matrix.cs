using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Rationals;

namespace LinearAlgebra
{
    [Serializable]
    class Matrix
    {
        /// <summary>
        /// The raw matrix with all values
        /// </summary>
        public double[,] matrix;
        
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
        public double determinant { get { return FindDeterminant(this); } }

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
        public Matrix inverse { get { return FindInverse(this); } }

        /// <summary>
        /// The reduced echelon form of the matrix
        /// </summary>
        public Matrix reducedEchelonForm { get { return FindReducedRowEchelonForm(this); } }

        /// <summary>
        /// Gets an element of the matrix
        /// </summary>
        /// <param name="indexX">The x-value of the value</param>
        /// <param name="indexY">The y-value of the value</param>
        public double this[int indexX, int indexY] { get { return matrix[indexY, indexX]; } set { matrix[indexY, indexX] = value; } }
        
        /// <summary>
        /// Initializes the matrix with a 2D array 
        /// </summary>
        /// <param name="inputMatrix">The 2D array</param>
        public Matrix(double[,] inputMatrix) { matrix = inputMatrix; }

        /// <summary>
        /// Initializes the matrix with a jaggered array
        /// </summary>
        /// <param name="inputMatrix">The jaggered array</param>
        public Matrix(double[][] inputMatrix)
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
            if(inputMatrix.Length == 0) { matrix = new double[0,0]; }

            // Copies the jaggered array
            matrix = new double[inputMatrix.Length, inputMatrix[0].Length];
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

        /// <summary>
        /// Makes an identity matrix
        /// </summary>
        /// <param name="length">The length of the matrix in the x- and y-axis</param>
        /// <returns>The identity matrix</returns>
        public static Matrix CreateAnIdentityMatrix(int length)
        {
            Matrix matrix = new Matrix(length, length);
            for (int i = 0; i < length; i++) { matrix[i, i] = 1; }
            return matrix;
        }

        /// <summary>
        /// Makes a 0 by 0 matrix
        /// </summary>
        public Matrix() { MakeNewMatrix(0, 0); }

        /// <summary>
        /// Makes a new matrix and saves it 
        /// </summary>
        /// <param name="initXLength">The length in the x-axis</param>
        /// <param name="initYLength">The length in the y-axis</param>
        private void MakeNewMatrix(int initXLength, int initYLength)
        {
            double[,] newMatrix = new double[initYLength,initXLength];
            matrix = newMatrix;
        }

        /// <summary>
        /// Gets a row with a ceritan index
        /// </summary>
        /// <param name="index">The index of the row</param>
        /// <returns>A 1(y) by n(x) matrix</returns>
        public Matrix GetRow(int index)
        {
            Matrix row = new Matrix(xLength, 1);
            for(int i = 0;  i < xLength; i++) { row[i,0] = this[i,index]; }
            return row;
        }

        /// <summary>
        /// Gets a colum with a ceritan index
        /// </summary>
        /// <param name="index">The index of the colum</param>
        /// <returns>A n(y) by 1(x) matrix</returns>
        public Matrix GetColum(int index)
        {
            Matrix colum = new Matrix(1, yLength);
            for(int i = 0; i < yLength; i++) { colum[0,i] = this[index, i]; }
            return colum;
        }

        /// <summary>
        /// Removes a row from the matrix
        /// </summary>
        /// <param name="index">The index of the row that should be removed</param>
        public void RemoveRow(int index)
        {
            double[,] result = new double[yLength - 1, xLength];
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
            double[,] result = new double[yLength, xLength - 1];
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
            double[,] result = new double[yLength + 1, xLength];
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
        public void AddRow(int index, Matrix row)
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
            double[,] result = new double[yLength, xLength + 1];
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
        public void AddColum(int index, Matrix colum)
        {
            AddColum(index);
            ReplaceColum(index, colum);
        }

        /// <summary>
        /// Replaces a row with anoter
        /// </summary>
        /// <param name="index">The index of the row that needs to be replaceds</param>
        /// <param name="row">The row that needs to be inserted</param>
        public void ReplaceRow(int index, Matrix row)
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
        public void ReplaceColum(int index, Matrix colum)
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
        public void AppendMatrix(Matrix inputMatrix, bool yDir = true)
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
        public Matrix SubMatrix(int subXLength, int subYLength, int xPoint = 0, int yPoint = 0)
        {
            if(subXLength + xPoint > xLength || subYLength + yPoint > yLength) { throw new Exception("Submatrix exeeds bounds of matrix"); }

            Matrix result = new Matrix(subXLength, subYLength);

            for (int i = 0; xPoint + i < xLength && i < subXLength; i++)
            {
                for (int j = 0; yPoint + j < yLength && j < subYLength; j++)
                {
                    result[i, j] = this[xPoint + i, yPoint + j];
                }
            }

            return result;
        }

        /// <summary>
        /// Writes the matrix out
        /// </summary>
        /// <param name="val">The string the matrix should the written to</param>
        public void WriteMatrix(out string val)
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

            val = result;
        }

        /// <summary>
        /// Writes a matrix out to the console
        /// </summary>
        public void WriteMatrix()
        {
            string val;
            WriteMatrix(out val);
            Console.WriteLine(val);
        }

        /// <summary>
        /// Flips the matrix around its identity (the diagonal)
        /// </summary>
        public void FlipMatrixAroundIdentity()
        {
            double[,] result = new double[yLength, xLength];
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
                {
                    result[i, j] = matrix[j, i];
                }
            }
            matrix = result;
        }

        public static Matrix operator *(Matrix algMatrix, double val)
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

        public static Matrix operator /(Matrix algMatrix, double val)
        {
            return algMatrix * (1 / val);
        }

        public static Matrix operator +(Matrix algMatrix, double val)
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

        public static Matrix operator -(Matrix algMatrix1, double val)
        {
            return algMatrix1 + (-val);
        }

        public static Matrix operator +(Matrix algMatrix1, Matrix algMatrix2)
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

        public static Matrix operator -(Matrix algMatrix1, Matrix algMatrix2)
        {
            return algMatrix1 + (algMatrix2 * -1);
        }


        /// <summary>
        /// Finds the determinant of a matrix
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The determinat as a double</returns>
        public double FindDeterminant(Matrix input)
        {
            if (input.xLength != input.yLength) { throw new Exception("Nonviable matrix: The matrix must have the same length in both directions."); }
            double determinant = 0;
            int length = input.yLength;
            if (length == 1) { return input[0, 0]; }

            if (length == 2)
            {
                return input[0, 0] * input[1, 1] - input[0, 1] * input[1, 0];
            }

            for (int i = 0; i < length; i++)
            {
                Matrix subMatrix = DeepCopy(input);
                subMatrix.RemoveRow(0); subMatrix.RemoveColum(i);

                double temp = Math.Pow(-1, i % 2);
                double subDeterminant = temp * FindDeterminant(subMatrix);

                determinant += input[i, 0] * subDeterminant;
            }

            return determinant;
        }
        
        /// <summary>
        /// Finds the rank of a matrix
        /// </summary>
        /// <param name="input">The matrix</param>
        /// <returns>The rank of that matrix</returns>
        public int FindRank(Matrix input)
        {
            int rank = Math.Min(input.xLength, input.yLength);
            double determinant = 0;
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
                        Matrix temp = input.SubMatrix(rank, rank, j, i);
                        determinant = temp.determinant;
                        if (determinant != 0)
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
        public Matrix FindInverse(Matrix matrix)
        {
            double determiant = (double)FindDeterminant(matrix);
            Matrix inverse = new Matrix(matrix.yLength, matrix.xLength);

            for (int i = 0; i < matrix.yLength; i++)
            {
                for (int j = 0; j < matrix.xLength; j++)
                {
                    Matrix tempMatrix = DeepCopy(matrix);
                    tempMatrix.RemoveRow(i); tempMatrix.RemoveColum(j);
                    inverse.matrix[i, j] = (double)Math.Pow(-1, (i + j) % 2) * (double)FindDeterminant(tempMatrix);
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
        public Matrix FindReducedRowEchelonForm(Matrix input)
        {
            int lead = 0;
            for (int r = 0; r < input.yLength; r++)
            {
                if (input.xLength <= lead) { break; }
                int i = r;
                while (input[lead, i] == 0)
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

                if (input[lead, r] != 0)
                {
                    input.ReplaceRow(r, input.GetRow(r) / input[lead, r]);
                }

                for (; i < input.yLength; i++)
                {
                    if (r != i)
                    {
                        Matrix tempMatrix1 = input.GetRow(i);
                        Matrix tempMatrix2 = input.GetRow(r) * input[lead, i];
                        input.ReplaceRow(i, tempMatrix1 - tempMatrix2);
                    }
                }
                lead++;
            }

            return input;

            void SwapRows(int rowIndex1, int rowIndex2)
            {
                Matrix firstRow = input.GetRow(rowIndex1);
                Matrix lastRow = input.GetRow(rowIndex2);
                input.ReplaceRow(rowIndex2, firstRow);
                input.ReplaceRow(rowIndex1, lastRow);
            }
        }

        /// <summary>
        /// Reduces the matrix to a 1D array
        /// </summary>
        /// <returns>The matrix as an array</returns>
        public double[] ReduceToArray()
        {
            double[] result = new double[yLength * xLength];

            for(int i = 0; i < yLength; i++)
            {
                for(int j = 0; j < xLength; j++)
                {
                    result[i * xLength + j] = this[j, i];
                }
            }

            return result;
        }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
