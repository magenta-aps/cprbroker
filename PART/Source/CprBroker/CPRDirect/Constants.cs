using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public static class Constants
    {
        static Constants()
        {
            _ErrorCodes = new Dictionary<string, string>();
            _ErrorCodes["01"] = "Incorrect user ID / remote server)";
            _ErrorCodes["02"] = "Remote server expired, new remote server required";
            _ErrorCodes["03"] = "New remote server does not meet the format (8 characters, at least 2 numbers and 2 letters and not previously used)";
            _ErrorCodes["04"] = "No access to CPR";
            _ErrorCodes["05"] = "PNR unknown in CPR";
            _ErrorCodes["06"] = "Unknown Customer Number";
            _ErrorCodes["07"] = "Timeout - new LOGON required";
            _ErrorCodes["08"] = "'DEAD-LOCK' while reading the CPR system";
            _ErrorCodes["09"] = "Serious problem. Meaning: There is no connection between the client and CPR system; contact CSC Service Center by phone 36146192";
            _ErrorCodes["10"] = "ABON_TYPE unknown";
            _ErrorCodes["11"] = "DATA_TYPE unknown";
            _ErrorCodes["12"] = "(reserved error number)";
            _ErrorCodes["13"] = "(reserved error number)";
            _ErrorCodes["14"] = "(reserved error number)";
            _ErrorCodes["15"] = "(reserved error number)";
            _ErrorCodes["16"] = "IP address is incorrect";

            for (int err = 19; err < 100; err++)
            {
                _ErrorCodes[err.ToString()] = "(reserved error number)";
            }
        }

        public static Dictionary<string, Type> DataObjectMap
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

        public const int DataObjectCodeLength = 3;

        public static Encoding DefaultEncoding
        {
            get { return Encoding.UTF7; }
        }

        private static Dictionary<string, string> _ErrorCodes;
        public static Dictionary<string, string> ErrorCodes
        {
            get { return new Dictionary<string, string>(_ErrorCodes); }
        }

        public static readonly Guid ActorId = new Guid("{2B2C1518-F466-491F-8149-57AFEF48CC01}");
        public static readonly string CommentText = "";
    }
}
