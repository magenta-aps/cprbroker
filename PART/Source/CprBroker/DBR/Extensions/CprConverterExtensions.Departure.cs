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
 * Dennis Amdi Skov Isaksen
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
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture, ElectionInformationType electionInfo)
        {
            Departure d = new Departure();
            if (currentDeparture != null)
            {
                d.PNR = Decimal.Parse(currentDeparture.PNR);
                if (currentDeparture.ExitDate.HasValue)
                    d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);

                d.ExitCountryCode = currentDeparture.ExitCountryCode != 0m ? currentDeparture.ExitCountryCode : null as decimal?;

                if (currentDeparture.ExitDate.HasValue)
                    d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);

                d.ExitUpdateDate = null; //TODO: Can be fetched in CPR Services, udrtimestamp
                d.ForeignAddressDate = null; //TODO: Can be fetched in CPR Services, udlandadrdto

                if (electionInfo != null && electionInfo.VotingDate.HasValue)
                    d.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(electionInfo.VotingDate.Value, 12);

                d.EntryCountryCode = null; //This is the current status
                d.EntryDate = null; //This is the current date
                d.EntryUpdateDate = null; //TODO: Can be fetched in CPR Services, indrtimestamp
                d.CorrectionMarker = null; //This is the current status
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress1))
                    d.ForeignAddressLine1 = currentDeparture.ForeignAddress1;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress2))
                    d.ForeignAddressLine2 = currentDeparture.ForeignAddress2;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress3))
                    d.ForeignAddressLine3 = currentDeparture.ForeignAddress3;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress4))
                    d.ForeignAddressLine4 = currentDeparture.ForeignAddress4;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress5))
                    d.ForeignAddressLine5 = currentDeparture.ForeignAddress5;
            }
            return d;
        }

        public static Departure ToDpr(this HistoricalDepartureType historicalDeparture/*, ElectionInformationType electionInfo*/)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(historicalDeparture.PNR);
            if (historicalDeparture.EntryDate.HasValue)
                d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);

            d.ExitCountryCode = historicalDeparture.ExitCountryCode != 0m ? historicalDeparture.ExitCountryCode : null as decimal?;

            if (historicalDeparture.ExitDate != null)
            {
                d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.ExitDate.Value, 12);
            }
            else
            {
                d.ExitDate = null;
            }
            d.ExitUpdateDate = null; //TODO: Can be fetched in CPR Services, udrtimestamp
            d.ForeignAddressDate = null; //TODO: Can be fetched in CPR Services, udlandadrdto
            //TODO: Tjek om valg dato findes i historiske data
            d.VotingDate = null;//CprBroker.Utilities.Dates.DateToDecimal(electionInfo.VotingDate.Value, 12);
            d.EntryCountryCode = historicalDeparture.EntryCountryCode;

            if (historicalDeparture.EntryDate.HasValue)
                d.EntryDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);

            d.EntryUpdateDate = null; //TODO: Can be fetched in CPR Services, indrtimestamp
            if (!char.IsWhiteSpace(historicalDeparture.CorrectionMarker))
                d.CorrectionMarker = historicalDeparture.CorrectionMarker.ToString();
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress1))
                d.ForeignAddressLine1 = historicalDeparture.ForeignAddress1;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress2))
                d.ForeignAddressLine2 = historicalDeparture.ForeignAddress2;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress3))
                d.ForeignAddressLine3 = historicalDeparture.ForeignAddress3;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress4))
                d.ForeignAddressLine4 = historicalDeparture.ForeignAddress4;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress5))
                d.ForeignAddressLine5 = historicalDeparture.ForeignAddress5;
            return d;
        }

    }
}
