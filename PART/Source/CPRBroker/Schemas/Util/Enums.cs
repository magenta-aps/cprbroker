using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part.Enums;

namespace CPRBroker.Schemas.Util
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

        public static LifeStatus ToLifeStatus(decimal civilRegistrationStatus)
        {
            switch (Convert.ToInt32(civilRegistrationStatus))
            {
                case 70:
                    return LifeStatus.disappeared;
                case 90:
                    return LifeStatus.dead;
                default:
                    return LifeStatus.born;
            }
        }
        public static Part.Enums.MaritalStatus ToPartMaritalStatus(char code)
        {
            switch (char.ToUpper(code))
            {
                case 'U':
                    return MaritalStatus.single;
                case 'G':
                    return MaritalStatus.married;
                case 'F':
                    return MaritalStatus.divorced;
                case 'D':
                    return MaritalStatus.deceased;
                case 'E':
                    return MaritalStatus.widow;
                case 'P':
                    return MaritalStatus.registeredpartner;
                case 'O':
                    return MaritalStatus.repealedpartnership;
                case 'L':
                default:
                    return MaritalStatus.surviving;
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
