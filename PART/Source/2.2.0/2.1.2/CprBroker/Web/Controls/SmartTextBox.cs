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
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;
using CprBroker.Engine;

namespace CprBroker.Web.Controls
{
    /// <summary>
    /// Extents the TextBox class by adding validation capabilities to it
    /// </summary>
    [Designer("CprBroker.Web.Controls.SmartTextBoxDesigner")]
    public class SmartTextBox : WebControl, INamingContainer
    {
        public T FromViewstate<T>(string key)
        {
            var obj = ViewState[key];
            if (obj == null)
                ViewState[key] = default(T);
            return (T)ViewState[key];
        }

        [DefaultValue(false)]
        public bool Required
        {
            get
            {
                return FromViewstate<bool>("Required");
            }
            set
            {
                ViewState["Required"] = value;
            }
        }

        public bool Confidential
        {
            get
            {
                return FromViewstate<bool>("Confidential");
            }
            set
            {
                ViewState["Confidential"] = value;
            }
        }

        public DataProviderConfigPropertyInfoTypes Type
        {
            get
            {
                return FromViewstate<DataProviderConfigPropertyInfoTypes>("Type");
            }
            set
            {
                ViewState["Type"] = value;
            }
        }

        [DefaultValue(null)]
        public string ValidationGroup
        {
            get
            {
                return Utilities.Web.ObjectFromViewState<string>(ViewState, "ValidationGroup");
            }
            set
            {
                ViewState["ValidationGroup"] = value;
            }
        }

        [DefaultValue("")]
        public string ValidationExpression
        {
            get
            {
                return regularExpressionValidator.ValidationExpression;
            }
            set
            {
                regularExpressionValidator.ValidationExpression = value;
                regularExpressionValidator.Enabled = !string.IsNullOrEmpty(value);
            }
        }

        public TextBox InnerTextBox = new TextBox();
        public RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
        public RegularExpressionValidator regularExpressionValidator = new RegularExpressionValidator();
        public RegularExpressionValidator intRegularExpressionValidator = new RegularExpressionValidator();
        public CheckBox booleanCheckBox = new CheckBox();

        protected override void CreateChildControls()
        {
            InnerTextBox.ID = "txt";
            InnerTextBox.TextMode = Confidential ? TextBoxMode.Password : TextBoxMode.SingleLine;
            InnerTextBox.Visible = Type != DataProviderConfigPropertyInfoTypes.Boolean;
            Controls.Add(InnerTextBox);

            requiredValidator.ControlToValidate = "txt";
            requiredValidator.Text = "Required";
            requiredValidator.ValidationGroup = ValidationGroup;
            requiredValidator.Enabled = Required && Type!= DataProviderConfigPropertyInfoTypes.Boolean;
            requiredValidator.Visible = requiredValidator.Enabled;
            Controls.Add(requiredValidator);

            regularExpressionValidator.ControlToValidate = "txt";
            regularExpressionValidator.Enabled = !string.IsNullOrEmpty(ValidationExpression) && Type != DataProviderConfigPropertyInfoTypes.Boolean;
            regularExpressionValidator.Visible = regularExpressionValidator.Enabled;
            regularExpressionValidator.Text = "Invalid Input";
            regularExpressionValidator.ValidationGroup = ValidationGroup;
            Controls.Add(regularExpressionValidator);


            intRegularExpressionValidator.ControlToValidate = "txt";
            intRegularExpressionValidator.Enabled = this.Type == DataProviderConfigPropertyInfoTypes.Integer;
            intRegularExpressionValidator.Visible = intRegularExpressionValidator.Enabled;
            intRegularExpressionValidator.ValidationGroup = ValidationGroup;
            intRegularExpressionValidator.Text = "Invalid Input";
            intRegularExpressionValidator.ValidationExpression = "\\d*";
            intRegularExpressionValidator.Text = "Digits only";            
            Controls.Add(intRegularExpressionValidator);

            booleanCheckBox.Visible = this.Type == DataProviderConfigPropertyInfoTypes.Boolean;            
            Controls.Add(booleanCheckBox);

        }

        public string Text
        {
            get
            {
                if (this.Type == DataProviderConfigPropertyInfoTypes.Boolean)
                {
                    return this.booleanCheckBox.Checked.ToString();
                }
                else
                {
                    return InnerTextBox.Text;
                }
            }
            set
            {
                if (this.Type == DataProviderConfigPropertyInfoTypes.Boolean)
                {
                    try
                    {
                        this.booleanCheckBox.Checked = Convert.ToBoolean(value);
                    }
                    catch
                    {
                        booleanCheckBox.Checked = false;
                    }
                }
                else
                {
                    InnerTextBox.Text = value;
                }
            }
        }
    }

    /// <summary>
    /// Designer class to show the SmartTextBox in design view
    /// </summary>
    public class SmartTextBoxDesigner : System.Web.UI.Design.ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            hw.RenderBeginTag(HtmlTextWriterTag.Span);

            hw.RenderBeginTag(HtmlTextWriterTag.Input);
            hw.RenderEndTag();

            hw.AddAttribute("style", "color:red");
            hw.RenderBeginTag(HtmlTextWriterTag.Span);
            hw.Write("*");
            hw.RenderEndTag();

            hw.RenderEndTag();
            return sw.ToString();
        }

    }

}
