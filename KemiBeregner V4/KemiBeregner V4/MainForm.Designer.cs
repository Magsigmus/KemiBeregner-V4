
namespace KemiBeregner_V2
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuperscriptButton = new System.Windows.Forms.Button();
            this.SubscriptButton = new System.Windows.Forms.Button();
            this.ArrowButton = new System.Windows.Forms.Button();
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.CalculateButton = new System.Windows.Forms.Button();
            this.dataTable = new System.Windows.Forms.DataGridView();
            this.TableButton = new System.Windows.Forms.Button();
            this.MassCalculationButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // inputTextBox
            // 
            this.inputTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.inputTextBox.Location = new System.Drawing.Point(12, 29);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(621, 25);
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            this.inputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Indskriv dit uafstemte reaktionsskema herunder:";
            // 
            // SuperscriptButton
            // 
            this.SuperscriptButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SuperscriptButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SuperscriptButton.Location = new System.Drawing.Point(11, 60);
            this.SuperscriptButton.Name = "SuperscriptButton";
            this.SuperscriptButton.Size = new System.Drawing.Size(27, 27);
            this.SuperscriptButton.TabIndex = 2;
            this.SuperscriptButton.Text = "x²";
            this.SuperscriptButton.UseVisualStyleBackColor = true;
            this.SuperscriptButton.Click += new System.EventHandler(this.SuperscriptButton_Click);
            // 
            // SubscriptButton
            // 
            this.SubscriptButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SubscriptButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SubscriptButton.Location = new System.Drawing.Point(44, 59);
            this.SubscriptButton.Name = "SubscriptButton";
            this.SubscriptButton.Size = new System.Drawing.Size(27, 28);
            this.SubscriptButton.TabIndex = 3;
            this.SubscriptButton.Text = "x₂";
            this.SubscriptButton.UseVisualStyleBackColor = true;
            this.SubscriptButton.Click += new System.EventHandler(this.SubscriptButton_Click);
            // 
            // ArrowButton
            // 
            this.ArrowButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ArrowButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ArrowButton.Location = new System.Drawing.Point(77, 59);
            this.ArrowButton.Name = "ArrowButton";
            this.ArrowButton.Size = new System.Drawing.Size(27, 28);
            this.ArrowButton.TabIndex = 4;
            this.ArrowButton.Text = "→";
            this.ArrowButton.UseVisualStyleBackColor = true;
            this.ArrowButton.Click += new System.EventHandler(this.ArrowButton_Click);
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.OutputTextBox.Location = new System.Drawing.Point(12, 94);
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.Size = new System.Drawing.Size(621, 25);
            this.OutputTextBox.TabIndex = 5;
            this.OutputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // CalculateButton
            // 
            this.CalculateButton.Location = new System.Drawing.Point(482, 60);
            this.CalculateButton.Name = "CalculateButton";
            this.CalculateButton.Size = new System.Drawing.Size(151, 28);
            this.CalculateButton.TabIndex = 6;
            this.CalculateButton.Text = "Afstem reaktionsskema";
            this.CalculateButton.UseVisualStyleBackColor = true;
            this.CalculateButton.Click += new System.EventHandler(this.CalculateButton_Click);
            // 
            // dataTable
            // 
            this.dataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataTable.Location = new System.Drawing.Point(12, 154);
            this.dataTable.Name = "dataTable";
            this.dataTable.RowTemplate.Height = 25;
            this.dataTable.Size = new System.Drawing.Size(621, 243);
            this.dataTable.TabIndex = 7;
            this.dataTable.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTable_CellEnter);
            this.dataTable.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTable_CellValidated);
            // 
            // TableButton
            // 
            this.TableButton.Location = new System.Drawing.Point(12, 125);
            this.TableButton.Name = "TableButton";
            this.TableButton.Size = new System.Drawing.Size(136, 23);
            this.TableButton.TabIndex = 8;
            this.TableButton.Text = "Lav beregningsskema";
            this.TableButton.UseVisualStyleBackColor = true;
            this.TableButton.Click += new System.EventHandler(this.TableButton_Click);
            // 
            // MassCalculationButton
            // 
            this.MassCalculationButton.Location = new System.Drawing.Point(441, 125);
            this.MassCalculationButton.Name = "MassCalculationButton";
            this.MassCalculationButton.Size = new System.Drawing.Size(192, 23);
            this.MassCalculationButton.TabIndex = 9;
            this.MassCalculationButton.Text = "Udregn masser og stofmængder";
            this.MassCalculationButton.UseVisualStyleBackColor = true;
            this.MassCalculationButton.Click += new System.EventHandler(this.MassCalculationButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 409);
            this.Controls.Add(this.MassCalculationButton);
            this.Controls.Add(this.TableButton);
            this.Controls.Add(this.dataTable);
            this.Controls.Add(this.CalculateButton);
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.ArrowButton);
            this.Controls.Add(this.SubscriptButton);
            this.Controls.Add(this.SuperscriptButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Kemi Beregner";
            ((System.ComponentModel.ISupportInitialize)(this.dataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SuperscriptButton;
        private System.Windows.Forms.Button SubscriptButton;
        private System.Windows.Forms.Button ArrowButton;
        private System.Windows.Forms.TextBox OutputTextBox;
        private System.Windows.Forms.Button CalculateButton;
        private System.Windows.Forms.DataGridView dataTable;
        private System.Windows.Forms.Button TableButton;
        private System.Windows.Forms.Button MassCalculationButton;
    }
}

