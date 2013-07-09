namespace BusinessApplicationDemo
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.btnCitizenData = new System.Windows.Forms.Button();
            this.personContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnApplication = new System.Windows.Forms.Button();
            this.btnOther = new System.Windows.Forms.Button();
            this.btnSubscription = new System.Windows.Forms.Button();
            this.applicationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.subscriptionContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.otherContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblUserToken = new System.Windows.Forms.Label();
            this.lblAppToken = new System.Windows.Forms.Label();
            this.lblCPRNumber = new System.Windows.Forms.Label();
            this.txtUserToken = new System.Windows.Forms.TextBox();
            this.txtAppToken = new System.Windows.Forms.TextBox();
            this.txtCPRNumber = new System.Windows.Forms.TextBox();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.lblNotificationChannel = new System.Windows.Forms.Label();
            this.txtNotifyURL = new System.Windows.Forms.TextBox();
            this.lblNotificatoinUrl = new System.Windows.Forms.Label();
            this.txtCPRArray = new System.Windows.Forms.TextBox();
            this.lblCPRArray = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGrid();
            this.txtChildCprNumber = new System.Windows.Forms.TextBox();
            this.lblChildCprNo = new System.Windows.Forms.Label();
            this.txtPersonServiceUrl = new System.Windows.Forms.TextBox();
            this.lblServiceUrl = new System.Windows.Forms.Label();
            this.outputBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtAdminServiceUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtResultXml = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCitizenData
            // 
            this.btnCitizenData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCitizenData.Location = new System.Drawing.Point(8, 655);
            this.btnCitizenData.Name = "btnCitizenData";
            this.btnCitizenData.Size = new System.Drawing.Size(75, 23);
            this.btnCitizenData.TabIndex = 0;
            this.btnCitizenData.Text = "Citizen";
            this.btnCitizenData.UseVisualStyleBackColor = true;
            this.btnCitizenData.Click += new System.EventHandler(this.btnCitizenData_Click);
            // 
            // personContextMenuStrip
            // 
            this.personContextMenuStrip.Name = "personContextMenuStrip";
            this.personContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // btnApplication
            // 
            this.btnApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApplication.Location = new System.Drawing.Point(89, 655);
            this.btnApplication.Name = "btnApplication";
            this.btnApplication.Size = new System.Drawing.Size(75, 23);
            this.btnApplication.TabIndex = 2;
            this.btnApplication.Text = "Application";
            this.btnApplication.UseVisualStyleBackColor = true;
            this.btnApplication.Click += new System.EventHandler(this.btnApplication_Click);
            // 
            // btnOther
            // 
            this.btnOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOther.Location = new System.Drawing.Point(251, 655);
            this.btnOther.Name = "btnOther";
            this.btnOther.Size = new System.Drawing.Size(75, 23);
            this.btnOther.TabIndex = 3;
            this.btnOther.Text = "Other";
            this.btnOther.UseVisualStyleBackColor = true;
            this.btnOther.Click += new System.EventHandler(this.btnOther_Click);
            // 
            // btnSubscription
            // 
            this.btnSubscription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSubscription.Location = new System.Drawing.Point(170, 655);
            this.btnSubscription.Name = "btnSubscription";
            this.btnSubscription.Size = new System.Drawing.Size(75, 23);
            this.btnSubscription.TabIndex = 4;
            this.btnSubscription.Text = "Subscription";
            this.btnSubscription.UseVisualStyleBackColor = true;
            this.btnSubscription.Click += new System.EventHandler(this.btnSubscription_Click);
            // 
            // applicationContextMenuStrip
            // 
            this.applicationContextMenuStrip.Name = "methodsContextMenuStrip";
            this.applicationContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // subscriptionContextMenuStrip
            // 
            this.subscriptionContextMenuStrip.Name = "methodsContextMenuStrip";
            this.subscriptionContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // otherContextMenuStrip
            // 
            this.otherContextMenuStrip.Name = "methodsContextMenuStrip";
            this.otherContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Item";
            this.dataGridViewTextBoxColumn1.HeaderText = "Item";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // lblUserToken
            // 
            this.lblUserToken.AutoSize = true;
            this.lblUserToken.Location = new System.Drawing.Point(12, 13);
            this.lblUserToken.Name = "lblUserToken";
            this.lblUserToken.Size = new System.Drawing.Size(66, 13);
            this.lblUserToken.TabIndex = 6;
            this.lblUserToken.Text = "User token :";
            // 
            // lblAppToken
            // 
            this.lblAppToken.AutoSize = true;
            this.lblAppToken.Location = new System.Drawing.Point(12, 42);
            this.lblAppToken.Name = "lblAppToken";
            this.lblAppToken.Size = new System.Drawing.Size(63, 13);
            this.lblAppToken.TabIndex = 7;
            this.lblAppToken.Text = "App token :";
            // 
            // lblCPRNumber
            // 
            this.lblCPRNumber.AutoSize = true;
            this.lblCPRNumber.Location = new System.Drawing.Point(12, 125);
            this.lblCPRNumber.Name = "lblCPRNumber";
            this.lblCPRNumber.Size = new System.Drawing.Size(74, 13);
            this.lblCPRNumber.TabIndex = 8;
            this.lblCPRNumber.Text = "CPR Number :";
            // 
            // txtUserToken
            // 
            this.txtUserToken.Location = new System.Drawing.Point(95, 13);
            this.txtUserToken.Name = "txtUserToken";
            this.txtUserToken.Size = new System.Drawing.Size(265, 20);
            this.txtUserToken.TabIndex = 9;
            this.txtUserToken.Text = "userToken";
            // 
            // txtAppToken
            // 
            this.txtAppToken.Location = new System.Drawing.Point(95, 39);
            this.txtAppToken.Name = "txtAppToken";
            this.txtAppToken.Size = new System.Drawing.Size(265, 20);
            this.txtAppToken.TabIndex = 10;
            this.txtAppToken.Text = "07059250-E448-4040-B695-9C03F9E59E38";
            // 
            // txtCPRNumber
            // 
            this.txtCPRNumber.Location = new System.Drawing.Point(95, 122);
            this.txtCPRNumber.Name = "txtCPRNumber";
            this.txtCPRNumber.Size = new System.Drawing.Size(265, 20);
            this.txtCPRNumber.TabIndex = 11;
            this.txtCPRNumber.Text = "1806803045";
            // 
            // txtChannel
            // 
            this.txtChannel.Location = new System.Drawing.Point(95, 182);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(265, 20);
            this.txtChannel.TabIndex = 13;
            this.txtChannel.Text = "web service";
            this.txtChannel.Visible = false;
            // 
            // lblNotificationChannel
            // 
            this.lblNotificationChannel.AutoSize = true;
            this.lblNotificationChannel.Location = new System.Drawing.Point(12, 185);
            this.lblNotificationChannel.Name = "lblNotificationChannel";
            this.lblNotificationChannel.Size = new System.Drawing.Size(53, 13);
            this.lblNotificationChannel.TabIndex = 12;
            this.lblNotificationChannel.Text = "Channel :";
            this.lblNotificationChannel.Visible = false;
            // 
            // txtNotifyURL
            // 
            this.txtNotifyURL.Location = new System.Drawing.Point(95, 208);
            this.txtNotifyURL.Name = "txtNotifyURL";
            this.txtNotifyURL.Size = new System.Drawing.Size(265, 20);
            this.txtNotifyURL.TabIndex = 15;
            this.txtNotifyURL.Text = "http://localhost/ManageWS/NotifyURL.asmx";
            this.txtNotifyURL.Visible = false;
            // 
            // lblNotificatoinUrl
            // 
            this.lblNotificatoinUrl.AutoSize = true;
            this.lblNotificatoinUrl.Location = new System.Drawing.Point(12, 211);
            this.lblNotificatoinUrl.Name = "lblNotificatoinUrl";
            this.lblNotificatoinUrl.Size = new System.Drawing.Size(65, 13);
            this.lblNotificatoinUrl.TabIndex = 14;
            this.lblNotificatoinUrl.Text = "Notify URL :";
            this.lblNotificatoinUrl.Visible = false;
            // 
            // txtCPRArray
            // 
            this.txtCPRArray.Location = new System.Drawing.Point(95, 234);
            this.txtCPRArray.Multiline = true;
            this.txtCPRArray.Name = "txtCPRArray";
            this.txtCPRArray.Size = new System.Drawing.Size(265, 51);
            this.txtCPRArray.TabIndex = 17;
            this.txtCPRArray.Text = "123456789 ,1234,12345";
            this.txtCPRArray.Visible = false;
            // 
            // lblCPRArray
            // 
            this.lblCPRArray.AutoSize = true;
            this.lblCPRArray.Location = new System.Drawing.Point(12, 237);
            this.lblCPRArray.Name = "lblCPRArray";
            this.lblCPRArray.Size = new System.Drawing.Size(64, 13);
            this.lblCPRArray.TabIndex = 16;
            this.lblCPRArray.Text = "CPR Array :";
            this.lblCPRArray.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Item";
            this.dataGridViewTextBoxColumn2.HeaderText = "Item";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Item";
            this.dataGridViewTextBoxColumn3.HeaderText = "Item";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.DataMember = "";
            this.dataGridView1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridView1.Location = new System.Drawing.Point(12, 303);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(659, 188);
            this.dataGridView1.TabIndex = 18;
            // 
            // txtChildCprNumber
            // 
            this.txtChildCprNumber.Location = new System.Drawing.Point(94, 150);
            this.txtChildCprNumber.Name = "txtChildCprNumber";
            this.txtChildCprNumber.Size = new System.Drawing.Size(265, 20);
            this.txtChildCprNumber.TabIndex = 20;
            this.txtChildCprNumber.Text = "123";
            this.txtChildCprNumber.Visible = false;
            // 
            // lblChildCprNo
            // 
            this.lblChildCprNo.AutoSize = true;
            this.lblChildCprNo.Location = new System.Drawing.Point(11, 153);
            this.lblChildCprNo.Name = "lblChildCprNo";
            this.lblChildCprNo.Size = new System.Drawing.Size(73, 13);
            this.lblChildCprNo.TabIndex = 19;
            this.lblChildCprNo.Text = "Chile CPR No:";
            this.lblChildCprNo.Visible = false;
            // 
            // txtPersonServiceUrl
            // 
            this.txtPersonServiceUrl.Location = new System.Drawing.Point(112, 65);
            this.txtPersonServiceUrl.Name = "txtPersonServiceUrl";
            this.txtPersonServiceUrl.Size = new System.Drawing.Size(247, 20);
            this.txtPersonServiceUrl.TabIndex = 22;
            this.txtPersonServiceUrl.Text = "http://cprbroker/Services/CPRPersonWS.asmx";
            // 
            // lblServiceUrl
            // 
            this.lblServiceUrl.AutoSize = true;
            this.lblServiceUrl.Location = new System.Drawing.Point(11, 68);
            this.lblServiceUrl.Name = "lblServiceUrl";
            this.lblServiceUrl.Size = new System.Drawing.Size(107, 13);
            this.lblServiceUrl.TabIndex = 21;
            this.lblServiceUrl.Text = "Person Service URL :";
            // 
            // outputBindingSource
            // 
            this.outputBindingSource.DataSource = typeof(BusinessApplicationDemo.CPRPersonWS.PersonBasicStructureType);
            // 
            // txtAdminServiceUrl
            // 
            this.txtAdminServiceUrl.Location = new System.Drawing.Point(113, 91);
            this.txtAdminServiceUrl.Name = "txtAdminServiceUrl";
            this.txtAdminServiceUrl.Size = new System.Drawing.Size(247, 20);
            this.txtAdminServiceUrl.TabIndex = 24;
            this.txtAdminServiceUrl.Text = "http://cprbroker/Services/CPRAdministrationWS.asmx";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Admin Service URL :";
            // 
            // txtResultXml
            // 
            this.txtResultXml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResultXml.Location = new System.Drawing.Point(12, 497);
            this.txtResultXml.Multiline = true;
            this.txtResultXml.Name = "txtResultXml";
            this.txtResultXml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResultXml.Size = new System.Drawing.Size(659, 152);
            this.txtResultXml.TabIndex = 25;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 690);
            this.Controls.Add(this.txtResultXml);
            this.Controls.Add(this.txtAdminServiceUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPersonServiceUrl);
            this.Controls.Add(this.lblServiceUrl);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtAppToken);
            this.Controls.Add(this.txtUserToken);
            this.Controls.Add(this.lblAppToken);
            this.Controls.Add(this.txtChildCprNumber);
            this.Controls.Add(this.lblChildCprNo);
            this.Controls.Add(this.txtCPRNumber);
            this.Controls.Add(this.txtCPRArray);
            this.Controls.Add(this.lblCPRArray);
            this.Controls.Add(this.txtNotifyURL);
            this.Controls.Add(this.lblNotificatoinUrl);
            this.Controls.Add(this.txtChannel);
            this.Controls.Add(this.lblNotificationChannel);
            this.Controls.Add(this.lblUserToken);
            this.Controls.Add(this.lblCPRNumber);
            this.Controls.Add(this.btnOther);
            this.Controls.Add(this.btnCitizenData);
            this.Controls.Add(this.btnApplication);
            this.Controls.Add(this.btnSubscription);
            this.Name = "frmMain";
            this.Text = "Business application demo";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCitizenData;
        private System.Windows.Forms.ContextMenuStrip personContextMenuStrip;
        private System.Windows.Forms.Button btnApplication;
        private System.Windows.Forms.Button btnOther;
        private System.Windows.Forms.Button btnSubscription;
        private System.Windows.Forms.ContextMenuStrip applicationContextMenuStrip;
        private System.Windows.Forms.ContextMenuStrip subscriptionContextMenuStrip;
        private System.Windows.Forms.ContextMenuStrip otherContextMenuStrip;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.Label lblUserToken;
        private System.Windows.Forms.Label lblAppToken;
        private System.Windows.Forms.Label lblCPRNumber;
        private System.Windows.Forms.TextBox txtUserToken;
        private System.Windows.Forms.TextBox txtAppToken;
        private System.Windows.Forms.TextBox txtCPRNumber;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.Label lblNotificationChannel;
        private System.Windows.Forms.TextBox txtNotifyURL;
        private System.Windows.Forms.Label lblNotificatoinUrl;
        private System.Windows.Forms.TextBox txtCPRArray;
        private System.Windows.Forms.Label lblCPRArray;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGrid dataGridView1;
        private System.Windows.Forms.TextBox txtChildCprNumber;
        private System.Windows.Forms.Label lblChildCprNo;
        private System.Windows.Forms.BindingSource outputBindingSource;
        private System.Windows.Forms.TextBox txtPersonServiceUrl;
        private System.Windows.Forms.Label lblServiceUrl;
        private System.Windows.Forms.TextBox txtAdminServiceUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtResultXml;
    }
}

