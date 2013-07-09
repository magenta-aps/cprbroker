namespace CprBroker.Installers
{
    partial class PatchDatabaseForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.loginInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.adminLoginInfo = new CprBroker.Installers.LoginInfo();
            this.databaseNameLabelabel = new System.Windows.Forms.Label();
            this.databaseNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loginInfoGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(392, 346);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(91, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "Next >";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // loginInfoGroupBox
            // 
            this.loginInfoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loginInfoGroupBox.Controls.Add(this.adminLoginInfo);
            this.loginInfoGroupBox.Location = new System.Drawing.Point(16, 193);
            this.loginInfoGroupBox.Name = "loginInfoGroupBox";
            this.loginInfoGroupBox.Size = new System.Drawing.Size(467, 145);
            this.loginInfoGroupBox.TabIndex = 5;
            this.loginInfoGroupBox.TabStop = false;
            this.loginInfoGroupBox.Text = "Login info";
            // 
            // adminLoginInfo
            // 
            this.adminLoginInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.adminLoginInfo.AuthenticationInfo = null;
            this.adminLoginInfo.Location = new System.Drawing.Point(6, 19);
            this.adminLoginInfo.Name = "adminLoginInfo";
            this.adminLoginInfo.Size = new System.Drawing.Size(215, 120);
            this.adminLoginInfo.TabIndex = 1;
            // 
            // databaseNameLabelabel
            // 
            this.databaseNameLabelabel.AutoSize = true;
            this.databaseNameLabelabel.Location = new System.Drawing.Point(16, 106);
            this.databaseNameLabelabel.Name = "databaseNameLabelabel";
            this.databaseNameLabelabel.Size = new System.Drawing.Size(86, 13);
            this.databaseNameLabelabel.TabIndex = 1;
            this.databaseNameLabelabel.Text = "Database name:";
            // 
            // databaseNameTextBox
            // 
            this.databaseNameTextBox.Location = new System.Drawing.Point(108, 103);
            this.databaseNameTextBox.Name = "databaseNameTextBox";
            this.databaseNameTextBox.ReadOnly = true;
            this.databaseNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.databaseNameTextBox.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(295, 346);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // PatchDatabaseForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(495, 388);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.databaseNameTextBox);
            this.Controls.Add(this.databaseNameLabelabel);
            this.Controls.Add(this.loginInfoGroupBox);
            this.Controls.Add(this.okButton);
            this.MaximumSize = new System.Drawing.Size(503, 415);
            this.MinimumSize = new System.Drawing.Size(503, 415);
            this.Name = "PatchDatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patch system database";
            this.Load += new System.EventHandler(this.DropDatabaseForm_Load);
            this.loginInfoGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox loginInfoGroupBox;
        private LoginInfo adminLoginInfo;
        private System.Windows.Forms.Label databaseNameLabelabel;
        private System.Windows.Forms.TextBox databaseNameTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}