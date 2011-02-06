using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class CprData
    {
        public CprBorgerType ToXmlType()
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = AddressNote,
                FolkekirkeMedlemIndikator = ChurchMember,
                FolkeregisterAdresse = Address.ToXmlType(),
                ForskerBeskyttelseIndikator = ResearchProtection,
                NavneAdresseBeskyttelseIndikator = NameAndAddressProtectionIndicator,
                PersonCivilRegistrationIdentifier = CprNumber,
                PersonNummerGyldighedStatusIndikator = CprNumberValidity,
                PersonNationalityCode = CountryIdentificationCodeType.Create((_CountryIdentificationSchemeType)NationalityCodeScheme, NationalityCode),
                TelefonNummerBeskyttelseIndikator = TelephoneNumberProtection,
            };
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<CprData>(cpr => cpr.Address);
        }

        public static CprData FromXmlType(CprBorgerType partCprData)
        {
            return new CprData()
            {
                AddressNote = partCprData.AdresseNoteTekst,
                ChurchMember = partCprData.FolkekirkeMedlemIndikator,
                Address = Address.FromXmlType(partCprData.FolkeregisterAdresse),
                ResearchProtection = partCprData.ForskerBeskyttelseIndikator,
                NameAndAddressProtectionIndicator = partCprData.NavneAdresseBeskyttelseIndikator,
                CprNumber = partCprData.PersonCivilRegistrationIdentifier,
                CprNumberValidity = partCprData.PersonNummerGyldighedStatusIndikator,
                NationalityCode = partCprData.PersonNationalityCode.Value,
                NationalityCodeScheme = (int)partCprData.PersonNationalityCode.scheme,
                TelephoneNumberProtection = partCprData.TelefonNummerBeskyttelseIndikator,
            };
        }
    }
}
