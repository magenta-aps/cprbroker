using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Part.Enums;

namespace CprBroker.Schemas.Util
{
    /// <summary>
    /// Contains some utility methods related to OIO enumerations
    /// </summary>
    public static class Enums
    {
        public static PersonCivilRegistrationStatusCodeType ToCivilRegistrationStatus(decimal status)
        {
            return ToCivilRegistrationStatus(status.ToString());
        }
        public static PersonCivilRegistrationStatusCodeType ToCivilRegistrationStatus(string status)
        {
            return (PersonCivilRegistrationStatusCodeType)Enum.Parse(
                    typeof(PersonCivilRegistrationStatusCodeType),
                    "Item" + Convert.ToInt32(status).ToString("D2")
                    );
        }

        public static MaritalStatusCodeType GetMaritalStatus(char code)
        {
            switch (char.ToUpper(code))
            {
                case 'U':
                    return MaritalStatusCodeType.unmarried;
                case 'G':
                    return MaritalStatusCodeType.married;
                case 'F':
                    return MaritalStatusCodeType.divorced;
                case 'D':
                    return MaritalStatusCodeType.deceased;
                case 'E':
                    return MaritalStatusCodeType.widow;
                case 'P':
                    return MaritalStatusCodeType.registeredpartnership;
                case 'O':
                    return MaritalStatusCodeType.abolitionofregistreredpartnership;
                case 'L':
                default:
                    return MaritalStatusCodeType.longestlivingpartner;
            }
        }

        public static LivStatusKode ToLifeStatus(decimal civilRegistrationStatus, DateTime? birthDate)
        {
            switch ((int)civilRegistrationStatus)
            {
                case 70:
                    return LivStatusKode.Forsvundet;
                case 90:
                    return LivStatusKode.Doed;
                default:
                    if (birthDate.HasValue)
                        return LivStatusKode.Foedt;
                    else
                        return LivStatusKode.Prenatal;
            }
        }

        public static KeyValuePair<string, TEnum>[] GetEnumValues<TEnum>() where TEnum : struct
        {
            Type t = typeof(TEnum);
            var names = Enum.GetNames(t);

            Func<string, string> valueGetter =
                (name) =>
                {
                    TEnum enumValue = (TEnum)Enum.Parse(t, name);
                    string ret = ((int)(object)enumValue).ToString();
                    System.Reflection.FieldInfo fieldInfo = t.GetField(name);
                    System.Xml.Serialization.XmlEnumAttribute attr = fieldInfo.GetCustomAttributes(typeof(System.Xml.Serialization.XmlEnumAttribute), false).SingleOrDefault() as System.Xml.Serialization.XmlEnumAttribute;
                    if (attr != null)
                    {
                        ret = attr.Name;
                    }
                    return ret;
                };
            return (from name in names select new KeyValuePair<string, TEnum>(valueGetter(name), (TEnum)Enum.Parse(t, name))).ToArray();

        }
    }
}
