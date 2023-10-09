using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chemistry;

namespace KemiBeregner_V2
{
    public partial class MainForm : Form
    {
        enum textState{
            normal,
            subscript,
            superscript
        }

        struct TableInfo
        {
            public string[] symbols;
            public double[] mass;
            public double[] molarmass;
            public double[] amoutOfSubstance;
            public int[] coefficants;
        }

        TableInfo tableData;

        textState textstate = textState.normal;

        TextBox recentTextBox;

        DataTable data = new DataTable();

        public MainForm()
        {
            InitializeComponent();
            recentTextBox = inputTextBox;
        }

        // Gets called when the superscript button is pressed
        private void SuperscriptButton_Click(object sender, EventArgs e)
        {
            if (textstate == textState.superscript) { textstate = textState.normal; }
            else { textstate = textState.superscript; }
            recentTextBox.Focus();
        }

        // Gets called when the subscript button is pressed
        private void SubscriptButton_Click(object sender, EventArgs e)
        {
            if (textstate == textState.subscript) { textstate = textState.normal; }
            else { textstate = textState.subscript; }
            recentTextBox.Focus();
        }
        
        // Gets called when the arrow button is pressed
        private void ArrowButton_Click(object sender, EventArgs e)
        {
            InsertCharactorInTextBox("→", recentTextBox);
            recentTextBox.Focus();
        }

        // Handles a key down
        bool HandleKeyDown(char inputChar, TextBox text)
        {
            recentTextBox = text;

            char newChar;
            if (textstate == textState.subscript)
            {
                // Converts the key press to subscript
                int val = inputChar - '0';
                if (val < 0 || val > 9) { return false; }
                newChar = (char)(val + '₀');

                // Adds the charactor
                InsertCharactorInTextBox(newChar.ToString(), text);

                return true;
            }
            if (textstate == textState.superscript)
            {
                // Converts the key press to superscript
                if (inputChar == '+') { newChar = '⁺'; }
                else if (inputChar == '-') { newChar = '⁻'; }
                else
                {
                    int val = inputChar - '0';
                    if (val < 0 || val > 9) { return false; }
                    newChar = "⁰¹²³⁴⁵⁶⁷⁸⁹"[val];
                }

                // Adds the charactor
                InsertCharactorInTextBox(newChar.ToString(), text);

                return true;
            }
            return false;
        }

        private void InsertCharactorInTextBox(string val, TextBox text)
        {
            int selectionIndex = text.SelectionStart + 1;
            text.Text = text.Text.Insert(text.SelectionStart, val);
            text.SelectionLength = 0;
            text.SelectionStart = selectionIndex;
        }
       
        // Gets called when a charactor is entered in a textbox
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = HandleKeyDown(e.KeyChar, (TextBox)sender);
        }

