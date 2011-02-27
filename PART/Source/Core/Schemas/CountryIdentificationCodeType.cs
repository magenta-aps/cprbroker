using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class CountryIdentificationCodeType
    {
        public static CountryIdentificationCodeType Create(_CountryIdentificationSchemeType codeScheme, string code)
        {
            return new CountryIdentificationCodeType()
            {
                scheme = codeScheme,
                Value = code,
            };
        }
    }
}
