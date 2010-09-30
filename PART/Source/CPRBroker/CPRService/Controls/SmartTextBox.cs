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

namespace CPRService.Controls
{
    /// <summary>
    /// Extents the TextBox class by adding validation capabilities to it
    /// </summary>
    [Designer("CPRService.Controls.SmartTextBoxDesigner")]
    public class SmartTextBox : WebControl, INamingContainer
    {
        [DefaultValue(false)]
        public bool Required
        {
            get
            {
                return requiredValidator.Enabled;
            }
            set
            {
                requiredValidator.Enabled = value;
            }
        }

        [DefaultValue(null)]
        public string ValidationGroup
        {
            get
            {
                return Utilities.ObjectFromViewState<string>(ViewState, "ValidationGroup");
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

        protected override void CreateChildControls()
        {
            InnerTextBox.ID = "txt";
            Controls.Add(InnerTextBox);

            requiredValidator.ControlToValidate = "txt";
            requiredValidator.Text = "Required";
            requiredValidator.ValidationGroup = ValidationGroup;
            Controls.Add(requiredValidator);

            regularExpressionValidator.ControlToValidate = "txt";
            regularExpressionValidator.Enabled = !string.IsNullOrEmpty(ValidationExpression);
            regularExpressionValidator.Text = "Invalid input";
            regularExpressionValidator.ValidationGroup = ValidationGroup;
            Controls.Add(regularExpressionValidator);
        }

        public string Text
        {
            get
            {
                return InnerTextBox.Text;
            }
            set
            {
                InnerTextBox.Text = value;
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
