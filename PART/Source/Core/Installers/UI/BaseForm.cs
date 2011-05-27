/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CprBroker.Installers
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
                throw new System.Configuration.Install.InstallException(Messages.AnErrorHasOccurredAndInstallationWillBeCancelled);
            }
            return result;
        }
    }
}
