using LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chemistry
{
    class ChemicalAnalyser
    {
        private static PeriodicTable table = new PeriodicTable();

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
            string[] compounds; Matrix chemicalMatrix = MakeChemicalMatrix(input, out compounds);

            // gets the reduced echelon form
            chemicalMatrix = chemicalMatrix.reducedEchelonForm;
            
            // Removes every row that contains only zeros
            for (int i = 0; i < chemicalMatrix.yLength; i++)
            {
                bool allZeros = true;
                for (int j = 0; j < chemicalMatrix.xLength; j++)
                {
                    if (chemicalMatrix[j, i] != 0) { allZeros = false; break; }
                }
                if (allZeros) { chemicalMatrix.RemoveRow(i); i--; }
            }

            // Gets the nullity
            int nullity = chemicalMatrix.nullity;

            if (nullity == 0) { return "Ingen løsning"; }

            // Modifices the matrix
            Matrix partMatrix1 = new Matrix(chemicalMatrix.xLength - nullity, nullity);
            partMatrix1.AppendMatrix(Matrix.CreateAnIdentityMatrix(nullity), false);
            chemicalMatrix.AppendMatrix(partMatrix1);

            // Gets the inverse of the matrix
            chemicalMatrix = chemicalMatrix.inverse;

            // Gets the raw solution
            double[] rawSolution = chemicalMatrix.GetColum(chemicalMatrix.xLength - 1).ReduceToArray();

            // Converts that to ints
            int[] solution = RawSolutionToInt(rawSolution);


            //Array.Sort(solution, compounds, IComparer<int>.)

            //Array.Sort(solution, compounds);
            //Array.Reverse(solution); Array.Reverse(compounds);

            return FormatSolution(solution, compounds);
        }

        /// <summary>
        /// Makes a chemical matrix on the basis of a chemical equation
        /// </summary>
        /// <param name="input">The chemical equation as a string</param>
        /// <param name="compounds">The compounds in the equation</param>
        /// <returns>The chemical matrix as a linear algebra matrix</returns>
        public static Matrix MakeChemicalMatrix(string input, out string[] compounds)
        {
            // Gets all the chemical compounds and elements in the reaktionsskema
            compounds = GetAllCompoundsInEquation(input);
            string[] elements = FindAllElements(input);
            ChemicalCompound[] compundInfos = compounds.Select(e => ParseChemcialCompound(e)).ToArray();

            Matrix chemicalMatrix = new Matrix(compundInfos.Length, elements.Length);

            // Goes through every column
            for (int i = 0; i < compundInfos.Length; i++)
            {
                ChemicalCompound compundElements = compundInfos[i];
                // Adds all the coefficents to the chemical matrix
                for (int j = 0; j < compundElements.elements.Count; j++)
                {
                    int index = 0;
                    for (; compundElements.elements[j] != elements[index] && index < elements.Length; index++) ;
                    chemicalMatrix[i, index] = compundElements.coefficents[j];
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

        private static int[] RawSolutionToInt(double[] solution)
        {
            // Converts the raw solution to a workable decimal
            double min = solution.Select(element => Math.Abs(element)).Min();
            solution = solution.Select(element => Math.Round(element / min, 5)).ToArray();

            // Converts the decimals to a integer
            int maxDecimalPoints = solution.Select(e => (e.ToString().IndexOf(',') != -1) ? e.ToString().Length - e.ToString().IndexOf(',') - 1 : 0).Max();
            solution = solution.Select(element => element * Math.Pow(10, maxDecimalPoints)).ToArray();

            // Finds the greatest common divisor
            int gcd = GCD((int)solution[0], (int)solution[1]);
            for (int i = 2; i < solution.Length; i++) { gcd = GCD((int)solution[i], gcd); }

            // Divies with the greatest  common divisor
            solution = solution.Select(element => Math.Round(element / gcd, 5)).ToArray();

            return solution.Select(e => (int)e).ToArray();
        }

        private static string FormatSolution(int[] solution, string[] compounds)
        {
            string writtenSolution = "";

            // Goes through every coefficent
            for (int i = 0; i < solution.Length - 1; i++)
            {
                // If the coefficent is 1, then do not write it
                string coefficent = ((Math.Abs(solution[i]) != 1) ? Math.Abs(solution[i]).ToString() : "");

                // Add the coefficent and compound
                writtenSolution += $"{coefficent}{compounds[i]} ";
                
                // Adds the connecting symbol 
                if (solution[i] > 0 != solution[i + 1] > 0) { writtenSolution += "→ "; }
                else { writtenSolution += "+ "; }
            }

            // Adds the last coefficent and compound
            string lastCoefficent = ((Math.Abs(solution[solution.Length - 1]) != 1) ? Math.Abs(solution[solution.Length - 1]).ToString() : "");
            writtenSolution += $"{lastCoefficent}{compounds[solution.Length - 1]}";

            return writtenSolution;
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
                if (char.IsUpper(input[i])) // If the charactor is capitalized then it must be start of a new element
                {
                    // Adds the element to the result if it is not added already
                    if (result.IndexOf(proposal) == -1) { result.Add(proposal); } 

                    proposal = input[i].ToString();
                }
                else if (char.IsLower(input[i])) // If the charactor is not capitalized then it must be the continuation of an element
                {
                    proposal += input[i];
                }
                else if (IsSubscript(input[i])) // Check for charge
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
                // If there is a start of a subcompound parse it (subcompounds are denoted with "(SUBCOMPOUND)" like Mg(OH)2)
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

                // Converts it to a double
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

        /// <summary>
        /// Gets the molarmass of a compound
        /// </summary>
        /// <param name="compound">The compound as a string</param>
        /// <returns>The molar mass</returns>
        public static double GetMolarMass(string compound)
        {
            ChemicalCompound info = ParseChemcialCompound(compound);
            double[] molarmasses = info.elements.Select(e => table.molarmasses[table.IndexOfElemet(e)]).ToArray();
            double sum = 0;
            for(int i = 0; i < info.elements.Count; i++)
            {
                sum += molarmasses[i] * (double)info.coefficents[i];
            }
            return sum;
        }

        static bool IsSuperscript(char input)
        {
            return "⁰¹²³⁴⁵⁶⁷⁸⁹⁺⁻".Contains(input);
        }

        static bool IsSubscript(char input)
        {
            return "₀₁₂₃₄₅₆₇₈₉".Contains(input);
        }

        private static int GCD(int a, int b)
        {
            if (b == 0) return a;
            return GCD(b, a % b);
        }
    }
}
