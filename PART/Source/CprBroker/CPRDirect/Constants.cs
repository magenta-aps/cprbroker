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


            _DataObjectMap = new Dictionary<string, Type>();
            _DataObjectMap["000"] = typeof(StartRecordType);
            _DataObjectMap["001"] = typeof(PersonInformationType);
            _DataObjectMap["002"] = typeof(CurrentAddressInformationType);
            _DataObjectMap["003"] = typeof(ClearWrittenAddressType);
            _DataObjectMap["004"] = typeof(ProtectionType);
            _DataObjectMap["005"] = typeof(CurrentDepartureDataType);
            _DataObjectMap["006"] = typeof(ContactAddressType);
            _DataObjectMap["007"] = typeof(CurrentDisappearanceInformationType);
            _DataObjectMap["008"] = typeof(CurrentNameInformationType);
            _DataObjectMap["009"] = typeof(BirthRegistrationInformationType);
            _DataObjectMap["010"] = typeof(CurrentCitizenshipType);
            _DataObjectMap["011"] = typeof(ChurchInformationType);
            _DataObjectMap["012"] = typeof(CurrentCivilStatusType);
            _DataObjectMap["013"] = typeof(CurrentSeparationType);
            _DataObjectMap["014"] = typeof(ChildType);
            _DataObjectMap["015"] = typeof(ParentsInformationType);
            _DataObjectMap["016"] = typeof(ParentalAuthorityType);
            _DataObjectMap["017"] = typeof(DisempowermentType);
            _DataObjectMap["018"] = typeof(MunicipalConditionsType);
            _DataObjectMap["026"] = typeof(HistoricalNameType);
            _DataObjectMap["029"] = typeof(HistoricalCivilStatusType);
            _DataObjectMap["999"] = typeof(EndRecordType);

            _ReversibleRelationshipMap = new Dictionary<string, bool>();
            foreach (var kvp in _DataObjectMap)
            {
                var type = kvp.Value;
                _ReversibleRelationshipMap[kvp.Key] = typeof(IReversibleRelationship).IsAssignableFrom(type);
            }
        }

        private static Dictionary<string, Type> _DataObjectMap;
        public static Dictionary<string, Type> DataObjectMap
        {
            get { return _DataObjectMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }


        private static Dictionary<string, bool> _ReversibleRelationshipMap;
        public static Dictionary<string, bool> ReversibleRelationshipMap
        {
            get { return _ReversibleRelationshipMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        public const int DataObjectCodeLength = 3;

        public const int StartRecordCode = 0;
        public const int EndRecordCode = 999;

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
        public static readonly short DenmarkCountryCode = 5100;

        public static class CorrectionMarker
        {
            public const char Edit_Overwritten = 'K';
            public const char Undo = 'A';
            public const char TechnicalChange = 'Æ';
            public const char OK = ' ';
        }

        public static class PropertyNames
        {
            public static readonly string Address = "Address";
            public static readonly string Port = "Port";
            public static readonly string PutSubscription = "Put subscription";
            public static readonly string ExtractsFolder = "Extracts folder";
        }
    }
}
