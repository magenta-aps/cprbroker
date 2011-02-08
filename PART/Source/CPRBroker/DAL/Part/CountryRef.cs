using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class CountryRef
    {
        public static CountryIdentificationCodeType ToXmlType(CountryRef db)
        {
            return new CountryIdentificationCodeType()
            {
                scheme = (_CountryIdentificationSchemeType)db.CountrySchemeTypeId,
                Value = db.Value
            };
        }

        public static CountryRef FromXmlType(CountryIdentificationCodeType oio)
        {
            return new CountryRef()
            {
                CountryRefId = Guid.NewGuid(),
                Value = oio.Value,
                CountrySchemeTypeId = (int)oio.scheme,
            };
        }
    }
}
