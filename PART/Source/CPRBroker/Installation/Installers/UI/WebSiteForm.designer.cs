namespace CprBroker.Installers
{
    partial class WebSiteForm
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
            this.virtualDirectoryRadio = new System.Windows.Forms.RadioButton();
            this.newWebSiteRadio = new System.Windows.Forms.RadioButton();
            this.sitesComboBox = new System.Windows.Forms.ComboBox();
            this.newWebSiteNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.virtualDirectoryNameTextBox = new CprBroker.Installers.CustomTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.virtualDirectoryGroupBox = new System.Windows.Forms.GroupBox();
            this.newWebSiteGroupBox = new System.Windows.Forms.GroupBox();
            this.websitePanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.newWebSiteNameTextBox.ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.virtualDirectoryNameTextBox.ErrorProvider)).BeginInit();
            this.virtualDirectoryGroupBox.SuspendLayout();
            this.newWebSiteGroupBox.SuspendLayout();
            this.websitePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // virtualDirectoryRadio
            // 
            this.virtualDirectoryRadio.AutoSize = true;
            this.virtualDirectoryRadio.Checked = true;
            this.virtualDirectoryRadio.Location = new System.Drawing.Point(12, 87);
            this.virtualDirectoryRadio.Name = "virtualDirectoryRadio";
            this.virtualDirectoryRadio.Size = new System.Drawing.Size(260, 17);
            this.virtualDirectoryRadio.TabIndex = 0;
            this.virtualDirectoryRadio.TabStop = true;
            this.virtualDirectoryRadio.Text = "Create as virtual directory in an existing web site";
            this.virtualDirectoryRadio.UseVisualStyleBackColor = true;
            this.virtualDirectoryRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // newWebSiteRadio
            // 
            this.newWebSiteRadio.AutoSize = true;
            this.newWebSiteRadio.Location = new System.Drawing.Point(3, 3);
            this.newWebSiteRadio.Name = "newWebSiteRadio";
            this.newWebSiteRadio.Size = new System.Drawing.Size(147, 17);
            this.newWebSiteRadio.TabIndex = 0;
            this.newWebSiteRadio.Text = "Create as a new web site";
            this.newWebSiteRadio.UseVisualStyleBackColor = true;
            this.newWebSiteRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // sitesComboBox
            // 
            this.sitesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sitesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sitesComboBox.FormattingEnabled = true;
            this.sitesComboBox.Location = new System.Drawing.Point(110, 19);
            this.sitesComboBox.Name = "sitesComboBox";
            this.sitesComboBox.Size = new System.Drawing.Size(324, 21);
            this.sitesComboBox.TabIndex = 1;
            // 
            // newWebSiteNameTextBox
            // 
            this.newWebSiteNameTextBox.AboveMaximumErrorMessage = null;
            this.newWebSiteNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newWebSiteNameTextBox.BelowMinimumErrorMessage = null;
            this.newWebSiteNameTextBox.Location = new System.Drawing.Point(48, 19);
            this.newWebSiteNameTextBox.MaximumValue = null;
            this.newWebSiteNameTextBox.MaximumValueIncluded = true;
            this.newWebSiteNameTextBox.MaxLength = null;
            this.newWebSiteNameTextBox.MinimumValue = null;
            this.newWebSiteNameTextBox.MinimumValueIncluded = true;
            this.newWebSiteNameTextBox.Name = "newWebSiteNameTextBox";
            this.newWebSiteNameTextBox.Required = true;
            this.newWebSiteNameTextBox.Size = new System.Drawing.Size(369, 20);
            this.newWebSiteNameTextBox.TabIndex = 1;
            this.newWebSiteNameTextBox.Text = "CprBroker";
            this.newWebSiteNameTextBox.ValidationExpression = "\\A\\w+\\Z";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(392, 346);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(91, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 2;
            this.label2.Tag = "";
            this.label2.Text = "Application name";
            // 
            // virtualDirectoryNameTextBox
            // 
            this.virtualDirectoryNameTextBox.AboveMaximumErrorMessage = null;
            this.virtualDirectoryNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.virtualDirectoryNameTextBox.BelowMinimumErrorMessage = null;
            this.virtualDirectoryNameTextBox.Location = new System.Drawing.Point(110, 46);
            this.virtualDirectoryNameTextBox.MaximumValue = null;
            this.virtualDirectoryNameTextBox.MaximumValueIncluded = true;
            this.virtualDirectoryNameTextBox.MaxLength = null;
            this.virtualDirectoryNameTextBox.MinimumValue = null;
            this.virtualDirectoryNameTextBox.MinimumValueIncluded = true;
            this.virtualDirectoryNameTextBox.Name = "virtualDirectoryNameTextBox";
            this.virtualDirectoryNameTextBox.Required = true;
            this.virtualDirectoryNameTextBox.Size = new System.Drawing.Size(305, 20);
            this.virtualDirectoryNameTextBox.TabIndex = 3;
            this.virtualDirectoryNameTextBox.Text = "CprBroker";
            this.virtualDirectoryNameTextBox.ValidationExpression = "\\A\\w+\\Z";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 0;
            this.label3.Tag = "";
            this.label3.Text = "Web site";
            // 
            // virtualDirectoryGroupBox
            // 
            this.virtualDirectoryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.virtualDirectoryGroupBox.Controls.Add(this.sitesComboBox);
            this.virtualDirectoryGroupBox.Controls.Add(this.label3);
            this.virtualDirectoryGroupBox.Controls.Add(this.virtualDirectoryNameTextBox);
            this.virtualDirectoryGroupBox.Controls.Add(this.label2);
            this.virtualDirectoryGroupBox.Location = new System.Drawing.Point(41, 110);
            this.virtualDirectoryGroupBox.Name = "virtualDirectoryGroupBox";
            this.virtualDirectoryGroupBox.Size = new System.Drawing.Size(442, 77);
            this.virtualDirectoryGroupBox.TabIndex = 1;
            this.virtualDirectoryGroupBox.TabStop = false;
            // 
            // newWebSiteGroupBox
            // 
            this.newWebSiteGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newWebSiteGroupBox.Controls.Add(this.newWebSiteNameTextBox);
            this.newWebSiteGroupBox.Controls.Add(this.label1);
            this.newWebSiteGroupBox.Enabled = false;
            this.newWebSiteGroupBox.Location = new System.Drawing.Point(32, 26);
            this.newWebSiteGroupBox.Name = "newWebSiteGroupBox";
            this.newWebSiteGroupBox.Size = new System.Drawing.Size(439, 51);
            this.newWebSiteGroupBox.TabIndex = 1;
            this.newWebSiteGroupBox.TabStop = false;
            // 
            // websitePanel
            // 
            this.websitePanel.Controls.Add(this.newWebSiteRadio);
            this.websitePanel.Controls.Add(this.newWebSiteGroupBox);
            this.websitePanel.Location = new System.Drawing.Point(9, 190);
            this.websitePanel.Margin = new System.Windows.Forms.Padding(0);
            this.websitePanel.Name = "websitePanel";
            this.websitePanel.Size = new System.Drawing.Size(474, 85);
            this.websitePanel.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(295, 346);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // WebSiteForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(495, 381);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.websitePanel);
            this.Controls.Add(this.virtualDirectoryGroupBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.virtualDirectoryRadio);
            this.MaximumSize = new System.Drawing.Size(503, 415);
            this.MinimumSize = new System.Drawing.Size(503, 415);
            this.Name = "WebSiteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Web properties";
            this.Load += new System.EventHandler(this.TargetWebSiteForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.newWebSiteNameTextBox.ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.virtualDirectoryNameTextBox.ErrorProvider)).EndInit();
            this.virtualDirectoryGroupBox.ResumeLayout(false);
            this.virtualDirectoryGroupBox.PerformLayout();
            this.newWebSiteGroupBox.ResumeLayout(false);
            this.newWebSiteGroupBox.PerformLayout();
            this.websitePanel.ResumeLayout(false);
            this.websitePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton virtualDirectoryRadio;
        private System.Windows.Forms.RadioButton newWebSiteRadio;
        private System.Windows.Forms.ComboBox sitesComboBox;
        private CprBroker.Installers.CustomTextBox newWebSiteNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label2;
        private CprBroker.Installers.CustomTextBox virtualDirectoryNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox virtualDirectoryGroupBox;
        private System.Windows.Forms.GroupBox newWebSiteGroupBox;
        private System.Windows.Forms.Panel websitePanel;
        private System.Windows.Forms.Button cancelButton;
    }
}