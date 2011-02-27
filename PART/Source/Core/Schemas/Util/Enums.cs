using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Schemas.Util
{
    /// <summary>
    /// Contains some utility methods related to OIO enumerations
    /// </summary>
    public static class Enums
    {
       
        public static LivStatusKodeType ToLifeStatus(decimal civilRegistrationStatus, DateTime? birthDate)
        {
            switch ((int)civilRegistrationStatus)
            {
                case 70:
                    return LivStatusKodeType.Forsvundet;
                case 90:
                    return LivStatusKodeType.Doed;
                default:
                    if (birthDate.HasValue)
                        return LivStatusKodeType.Foedt;
                    else
                        return LivStatusKodeType.Prenatal;
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
