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
using CprBroker.Schemas.Util;

namespace CprBroker.Providers.CPRDirect
{
    public partial class PersonInformationType : IBasicInformation
    {
        public LivStatusType ToLivStatusType()
        {
            return new LivStatusType()
            {
                // We are not using the uncertainty flag here because an uncertain birthdate still means the person is already born
                LivStatusKode = Enums.ToLifeStatus(this.Status, this.Birthdate.HasValue),
                TilstandVirkning = TilstandVirkningType.Create(this.ToStatusDate())
            };
        }

		public DateTime? ToBirthdate()
		{
			return ToBirthdate (false);
		}

        public DateTime? ToBirthdate(bool tryPnr)
        {
            // First, look at birthdate field
            var val = Converters.ToDateTime(this.Birthdate, this.BirthdateUncertainty);
            if (val.HasValue || !tryPnr)
                return val;

            // Now try the current CPR number - it should be more recent than PNR
            val = PartInterface.Strings.PersonNumberToDate(this.CurrentCprNumber);
            if (val.HasValue)
                return val;

            // Finally, use PNR
            val = PartInterface.Strings.PersonNumberToDate(this.ToPnr());
            return val;
        }

        public DateTime? ToStatusDate()
        {
            return Converters.ToDateTime(this.StatusStartDate, this.StatusDateUncertainty);
        }

        public string ToPnr()
        {
            return Converters.ToPnrStringOrNull(this.PNR);
        }

        public PersonGenderCodeType ToPersonGenderCodeType()
        {
            return Converters.ToPersonGenderCodeType(this.Gender);
        }

        public bool ToPersonNummerGyldighedStatusIndikator()
        {
            return Enums.IsActiveCivilRegistrationStatus(this.Status);
        }

        public DataTypeTags Tag
        {
            get { return DataTypeTags.BasicInformation; }
        }

        public DateTime? ToStartTS()
        {
            return PersonStartDate;
        }

        public bool ToStartTSCertainty()
        {
            return Converters.ToDateTimeUncertainty(PersonStartDateUncertainty);
        }

        public DateTime? ToEndTS()
        {
            return PersonEndDate;
        }

        public bool ToEndTSCertainty()
        {
            return Converters.ToDateTimeUncertainty(PersonEndDateUncertainty);
        }

        public PersonRelationType[] ToReplacedByRelationType(Func<string, Guid> cpr2UuidFunc)
        {
            var newPnr = Converters.ToPnrStringOrNull(CurrentCprNumber);
            if (!string.IsNullOrEmpty(newPnr))
            {
                return new PersonRelationType[] { PersonRelationType.Create(cpr2UuidFunc(newPnr), PersonEndDate, null) };
            }
            else
            {
                return new PersonRelationType[0];
            }
        }

    }
}
