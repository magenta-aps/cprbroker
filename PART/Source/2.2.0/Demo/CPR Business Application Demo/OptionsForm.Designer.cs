namespace CPR_Business_Application_Demo
{
    partial class OptionsForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.connectionTabPage = new System.Windows.Forms.TabPage();
            this.eventBrokerWebServiceUrlTextBox = new System.Windows.Forms.TextBox();
            this.appRegistrationGroupBox = new System.Windows.Forms.GroupBox();
            this.CPRBrokerLogPage = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.adminAppTokenTextBox = new System.Windows.Forms.TextBox();
            this.appRegistrationLabel = new System.Windows.Forms.Label();
            this.registerApplicationButton = new System.Windows.Forms.Button();
            this.testConnectionButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cprBrokerWebServiceUrlTextBox = new System.Windows.Forms.TextBox();
            this.notificationsTabPage = new System.Windows.Forms.TabPage();
            this.notificationModeDisabledRadioButton = new System.Windows.Forms.RadioButton();
            this.fileShareGroupBox = new System.Windows.Forms.GroupBox();
            this.fileShareBrowseButton = new System.Windows.Forms.Button();
            this.notificationFileShareTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.notificationModeFileShareRadioButton = new System.Windows.Forms.RadioButton();
            this.notificationModeCallbackWebServiceRadioButton = new System.Windows.Forms.RadioButton();
            this.callbackWebServiceGroupBox = new System.Windows.Forms.GroupBox();
            this.notificationCallBackWebServiceUrlTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.userTokenTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.mainTabControl.SuspendLayout();
            this.connectionTabPage.SuspendLayout();
            this.appRegistrationGroupBox.SuspendLayout();
            this.notificationsTabPage.SuspendLayout();
            this.fileShareGroupBox.SuspendLayout();
            this.callbackWebServiceGroupBox.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(333, 474);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(414, 474);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.connectionTabPage);
            this.mainTabControl.Controls.Add(this.notificationsTabPage);
            this.mainTabControl.Controls.Add(this.generalTabPage);
            this.mainTabControl.Location = new System.Drawing.Point(12, 12);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(559, 456);
            this.mainTabControl.TabIndex = 2;
            // 
            // connectionTabPage
            // 
            this.connectionTabPage.Controls.Add(this.label7);
            this.connectionTabPage.Controls.Add(this.eventBrokerWebServiceUrlTextBox);
            this.connectionTabPage.Controls.Add(this.appRegistrationGroupBox);
            this.connectionTabPage.Controls.Add(this.testConnectionButton);
            this.connectionTabPage.Controls.Add(this.label2);
            this.connectionTabPage.Controls.Add(this.label1);
            this.connectionTabPage.Controls.Add(this.cprBrokerWebServiceUrlTextBox);
            this.connectionTabPage.Location = new System.Drawing.Point(4, 22);
            this.connectionTabPage.Name = "connectionTabPage";
            this.connectionTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.connectionTabPage.Size = new System.Drawing.Size(551, 430);
            this.connectionTabPage.TabIndex = 1;
            this.connectionTabPage.Text = "Connection";
            this.connectionTabPage.UseVisualStyleBackColor = true;
            // 
            // eventBrokerWebServiceUrlTextBox
            // 
            this.eventBrokerWebServiceUrlTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CPR_Business_Application_Demo.Properties.Settings.Default, "EventBrokerWebServiceUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.eventBrokerWebServiceUrlTextBox.Location = new System.Drawing.Point(9, 127);
            this.eventBrokerWebServiceUrlTextBox.Name = "eventBrokerWebServiceUrlTextBox";
            this.eventBrokerWebServiceUrlTextBox.Size = new System.Drawing.Size(311, 20);
            this.eventBrokerWebServiceUrlTextBox.TabIndex = 5;
            this.eventBrokerWebServiceUrlTextBox.Text = global::CPR_Business_Application_Demo.Properties.Settings.Default.EventBrokerWebServiceUrl;
            // 
            // appRegistrationGroupBox
            // 
            this.appRegistrationGroupBox.Controls.Add(this.CPRBrokerLogPage);
            this.appRegistrationGroupBox.Controls.Add(this.label8);
            this.appRegistrationGroupBox.Controls.Add(this.label6);
            this.appRegistrationGroupBox.Controls.Add(this.adminAppTokenTextBox);
            this.appRegistrationGroupBox.Controls.Add(this.appRegistrationLabel);
            this.appRegistrationGroupBox.Controls.Add(this.registerApplicationButton);
            this.appRegistrationGroupBox.Enabled = false;
            this.appRegistrationGroupBox.Location = new System.Drawing.Point(9, 173);
            this.appRegistrationGroupBox.Name = "appRegistrationGroupBox";
            this.appRegistrationGroupBox.Size = new System.Drawing.Size(436, 211);
            this.appRegistrationGroupBox.TabIndex = 4;
            this.appRegistrationGroupBox.TabStop = false;
            this.appRegistrationGroupBox.Text = "Application Registration";
            // 
            // CPRBrokerLogPage
            // 
            this.CPRBrokerLogPage.AutoSize = true;
            this.CPRBrokerLogPage.Location = new System.Drawing.Point(89, 236);
            this.CPRBrokerLogPage.Name = "CPRBrokerLogPage";
            this.CPRBrokerLogPage.Size = new System.Drawing.Size(81, 13);
            this.CPRBrokerLogPage.TabIndex = 5;
            this.CPRBrokerLogPage.TabStop = true;
            this.CPRBrokerLogPage.Text = "CPR Broker Log";
            this.CPRBrokerLogPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CPRBrokerLogPage_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(77, 138);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Registration status:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Admin-application-token";
            // 
            // adminAppTokenTextBox
            // 
            this.adminAppTokenTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CPR_Business_Application_Demo.Properties.Settings.Default, "AdminAppToken", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.adminAppTokenTextBox.Location = new System.Drawing.Point(27, 39);
            this.adminAppTokenTextBox.Name = "adminAppTokenTextBox";
            this.adminAppTokenTextBox.Size = new System.Drawing.Size(382, 20);
            this.adminAppTokenTextBox.TabIndex = 2;
            this.adminAppTokenTextBox.Text = global::CPR_Business_Application_Demo.Properties.Settings.Default.AdminAppToken;
            // 
            // appRegistrationLabel
            // 
            this.appRegistrationLabel.Location = new System.Drawing.Point(93, 151);
            this.appRegistrationLabel.Name = "appRegistrationLabel";
            this.appRegistrationLabel.Size = new System.Drawing.Size(316, 75);
            this.appRegistrationLabel.TabIndex = 1;
            this.appRegistrationLabel.Text = "Application not yet registered.";
            // 
            // registerApplicationButton
            // 
            this.registerApplicationButton.Location = new System.Drawing.Point(96, 84);
            this.registerApplicationButton.Name = "registerApplicationButton";
            this.registerApplicationButton.Size = new System.Drawing.Size(237, 37);
            this.registerApplicationButton.TabIndex = 0;
            this.registerApplicationButton.Text = "Register and approve Application with Web Service";
            this.registerApplicationButton.UseVisualStyleBackColor = true;
            this.registerApplicationButton.Click += new System.EventHandler(this.registerApplicationButton_Click);
            // 
            // testConnectionButton
            // 
            this.testConnectionButton.Location = new System.Drawing.Point(354, 76);
            this.testConnectionButton.Name = "testConnectionButton";
            this.testConnectionButton.Size = new System.Drawing.Size(91, 71);
            this.testConnectionButton.TabIndex = 2;
            this.testConnectionButton.Text = "Test connection";
            this.testConnectionButton.UseVisualStyleBackColor = true;
            this.testConnectionButton.Click += new System.EventHandler(this.testConnectionButton_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(439, 35);
            this.label2.TabIndex = 2;
            this.label2.Text = "Note: This is the base URL in which all CPR web services reside. Use \"Test connec" +
                "tion\" to enable Application Registration";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CPR broker services base URL:";
            // 
            // cprBrokerWebServiceUrlTextBox
            // 
            this.cprBrokerWebServiceUrlTextBox.Location = new System.Drawing.Point(9, 76);
            this.cprBrokerWebServiceUrlTextBox.Name = "cprBrokerWebServiceUrlTextBox";
            this.cprBrokerWebServiceUrlTextBox.Size = new System.Drawing.Size(311, 20);
            this.cprBrokerWebServiceUrlTextBox.TabIndex = 1;
            this.cprBrokerWebServiceUrlTextBox.Text = global::CPR_Business_Application_Demo.Properties.Settings.Default.CPRBrokerWebServiceUrl;
            // 
            // notificationsTabPage
            // 
            this.notificationsTabPage.Controls.Add(this.notificationModeDisabledRadioButton);
            this.notificationsTabPage.Controls.Add(this.fileShareGroupBox);
            this.notificationsTabPage.Controls.Add(this.notificationModeFileShareRadioButton);
            this.notificationsTabPage.Controls.Add(this.notificationModeCallbackWebServiceRadioButton);
            this.notificationsTabPage.Controls.Add(this.callbackWebServiceGroupBox);
            this.notificationsTabPage.Location = new System.Drawing.Point(4, 22);
            this.notificationsTabPage.Name = "notificationsTabPage";
            this.notificationsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.notificationsTabPage.Size = new System.Drawing.Size(551, 430);
            this.notificationsTabPage.TabIndex = 2;
            this.notificationsTabPage.Text = "Notifications";
            this.notificationsTabPage.UseVisualStyleBackColor = true;
            // 
            // notificationModeDisabledRadioButton
            // 
            this.notificationModeDisabledRadioButton.AutoSize = true;
            this.notificationModeDisabledRadioButton.Checked = true;
            this.notificationModeDisabledRadioButton.Location = new System.Drawing.Point(6, 27);
            this.notificationModeDisabledRadioButton.Name = "notificationModeDisabledRadioButton";
            this.notificationModeDisabledRadioButton.Size = new System.Drawing.Size(65, 17);
            this.notificationModeDisabledRadioButton.TabIndex = 4;
            this.notificationModeDisabledRadioButton.TabStop = true;
            this.notificationModeDisabledRadioButton.Text = "Disabled";
            this.notificationModeDisabledRadioButton.UseVisualStyleBackColor = true;
            this.notificationModeDisabledRadioButton.CheckedChanged += new System.EventHandler(this.notificationModeDisabledRadioButton_CheckedChanged);
            // 
            // fileShareGroupBox
            // 
            this.fileShareGroupBox.Controls.Add(this.fileShareBrowseButton);
            this.fileShareGroupBox.Controls.Add(this.notificationFileShareTextBox);
            this.fileShareGroupBox.Controls.Add(this.label5);
            this.fileShareGroupBox.Enabled = false;
            this.fileShareGroupBox.Location = new System.Drawing.Point(26, 255);
            this.fileShareGroupBox.Name = "fileShareGroupBox";
            this.fileShareGroupBox.Size = new System.Drawing.Size(519, 169);
            this.fileShareGroupBox.TabIndex = 3;
            this.fileShareGroupBox.TabStop = false;
            this.fileShareGroupBox.Text = "File Share";
            // 
            // fileShareBrowseButton
            // 
            this.fileShareBrowseButton.Location = new System.Drawing.Point(438, 30);
            this.fileShareBrowseButton.Name = "fileShareBrowseButton";
            this.fileShareBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.fileShareBrowseButton.TabIndex = 2;
            this.fileShareBrowseButton.Text = "Browse...";
            this.fileShareBrowseButton.UseVisualStyleBackColor = true;
            this.fileShareBrowseButton.Click += new System.EventHandler(this.fileShareBrowseButton_Click);
            // 
            // notificationFileShareTextBox
            // 
            this.notificationFileShareTextBox.Location = new System.Drawing.Point(109, 30);
            this.notificationFileShareTextBox.Name = "notificationFileShareTextBox";
            this.notificationFileShareTextBox.Size = new System.Drawing.Size(323, 20);
            this.notificationFileShareTextBox.TabIndex = 1;
            this.notificationFileShareTextBox.Text = global::CPR_Business_Application_Demo.Properties.Settings.Default.NotificationFileShare;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "File Share:";
            // 
            // notificationModeFileShareRadioButton
            // 
            this.notificationModeFileShareRadioButton.AutoSize = true;
            this.notificationModeFileShareRadioButton.Location = new System.Drawing.Point(6, 255);
            this.notificationModeFileShareRadioButton.Name = "notificationModeFileShareRadioButton";
            this.notificationModeFileShareRadioButton.Size = new System.Drawing.Size(14, 13);
            this.notificationModeFileShareRadioButton.TabIndex = 2;
            this.notificationModeFileShareRadioButton.UseVisualStyleBackColor = true;
            this.notificationModeFileShareRadioButton.CheckedChanged += new System.EventHandler(this.notificationModeFileShareRadioButton_CheckedChanged);
            // 
            // notificationModeCallbackWebServiceRadioButton
            // 
            this.notificationModeCallbackWebServiceRadioButton.AutoSize = true;
            this.notificationModeCallbackWebServiceRadioButton.Location = new System.Drawing.Point(6, 66);
            this.notificationModeCallbackWebServiceRadioButton.Name = "notificationModeCallbackWebServiceRadioButton";
            this.notificationModeCallbackWebServiceRadioButton.Size = new System.Drawing.Size(14, 13);
            this.notificationModeCallbackWebServiceRadioButton.TabIndex = 1;
            this.notificationModeCallbackWebServiceRadioButton.UseVisualStyleBackColor = true;
            this.notificationModeCallbackWebServiceRadioButton.CheckedChanged += new System.EventHandler(this.notificationModeCallbackWebServiceRadioButton_CheckedChanged);
            // 
            // callbackWebServiceGroupBox
            // 
            this.callbackWebServiceGroupBox.Controls.Add(this.notificationCallBackWebServiceUrlTextBox);
            this.callbackWebServiceGroupBox.Controls.Add(this.label4);
            this.callbackWebServiceGroupBox.Enabled = false;
            this.callbackWebServiceGroupBox.Location = new System.Drawing.Point(26, 66);
            this.callbackWebServiceGroupBox.Name = "callbackWebServiceGroupBox";
            this.callbackWebServiceGroupBox.Size = new System.Drawing.Size(519, 160);
            this.callbackWebServiceGroupBox.TabIndex = 0;
            this.callbackWebServiceGroupBox.TabStop = false;
            this.callbackWebServiceGroupBox.Text = "Callback Web Service";
            // 
            // notificationCallBackWebServiceUrlTextBox
            // 
            this.notificationCallBackWebServiceUrlTextBox.Location = new System.Drawing.Point(109, 30);
            this.notificationCallBackWebServiceUrlTextBox.Name = "notificationCallBackWebServiceUrlTextBox";
            this.notificationCallBackWebServiceUrlTextBox.Size = new System.Drawing.Size(404, 20);
            this.notificationCallBackWebServiceUrlTextBox.TabIndex = 1;
            this.notificationCallBackWebServiceUrlTextBox.Text = global::CPR_Business_Application_Demo.Properties.Settings.Default.NotificationCallbackWebServiceUrl;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Web Service URL:";
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.userTokenTextBox);
            this.generalTabPage.Controls.Add(this.label3);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(551, 430);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "User Token";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // userTokenTextBox
            // 
            this.userTokenTextBox.Location = new System.Drawing.Point(194, 15);
            this.userTokenTextBox.Name = "userTokenTextBox";
            this.userTokenTextBox.Size = new System.Drawing.Size(351, 20);
            this.userTokenTextBox.TabIndex = 1;
            this.userTokenTextBox.Text = global::CPR_Business_Application_Demo.Properties.Settings.Default.UserToken;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "User Token:";
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(496, 474);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Event broker services base URL:";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 508);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "OptionsForm";
            this.Text = "OptionsForm";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.Shown += new System.EventHandler(this.OptionsForm_Shown);
            this.mainTabControl.ResumeLayout(false);
            this.connectionTabPage.ResumeLayout(false);
            this.connectionTabPage.PerformLayout();
            this.appRegistrationGroupBox.ResumeLayout(false);
            this.appRegistrationGroupBox.PerformLayout();
            this.notificationsTabPage.ResumeLayout(false);
            this.notificationsTabPage.PerformLayout();
            this.fileShareGroupBox.ResumeLayout(false);
            this.fileShareGroupBox.PerformLayout();
            this.callbackWebServiceGroupBox.ResumeLayout(false);
            this.callbackWebServiceGroupBox.PerformLayout();
            this.generalTabPage.ResumeLayout(false);
            this.generalTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.TabPage connectionTabPage;
        private System.Windows.Forms.Button testConnectionButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox cprBrokerWebServiceUrlTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox userTokenTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage notificationsTabPage;
        private System.Windows.Forms.GroupBox callbackWebServiceGroupBox;
        private System.Windows.Forms.RadioButton notificationModeCallbackWebServiceRadioButton;
        private System.Windows.Forms.RadioButton notificationModeFileShareRadioButton;
        private System.Windows.Forms.GroupBox fileShareGroupBox;
        private System.Windows.Forms.Button fileShareBrowseButton;
        private System.Windows.Forms.TextBox notificationFileShareTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox notificationCallBackWebServiceUrlTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton notificationModeDisabledRadioButton;
        private System.Windows.Forms.GroupBox appRegistrationGroupBox;
        private System.Windows.Forms.Label appRegistrationLabel;
        private System.Windows.Forms.Button registerApplicationButton;
        private System.Windows.Forms.TextBox adminAppTokenTextBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel CPRBrokerLogPage;
        private System.Windows.Forms.TextBox eventBrokerWebServiceUrlTextBox;
        private System.Windows.Forms.Label label7;
    }
}