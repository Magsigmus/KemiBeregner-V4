using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Linq;

namespace Chemistry
{
    public class PeriodicTable
    {
        public PeriodicTable()
        {
            InitializeTable();
        }

        public struct elementInfo
        {
            public string symbol, name;
            public double molarmass, electronegativity;

            public elementInfo(string symbol, string name, double molarmass, double electronegativity)
            {
                this.symbol = symbol;
                this.name = name;
                this.molarmass = molarmass;
                this.electronegativity = electronegativity;
            }
        }

        public string[] symbols, names;
        public double[] molarmasses, electronegativity;

        public elementInfo this[int i] { get { return new elementInfo(symbols[i], names[i], molarmasses[i], electronegativity[i]); } }

        /// <summary>
        /// Takes the symbol of an element and returns its index
        /// </summary>
        /// <param name="symbol">The symbol of the element</param>
        /// <returns>The index of the element</returns>
        public int IndexOfElemet(string symbol)
        {
            for(int i = 0; i < symbols.Length; i++)
            {
                if(symbols[i] == symbol) { return i; }
            }
            return -1;
        }

        /// <summary>
        /// Initializes the periodicTable
        /// </summary>
        public void InitializeTable()
        {
            // Gets the raw periodic table
            StreamReader molarmassesTxt = new StreamReader(Directory.GetCurrentDirectory() + "\\periodicTable.txt");
            string rawPeriodicTable = molarmassesTxt.ReadToEnd();

            rawPeriodicTable = string.Join("", rawPeriodicTable.Split('\r'));

            string[] lines = rawPeriodicTable.Split('\n');
            names = new string[lines.Length];
            symbols = new string[lines.Length];
            molarmasses = new double[lines.Length];
            electronegativity = new double[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                // Adds the information to the periodic table
                string[] temp = lines[i].Split('\t');
                int index = Convert.ToInt32(temp[2]) - 1;
                symbols[index] = temp[0];
                names[index] = temp[1];
                molarmasses[index] = Convert.ToDouble(temp[3]);
                electronegativity[index] = Convert.ToDouble(temp[4]);
            }
        }
    }
}
