namespace CprBroker.Installers.EventBrokerInstallers
{
    partial class ServiceNameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceNameForm));
            this.label1 = new System.Windows.Forms.Label();
            this.serviceNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cprBrokerEventsServiceUrlCustomTextBox = new CprBroker.Installers.CustomTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.databaseNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.serverNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.loginGroupBox = new System.Windows.Forms.GroupBox();
            this.applicationLoginInfo = new CprBroker.Installers.LoginInfo();
            this.cprBrokerEventsServiceUrlLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.serviceNameTextBox.ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cprBrokerEventsServiceUrlCustomTextBox.ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNameTextBox.ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverNameTextBox.ErrorProvider)).BeginInit();
            this.loginGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Service name";
            // 
            // serviceNameTextBox
            // 
            this.serviceNameTextBox.AboveMaximumErrorMessage = null;
            this.serviceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceNameTextBox.BelowMinimumErrorMessage = null;
            this.serviceNameTextBox.Location = new System.Drawing.Point(173, 30);
            this.serviceNameTextBox.MaximumValue = null;
            this.serviceNameTextBox.MaximumValueIncluded = true;
            this.serviceNameTextBox.MaxLength = null;
            this.serviceNameTextBox.MinimumValue = null;
            this.serviceNameTextBox.MinimumValueIncluded = true;
            this.serviceNameTextBox.Name = "serviceNameTextBox";
            this.serviceNameTextBox.Required = true;
            this.serviceNameTextBox.Size = new System.Drawing.Size(310, 20);
            this.serviceNameTextBox.TabIndex = 1;
            this.serviceNameTextBox.Text = "Event broker service";
            this.serviceNameTextBox.ValidationExpression = "\\A\\w.*\\w\\Z";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(295, 346);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(392, 346);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(91, 23);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cprBrokerEventsServiceUrlCustomTextBox
            // 
            this.cprBrokerEventsServiceUrlCustomTextBox.AboveMaximumErrorMessage = null;
            this.cprBrokerEventsServiceUrlCustomTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cprBrokerEventsServiceUrlCustomTextBox.BelowMinimumErrorMessage = null;
            this.cprBrokerEventsServiceUrlCustomTextBox.Location = new System.Drawing.Point(173, 56);
            this.cprBrokerEventsServiceUrlCustomTextBox.MaximumValue = null;
            this.cprBrokerEventsServiceUrlCustomTextBox.MaximumValueIncluded = true;
            this.cprBrokerEventsServiceUrlCustomTextBox.MaxLength = null;
            this.cprBrokerEventsServiceUrlCustomTextBox.MinimumValue = null;
            this.cprBrokerEventsServiceUrlCustomTextBox.MinimumValueIncluded = true;
            this.cprBrokerEventsServiceUrlCustomTextBox.Name = "cprBrokerEventsServiceUrlCustomTextBox";
            this.cprBrokerEventsServiceUrlCustomTextBox.Required = true;
            this.cprBrokerEventsServiceUrlCustomTextBox.Size = new System.Drawing.Size(196, 20);
            this.cprBrokerEventsServiceUrlCustomTextBox.TabIndex = 3;
            this.cprBrokerEventsServiceUrlCustomTextBox.Text = "http://CprBroker";
            this.cprBrokerEventsServiceUrlCustomTextBox.ValidationExpression = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "CPR Broker events interface";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(256, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Database name";
            // 
            // databaseNameTextBox
            // 
            this.databaseNameTextBox.AboveMaximumErrorMessage = null;
            this.databaseNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseNameTextBox.BelowMinimumErrorMessage = null;
            this.databaseNameTextBox.Location = new System.Drawing.Point(344, 127);
            this.databaseNameTextBox.MaximumValue = null;
            this.databaseNameTextBox.MaximumValueIncluded = true;
            this.databaseNameTextBox.MaxLength = null;
            this.databaseNameTextBox.MinimumValue = null;
            this.databaseNameTextBox.MinimumValueIncluded = true;
            this.databaseNameTextBox.Name = "databaseNameTextBox";
            this.databaseNameTextBox.Required = true;
            this.databaseNameTextBox.Size = new System.Drawing.Size(122, 20);
            this.databaseNameTextBox.TabIndex = 7;
            this.databaseNameTextBox.Text = "CprBroker";
            this.databaseNameTextBox.ValidationExpression = null;
            this.databaseNameTextBox.TextChanged += new System.EventHandler(this.databaseNameTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Server name";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.AboveMaximumErrorMessage = null;
            this.serverNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverNameTextBox.BelowMinimumErrorMessage = null;
            this.serverNameTextBox.Location = new System.Drawing.Point(111, 127);
            this.serverNameTextBox.MaximumValue = null;
            this.serverNameTextBox.MaximumValueIncluded = true;
            this.serverNameTextBox.MaxLength = null;
            this.serverNameTextBox.MinimumValue = null;
            this.serverNameTextBox.MinimumValueIncluded = true;
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Required = true;
            this.serverNameTextBox.Size = new System.Drawing.Size(122, 20);
            this.serverNameTextBox.TabIndex = 5;
            this.serverNameTextBox.Text = "localhost";
            this.serverNameTextBox.ValidationExpression = null;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.serverNameTextBox_TextChanged);
            // 
            // loginGroupBox
            // 
            this.loginGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loginGroupBox.Controls.Add(this.applicationLoginInfo);
            this.loginGroupBox.Location = new System.Drawing.Point(26, 153);
            this.loginGroupBox.Name = "loginGroupBox";
            this.loginGroupBox.Size = new System.Drawing.Size(228, 174);
            this.loginGroupBox.TabIndex = 8;
            this.loginGroupBox.TabStop = false;
            this.loginGroupBox.Tag = "";
            this.loginGroupBox.Text = "Login";
            // 
            // applicationLoginInfo
            // 
            this.applicationLoginInfo.AuthenticationInfo = ((CprBroker.Installers.DatabaseSetupInfo.AuthenticationInfo)(resources.GetObject("applicationLoginInfo.AuthenticationInfo")));
            this.applicationLoginInfo.Location = new System.Drawing.Point(6, 19);
            this.applicationLoginInfo.Name = "applicationLoginInfo";
            this.applicationLoginInfo.Size = new System.Drawing.Size(216, 149);
            this.applicationLoginInfo.TabIndex = 9;
            // 
            // cprBrokerEventsServiceUrlLabel
            // 
            this.cprBrokerEventsServiceUrlLabel.AutoSize = true;
            this.cprBrokerEventsServiceUrlLabel.Location = new System.Drawing.Point(375, 59);
            this.cprBrokerEventsServiceUrlLabel.Name = "cprBrokerEventsServiceUrlLabel";
            this.cprBrokerEventsServiceUrlLabel.Size = new System.Drawing.Size(117, 13);
            this.cprBrokerEventsServiceUrlLabel.TabIndex = 12;
            this.cprBrokerEventsServiceUrlLabel.Text = "/Services/Events.asmx";
            // 
            // ServiceNameForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(495, 381);
            this.Controls.Add(this.cprBrokerEventsServiceUrlLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.databaseNameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.loginGroupBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cprBrokerEventsServiceUrlCustomTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.serviceNameTextBox);
            this.Controls.Add(this.label1);
            this.Name = "ServiceNameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Event broker parameters";
            ((System.ComponentModel.ISupportInitialize)(this.serviceNameTextBox.ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cprBrokerEventsServiceUrlCustomTextBox.ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseNameTextBox.ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverNameTextBox.ErrorProvider)).EndInit();
            this.loginGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private CustomTextBox serviceNameTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private CustomTextBox cprBrokerEventsServiceUrlCustomTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private CustomTextBox databaseNameTextBox;
        private System.Windows.Forms.Label label3;
        private CustomTextBox serverNameTextBox;
        private System.Windows.Forms.GroupBox loginGroupBox;
        private LoginInfo applicationLoginInfo;
        private System.Windows.Forms.Label cprBrokerEventsServiceUrlLabel;
    }
}