using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public static class Constants
    {
        public static Dictionary<string, Type> ChildMap
        {
            get
            {
                var ret = new Dictionary<string, Type>();
                ret["000"] = typeof(StartRecordType);
                ret["001"] = typeof(PersonInformationType);
                ret["002"] = typeof(CurrentAddressInformationType);
                ret["003"] = typeof(ClearWrittenAddressType);
                ret["004"] = typeof(ProtectionType);
                ret["005"] = typeof(CurrentDepartureDataType);
                ret["006"] = typeof(ContactAddressType);
                ret["007"] = typeof(CurrentDisappearanceInformationType);
                ret["008"] = typeof(CurrentNameInformationType);
                ret["009"] = typeof(BirthRegistrationInformationType);
                ret["010"] = typeof(CurrentCitizenshipType);
                ret["011"] = typeof(ChurchInformationType);
                ret["012"] = typeof(CurrentCivilStatusType);
                ret["013"] = typeof(CurrentSeparationType);
                ret["014"] = typeof(ChildType);
                ret["015"] = typeof(ParentsInformationType);
                ret["016"] = typeof(ParentalAuthorityType);
                ret["017"] = typeof(DisempowermentType);
                ret["018"] = typeof(MunicipalConditionsType);
                ret["999"] = typeof(EndRecordType);
                return ret;
            }
        }
    }
}
