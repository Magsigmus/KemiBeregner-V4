using LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Chemistry
{
    class ChemicalAnalyser
    {
        public struct ChemicalCompound
        {
            public List<string> elements;
            public List<double> coefficents;
            public int charge;
            public double coefficent;
            public string symbol;

            public ChemicalCompound(List<string> elements, List<double> coefficents, int charge, double coefficent, string symbol)
            {
                this.elements = elements;
                this.coefficents = coefficents;
                this.charge = charge;
                this.coefficent = coefficent;
                this.symbol = symbol;
            }
        }

        /// <summary>
        /// Balences a chemical eqation
        /// </summary>
        /// <param name="input">The chemical eqation as a string</param>
        /// <returns>The solution as a string</returns>
        public static string BalenceChemicalEqation(string input)
        {
            // Gets the compounds and chemical matrix
            string[] compounds; Matrix<BigRational> chemicalMatrix = MakeChemicalMatrix(input, out compounds);

            // gets the reduced echelon form
            chemicalMatrix = chemicalMatrix.reducedEchelonForm;

            // Removes every row that contains only zeros
            for (int i = 0; i < chemicalMatrix.rowNumber; i++)
            {
                bool allZeros = true;
                for (int j = 0; j < chemicalMatrix.columnNumber; j++)
                {
                    if (chemicalMatrix[j, i] != 0) { allZeros = false; break; }
                }
                if (allZeros) { chemicalMatrix.RemoveRow(i); i--; }
            }

            // Gets the nullity
            int nullity = chemicalMatrix.columnNumber - chemicalMatrix.rowNumber;

            if (nullity <= 0) { return "Ingen løsning"; }
            //if (nullity > 1) { return "Uendelige løsninger"; }

            // Modifices the matrix
            Matrix<BigRational> partMatrix = new Matrix<BigRational>(chemicalMatrix.columnNumber - nullity, nullity);
            partMatrix.AppendMatrix(Matrix<BigRational>.CreateAnIdentityMatrix(nullity), false); // MAYBE - HERE? AVOIDS INVERSE CALCULATION
            chemicalMatrix.AppendMatrix(partMatrix);

            // Gets the inverse of the matrix
            chemicalMatrix = chemicalMatrix.inverse;

            string solution = "";
            for(int i = 1; i <= nullity; i++)
            {
                // Gets the raw solution
                BigRational[] rawSolution = chemicalMatrix.GetColum(chemicalMatrix.columnNumber - i).ReduceToArray();

                solution += FormatSolution(RawSolutionToInt(rawSolution), compounds);
                if(i != nullity) { solution += " // "; }
            }

            return solution;
        }

        /// <summary>
        /// Makes a chemical matrix on the basis of a chemical equation
        /// </summary>
        /// <param name="input">The chemical equation as a string</param>
        /// <param name="compounds">The compounds in the equation</param>
        /// <returns>The chemical matrix as a linear algebra matrix</returns>
        public static Matrix<BigRational> MakeChemicalMatrix(string input, out string[] compounds)
        {
            // Gets all the chemical compounds and elements in the chemical 
            compounds = GetAllCompoundsInEquation(input);
            string[] elements = FindAllElements(input);
            ChemicalCompound[] compundInfos = compounds.Select(e => ParseChemcialCompound(e)).ToArray();

            Matrix<BigRational> chemicalMatrix = new Matrix<BigRational>(compundInfos.Length, elements.Length);

            // Goes through every column
            for (int i = 0; i < compundInfos.Length; i++)
            {
                ChemicalCompound compundElements = compundInfos[i];
                // Adds all the coefficents to the chemical matrix
                for (int j = 0; j < compundElements.elements.Count; j++)
                {
                    int index = 0;
                    for (; compundElements.elements[j] != elements[index] && index < elements.Length; index++) ;
                    chemicalMatrix[i, index] = compundElements.coefficents[j];  //((i >= reactantNum)?-1:1) *
                }

                // Adds the charge to the chemical matrix
                if (compundElements.charge != 0)
                {
                    int chargeIndex = 0;
                    for (; "Charge" != elements[chargeIndex] && chargeIndex < elements.Length; chargeIndex++) ;
                    chemicalMatrix[i, chargeIndex] = compundElements.charge;
                }
            }

            return chemicalMatrix;
        }

        public static string[] GetAllCompoundsInEquation(string input)
        {
             return string.Join("", input.Split(' ')).Split("→").Select(element => element.Split('+')).SelectMany(inner => inner).ToArray();
        }

        private static int[] RawSolutionToInt(BigRational[] solution)
        {
            // Turns the solution into coprime whole numbers
            BigRational lcm = LCM(solution.Select(e => (BigInteger)BigRational.NumDen(e).Denumerator).ToArray()),
                        gcd = GCD(solution.Select(e => (BigInteger)BigRational.NumDen(e).Numerator).ToArray());
            BigRational k = lcm / gcd;
            solution = solution.Select(e => e * k).ToArray();

            return solution.Select(e => (int)e).ToArray();
        }

        private static string FormatSolution(int[] solution, string[] compounds)
        {
            List<string> reacants = new List<string>(); 
            List<string> products = new List<string>(); 
            int sign = Math.Sign(solution.Where(e=> e!=0).ToArray()[0]); // The sign of the first non-zero value

            // Goes through every coefficent
            for (int i = 0; i < solution.Length; i++)
            {
                // If the compound doesn't exist in the chemical equation, then skip it
                if(solution[i] == 0) { continue; }

                // If the coefficent is 1, then do not write it
                string coefficent = ((Math.Abs(solution[i]) != 1) ? Math.Abs(solution[i]).ToString() : "");

                // Add the coefficent and compound
                if (Math.Sign(solution[i]) == sign)
                {
                    reacants.Add($"{coefficent}{compounds[i]}");
                }
                else
                {
                    products.Add($"{coefficent}{compounds[i]}");
                }
            }

            return string.Join(" + ", reacants) + " → " + string.Join(" + ", products);
        }

        /// <summary>
        /// Finds all the elements in a compound(s) (charge being added as "Charge")
        /// </summary>
        /// <param name="input">The compund(s) as a string</param>
        /// <returns>A string array with all the symbols of the compund(s)</returns>
        static string[] FindAllElements(string input)
        {
            List<string> result = new List<string>();
            string proposal = input[0].ToString();
            bool usesCharge = false;

            // Goes though every charactor of the input
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i])) // If the character is capitalized then it must be start of a new element
                {
                    // Adds the element to the result if it is not added already
                    if (result.IndexOf(proposal) == -1) { result.Add(proposal); } 

                    proposal = input[i].ToString();
                }
                else if (char.IsLower(input[i])) // If the charactor is not capitalized then it must be the continuation of an element
                {
                    proposal += input[i];
                }
                else if (IsSuperscript(input[i])) // Check for charge
                {
                    usesCharge = true;
                }
            }

            // Add the last element and charge
            if (result.IndexOf(proposal) == -1) { result.Add(proposal); }
            if (usesCharge) { result.Add("Charge"); }

            return result.ToArray();
        }

        /// <summary>
        /// Parses a compound
        /// </summary>
        /// <param name="input">The compound as string</param>
        /// <returns>A struct containing the charge, elements and coefficents for those elements, and the overall compound coefficent</returns>
        public static ChemicalCompound ParseChemcialCompound(string input)
        {
            // Initialization
            ChemicalCompound result = new ChemicalCompound(new List<string>(), new List<double>(), 0, 1, "");

            // Gets the compound symbol
            int k = 0;
            for (; k < input.Length && Char.IsDigit(input[k]); k++) ;
            result.symbol = input.Substring(k);

            // Edgecase 
            if (input.Length == 0) { throw new Exception("Invalid input"); }

            int i = 0;

            // Checks for a coefficent 
            if (char.IsDigit(input[0]))
            {
                result.coefficent = ParseCompoundCoefficent();
            }

            string element = "";
            double elementCoefficent = 1;

            // Goes through the input
            while (i < input.Length)
            {
                // If the current char is the start of a element, then the recent element is parsed, so update, and parse the next element
                if (char.IsUpper(input[i]))
                {
                    element = ParseElement();
                    elementCoefficent = 1;
                    if (i < input.Length)
                    {
                        if (IsSubscript(input[i]))
                        {
                            elementCoefficent = ParseCoefficent();
                        }
                    }

                    UpdateResult(element, elementCoefficent);
                }
                // If there is a start of a subcompound parse it (subcompounds are denoted with "(SUBCOMPOUND)" like Mg(OH)2
                else if (input[i] == '(')
                {
                    // Find the index of the end of the paraentese - the index of ')'
                    int lastIndex;
                    for (lastIndex = input.Length - 1; lastIndex >= 0 && input[lastIndex] != ')'; lastIndex--) ;

                    i++;
                    string rawSubCompound = input.Substring(i, lastIndex - i); // Get the raw subcompound

                    ChemicalCompound subCompound = ParseChemcialCompound(rawSubCompound); // Use recursion to allow for subsubcompunds

                    // Check for a coefficent
                    i = lastIndex + 1;
                    if (i < input.Length)
                    {
                        if (IsSubscript(input[i])) { subCompound.coefficent *= ParseCoefficent(); }
                    }

                    // Update the result
                    for (int j = 0; j < subCompound.elements.Count; j++)
                    {
                        UpdateResult(subCompound.elements[j], subCompound.coefficent * subCompound.coefficents[j]);
                    }
                }
                // Check for a charge
                else if (IsSuperscript(input[i]))
                {
                    result.charge = ParseCharge();
                }
                else
                {
                    throw new Exception("Invalid Input");
                }
            }

            return result;

            int ParseCoefficent()
            {
                // Parses the coefficent from the string
                string rawNum = "";
                for (; IsSubscript(input[i]); i++)
                {
                    rawNum += (char)(input[i] - '₀' + '0');
                    if (i == input.Length - 1) { i++; break; }
                }

                // Converts it to a int
                return Convert.ToInt32(rawNum);
            }

            double ParseCompoundCoefficent()
            {
                // Parses the coefficent from the string
                string rawNum = "";
                for (; char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'; i++)
                {
                    rawNum += input[i];
                    if (i == input.Length - 1) { i++; break; }
                }

                // Converts it to a double
                return Convert.ToDouble(rawNum);
            }

            int ParseCharge()
            {
                string rawNum = "";
                for (; "⁰¹²³⁴⁵⁶⁷⁸⁹".Contains(input[i]); i++)
                {
                    rawNum += (char)("⁰¹²³⁴⁵⁶⁷⁸⁹".IndexOf(input[i]) + '0');
                    if (i == input.Length - 1) { i++; break; }
                }
                int charge = 1;
                if(rawNum.Length != 0) { charge = Convert.ToInt32(rawNum); }

                if(i >= input.Length) { throw new Exception("Superscript needs to be ended with + or -"); }

                if(input[i] == '⁻') { charge *= -1; }
                i++;

                return charge;
            }

            string ParseElement()
            {
                string element = "";

                // Checks if there is an element 
                if (!char.IsUpper(input[i])) { throw new Exception("Invalid input"); }

                element += input[i]; i++; // Gets the first character (which is capitalized)

                if (i == input.Length) { return element; } // Edgecase

                // Gets the rest of the element
                for (; char.IsLower(input[i]); i++)
                {
                    element += input[i];

                    if (i == input.Length - 1) { i++; break; }
                }

                return element;
            }

            void UpdateResult(string element, double coefficent)
            {
                int index = result.elements.IndexOf(element);
                // If the element doesn't exist in the result, then add it
                if (index == -1)
                {
                    result.elements.Add(element);
                    result.coefficents.Add(coefficent);
                }
                // If the element exists in the result, then add the coefficents together
                else
                {
                    result.coefficents[index] += coefficent;
                }
            }
        }

        static bool IsSuperscript(char input)
        {
            return "⁰¹²³⁴⁵⁶⁷⁸⁹⁺⁻".Contains(input);
        }

        static bool IsSubscript(char input)
        {
            return "₀₁₂₃₄₅₆₇₈₉".Contains(input);
        }

        private static BigInteger GCD(BigInteger[] nums)
        {
            BigInteger gcd = nums[0];
            for (int i = 1; i < nums.Length; i++) { gcd = GCD(gcd, nums[i]); }
            return gcd;
        }

        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (b == 0) return a;
            return GCD(b, a % b);
        }

        private static BigInteger LCM(BigInteger[] nums)
        {
            BigInteger lcm = nums[0];
            for(int i = 1; i < nums.Length; i++) { lcm = LCM(lcm, nums[i]); }
            return lcm;

            static BigInteger LCM(BigInteger a, BigInteger b)
            {
                return BigInteger.Abs(a * b / GCD(a, b));
            }
        }

        public static string WriteBigRationalMatrixAsFractions(Matrix<BigRational> matrix)
        {
            string result = "";
            for(int i = 0; i < matrix.rowNumber; i++)
            {
                result += string.Join(", ", matrix.GetRow(i).ReduceToArray().Select(BigRational.NumDen).Select(e => e.Numerator.ToString() + ((e.Denumerator.IsOne)?"": "/" + e.Denumerator.ToString()))) + "\n";
            }
            return result;
        }
    }
}
