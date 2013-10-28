namespace DatabaseFormsTester
{
    partial class Databaseformstester
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TestDatabaseForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TestDatabaseForm
            // 
            this.TestDatabaseForm.Location = new System.Drawing.Point(51, 98);
            this.TestDatabaseForm.Name = "TestDatabaseForm";
            this.TestDatabaseForm.Size = new System.Drawing.Size(75, 23);
            this.TestDatabaseForm.TabIndex = 0;
            this.TestDatabaseForm.Text = "Test";
            this.TestDatabaseForm.UseVisualStyleBackColor = true;
            this.TestDatabaseForm.Click += new System.EventHandler(this.TestDatabaseForm_Click);
            // 
            // Databaseformstester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.TestDatabaseForm);
            this.Name = "Databaseformstester";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TestDatabaseForm;
    }
}

