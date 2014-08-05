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
        public static ContactAddress ToDpr(this ContactAddressType contactAddress)
        {
            ContactAddress ca = new ContactAddress();
            ca.PNR = Decimal.Parse(contactAddress.PNR);
            ca.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(contactAddress.Registration.RegistrationDate, 12);
            ca.MunicipalityCode = 0; //TODO: Can be fetched in CPR Services, CATX_STARTMYNKOD

            if (contactAddress.StartDate.HasValue)
                ca.AddressDate = CprBroker.Utilities.Dates.DateToDecimal(contactAddress.StartDate.Value, 8);

            ca.ContactAddressLine1 = contactAddress.Line1;
            ca.ContactAddressLine2 = contactAddress.Line2;
            ca.ContactAddressLine3 = contactAddress.Line3;
            ca.ContactAddressLine4 = contactAddress.Line4;
            ca.ContactAddressLine5 = contactAddress.Line5;

            return ca;
        }
    }
}
