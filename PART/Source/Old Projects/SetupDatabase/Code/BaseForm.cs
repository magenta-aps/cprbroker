using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPRBroker.SetupDatabase
{
    /// <summary>
    /// Base form for all forms
    /// </summary>
    public class BaseForm : Form
    {

        public BaseForm()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "BaseForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BaseForm_FormClosing);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Validates the form contents
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateContents()
        {
            return true;
        }

        private bool IsAcceptButtonClicked;

        /// <summary>
        /// Marks the form as OK has been pressed then closes the form
        /// </summary>
        protected void CloseAsOK()
        {
            IsAcceptButtonClicked = true;
            Close();
        }

        /// <summary>
        /// Forces logical validation of form contents before it is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
            if (IsAcceptButtonClicked)
            {
                IsAcceptButtonClicked = false;
                if (!ValidateChildren(ValidationConstraints.Enabled))
                {
                    e.Cancel = true;
                }
                else if (!ValidateContents())
                {
                    e.Cancel = true;
                }
            }
            else
            {
                DialogResult confirm = MessageBox.Show(this, Messages.AreYouSureYouWantToCancel, Messages.CancelSetup, MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Cancel;
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public static DialogResult ShowAsDialog(BaseForm form,IWin32Window owner)
        {
            DialogResult result = form.ShowDialog(owner);
            if (result == DialogResult.Cancel)
            {
                throw new System.Configuration.Install.InstallException(Messages.AnErrorHasOccuredAndInstallationWillBeCancelled);
            }
            return result;
        }
    }
}
