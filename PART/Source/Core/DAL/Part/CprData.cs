using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the CprData table
    /// </summary>
    public partial class CprData
    {
        public static CprBorgerType ToXmlType(CprData db)
        {
            if (db != null)
            {
                return new CprBorgerType()
                {
                    AdresseNoteTekst = db.AddressNote,
                    FolkekirkeMedlemIndikator = db.ChurchMember,
                    FolkeregisterAdresse = Address.ToXmlType(db.Address),
                    ForskerBeskyttelseIndikator = db.ResearchProtection,
                    NavneAdresseBeskyttelseIndikator = db.NameAndAddressProtectionIndicator,
                    PersonCivilRegistrationIdentifier = db.CprNumber,
                    PersonNummerGyldighedStatusIndikator = db.CprNumberValidity,
                    PersonNationalityCode = CountryRef.ToXmlType(db.NationalityCountryRef),
                    TelefonNummerBeskyttelseIndikator = db.TelephoneNumberProtection,
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<CprData>(cpr => cpr.Address);
        }

        public static CprData FromXmlType(RegisterOplysningType[] oio)
        {
            if (oio != null && oio.Length > 0 && oio[0] != null)
            {
                return FromXmlType(oio[0].Item as CprBorgerType);
            }
            return null;
        }

        public static CprData FromXmlType(CprBorgerType partCprData)
        {
            if (partCprData != null)
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
                    NationalityCountryRef = CountryRef.FromXmlType(partCprData.PersonNationalityCode),
                    TelephoneNumberProtection = partCprData.TelefonNummerBeskyttelseIndikator,
                };
            }
            return null;
        }
    }
}
