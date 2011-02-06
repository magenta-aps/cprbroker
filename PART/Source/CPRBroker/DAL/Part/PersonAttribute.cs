using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonAttributes
    {
        public Schemas.Part.AttributListeType ToXmlType()
        {
            var ret = new AttributListeType()
            {
                Egenskab = PersonProperties != null ? new EgenskabType[] { this.PersonProperties.ToXmlType() } : null,
                RegisterOplysning = ToRegisterOplysningType(),
                LokalUdvidelse = null,
                SundhedOplysning = HealthInformation != null ? new SundhedOplysningType[] { HealthInformation.ToXmlType() } : null,
            };
            return ret;
        }

        public RegisterOplysningType[] ToRegisterOplysningType()
        {
            var ret = new RegisterOplysningType()
            {
                Virkning = Effect.ToXmlType(),
                Item = null
            };

            if (this.CprData != null)
            {
                ret.Item = CprData.ToXmlType();
            }
            else if (this.ForeignCitizenData != null)
            {
                ret.Item = ForeignCitizenData.ToXmlType();
            }
            else if (this.UnknownCitizenData != null)
            {
                ret.Item = UnknownCitizenData.ToXmlType();
            }

            if (ret.Item != null)
            {
                return new RegisterOplysningType[] { ret };
            }
            else
            {
                return null;
            }
        }

        public static PersonAttributes FromXmlType(Schemas.Part.AttributListeType oio)
        {
            var oiosss = oio.Egenskab[0];
            var ret = new PersonAttributes()
            {
                PersonProperties = oio.Egenskab != null && oio.Egenskab.Length > 0 && oio.Egenskab[0] != null ? PersonProperties.FromXmlType(oio.Egenskab[0]) : null,
                CprData = null,
                ForeignCitizenData = null,
                HealthInformation = null,
                UnknownCitizenData = null,
            };

            if (oio.RegisterOplysning != null && oio.RegisterOplysning.Length > 0)
            {
                if (oio.RegisterOplysning[0].Item is Schemas.Part.CprData)
                {
                    ret.CprData = DAL.Part.CprData.FromXmlType(oio.RegisterOplysning[0].Item as CprBorgerType);
                }
                else if (oio.RegisterOplysning[0].Item is UdenlandskBorgerType)
                {
                    ret.ForeignCitizenData = DAL.Part.ForeignCitizenData.FromXmlType(oio.RegisterOplysning[0].Item as UdenlandskBorgerType);
                }
                else if (oio.RegisterOplysning[0].Item is UkendtBorgerType)
                {
                    ret.UnknownCitizenData = DAL.Part.UnknownCitizenData.FromXmlType(oio.RegisterOplysning[0].Item as UkendtBorgerType);
                }
            }
            return ret;
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
        }

    }
}
