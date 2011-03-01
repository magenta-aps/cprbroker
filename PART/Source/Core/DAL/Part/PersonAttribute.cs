using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the PersonAttributes table
    /// </summary>
    public partial class PersonAttributes
    {
        public static Schemas.Part.AttributListeType ToXmlType(PersonAttributes db)
        {
            if (db != null)
            {
                return new AttributListeType()
                {
                    Egenskab = PersonProperties.ToXmlType(db.PersonProperties),
                    RegisterOplysning = ToRegisterOplysningType(db),
                    LokalUdvidelse = null,
                    SundhedOplysning = HealthInformation.ToXmlType(db.HealthInformation),
                };
            }
            return null;
        }

        public static RegisterOplysningType[] ToRegisterOplysningType(PersonAttributes db)
        {
            if (db != null)
            {
                var ret = new RegisterOplysningType()
                {
                    Virkning = Effect.ToVirkningType(db.Effect),
                    Item = null
                };

                if (db.CprData != null)
                {
                    ret.Item = CprData.ToXmlType(db.CprData);
                }
                else if (db.ForeignCitizenData != null)
                {
                    ret.Item = ForeignCitizenData.ToXmlType(db.ForeignCitizenData);
                }
                else if (db.UnknownCitizenData != null)
                {
                    ret.Item = UnknownCitizenData.ToXmlType(db.UnknownCitizenData);
                }

                if (ret.Item != null)
                {
                    return new RegisterOplysningType[] { ret };
                }
            }
            return new RegisterOplysningType[0];

        }

        public static PersonAttributes FromXmlType(Schemas.Part.AttributListeType oio)
        {
            if (oio != null)
            {
                return new PersonAttributes()
                {
                    PersonProperties = PersonProperties.FromXmlType(oio.Egenskab),
                    CprData = CprData.FromXmlType(oio.RegisterOplysning),
                    ForeignCitizenData = ForeignCitizenData.FromXmlType(oio.RegisterOplysning),
                    UnknownCitizenData = UnknownCitizenData.FromXmlType(oio.RegisterOplysning),
                    HealthInformation = HealthInformation.FromXmlType(oio.SundhedOplysning),
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonAttributes>(pa => pa.Effect);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.PersonProperties);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.CprData);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.ForeignCitizenData);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.UnknownCitizenData);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.HealthInformation);

            PersonProperties.SetChildLoadOptions(loadOptions);
            CprData.SetChildLoadOptions(loadOptions);
            ForeignCitizenData.SetChildLoadOptions(loadOptions);
            HealthInformation.SetChildLoadOptions(loadOptions);
        }

    }
}
