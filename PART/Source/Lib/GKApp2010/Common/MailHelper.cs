//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
