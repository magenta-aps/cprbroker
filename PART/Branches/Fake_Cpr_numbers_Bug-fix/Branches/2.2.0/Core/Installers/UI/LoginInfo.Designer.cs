namespace CprBroker.Installers
{
    partial class LoginInfo
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loginInfoPanel = new System.Windows.Forms.Panel();
            this.passwordTextBox = new CprBroker.Installers.CustomTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.userIdTextBox = new CprBroker.Installers.CustomTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.windowsAuthenticationRadio = new System.Windows.Forms.RadioButton();
            this.sqlAuthenticationRadio = new System.Windows.Forms.RadioButton();
            this.loginInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.passwordTextBox.ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userIdTextBox.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // loginInfoPanel
            // 
            this.loginInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loginInfoPanel.Controls.Add(this.passwordTextBox);
            this.loginInfoPanel.Controls.Add(this.label3);
            this.loginInfoPanel.Controls.Add(this.userIdTextBox);
            this.loginInfoPanel.Controls.Add(this.label2);
            this.loginInfoPanel.Enabled = false;
            this.loginInfoPanel.Location = new System.Drawing.Point(19, 58);
            this.loginInfoPanel.Name = "loginInfoPanel";
            this.loginInfoPanel.Size = new System.Drawing.Size(209, 60);
            this.loginInfoPanel.TabIndex = 15;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.AboveMaximumErrorMessage = null;
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.BelowMinimumErrorMessage = null;
            this.passwordTextBox.Location = new System.Drawing.Point(61, 39);
            this.passwordTextBox.MaximumValue = null;
            this.passwordTextBox.MaximumValueIncluded = true;
            this.passwordTextBox.MaxLength = null;
            this.passwordTextBox.MinimumValue = null;
            this.passwordTextBox.MinimumValueIncluded = true;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Required = true;
            this.passwordTextBox.Size = new System.Drawing.Size(118, 20);
            this.passwordTextBox.TabIndex = 13;
            this.passwordTextBox.ValidationExpression = null;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.passwordTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Password";
            // 
            // userIdTextBox
            // 
            this.userIdTextBox.AboveMaximumErrorMessage = null;
            this.userIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userIdTextBox.BelowMinimumErrorMessage = null;
            this.userIdTextBox.Location = new System.Drawing.Point(61, 8);
            this.userIdTextBox.MaximumValue = null;
            this.userIdTextBox.MaximumValueIncluded = true;
            this.userIdTextBox.MaxLength = null;
            this.userIdTextBox.MinimumValue = null;
            this.userIdTextBox.MinimumValueIncluded = true;
            this.userIdTextBox.Name = "userIdTextBox";
            this.userIdTextBox.Required = true;
            this.userIdTextBox.Size = new System.Drawing.Size(118, 20);
            this.userIdTextBox.TabIndex = 11;
            this.userIdTextBox.ValidationExpression = null;
            this.userIdTextBox.TextChanged += new System.EventHandler(this.userIdTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "User ID";
            // 
            // windowsAuthenticationRadio
            // 
            this.windowsAuthenticationRadio.AutoSize = true;
            this.windowsAuthenticationRadio.Checked = true;
            this.windowsAuthenticationRadio.Location = new System.Drawing.Point(3, 2);
            this.windowsAuthenticationRadio.Name = "windowsAuthenticationRadio";
            this.windowsAuthenticationRadio.Size = new System.Drawing.Size(140, 17);
            this.windowsAuthenticationRadio.TabIndex = 13;
            this.windowsAuthenticationRadio.TabStop = true;
            this.windowsAuthenticationRadio.Tag = "";
            this.windowsAuthenticationRadio.Text = "Windows authentication";
            this.windowsAuthenticationRadio.UseVisualStyleBackColor = true;
            this.windowsAuthenticationRadio.CheckedChanged += new System.EventHandler(this.authenticationRadio_CheckedChanged);
            // 
            // sqlAuthenticationRadio
            // 
            this.sqlAuthenticationRadio.AutoSize = true;
            this.sqlAuthenticationRadio.Location = new System.Drawing.Point(3, 25);
            this.sqlAuthenticationRadio.Name = "sqlAuthenticationRadio";
            this.sqlAuthenticationRadio.Size = new System.Drawing.Size(151, 17);
            this.sqlAuthenticationRadio.TabIndex = 14;
            this.sqlAuthenticationRadio.TabStop = true;
            this.sqlAuthenticationRadio.Text = "SQL Server authentication";
            this.sqlAuthenticationRadio.UseVisualStyleBackColor = true;
            this.sqlAuthenticationRadio.CheckedChanged += new System.EventHandler(this.authenticationRadio_CheckedChanged);
            // 
            // LoginInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loginInfoPanel);
            this.Controls.Add(this.windowsAuthenticationRadio);
            this.Controls.Add(this.sqlAuthenticationRadio);
            this.Name = "LoginInfo";
            this.Size = new System.Drawing.Size(230, 121);
            this.loginInfoPanel.ResumeLayout(false);
            this.loginInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.passwordTextBox.ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userIdTextBox.ErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel loginInfoPanel;
        private CustomTextBox passwordTextBox;
        private System.Windows.Forms.Label label3;
        private CustomTextBox userIdTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton windowsAuthenticationRadio;
        private System.Windows.Forms.RadioButton sqlAuthenticationRadio;
    }
}
