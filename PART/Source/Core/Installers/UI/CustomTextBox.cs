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
using System.ComponentModel;
using System.Windows.Forms;

namespace CprBroker.Installers
{
    /// <summary>
    /// Extentds the TextBox class by adding validation capabilites
    /// </summary>
    public class CustomTextBox : TextBox
    {
        #region Fields
        private Nullable<int> _MaxLength;
        private bool _Required;
        private string _ValidationExpression;
        private System.Web.UI.WebControls.ValidationDataType _ValidationDataType;
        private bool _MinimumValueIncluded = true;
        private bool _MaximumValueIncluded = true;
        private string _MinimumValue;
        private string _MaximumValue;
        private System.Windows.Forms.ErrorProvider _ErrorProvider = new System.Windows.Forms.ErrorProvider();
        private delegate bool TryParseMethod<T>(string s, out T result);
        private string _AboveMaximumErrorMessage;
        private string _BelowMinimumErrorMessage;

        #endregion

        public CustomTextBox()
        {
            _ErrorProvider = new System.Windows.Forms.ErrorProvider();
            this.Validating += new CancelEventHandler(CustomTextBox_Validating);
            this.EnabledChanged += new EventHandler(CustomTextBox_EnabledChanged);
        }

        void CustomTextBox_EnabledChanged(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                ErrorProvider.Clear();
            }
        }

        #region Properties
        /// <summary>
        /// Maximum length allowed
        /// </summary>
        public new Nullable<int> MaxLength
        {
            get
            {
                return _MaxLength;
            }
            set
            {
                _MaxLength = value;
            }
        }

        /// <summary>
        /// Is this TextBox required
        /// </summary>
        [DefaultValue(false)]
        public bool Required
        {
            get
            {
                return _Required;
            }
            set
            {
                _Required = value;
            }
        }

        /// <summary>
        /// A regular expression to match the Text property against
        /// </summary>
        [DefaultValue("")]
        public string ValidationExpression
        {
            get
            {
                return _ValidationExpression;
            }
            set
            {
                _ValidationExpression = value;
            }
        }

        /// <summary>
        /// Type of range validation 
        /// </summary>
        [DefaultValue(System.Web.UI.WebControls.ValidationDataType.String)]
        public System.Web.UI.WebControls.ValidationDataType ValidationDataType
        {
            get
            {
                return _ValidationDataType;
            }
            set
            {
                _ValidationDataType = value;
            }
        }

        /// <summary>
        /// Minimum allowed value for the Text property (related to ValidationDataType)
        /// </summary>
        [DefaultValue("")]
        public string MinimumValue
        {
            get
            {
                return _MinimumValue;
            }
            set
            {
                _MinimumValue = value;
            }
        }

        /// <summary>
        /// Maximum allowed value for the Text property (related to ValidationDataType)
        /// </summary>    
        [DefaultValue("")]
        public string MaximumValue
        {
            get
            {
                return _MaximumValue;
            }
            set
            {
                _MaximumValue = value;
            }
        }

        /// <summary>
        /// Is Minimum value included during validating the input text
        /// </summary>
        public bool MinimumValueIncluded
        {
            get
            {
                return _MinimumValueIncluded;
            }
            set
            {
                _MinimumValueIncluded = value;
            }
        }

        /// <summary>
        /// Is Maximum value included during validating the input text
        /// </summary>    
        public bool MaximumValueIncluded
        {
            get
            {
                return _MaximumValueIncluded;
            }
            set
            {
                _MaximumValueIncluded = value;
            }
        }

        /// <summary>
        /// Message to display if the value is greater than maximum
        /// </summary>
        public string AboveMaximumErrorMessage
        {
            get
            {
                return _AboveMaximumErrorMessage;
            }
            set
            {
                _AboveMaximumErrorMessage = value;
            }
        }

        /// <summary>
        /// Message to display if the value is less than minimum
        /// </summary>
        public string BelowMinimumErrorMessage
        {
            get
            {
                return _BelowMinimumErrorMessage;
            }
            set
            {
                _BelowMinimumErrorMessage = value;
            }
        }

