namespace CprBroker.Installers
{
    partial class DatabaseForm
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
            CprBroker.Installers.DatabaseSetupInfo.AuthenticationInfo authenticationInfo1 = new CprBroker.Installers.DatabaseSetupInfo.AuthenticationInfo();
            CprBroker.Installers.DatabaseSetupInfo.AuthenticationInfo authenticationInfo2 = new CprBroker.Installers.DatabaseSetupInfo.AuthenticationInfo();
            this.adminLoginGroupBox = new System.Windows.Forms.GroupBox();
            this.adminLoginInfo = new CprBroker.Installers.LoginInfo();
            this.testAdminConnectionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.applicationLoginGroupBox = new System.Windows.Forms.GroupBox();
            this.sameAsAdminCheckBox = new System.Windows.Forms.CheckBox();
            this.applicationLoginInfo = new CprBroker.Installers.LoginInfo();
            this.databaseNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.serverNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.adminLoginGroupBox.SuspendLayout();
            this.applicationLoginGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNameTextBox.ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverNameTextBox.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // adminLoginGroupBox
            // 
            this.adminLoginGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.adminLoginGroupBox.Controls.Add(this.adminLoginInfo);
            this.adminLoginGroupBox.Location = new System.Drawing.Point(12, 168);
            this.adminLoginGroupBox.Name = "adminLoginGroupBox";
            this.adminLoginGroupBox.Size = new System.Drawing.Size(228, 174);
            this.adminLoginGroupBox.TabIndex = 6;
            this.adminLoginGroupBox.TabStop = false;
            this.adminLoginGroupBox.Tag = "";
            this.adminLoginGroupBox.Text = "Admin login";
            // 
            // adminLoginInfo
            // 
            this.adminLoginInfo.AuthenticationInfo = authenticationInfo1;
            this.adminLoginInfo.Location = new System.Drawing.Point(7, 17);
            this.adminLoginInfo.Name = "adminLoginInfo";
            this.adminLoginInfo.Size = new System.Drawing.Size(215, 127);
            this.adminLoginInfo.TabIndex = 0;
            // 
            // testAdminConnectionButton
            // 
            this.testAdminConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.testAdminConnectionButton.CausesValidation = false;
            this.testAdminConnectionButton.Location = new System.Drawing.Point(12, 348);
            this.testAdminConnectionButton.Name = "testAdminConnectionButton";
            this.testAdminConnectionButton.Size = new System.Drawing.Size(97, 23);
            this.testAdminConnectionButton.TabIndex = 8;
            this.testAdminConnectionButton.Text = "Test connection";
            this.testAdminConnectionButton.UseVisualStyleBackColor = true;
            this.testAdminConnectionButton.Click += new System.EventHandler(this.testConnectionButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Database name";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(392, 348);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(91, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "Next >";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(240, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "The setup will now create the system\'s database";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(216, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Please enter the database information here";
            // 
            // applicationLoginGroupBox
            // 
            this.applicationLoginGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationLoginGroupBox.Controls.Add(this.sameAsAdminCheckBox);
            this.applicationLoginGroupBox.Controls.Add(this.applicationLoginInfo);
            this.applicationLoginGroupBox.Location = new System.Drawing.Point(255, 168);
            this.applicationLoginGroupBox.Name = "applicationLoginGroupBox";
            this.applicationLoginGroupBox.Size = new System.Drawing.Size(228, 174);
            this.applicationLoginGroupBox.TabIndex = 7;
            this.applicationLoginGroupBox.TabStop = false;
            this.applicationLoginGroupBox.Tag = "";
            this.applicationLoginGroupBox.Text = "Application login";
            // 
            // sameAsAdminCheckBox
            // 
            this.sameAsAdminCheckBox.AutoSize = true;
            this.sameAsAdminCheckBox.Location = new System.Drawing.Point(7, 20);
            this.sameAsAdminCheckBox.Name = "sameAsAdminCheckBox";
            this.sameAsAdminCheckBox.Size = new System.Drawing.Size(105, 17);
            this.sameAsAdminCheckBox.TabIndex = 0;
            this.sameAsAdminCheckBox.Text = "(Same as admin)";
            this.sameAsAdminCheckBox.UseVisualStyleBackColor = true;
            this.sameAsAdminCheckBox.CheckedChanged += new System.EventHandler(this.sameAsAdminCheckBox_CheckedChanged);
            // 
            // applicationLoginInfo
            // 
            this.applicationLoginInfo.AuthenticationInfo = authenticationInfo2;
            this.applicationLoginInfo.Enabled = false;
            this.applicationLoginInfo.Location = new System.Drawing.Point(6, 43);
            this.applicationLoginInfo.Name = "applicationLoginInfo";
            this.applicationLoginInfo.Size = new System.Drawing.Size(216, 127);
            this.applicationLoginInfo.TabIndex = 1;
            // 
            // databaseNameTextBox
            // 
            this.databaseNameTextBox.AboveMaximumErrorMessage = null;
            this.databaseNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseNameTextBox.BelowMinimumErrorMessage = null;
            this.databaseNameTextBox.Location = new System.Drawing.Point(104, 138);
            this.databaseNameTextBox.MaximumValue = null;
            this.databaseNameTextBox.MaximumValueIncluded = true;
            this.databaseNameTextBox.MaxLength = null;
            this.databaseNameTextBox.MinimumValue = null;
            this.databaseNameTextBox.MinimumValueIncluded = true;
            this.databaseNameTextBox.Name = "databaseNameTextBox";
            this.databaseNameTextBox.Required = true;
            this.databaseNameTextBox.Size = new System.Drawing.Size(122, 20);
            this.databaseNameTextBox.TabIndex = 5;
            this.databaseNameTextBox.Text = "CprBroker";
            this.databaseNameTextBox.ValidationExpression = null;
            this.databaseNameTextBox.TextChanged += new System.EventHandler(this.databaseNameTextBox_TextChanged);
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.AboveMaximumErrorMessage = null;
            this.serverNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverNameTextBox.BelowMinimumErrorMessage = null;
            this.serverNameTextBox.Location = new System.Drawing.Point(104, 112);
            this.serverNameTextBox.MaximumValue = null;
            this.serverNameTextBox.MaximumValueIncluded = true;
            this.serverNameTextBox.MaxLength = null;
            this.serverNameTextBox.MinimumValue = null;
            this.serverNameTextBox.MinimumValueIncluded = true;
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Required = true;
            this.serverNameTextBox.Size = new System.Drawing.Size(122, 20);
            this.serverNameTextBox.TabIndex = 3;
            this.serverNameTextBox.Text = "localhost";
            this.serverNameTextBox.ValidationExpression = null;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.serverNameTextBox_TextChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(295, 348);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // DatabaseForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(495, 381);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applicationLoginGroupBox);
            this.Controls.Add(this.testAdminConnectionButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.databaseNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.adminLoginGroupBox);
            this.Name = "DatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPR Broker database setup";
            this.Load += new System.EventHandler(this.DatabaseForm_Load);
            this.adminLoginGroupBox.ResumeLayout(false);
            this.applicationLoginGroupBox.ResumeLayout(false);
            this.applicationLoginGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNameTextBox.ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverNameTextBox.ErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox adminLoginGroupBox;
        private CprBroker.Installers.CustomTextBox serverNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private CprBroker.Installers.CustomTextBox databaseNameTextBox;
        private System.Windows.Forms.Button testAdminConnectionButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox applicationLoginGroupBox;
        private LoginInfo adminLoginInfo;
        private LoginInfo applicationLoginInfo;
        private System.Windows.Forms.CheckBox sameAsAdminCheckBox;
        private System.Windows.Forms.Button cancelButton;
    }
}