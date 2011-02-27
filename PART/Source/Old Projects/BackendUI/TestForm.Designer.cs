namespace CprBroker.BackendUI
{
    partial class TestForm
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
            this.notifyButton = new System.Windows.Forms.Button();
            this.notifyDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.resultGroupBox = new System.Windows.Forms.GroupBox();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.DataChangeEventEnqueuer = new CprBroker.EventBroker.Notifications.DataChangeEventEnqueuer(this.components);
            this.BirthdateEventEnqueuer = new CprBroker.EventBroker.Notifications.BirthdateEventEnqueuer(this.components);
            this.NotificationSender = new CprBroker.EventBroker.Notifications.NotificationSender(this.components);
            this.groupBox1.SuspendLayout();
            this.resultGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyButton
            // 
            this.notifyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.notifyButton.Location = new System.Drawing.Point(178, 74);
            this.notifyButton.Name = "notifyButton";
            this.notifyButton.Size = new System.Drawing.Size(75, 23);
            this.notifyButton.TabIndex = 0;
            this.notifyButton.Text = "Notify";
            this.notifyButton.UseVisualStyleBackColor = true;
            this.notifyButton.Click += new System.EventHandler(this.notifyButton_Click);
            // 
            // notifyDateTimePicker
            // 
            this.notifyDateTimePicker.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.notifyDateTimePicker.Location = new System.Drawing.Point(38, 75);
            this.notifyDateTimePicker.Name = "notifyDateTimePicker";
            this.notifyDateTimePicker.Size = new System.Drawing.Size(134, 20);
            this.notifyDateTimePicker.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.descriptionTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.notifyButton);
            this.groupBox1.Controls.Add(this.notifyDateTimePicker);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 104);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.descriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descriptionTextBox.Location = new System.Drawing.Point(6, 32);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(256, 36);
            this.descriptionTextBox.TabIndex = 4;
            this.descriptionTextBox.Text = "Enter a date and click \'Notify\' in order to emulate the notification engine for t" +
                "hat date";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Date";
            // 
            // resultGroupBox
            // 
            this.resultGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultGroupBox.Controls.Add(this.resultTextBox);
            this.resultGroupBox.Location = new System.Drawing.Point(12, 122);
            this.resultGroupBox.Name = "resultGroupBox";
            this.resultGroupBox.Size = new System.Drawing.Size(498, 436);
            this.resultGroupBox.TabIndex = 3;
            this.resultGroupBox.TabStop = false;
            this.resultGroupBox.Text = "Result";
            // 
            // resultTextBox
            // 
            this.resultTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultTextBox.Location = new System.Drawing.Point(6, 19);
            this.resultTextBox.Multiline = true;
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.resultTextBox.Size = new System.Drawing.Size(486, 411);
            this.resultTextBox.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(435, 564);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 599);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.resultGroupBox);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPR Broker - notification tester";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.resultGroupBox.ResumeLayout(false);
            this.resultGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button notifyButton;
        private System.Windows.Forms.DateTimePicker notifyDateTimePicker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox resultGroupBox;
        private System.Windows.Forms.TextBox resultTextBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label1;
        private CprBroker.EventBroker.Notifications.DataChangeEventEnqueuer DataChangeEventEnqueuer;
        private CprBroker.EventBroker.Notifications.BirthdateEventEnqueuer BirthdateEventEnqueuer;
        private CprBroker.EventBroker.Notifications.NotificationSender NotificationSender;
    }
}