        /// <summary>
        /// Effective message to display if value is above maximum
        /// </summary>
        private string ActualAboveMaximumErrorMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(AboveMaximumErrorMessage))
                {
                    return AboveMaximumErrorMessage;
                }
                else
                {
                    return string.Format("<{0} {1}", MaximumValueIncluded ? "=" : "", MaximumValue);
                }
            }
        }

        /// Effective message to display if value is below minimum
        private string ActualBelowMinimumErrorMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(BelowMinimumErrorMessage))
                {
                    return BelowMinimumErrorMessage;
                }
                else
                {
                    return string.Format(">{0} {1}", MinimumValueIncluded ? "=" : "", MinimumValue);
                }
            }
        }

        /// <summary>
        /// Integer value of the text box
        /// </summary>
        public Nullable<int> IntValue
        {
            get
            {
                int ret;
                if (int.TryParse(Text, out ret))
                {
                    return ret;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The error provider that will show up next to the control if the validation fails
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public System.Windows.Forms.ErrorProvider ErrorProvider
        {
            get
            {
                return _ErrorProvider;
            }
            set
            {
                _ErrorProvider = value;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Provides custom validation logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomTextBox_Validating(object sender, CancelEventArgs e)
        {
            //Required
            if (Required)
            {
                e.Cancel = string.IsNullOrEmpty(Text);
                if (e.Cancel)
                {
                    ErrorProvider.SetError(this, Messages.Required);
                    return;
                }
            }

            if (MaxLength.HasValue && !string.IsNullOrEmpty(Text))
            {
                e.Cancel = Text.Length > MaxLength;
                if (e.Cancel)
                {
                    ErrorProvider.SetError(this, Messages.MaxLength + MaxLength);
                    return;
                }
            }

            // Regular expression
            if (!string.IsNullOrEmpty(ValidationExpression))
            {
                e.Cancel = !System.Text.RegularExpressions.Regex.Match(Text, ValidationExpression).Success;
                if (e.Cancel)
                {
                    ErrorProvider.SetError(this, Messages.InvalidInput);
                    return;
                }
            }

            // Range
            if (!string.IsNullOrEmpty(Text))
            {
                switch (ValidationDataType)
                {
                    case System.Web.UI.WebControls.ValidationDataType.Currency:
                        ValidateValueRange<decimal>(e, new TryParseMethod<decimal>(decimal.TryParse));
                        break;
                    case System.Web.UI.WebControls.ValidationDataType.Date:
                        ValidateValueRange<DateTime>(e, new TryParseMethod<DateTime>(DateTime.TryParse));
                        break;
                    case System.Web.UI.WebControls.ValidationDataType.Double:
                        ValidateValueRange<double>(e, new TryParseMethod<double>(double.TryParse));
                        break;
                    case System.Web.UI.WebControls.ValidationDataType.Integer:
                        ValidateValueRange<int>(e, new TryParseMethod<int>(int.TryParse));
                        break;
                }
            }

            if (!e.Cancel)
            {
                ErrorProvider.Clear();
            }
        }

        /// <summary>
        /// Validates a value for range constraints
        /// </summary>
        /// <typeparam name="T">Type of object to be validated (int, double,...etc)</typeparam>
        /// <param name="e">e.Cancel should be set to true if validation fails</param>
        /// <param name="method">Delegate to try parsing the Text property</param>
        private void ValidateValueRange<T>(CancelEventArgs e, TryParseMethod<T> tryParseMethod) where T : struct, IComparable, IConvertible
        {
            bool parseError = false;
            T val;
            if (tryParseMethod.Invoke(Text, out val))
            {
                T minimumValue, maximumValue;

                // Minimum
                if (!string.IsNullOrEmpty(MinimumValue))
                {
                    if (tryParseMethod.Invoke(MinimumValue, out minimumValue))
                    {
                        if (MinimumValueIncluded)
                        {
                            e.Cancel = val.CompareTo(minimumValue) < 0;
                        }
                        else
                        {
                            e.Cancel = val.CompareTo(minimumValue) < 0 || val.CompareTo(minimumValue) == 0;
                        }
                        if (e.Cancel)
                        {
                            ErrorProvider.SetError(this, ActualBelowMinimumErrorMessage);
                            return;
                        }
                    }
                    else
                    {
                        parseError = true;
                    }
                }

                // Maximum
                if (!string.IsNullOrEmpty(MaximumValue))
                {
                    if (tryParseMethod.Invoke(MaximumValue, out maximumValue))
                    {
                        if (MaximumValueIncluded)
                        {
                            e.Cancel = val.CompareTo(maximumValue) > 0;
                        }
                        else
                        {
                            e.Cancel = val.CompareTo(maximumValue) > 0 || val.CompareTo(maximumValue) == 0;
                        }
                        if (e.Cancel)
                        {
                            ErrorProvider.SetError(this, ActualAboveMaximumErrorMessage);
                            return;
                        }
                    }
                    else
                    {
                        parseError = true;
                    }
                }
            }
            else
            {
                parseError = true;
            }

            if (parseError)
            {
                e.Cancel = true;
                ErrorProvider.SetError(this, Messages.InvalidInput);
            }
        }

        #endregion

    }
}
