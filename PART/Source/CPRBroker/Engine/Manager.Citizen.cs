using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.Engine
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public static class Citizen
        {
            public static PersonNameAndAddressStructureType GetCitizenNameAndAddress(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPersonNameAndAddressDataProvider, PersonNameAndAddressStructureType>
                (userToken, appToken, true, true, (prov) => prov.GetCitizenNameAndAddress(userToken, appToken, cprNumber, out ql), true, (oioPerson) => Local.UpdateDatabase.UpdateCitizenNameAndAddress(oioPerson));

                qualityLevel = ql;
                return ret;
            }

            public static PersonBasicStructureType GetCitizenBasic(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPersonBasicDataProvider, PersonBasicStructureType>
                (userToken, appToken, true, true, (prov) => prov.GetCitizenBasic(userToken, appToken, cprNumber, out ql), true, (oioPerson) => Local.UpdateDatabase.UpdateCitizenBasic(oioPerson));
                qualityLevel = ql;
                return ret;
            }

            public static PersonFullStructureType GetCitizenFull(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
            {
                return GetCitizenFull(userToken, appToken, true, cprNumber, out qualityLevel);
            }

            public static PersonFullStructureType GetCitizenFull(string userToken, string appToken, bool allowLocalProvider, string cprNumber, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPersonFullDataProvider, PersonFullStructureType>
                (userToken, appToken, true, allowLocalProvider, (prov) => prov.GetCitizenFull(userToken, appToken, cprNumber, out ql), true, (oioPerson) => Local.UpdateDatabase.UpdateCitizenFull(oioPerson));
                qualityLevel = ql;
                return ret;
            }

            public static SimpleCPRPersonType[] GetCitizenChildren(string userToken, string appToken, string cprNumber, bool includeCustodies, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPersonChildrenDataProvider, SimpleCPRPersonType[]>
                (userToken, appToken, true, true, (prov) => prov.GetCitizenChildren(userToken, appToken, cprNumber, includeCustodies, out ql), true, (children) => Local.UpdateDatabase.UpdateCitizenChildren(cprNumber, children));
                qualityLevel = ql;
                return ret;
            }

            public static PersonRelationsType GetCitizenRelations(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
            {
                QualityLevel? ql = null;
                var ret = CallMethod<IPersonRelationsDataProvider, PersonRelationsType>
                (userToken, appToken, true, true, (prov) => prov.GetCitizenRelations(userToken, appToken, cprNumber, out ql), true, (relations) => Local.UpdateDatabase.UpdateCitizenRelations(cprNumber, relations));
                qualityLevel = ql;
                return ret;
            }

            public static bool RemoveParentAuthorityOverChild(string userToken, string appToken, string cprNumber, string cprChildNumber)
            {
                return CallMethod<IPersonCustodyDataProvider, bool>
                (userToken, appToken, true, true, (prov) => prov.RemoveParentAuthorityOverChild(userToken, appToken, cprNumber, cprChildNumber), true, null);
            }

            public static bool SetParentAuthorityOverChild(string userToken, string appToken, string cprNumber, string cprChildNumber)
            {
                return CallMethod<IPersonCustodyDataProvider, bool>
                (userToken, appToken, true, true, (prov) => prov.SetParentAuthorityOverChild(userToken, appToken, cprNumber, cprChildNumber), true, null);
            }

            public static ParentAuthorityRelationshipType[] GetParentAuthorityOverChildChanges(string userToken, string appToken, string cprChildNumber)
            {
                return CallMethod<IPersonCustodyDataProvider, ParentAuthorityRelationshipType[]>
                (userToken, appToken, true, true, (prov) => prov.GetParentAuthorityOverChildChanges(userToken, appToken, cprChildNumber), true, null);
            }
        }
    }
}
