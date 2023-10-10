
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
            inputTextBox = new TextBox();
            label1 = new Label();
            SuperscriptButton = new Button();
            SubscriptButton = new Button();
            ArrowButton = new Button();
            OutputTextBox = new TextBox();
            CalculateButton = new Button();
            SuspendLayout();
            // 
            // inputTextBox
            // 
            inputTextBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            inputTextBox.Location = new Point(12, 29);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(621, 25);
            inputTextBox.TabIndex = 0;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
            inputTextBox.KeyPress += textBox_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(304, 17);
            label1.TabIndex = 1;
            label1.Text = "Indskriv dit uafstemte reaktionsskema herunder:";
            // 
            // SuperscriptButton
            // 
            SuperscriptButton.Cursor = Cursors.Hand;
            SuperscriptButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            SuperscriptButton.Location = new Point(11, 60);
            SuperscriptButton.Name = "SuperscriptButton";
            SuperscriptButton.Size = new Size(27, 27);
            SuperscriptButton.TabIndex = 2;
            SuperscriptButton.Text = "x²";
            SuperscriptButton.UseVisualStyleBackColor = true;
            SuperscriptButton.Click += SuperscriptButton_Click;
            // 
            // SubscriptButton
            // 
            SubscriptButton.Cursor = Cursors.Hand;
            SubscriptButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            SubscriptButton.Location = new Point(44, 59);
            SubscriptButton.Name = "SubscriptButton";
            SubscriptButton.Size = new Size(27, 28);
            SubscriptButton.TabIndex = 3;
            SubscriptButton.Text = "x₂";
            SubscriptButton.UseVisualStyleBackColor = true;
            SubscriptButton.Click += SubscriptButton_Click;
            // 
            // ArrowButton
            // 
            ArrowButton.Cursor = Cursors.Hand;
            ArrowButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ArrowButton.Location = new Point(77, 59);
            ArrowButton.Name = "ArrowButton";
            ArrowButton.Size = new Size(27, 28);
            ArrowButton.TabIndex = 4;
            ArrowButton.Text = "→";
            ArrowButton.UseVisualStyleBackColor = true;
            ArrowButton.Click += ArrowButton_Click;
            // 
            // OutputTextBox
            // 
            OutputTextBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            OutputTextBox.Location = new Point(12, 94);
            OutputTextBox.Name = "OutputTextBox";
            OutputTextBox.Size = new Size(621, 25);
            OutputTextBox.TabIndex = 5;
            OutputTextBox.KeyPress += textBox_KeyPress;
            // 
            // CalculateButton
            // 
            CalculateButton.Location = new Point(482, 60);
            CalculateButton.Name = "CalculateButton";
            CalculateButton.Size = new Size(151, 28);
            CalculateButton.TabIndex = 6;
            CalculateButton.Text = "Afstem reaktionsskema";
            CalculateButton.UseVisualStyleBackColor = true;
            CalculateButton.Click += CalculateButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(645, 130);
            Controls.Add(CalculateButton);
            Controls.Add(OutputTextBox);
            Controls.Add(ArrowButton);
            Controls.Add(SubscriptButton);
            Controls.Add(SuperscriptButton);
            Controls.Add(label1);
            Controls.Add(inputTextBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Kemi Beregner";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SuperscriptButton;
        private System.Windows.Forms.Button SubscriptButton;
        private System.Windows.Forms.Button ArrowButton;
        private System.Windows.Forms.TextBox OutputTextBox;
        private System.Windows.Forms.Button CalculateButton;
    }
}