        // Gets called just before a charactor is entered in a textbox
        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.Handled = true; OutputTextBox.Text = FindSolution(inputTextBox.Text); }
        }

        void CalculateButton_Click(object sender, EventArgs e)
        {
            OutputTextBox.Text = FindSolution(inputTextBox.Text);
        }

        string FindSolution(string input)
        {
            string solution = ChemicalAnalyser.BalenceChemicalEqation(input);
            if(solution.Contains("//")) { MessageBox.Show("Reaktionsskemaet har en uendelig mængde af løsninger. \nKombiner følgende reaktionsskemaer for at finde det afstemte reaktionsskema"); }
            return solution;
        }

        // Gets called when the table button is pressed
        private void TableButton_Click(object sender, EventArgs e)
        {
            // Gets all the compounds in the equation
            string[] compounds = ChemicalAnalyser.GetAllCompoundsInEquation(OutputTextBox.Text);

            int length = compounds.Length;

            // Initialization
            tableData.symbols = new string[length];
            tableData.mass = new double[length];
            tableData.molarmass = new double[length];
            tableData.amoutOfSubstance = new double[length];
            tableData.coefficants = new int[length];

            // Parses the compounds
            for(int i = 0; i < length; i++)
            {
                ChemicalAnalyser.ChemicalCompound compoundinfo = ChemicalAnalyser.ParseChemcialCompound(compounds[i]);
                tableData.symbols[i] = compoundinfo.symbol;
                tableData.mass[i] = -1;
                tableData.molarmass[i] = ChemicalAnalyser.GetMolarMass(compoundinfo.symbol);
                tableData.amoutOfSubstance[i] = -1;
                tableData.coefficants[i] = (int)compoundinfo.coefficent;
            }

            UpdateDisplayTable(tableData);
        }

        private void UpdateDisplayTable(TableInfo input)
        {
            data = new DataTable();

            int length = input.symbols.Length;
            
            // Initializes all the rows and columns
            for (int i = 0; i < length + 1; i++)
            {
                data.Columns.Add("----" + i.ToString() + "----");
            }

            object[] syms = new object[length + 1];
            object[] displayMass = new object[length + 1];
            object[] displayMolar = new object[length + 1];
            object[] displayAOS = new object[length + 1];
            object[] displayCoefficent = new object[length + 1];
            
            syms[0] = "Kemisk formel";
            displayMass[0] = "Masse (g)";
            displayMolar[0] = "Molarmasse (g/mol)";
            displayAOS[0] = "Stofmængde (mol)";
            displayCoefficent[0] = "Forhold";
            
            // Converts the info from input
            for (int i = 1; i < syms.Length; i++) { syms[i] = input.symbols[i - 1]; }
            for (int i = 1; i < displayMass.Length; i++)
            {
                displayMass[i] = (input.mass[i - 1] != -1) ? Math.Round(input.mass[i - 1], 4).ToString() : "Indsæt værdi";
            }
            for (int i = 1; i < displayMolar.Length; i++)
            {
                displayMolar[i] = Math.Round(input.molarmass[i - 1], 4).ToString();
            }
            for (int i = 1; i < displayAOS.Length; i++)
            {
                displayAOS[i] = (input.mass[i - 1] != -1) ? Math.Round(input.amoutOfSubstance[i - 1], 4).ToString() : "Indsæt værdi";
            }
            for (int i = 1; i < displayCoefficent.Length; i++)
            {
                displayCoefficent[i] = input.coefficants[i - 1].ToString();
            }

            // Updates the table
            data.Rows.Add(syms);
            data.Rows.Add(displayMass);
            data.Rows.Add(displayMolar);
            data.Rows.Add(displayAOS);
            data.Rows.Add(displayCoefficent);

            dataTable.DataSource = data;

            dataTable.Columns[0].Width = 120;
        }

        int rowIndex = 0, columnIndex = 0;
        string value;

        // Is called when a datacell is entered
        private void dataTable_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            value = (string)data.Rows[e.RowIndex][e.ColumnIndex];
        }

        // Is called after the datacell value is updated
        private void dataTable_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((string)data.Rows[e.RowIndex][e.ColumnIndex] != value)
            {
                rowIndex = e.RowIndex;
                columnIndex = e.ColumnIndex;
            }
        }

        // Is called when the amout of substance and mass calculation button is pressed
        private void MassCalculationButton_Click(object sender, EventArgs e)
        {
            double temp;
            if ((rowIndex == 1 || rowIndex == 3) && columnIndex > 0 && double.TryParse((string)data.Rows[rowIndex][columnIndex], out temp))
            {
                tableData.mass = tableData.mass.Select(e => -1d).ToArray();
                tableData.amoutOfSubstance = tableData.amoutOfSubstance.Select(e => -1d).ToArray();

                if (rowIndex == 1)
                {
                    tableData.mass[columnIndex - 1] = Convert.ToDouble((string)data.Rows[rowIndex][columnIndex]);
                    tableData.amoutOfSubstance[columnIndex - 1] = tableData.mass[columnIndex - 1] / tableData.molarmass[columnIndex - 1];
                }

                if (rowIndex == 3)
                {
                    tableData.amoutOfSubstance[columnIndex - 1] = Convert.ToDouble((string)data.Rows[rowIndex][columnIndex]);
                    tableData.mass[columnIndex - 1] = tableData.molarmass[columnIndex - 1] * tableData.amoutOfSubstance[columnIndex - 1];
                }

                double unit = tableData.amoutOfSubstance[columnIndex - 1] / tableData.coefficants[columnIndex - 1];

                for (int i = 0; i < tableData.mass.Length; i++)
                {
                    if (columnIndex - 1 == i) { continue; }
                    tableData.amoutOfSubstance[i] = unit * tableData.coefficants[i];
                    tableData.mass[i] = tableData.amoutOfSubstance[i] * tableData.molarmass[i];
                }

                UpdateDisplayTable(tableData);
            }
        }
    }
}
