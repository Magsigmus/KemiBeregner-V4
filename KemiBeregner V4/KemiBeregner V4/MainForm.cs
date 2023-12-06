using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chemistry;

namespace KemiBeregner_V2
{
    public partial class MainForm : Form
    {
        enum textState
        {
            normal,
            subscript,
            superscript
        }

        textState textstate = textState.normal;

        TextBox recentTextBox;

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
            /* Is used to figure out how long the algoritmn takes to calculate the answer
            long sum = 0;

            for(int i = 0; i < 1000; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                ChemicalAnalyser.BalenceChemicalEqation(input);
                sum += sw.ElapsedMilliseconds;
            }

            MessageBox.Show($"Gennemsnitlig tid for at afstemme reaktionsskemaet: {sum / 1000} ms");*/

            string solution = ChemicalAnalyser.BalenceChemicalEqation(input);
            if (solution.Contains("//")) { MessageBox.Show("Reaktionsskemaet har en uendelig mængde af løsninger. \nKombiner følgende reaktionsskemaer for at finde det afstemte reaktionsskema."); }
            return solution;
        }
    }
}
