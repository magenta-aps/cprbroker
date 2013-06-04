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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    /// <summary>
    /// A wrapper that gathers ClearWrittenAddress and CurrentAddressInformation
    /// Reason is that ClearWrittenAddress has more address details
    /// But CurrentAddressInformation has the address dates
    /// The test data suggest that ClearWrittenAddress is empty when CurrentAddressInformation is null
    /// </summary>
    public class CurrentAddressWrapper : IAddressSource
    {
        private CurrentAddressWrapper()
        {
        }

        public CurrentAddressWrapper(CurrentAddressInformationType currentAddress, ClearWrittenAddressType clearAddress)
        {
            this.ClearWrittenAddress = clearAddress;
            this.CurrentAddressInformation = currentAddress;
        }

        public ClearWrittenAddressType ClearWrittenAddress { get; private set; }
        public CurrentAddressInformationType CurrentAddressInformation { get; private set; }

        public AdresseType ToAdresseType()
        {
            return ClearWrittenAddress.ToAdresseType();
        }

        public string ToAddressNoteTekste()
        {
            return this.ClearWrittenAddress.ToAddressNoteTekste();
        }

        public DataTypeTags Tag
        {
            get { return DataTypeTags.Address; }
        }

        public DateTime? ToStartTS()
        {
            return CurrentAddressInformation.RelocationDate;

            /*
             * Explanation
             * -----------
             * RelocationDate: Is the address date
             * MunicipalityArrivalDate: seems to be some hsitorical date when the municipality started registration of this person. Usually (but not always) this is the same as RelocationDate
             * LeavingMunicipalityDepartureDate: is the date when the person left the previous municipality. This is zero if the person came from abroad (HistoricalExitEntry)
             
             */

            //return CurrentAddressInformation.RelocationDate;
            // TODO: What is the difference between StartDate and RelocaltionDate?
            //return Converters.ToDateTime(this.CurrentAddressInformation.RelocationDate, this.CurrentAddressInformation.RelocationDateUncertainty);
        }

        public DateTime? ToEndTS()
        {
            return null;
        }

        public IRegistrationInfo Registration
        {
            get
            {
                return this.ClearWrittenAddress.Registration;
            }
        }
    }
}
