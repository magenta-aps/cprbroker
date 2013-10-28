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

using System.Collections;

using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;

using GKApp2010.Core;

namespace GKApp2010.Common
{
    // ================================================================================
    public class MailHelper
    {
        // -----------------------------------------------------------------------------
        public static void SendMessage(string message)
        {
            string sendToAddress = Properties.Settings.Default.DefaultSendToAddress;
            string subject = Properties.Settings.Default.DefaultSubjectPrefix;

            SendMessage(sendToAddress, subject, message);
        }

        // -----------------------------------------------------------------------------
        public static void SendMessage(string subject, string message)
        {
            string sendToAddress = Properties.Settings.Default.DefaultSendToAddress;
            SendMessage(sendToAddress, subject, message);
        }

        // -----------------------------------------------------------------------------
        public static void SendMessage(string sendTo, string subject, string message)
        {
            try
            {
                // Validate the email address
                ValidateEmailAddress(sendTo, true);

                // Create the email message
                MailMessage mailMessage = new MailMessage(Properties.Settings.Default.DefaultSendFromAddress, sendTo, subject, message);

                // Create smtp client at mail server location
                SmtpClient client = new SmtpClient(Properties.Settings.Default.SMTPServerAddress);

                // Add credentials
                client.UseDefaultCredentials = true;

                // Send message
                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                string msg = "Exception thrown trying to send mail inside  GKApp2010.Common.MailHelper.SendMessage(). Msg=[" + ex.Message + "]. ";
                EventLogHelper.WriteErrorEntry(msg);
            }

        }

        // -----------------------------------------------------------------------------
        public static void SendMessageWithAttachment(string sendTo, string subject, string message, ArrayList attachments)
        {
            try
            {
                // Validate the email address
                ValidateEmailAddress(sendTo, true);

                // Create the email message
                MailMessage mailMessage = new MailMessage(Properties.Settings.Default.DefaultSendFromAddress, sendTo, subject, message);

                // Add attachments
                foreach (string attach in attachments)
                {
                    Attachment attached = new Attachment(attach, MediaTypeNames.Application.Octet);
                    mailMessage.Attachments.Add(attached);
                }

                // Create smtp client at mail server location
                SmtpClient client = new SmtpClient(Properties.Settings.Default.SMTPServerAddress);

                // Add credentials
                client.UseDefaultCredentials = true;

                // Send message
                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                string msg = "Exception thrown trying to send mail inside  GKApp2010.Common.MailHelper.SendMessageWithAttachment(). Msg=[" + ex.Message + "]. ";
                EventLogHelper.WriteErrorEntry(msg);
            }
        }

        // -----------------------------------------------------------------------------
        public static bool ValidateEmailAddress(string emailAddress)
        {
            return ValidateEmailAddress(emailAddress, false);
        }

        // -----------------------------------------------------------------------------
        public static bool ValidateEmailAddress(string emailAddress, bool throwExceptionOnValidationError)
        {
            Regex expression = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");

            // Test email address with expression
            bool b = expression.IsMatch(emailAddress);

            if (!b && throwExceptionOnValidationError)
            {
                string msg = "Email address (" + emailAddress + ") is not formatted proberly (inside GKApp2010.Common.ValidateEmailAddress()). ";
                throw new GKAInvalidFormatException(msg);
            }

            return expression.IsMatch(emailAddress);
        }
    }
}
