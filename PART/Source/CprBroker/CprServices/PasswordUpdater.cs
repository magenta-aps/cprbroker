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
 * Mathias Dam Hedelund
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
using System.Threading.Tasks;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Engine.Tasks;
using CprBroker.Utilities;
using CprBroker.Data.DataProviders;

namespace CprBroker.Providers.CprServices
{
    public class PasswordUpdater : PeriodicTaskExecuter
    {
        private void UpdatePassword(CprServicesDataProvider provider)
        {
            PasswordGenerator passwordGen = new PasswordGenerator();

            passwordGen.MinimumLength = CprServices.Constants.PasswordRules.MinimumLength;
            passwordGen.MaximumLength = CprServices.Constants.PasswordRules.MaximumLength;
            passwordGen.MinimumNumberOfLowercaseChars = CprServices.Constants.PasswordRules.MinimumNumberOfLowercaseChars;
            passwordGen.MinimumNumberOfUppercaseChars = CprServices.Constants.PasswordRules.MinimumNumberOfUppercaseChars;
            passwordGen.MinimumNumberOfNumericDigits = CprServices.Constants.PasswordRules.MinimumNumberOfNumericDigits;
            passwordGen.MinimumNumberOfSymbols = CprServices.Constants.PasswordRules.MinimumNumberOfSymbols;
            passwordGen.ExcludedCharacters = CprServices.Constants.PasswordRules.ExcludedCharacters;

            String newPassword = passwordGen.Generate();

            // Store old password in case something goes wrong
            String oldPassword = provider.Password;

            Kvit kvit = provider.ChangePassword(newPassword);

            if (kvit.OK)
            {
                // Only change stored password if it was changed successfully
                provider.Password = newPassword;

                Admin.LogFormattedSuccess("Password automatically updated. Last update was at {0}.", provider.DateUpdatedPassword);
                provider.DateUpdatedPassword = DateTime.Now;
            }
            else
            {
                Admin.LogFormattedError("Failed to update password. Last update was at {0}.", provider.DateUpdatedPassword);
            }
        }
        protected override void PerformTimerAction()
        {
            var factory = new DataProviderFactory();
            using (var provDataContext = new DataProvidersDataContext()) {
                foreach (var dbProvider in provDataContext.DataProviders)
                {
                    if (dbProvider.IsEnabled)
                    {
                        var provider = factory.CreateDataProvider(dbProvider) as CprServicesDataProvider;
                        if (provider != null && provider.AutomaticPasswordUpdate)
                        {
                            UpdatePassword(provider);
                        }
                    }
                }
                
            }
        }
    }

    
}
