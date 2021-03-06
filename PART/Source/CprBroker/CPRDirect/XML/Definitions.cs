
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CprBroker.Schemas.Wrappers;
    
    namespace CprBroker.Providers.CPRDirect
    {
    
    public partial class IndividualRequestType: Wrapper
    {
        #region Common

        public override int Length
        {
            get { return 39; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// CPR's transaction code
        ///  </summary>
        public string CPRTRANS
        {
            get { return this.GetString(1, 4); }
            set { this.SetString(value, 1, 4); }
        }
        ///  <summary>
        /// Danish: KOMMA
        /// Comma character used as separator
        ///  </summary>
        public char Comma
        {
            get { return this.GetChar(5); }
            set { this.SetChar(value, 5); }
        }
        ///  <summary>
        /// Danish: KUNDENR
        /// Identification of the customer
        ///  </summary>
        public decimal CustomerNumber
        {
            get { return this.GetDecimal(6, 4); }
            set { this.SetDecimal(value, 6, 4); }
        }
        ///  <summary>
        /// Danish: ABON_TYPE
        /// Subscription phrase / delete or not
        ///  </summary>
        public decimal SubscriptionType
        {
            get { return this.GetDecimal(10, 1); }
            set { this.SetDecimal(value, 10, 1); }
        }
        ///  <summary>
        /// Danish: DATA_TYPE
        /// Desired output - 0 in LOGONINDIVID
        ///  </summary>
        public decimal DataType
        {
            get { return this.GetDecimal(11, 1); }
            set { this.SetDecimal(value, 11, 1); }
        }
        ///  <summary>
        /// Danish: TOKEN
        ///  </summary>
        public string Token
        {
            get { return this.GetString(12, 8); }
            set { this.SetString(value, 12, 8); }
        }
        ///  <summary>
        /// Danish: BRUGER_ID
        /// The CPR Unit assigned system user code
        ///  </summary>
        public string UserId
        {
            get { return this.GetString(20, 8); }
            set { this.SetString(value, 20, 8); }
        }
        ///  <summary>
        /// Danish: FEJLNR
        /// Indicator of the communication process
        ///  </summary>
        public decimal ErrorNumber
        {
            get { return this.GetDecimal(28, 2); }
            set { this.SetDecimal(value, 28, 2); }
        }
        ///  <summary>
        /// Danish: PNR
        /// Request PNR
        ///  </summary>
        public decimal PNR
        {
            get { return this.GetDecimal(30, 10); }
            set { this.SetDecimal(value, 30, 10); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("CPRTRANS", 1, 4),
                    new Tuple<string, int, int>("Comma", 5, 1),
                    new Tuple<string, int, int>("CustomerNumber", 6, 4),
                    new Tuple<string, int, int>("SubscriptionType", 10, 1),
                    new Tuple<string, int, int>("DataType", 11, 1),
                    new Tuple<string, int, int>("Token", 12, 8),
                    new Tuple<string, int, int>("UserId", 20, 8),
                    new Tuple<string, int, int>("ErrorNumber", 28, 2),
                    new Tuple<string, int, int>("PNR", 30, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class IndividualResponseType: CompositeWrapper
    {
        #region Common

        public override int Length
        {
            get { return 0; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: KUNDENR
        /// Identification of the customer
        ///  </summary>
        public decimal CustomerNumber
        {
            get { return this.GetDecimal(1, 4); }
            set { this.SetDecimal(value, 1, 4); }
        }
        ///  <summary>
        /// Danish: ABON_TYPE
        /// Subscription phrase / delete or not
        ///  </summary>
        public decimal SubscriptionType
        {
            get { return this.GetDecimal(5, 1); }
            set { this.SetDecimal(value, 5, 1); }
        }
        ///  <summary>
        /// Danish: DATA_TYPE
        /// 0 in LOGONINDIVID (see Annex 2)Desired output
        ///  </summary>
        public decimal DataType
        {
            get { return this.GetDecimal(6, 1); }
            set { this.SetDecimal(value, 6, 1); }
        }
        ///  <summary>
        /// Danish: TOKEN
        /// Taken from the logon
        ///  </summary>
        public string Token
        {
            get { return this.GetString(7, 8); }
            set { this.SetString(value, 7, 8); }
        }
        ///  <summary>
        /// Danish: BRUGER-ID
        /// The CPR Unit assigned system user code
        ///  </summary>
        public string UserId
        {
            get { return this.GetString(15, 8); }
            set { this.SetString(value, 15, 8); }
        }
        ///  <summary>
        /// Danish: FEJLNR
        /// Indicator of the communication process
        ///  </summary>
        public decimal ErrorNumber
        {
            get { return this.GetDecimal(23, 2); }
            set { this.SetDecimal(value, 23, 2); }
        }
        ///  <summary>
        /// Length of structure 28 + data MAX 2880
        ///  </summary>
        public decimal DataLength
        {
            get { return this.GetDecimal(25, 4); }
            set { this.SetDecimal(value, 25, 4); }
        }
        ///  <summary>
        /// Danish: DATA
        /// Personal data from the CPR (format and amount depends on DATA_TYPE
        ///  </summary>
        public string Data
        {
            get { return this.GetString(29, 2880); }
            set { this.SetString(value, 29, 2880); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("CustomerNumber", 1, 4),
                    new Tuple<string, int, int>("SubscriptionType", 5, 1),
                    new Tuple<string, int, int>("DataType", 6, 1),
                    new Tuple<string, int, int>("Token", 7, 8),
                    new Tuple<string, int, int>("UserId", 15, 8),
                    new Tuple<string, int, int>("ErrorNumber", 23, 2),
                    new Tuple<string, int, int>("DataLength", 25, 4),
                    new Tuple<string, int, int>("Data", 29, 2880)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

        #region Sub objects

        private StartRecordType _StartRecord = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public StartRecordType StartRecord
        {
            get { return _StartRecord; }
            set { _StartRecord = value; }
        }

        private PersonInformationType _PersonInformation = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public PersonInformationType PersonInformation
        {
            get { return _PersonInformation; }
            set { _PersonInformation = value; }
        }

        private CurrentAddressInformationType _CurrentAddressInformation = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public CurrentAddressInformationType CurrentAddressInformation
        {
            get { return _CurrentAddressInformation; }
            set { _CurrentAddressInformation = value; }
        }

        private ClearWrittenAddressType _ClearWrittenAddress = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public ClearWrittenAddressType ClearWrittenAddress
        {
            get { return _ClearWrittenAddress; }
            set { _ClearWrittenAddress = value; }
        }

        private List<ProtectionType> _Protection = new List<ProtectionType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 4)]
        public List<ProtectionType> Protection
        {
            get { return _Protection; }
            set { _Protection = value; }
        }

        private CurrentDepartureDataType _CurrentDepartureData = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public CurrentDepartureDataType CurrentDepartureData
        {
            get { return _CurrentDepartureData; }
            set { _CurrentDepartureData = value; }
        }

        private ContactAddressType _ContactAddress = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public ContactAddressType ContactAddress
        {
            get { return _ContactAddress; }
            set { _ContactAddress = value; }
        }

        private CurrentDisappearanceInformationType _CurrentDisappearanceInformation = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public CurrentDisappearanceInformationType CurrentDisappearanceInformation
        {
            get { return _CurrentDisappearanceInformation; }
            set { _CurrentDisappearanceInformation = value; }
        }

        private CurrentNameInformationType _CurrentNameInformation = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public CurrentNameInformationType CurrentNameInformation
        {
            get { return _CurrentNameInformation; }
            set { _CurrentNameInformation = value; }
        }

        private BirthRegistrationInformationType _BirthRegistrationInformation = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public BirthRegistrationInformationType BirthRegistrationInformation
        {
            get { return _BirthRegistrationInformation; }
            set { _BirthRegistrationInformation = value; }
        }

        private CurrentCitizenshipType _CurrentCitizenship = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public CurrentCitizenshipType CurrentCitizenship
        {
            get { return _CurrentCitizenship; }
            set { _CurrentCitizenship = value; }
        }

        private ChurchInformationType _ChurchInformation = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public ChurchInformationType ChurchInformation
        {
            get { return _ChurchInformation; }
            set { _ChurchInformation = value; }
        }

        private CurrentCivilStatusType _CurrentCivilStatus = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public CurrentCivilStatusType CurrentCivilStatus
        {
            get { return _CurrentCivilStatus; }
            set { _CurrentCivilStatus = value; }
        }

        private CurrentSeparationType _CurrentSeparation = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public CurrentSeparationType CurrentSeparation
        {
            get { return _CurrentSeparation; }
            set { _CurrentSeparation = value; }
        }

        private List<ChildType> _Child = new List<ChildType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 25)]
        public List<ChildType> Child
        {
            get { return _Child; }
            set { _Child = value; }
        }

        private ParentsInformationType _ParentsInformation = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public ParentsInformationType ParentsInformation
        {
            get { return _ParentsInformation; }
            set { _ParentsInformation = value; }
        }

        private List<ParentalAuthorityType> _ParentalAuthority = new List<ParentalAuthorityType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 2)]
        public List<ParentalAuthorityType> ParentalAuthority
        {
            get { return _ParentalAuthority; }
            set { _ParentalAuthority = value; }
        }

        private DisempowermentType _Disempowerment = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public DisempowermentType Disempowerment
        {
            get { return _Disempowerment; }
            set { _Disempowerment = value; }
        }

        private List<MunicipalConditionsType> _MunicipalConditions = new List<MunicipalConditionsType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<MunicipalConditionsType> MunicipalConditions
        {
            get { return _MunicipalConditions; }
            set { _MunicipalConditions = value; }
        }

        private List<NotesType> _Notes = new List<NotesType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<NotesType> Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }

        private List<ElectionInformationType> _ElectionInformation = new List<ElectionInformationType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<ElectionInformationType> ElectionInformation
        {
            get { return _ElectionInformation; }
            set { _ElectionInformation = value; }
        }

        private RelocationOrderType _RelocationOrder = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public RelocationOrderType RelocationOrder
        {
            get { return _RelocationOrder; }
            set { _RelocationOrder = value; }
        }

        private List<HistoricalPNRType> _HistoricalPNR = new List<HistoricalPNRType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalPNRType> HistoricalPNR
        {
            get { return _HistoricalPNR; }
            set { _HistoricalPNR = value; }
        }

        private List<HistoricalAddressType> _HistoricalAddress = new List<HistoricalAddressType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000)]
        public List<HistoricalAddressType> HistoricalAddress
        {
            get { return _HistoricalAddress; }
            set { _HistoricalAddress = value; }
        }

        private List<HistoricalDepartureType> _HistoricalDeparture = new List<HistoricalDepartureType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalDepartureType> HistoricalDeparture
        {
            get { return _HistoricalDeparture; }
            set { _HistoricalDeparture = value; }
        }

        private List<HistoricalDisappearanceType> _HistoricalDisappearance = new List<HistoricalDisappearanceType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalDisappearanceType> HistoricalDisappearance
        {
            get { return _HistoricalDisappearance; }
            set { _HistoricalDisappearance = value; }
        }

        private List<HistoricalNameType> _HistoricalName = new List<HistoricalNameType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalNameType> HistoricalName
        {
            get { return _HistoricalName; }
            set { _HistoricalName = value; }
        }

        private List<HistoricalCitizenshipType> _HistoricalCitizenship = new List<HistoricalCitizenshipType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalCitizenshipType> HistoricalCitizenship
        {
            get { return _HistoricalCitizenship; }
            set { _HistoricalCitizenship = value; }
        }

        private List<HistoricalChurchInformationType> _HistoricalChurchInformation = new List<HistoricalChurchInformationType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalChurchInformationType> HistoricalChurchInformation
        {
            get { return _HistoricalChurchInformation; }
            set { _HistoricalChurchInformation = value; }
        }

        private List<HistoricalCivilStatusType> _HistoricalCivilStatus = new List<HistoricalCivilStatusType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalCivilStatusType> HistoricalCivilStatus
        {
            get { return _HistoricalCivilStatus; }
            set { _HistoricalCivilStatus = value; }
        }

        private List<HistoricalSeparationType> _HistoricalSeparation = new List<HistoricalSeparationType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<HistoricalSeparationType> HistoricalSeparation
        {
            get { return _HistoricalSeparation; }
            set { _HistoricalSeparation = value; }
        }

        private _RelativeClearWrittenAddressType __RelativeClearWrittenAddress = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public _RelativeClearWrittenAddressType _RelativeClearWrittenAddress
        {
            get { return __RelativeClearWrittenAddress; }
            set { __RelativeClearWrittenAddress = value; }
        }

        private MotherWithClearWrittenAddressType _MotherWithClearWrittenAddress = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public MotherWithClearWrittenAddressType MotherWithClearWrittenAddress
        {
            get { return _MotherWithClearWrittenAddress; }
            set { _MotherWithClearWrittenAddress = value; }
        }

        private FatherWithClearWrittenAddressType _FatherWithClearWrittenAddress = null;

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1)]
        public FatherWithClearWrittenAddressType FatherWithClearWrittenAddress
        {
            get { return _FatherWithClearWrittenAddress; }
            set { _FatherWithClearWrittenAddress = value; }
        }

        private List<ChildWithClearWrittenAddressType> _ChildWithClearWrittenAddress = new List<ChildWithClearWrittenAddressType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 25)]
        public List<ChildWithClearWrittenAddressType> ChildWithClearWrittenAddress
        {
            get { return _ChildWithClearWrittenAddress; }
            set { _ChildWithClearWrittenAddress = value; }
        }

        private List<EventsType> _Events = new List<EventsType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<EventsType> Events
        {
            get { return _Events; }
            set { _Events = value; }
        }

        private List<ErrorRecordType> _ErrorRecord = new List<ErrorRecordType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<ErrorRecordType> ErrorRecord
        {
            get { return _ErrorRecord; }
            set { _ErrorRecord = value; }
        }

        private List<SubscriptionDeletionReceiptType> _SubscriptionDeletionReceipt = new List<SubscriptionDeletionReceiptType>();

        [MinMaxOccurs(MinOccurs = 0, MaxOccurs = 1000000)]
        public List<SubscriptionDeletionReceiptType> SubscriptionDeletionReceipt
        {
            get { return _SubscriptionDeletionReceipt; }
            set { _SubscriptionDeletionReceipt = value; }
        }

        private EndRecordType _EndRecord = null;

        [MinMaxOccurs(MinOccurs = 1, MaxOccurs = 1)]
        public EndRecordType EndRecord
        {
            get { return _EndRecord; }
            set { _EndRecord = value; }
        }


        #endregion
    
    }

  
    public partial class StartRecordType: Wrapper
    {
        #region Common

        public override int Length
        {
            get { return 35; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: SORTFELT-10
        /// BLACK BOX-10
        ///  </summary>
        public string BlackBox10
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: OPGAVENR
        ///  </summary>
        public decimal TaskNumber
        {
            get { return this.GetDecimal(14, 6); }
            set { this.SetDecimal(value, 14, 6); }
        }
        ///  <summary>
        /// Danish: PRODDTO
        /// Production date yyyyMMdd
        ///  </summary>
        public DateTime? ProductionDate
        {
            get { return this.GetDateTime(20, 8, "yyyyMMdd"); }
            set { this.SetDateTime(value, 20, 8, "yyyyMMdd"); }
        }

        public Decimal ProductionDateDecimal
        {
            get { return this.GetDecimal(20, 8); }
            set { this.SetDecimal(value, 20, 8); }
        }

        ///  <summary>
        /// Danish: PRODDTOFORRIG
        /// Previous production date yyyyMMdd
        ///  </summary>
        public DateTime? PreviousProductionDate
        {
            get { return this.GetDateTime(28, 8, "yyyyMMdd"); }
            set { this.SetDateTime(value, 28, 8, "yyyyMMdd"); }
        }

        public Decimal PreviousProductionDateDecimal
        {
            get { return this.GetDecimal(28, 8); }
            set { this.SetDecimal(value, 28, 8); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("BlackBox10", 4, 10),
                    new Tuple<string, int, int>("TaskNumber", 14, 6),
                    new Tuple<string, int, int>("ProductionDate", 20, 8),
                    new Tuple<string, int, int>("PreviousProductionDate", 28, 8)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class PersonInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 106; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: PNRGAELD
        /// Current CPR Number
        ///  </summary>
        public string CurrentCprNumber
        {
            get { return this.GetString(14, 10); }
            set { this.SetString(value, 14, 10); }
        }
        ///  <summary>
        /// Danish: STATUS
        /// Status
        ///  </summary>
        public decimal Status
        {
            get { return this.GetDecimal(24, 2); }
            set { this.SetDecimal(value, 24, 2); }
        }
        ///  <summary>
        /// Danish: STATUSHAENSTART
        /// Status date yyyyMMddHHmm
        ///  </summary>
        public DateTime? StatusStartDate
        {
            get { return this.GetDateTime(26, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 26, 12, "yyyyMMddHHmm"); }
        }

        public Decimal StatusStartDateDecimal
        {
            get { return this.GetDecimal(26, 12); }
            set { this.SetDecimal(value, 26, 12); }
        }

        ///  <summary>
        /// Danish: STATUSDTO_UMRK
        /// Status date uncertainty marker
        ///  </summary>
        public char StatusDateUncertainty
        {
            get { return this.GetChar(38); }
            set { this.SetChar(value, 38); }
        }
        ///  <summary>
        /// Danish: KOEN
        /// CPR Number
        ///  </summary>
        public char Gender
        {
            get { return this.GetChar(39); }
            set { this.SetChar(value, 39); }
        }
        ///  <summary>
        /// Danish: FOED_DT
        /// Birth date yyyy-MM-dd
        ///  </summary>
        public DateTime? Birthdate
        {
            get { return this.GetDateTime(40, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 40, 10, "yyyy-MM-dd"); }
        }

        public Decimal BirthdateDecimal
        {
            get { return this.GetDecimal(40, 10); }
            set { this.SetDecimal(value, 40, 10); }
        }

        ///  <summary>
        /// Danish: FOED_DT_UMRK
        /// Birth date uncertainty marker
        ///  </summary>
        public char BirthdateUncertainty
        {
            get { return this.GetChar(50); }
            set { this.SetChar(value, 50); }
        }
        ///  <summary>
        /// Danish: START_DT-PERSON
        /// Person start date yyyy-MM-dd
        ///  </summary>
        public DateTime? PersonStartDate
        {
            get { return this.GetDateTime(51, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 51, 10, "yyyy-MM-dd"); }
        }

        public Decimal PersonStartDateDecimal
        {
            get { return this.GetDecimal(51, 10); }
            set { this.SetDecimal(value, 51, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-PERSON
        /// Start date uncertainty marker
        ///  </summary>
        public char PersonStartDateUncertainty
        {
            get { return this.GetChar(61); }
            set { this.SetChar(value, 61); }
        }
        ///  <summary>
        /// Danish: SLUT_DT-PERSON
        /// Person end date yyyy-MM-dd
        ///  </summary>
        public DateTime? PersonEndDate
        {
            get { return this.GetDateTime(62, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 62, 10, "yyyy-MM-dd"); }
        }

        public Decimal PersonEndDateDecimal
        {
            get { return this.GetDecimal(62, 10); }
            set { this.SetDecimal(value, 62, 10); }
        }

        ///  <summary>
        /// Danish: PNR
        /// End date uncertainty marker
        ///  </summary>
        public char PersonEndDateUncertainty
        {
            get { return this.GetChar(72); }
            set { this.SetChar(value, 72); }
        }
        ///  <summary>
        /// Danish: STILLING
        /// Job
        ///  </summary>
        public string Job
        {
            get { return this.GetString(73, 34); }
            set { this.SetString(value, 73, 34); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CurrentCprNumber", 14, 10),
                    new Tuple<string, int, int>("Status", 24, 2),
                    new Tuple<string, int, int>("StatusStartDate", 26, 12),
                    new Tuple<string, int, int>("StatusDateUncertainty", 38, 1),
                    new Tuple<string, int, int>("Gender", 39, 1),
                    new Tuple<string, int, int>("Birthdate", 40, 10),
                    new Tuple<string, int, int>("BirthdateUncertainty", 50, 1),
                    new Tuple<string, int, int>("PersonStartDate", 51, 10),
                    new Tuple<string, int, int>("PersonStartDateUncertainty", 61, 1),
                    new Tuple<string, int, int>("PersonEndDate", 62, 10),
                    new Tuple<string, int, int>("PersonEndDateUncertainty", 72, 1),
                    new Tuple<string, int, int>("Job", 73, 34)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentAddressInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 306; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: KOMKOD
        /// Municipality
        ///  </summary>
        public decimal MunicipalityCode
        {
            get { return this.GetDecimal(14, 4); }
            set { this.SetDecimal(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: VEJKOD
        /// Street
        ///  </summary>
        public decimal StreetCode
        {
            get { return this.GetDecimal(18, 4); }
            set { this.SetDecimal(value, 18, 4); }
        }
        ///  <summary>
        /// Danish: HUSNR
        /// House
        ///  </summary>
        public string HouseNumber
        {
            get { return this.GetString(22, 4); }
            set { this.SetString(value, 22, 4); }
        }
        ///  <summary>
        /// Danish: ETAGE
        /// Floor
        ///  </summary>
        public string Floor
        {
            get { return this.GetString(26, 2); }
            set { this.SetString(value, 26, 2); }
        }
        ///  <summary>
        /// Danish: SIDEDOER
        /// Door
        ///  </summary>
        public string Door
        {
            get { return this.GetString(28, 4); }
            set { this.SetString(value, 28, 4); }
        }
        ///  <summary>
        /// Danish: BNR
        /// Building number
        ///  </summary>
        public string BuildingNumber
        {
            get { return this.GetString(32, 4); }
            set { this.SetString(value, 32, 4); }
        }
        ///  <summary>
        /// Danish: CONVN
        /// C/O name
        ///  </summary>
        public string CareOfName
        {
            get { return this.GetString(36, 34); }
            set { this.SetString(value, 36, 34); }
        }
        ///  <summary>
        /// Danish: TILFLYDTO
        /// Relocation date yyyyMMddTTMM
        ///  </summary>
        public DateTime? RelocationDate
        {
            get { return this.GetDateTime(70, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 70, 12, "yyyyMMddHHmm"); }
        }

        public Decimal RelocationDateDecimal
        {
            get { return this.GetDecimal(70, 12); }
            set { this.SetDecimal(value, 70, 12); }
        }

        ///  <summary>
        /// Danish: TILFLYDT_UMRK
        /// Relocation date uncertainty marker
        ///  </summary>
        public char RelocationDateUncertainty
        {
            get { return this.GetChar(82); }
            set { this.SetChar(value, 82); }
        }
        ///  <summary>
        /// Danish: TILFLYKOMDTO
        /// Municipality arrival date yyyyMMddTTMM
        ///  </summary>
        public DateTime? MunicipalityArrivalDate
        {
            get { return this.GetDateTime(83, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 83, 12, "yyyyMMddHHmm"); }
        }

        public Decimal MunicipalityArrivalDateDecimal
        {
            get { return this.GetDecimal(83, 12); }
            set { this.SetDecimal(value, 83, 12); }
        }

        ///  <summary>
        /// Danish: TILFLYKOMDT_UMRK
        /// Municipality arrival date uncertainty marker
        ///  </summary>
        public char MunicipalityArrivalDateUncertainty
        {
            get { return this.GetChar(95); }
            set { this.SetChar(value, 95); }
        }
        ///  <summary>
        /// Danish: FRAFLYKOMKOD
        /// Leaving municipality code
        ///  </summary>
        public decimal LeavingMunicipalityCode
        {
            get { return this.GetDecimal(96, 4); }
            set { this.SetDecimal(value, 96, 4); }
        }
        ///  <summary>
        /// Danish: FRAFLYKOMDTO
        /// Leaving previous municipality departure date yyyyMMddTTMM
        ///  </summary>
        public DateTime? LeavingMunicipalityDepartureDate
        {
            get { return this.GetDateTime(100, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 100, 12, "yyyyMMddHHmm"); }
        }

        public Decimal LeavingMunicipalityDepartureDateDecimal
        {
            get { return this.GetDecimal(100, 12); }
            set { this.SetDecimal(value, 100, 12); }
        }

        ///  <summary>
        /// Danish: FRAFLYKOMDT_UMRK
        /// Leaving previous municipality departure date uncertainty
        ///  </summary>
        public char LeavingMunicipalityDepartureDateUncertainty
        {
            get { return this.GetChar(112); }
            set { this.SetChar(value, 112); }
        }
        ///  <summary>
        /// Danish: START_MYNKOD-ADRTXT
        /// HomeAuthority
        ///  </summary>
        public decimal HomeAuthority
        {
            get { return this.GetDecimal(113, 4); }
            set { this.SetDecimal(value, 113, 4); }
        }
        ///  <summary>
        /// Danish: ADR1-SUPLADR
        /// First line of supplementary address
        ///  </summary>
        public string SupplementaryAddress1
        {
            get { return this.GetString(117, 34); }
            set { this.SetString(value, 117, 34); }
        }
        ///  <summary>
        /// Danish: ADR2-SUPLADR
        /// Second line of supplementary address
        ///  </summary>
        public string SupplementaryAddress2
        {
            get { return this.GetString(151, 34); }
            set { this.SetString(value, 151, 34); }
        }
        ///  <summary>
        /// Danish: ADR3-SUPLADR
        /// Third line of supplementary address
        ///  </summary>
        public string SupplementaryAddress3
        {
            get { return this.GetString(185, 34); }
            set { this.SetString(value, 185, 34); }
        }
        ///  <summary>
        /// Danish: ADR4-SUPLADR
        /// Fourth line of supplementary address
        ///  </summary>
        public string SupplementaryAddress4
        {
            get { return this.GetString(219, 34); }
            set { this.SetString(value, 219, 34); }
        }
        ///  <summary>
        /// Danish: ADR5-SUPLADR
        /// Fifth line of supplementary address
        ///  </summary>
        public string SupplementaryAddress5
        {
            get { return this.GetString(253, 34); }
            set { this.SetString(value, 253, 34); }
        }
        ///  <summary>
        /// Danish: START_DT-ADRTXT
        /// Start date yyyy-MM-dd
        ///  </summary>
        public DateTime? StartDate
        {
            get { return this.GetDateTime(287, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 287, 10, "yyyy-MM-dd"); }
        }

        public Decimal StartDateDecimal
        {
            get { return this.GetDecimal(287, 10); }
            set { this.SetDecimal(value, 287, 10); }
        }

        ///  <summary>
        /// Danish: SLET_DT-ADRTXT
        /// End date yyyy-MM-dd
        ///  </summary>
        public DateTime? EndDate
        {
            get { return this.GetDateTime(297, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 297, 10, "yyyy-MM-dd"); }
        }

        public Decimal EndDateDecimal
        {
            get { return this.GetDecimal(297, 10); }
            set { this.SetDecimal(value, 297, 10); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("MunicipalityCode", 14, 4),
                    new Tuple<string, int, int>("StreetCode", 18, 4),
                    new Tuple<string, int, int>("HouseNumber", 22, 4),
                    new Tuple<string, int, int>("Floor", 26, 2),
                    new Tuple<string, int, int>("Door", 28, 4),
                    new Tuple<string, int, int>("BuildingNumber", 32, 4),
                    new Tuple<string, int, int>("CareOfName", 36, 34),
                    new Tuple<string, int, int>("RelocationDate", 70, 12),
                    new Tuple<string, int, int>("RelocationDateUncertainty", 82, 1),
                    new Tuple<string, int, int>("MunicipalityArrivalDate", 83, 12),
                    new Tuple<string, int, int>("MunicipalityArrivalDateUncertainty", 95, 1),
                    new Tuple<string, int, int>("LeavingMunicipalityCode", 96, 4),
                    new Tuple<string, int, int>("LeavingMunicipalityDepartureDate", 100, 12),
                    new Tuple<string, int, int>("LeavingMunicipalityDepartureDateUncertainty", 112, 1),
                    new Tuple<string, int, int>("HomeAuthority", 113, 4),
                    new Tuple<string, int, int>("SupplementaryAddress1", 117, 34),
                    new Tuple<string, int, int>("SupplementaryAddress2", 151, 34),
                    new Tuple<string, int, int>("SupplementaryAddress3", 185, 34),
                    new Tuple<string, int, int>("SupplementaryAddress4", 219, 34),
                    new Tuple<string, int, int>("SupplementaryAddress5", 253, 34),
                    new Tuple<string, int, int>("StartDate", 287, 10),
                    new Tuple<string, int, int>("EndDate", 297, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ClearWrittenAddressType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 249; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ADRNVN
        /// Addressing name
        ///  </summary>
        public string AddressingName
        {
            get { return this.GetString(14, 34); }
            set { this.SetString(value, 14, 34); }
        }
        ///  <summary>
        /// Danish: CONVN
        /// C/O name
        ///  </summary>
        public string CareOfName
        {
            get { return this.GetString(48, 34); }
            set { this.SetString(value, 48, 34); }
        }
        ///  <summary>
        /// Danish: LOKALITET
        /// Location
        ///  </summary>
        public string Location
        {
            get { return this.GetString(82, 34); }
            set { this.SetString(value, 82, 34); }
        }
        ///  <summary>
        /// Danish: STANDARDADR
        /// Road addressing name, house number, floor, side doors BNR. Labelled Address
        ///  </summary>
        public string LabelledAddress
        {
            get { return this.GetString(116, 34); }
            set { this.SetString(value, 116, 34); }
        }
        ///  <summary>
        /// Danish: BYNVN
        /// City name
        ///  </summary>
        public string CityName
        {
            get { return this.GetString(150, 34); }
            set { this.SetString(value, 150, 34); }
        }
        ///  <summary>
        /// Danish: POSTNR
        /// Post code
        ///  </summary>
        public decimal PostCode
        {
            get { return this.GetDecimal(184, 4); }
            set { this.SetDecimal(value, 184, 4); }
        }
        ///  <summary>
        /// Danish: POSTDISTTXT
        /// Post district text
        ///  </summary>
        public string PostDistrictText
        {
            get { return this.GetString(188, 20); }
            set { this.SetString(value, 188, 20); }
        }
        ///  <summary>
        /// Danish: KOMKOD
        /// Municipality code
        ///  </summary>
        public decimal MunicipalityCode
        {
            get { return this.GetDecimal(208, 4); }
            set { this.SetDecimal(value, 208, 4); }
        }
        ///  <summary>
        /// Danish: VEJKOD
        /// Street code
        ///  </summary>
        public decimal StreetCode
        {
            get { return this.GetDecimal(212, 4); }
            set { this.SetDecimal(value, 212, 4); }
        }
        ///  <summary>
        /// Danish: HUSNR
        /// House number
        ///  </summary>
        public string HouseNumber
        {
            get { return this.GetString(216, 4); }
            set { this.SetString(value, 216, 4); }
        }
        ///  <summary>
        /// Danish: ETAGE
        /// Floor
        ///  </summary>
        public string Floor
        {
            get { return this.GetString(220, 2); }
            set { this.SetString(value, 220, 2); }
        }
        ///  <summary>
        /// Danish: SIDEDOER
        /// Door
        ///  </summary>
        public string Door
        {
            get { return this.GetString(222, 4); }
            set { this.SetString(value, 222, 4); }
        }
        ///  <summary>
        /// Danish: BNR
        /// Building number
        ///  </summary>
        public string BuildingNumber
        {
            get { return this.GetString(226, 4); }
            set { this.SetString(value, 226, 4); }
        }
        ///  <summary>
        /// Danish: VEJADRNVN
        /// Street addressing name
        ///  </summary>
        public string StreetAddressingName
        {
            get { return this.GetString(230, 20); }
            set { this.SetString(value, 230, 20); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("AddressingName", 14, 34),
                    new Tuple<string, int, int>("CareOfName", 48, 34),
                    new Tuple<string, int, int>("Location", 82, 34),
                    new Tuple<string, int, int>("LabelledAddress", 116, 34),
                    new Tuple<string, int, int>("CityName", 150, 34),
                    new Tuple<string, int, int>("PostCode", 184, 4),
                    new Tuple<string, int, int>("PostDistrictText", 188, 20),
                    new Tuple<string, int, int>("MunicipalityCode", 208, 4),
                    new Tuple<string, int, int>("StreetCode", 212, 4),
                    new Tuple<string, int, int>("HouseNumber", 216, 4),
                    new Tuple<string, int, int>("Floor", 220, 2),
                    new Tuple<string, int, int>("Door", 222, 4),
                    new Tuple<string, int, int>("BuildingNumber", 226, 4),
                    new Tuple<string, int, int>("StreetAddressingName", 230, 20)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ProtectionType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 37; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: BESKYTTYPE
        /// Protection type
        ///  </summary>
        public decimal ProtectionType_
        {
            get { return this.GetDecimal(14, 4); }
            set { this.SetDecimal(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: START_DT-BESKYTTELSE
        /// Start date yyyy-MM-dd
        ///  </summary>
        public DateTime? StartDate
        {
            get { return this.GetDateTime(18, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 18, 10, "yyyy-MM-dd"); }
        }

        public Decimal StartDateDecimal
        {
            get { return this.GetDecimal(18, 10); }
            set { this.SetDecimal(value, 18, 10); }
        }

        ///  <summary>
        /// Danish: SLET_DT-BESKYTTELSE
        /// End date yyyy-MM-dd
        ///  </summary>
        public DateTime? EndDate
        {
            get { return this.GetDateTime(28, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 28, 10, "yyyy-MM-dd"); }
        }

        public Decimal EndDateDecimal
        {
            get { return this.GetDecimal(28, 10); }
            set { this.SetDecimal(value, 28, 10); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ProtectionType_", 14, 4),
                    new Tuple<string, int, int>("StartDate", 18, 10),
                    new Tuple<string, int, int>("EndDate", 28, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentDepartureDataType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 200; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: UDR_LANDEKOD
        /// Exit country code
        ///  </summary>
        public decimal ExitCountryCode
        {
            get { return this.GetDecimal(14, 4); }
            set { this.SetDecimal(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: UDRDTO
        /// Exit date yyyyMMddTTMM
        ///  </summary>
        public DateTime? ExitDate
        {
            get { return this.GetDateTime(18, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 18, 12, "yyyyMMddHHmm"); }
        }

        public Decimal ExitDateDecimal
        {
            get { return this.GetDecimal(18, 12); }
            set { this.SetDecimal(value, 18, 12); }
        }

        ///  <summary>
        /// Danish: UDRDTO_UMRK
        /// Exit date uncertainty marker
        ///  </summary>
        public char ExitDateUncertainty
        {
            get { return this.GetChar(30); }
            set { this.SetChar(value, 30); }
        }
        ///  <summary>
        /// Danish: UDLANDADR1
        /// Foreign Address 1
        ///  </summary>
        public string ForeignAddress1
        {
            get { return this.GetString(31, 34); }
            set { this.SetString(value, 31, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR2
        /// Foreign Address 2
        ///  </summary>
        public string ForeignAddress2
        {
            get { return this.GetString(65, 34); }
            set { this.SetString(value, 65, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR3
        /// Foreign Address 3
        ///  </summary>
        public string ForeignAddress3
        {
            get { return this.GetString(99, 34); }
            set { this.SetString(value, 99, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR4
        /// Foreign Address 4
        ///  </summary>
        public string ForeignAddress4
        {
            get { return this.GetString(133, 34); }
            set { this.SetString(value, 133, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR5
        /// Foreign Address 5
        ///  </summary>
        public string ForeignAddress5
        {
            get { return this.GetString(167, 34); }
            set { this.SetString(value, 167, 34); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ExitCountryCode", 14, 4),
                    new Tuple<string, int, int>("ExitDate", 18, 12),
                    new Tuple<string, int, int>("ExitDateUncertainty", 30, 1),
                    new Tuple<string, int, int>("ForeignAddress1", 31, 34),
                    new Tuple<string, int, int>("ForeignAddress2", 65, 34),
                    new Tuple<string, int, int>("ForeignAddress3", 99, 34),
                    new Tuple<string, int, int>("ForeignAddress4", 133, 34),
                    new Tuple<string, int, int>("ForeignAddress5", 167, 34)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ContactAddressType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 203; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ADR1-KONTAKTADR
        /// Contact address line 1
        ///  </summary>
        public string Line1
        {
            get { return this.GetString(14, 34); }
            set { this.SetString(value, 14, 34); }
        }
        ///  <summary>
        /// Danish: ADR2-KONTAKTADR
        /// Contact address line 2
        ///  </summary>
        public string Line2
        {
            get { return this.GetString(48, 34); }
            set { this.SetString(value, 48, 34); }
        }
        ///  <summary>
        /// Danish: ADR3-KONTAKTADR
        /// Contact address line 3
        ///  </summary>
        public string Line3
        {
            get { return this.GetString(82, 34); }
            set { this.SetString(value, 82, 34); }
        }
        ///  <summary>
        /// Danish: ADR4-KONTAKTADR
        /// Contact address line 4
        ///  </summary>
        public string Line4
        {
            get { return this.GetString(116, 34); }
            set { this.SetString(value, 116, 34); }
        }
        ///  <summary>
        /// Danish: ADR5-KONTAKTADR
        /// Contact address line 5
        ///  </summary>
        public string Line5
        {
            get { return this.GetString(150, 34); }
            set { this.SetString(value, 150, 34); }
        }
        ///  <summary>
        /// Danish: START_DT-ADRTXT
        /// Start date yyyy-MM-dd
        ///  </summary>
        public DateTime? StartDate
        {
            get { return this.GetDateTime(184, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 184, 10, "yyyy-MM-dd"); }
        }

        public Decimal StartDateDecimal
        {
            get { return this.GetDecimal(184, 10); }
            set { this.SetDecimal(value, 184, 10); }
        }

        ///  <summary>
        /// Danish: SLET_DT-ADRTXT
        /// End date yyyy-MM-dd
        ///  </summary>
        public DateTime? EndDate
        {
            get { return this.GetDateTime(194, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 194, 10, "yyyy-MM-dd"); }
        }

        public Decimal EndDateDecimal
        {
            get { return this.GetDecimal(194, 10); }
            set { this.SetDecimal(value, 194, 10); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("Line1", 14, 34),
                    new Tuple<string, int, int>("Line2", 48, 34),
                    new Tuple<string, int, int>("Line3", 82, 34),
                    new Tuple<string, int, int>("Line4", 116, 34),
                    new Tuple<string, int, int>("Line5", 150, 34),
                    new Tuple<string, int, int>("StartDate", 184, 10),
                    new Tuple<string, int, int>("EndDate", 194, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentDisappearanceInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 26; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: FORSVINddTO
        /// Disappearance date yyyyMMddTTMM
        ///  </summary>
        public DateTime? DisappearanceDate
        {
            get { return this.GetDateTime(14, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 14, 12, "yyyyMMddHHmm"); }
        }

        public Decimal DisappearanceDateDecimal
        {
            get { return this.GetDecimal(14, 12); }
            set { this.SetDecimal(value, 14, 12); }
        }

        ///  <summary>
        /// Danish: FORSVINDDTO_UMRK
        /// Disappearance date uncertainty marker
        ///  </summary>
        public char DisappearanceDateUncertainty
        {
            get { return this.GetChar(26); }
            set { this.SetChar(value, 26); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("DisappearanceDate", 14, 12),
                    new Tuple<string, int, int>("DisappearanceDateUncertainty", 26, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentNameInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 193; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: FORNVN
        /// First name (s)
        ///  </summary>
        public string FirstName_s
        {
            get { return this.GetString(14, 50); }
            set { this.SetString(value, 14, 50); }
        }
        ///  <summary>
        /// Danish: FORNVN_MRK
        /// First name marker
        ///  </summary>
        public char FirstNameMarker
        {
            get { return this.GetChar(64); }
            set { this.SetChar(value, 64); }
        }
        ///  <summary>
        /// Danish: MELNVN
        /// Middle name
        ///  </summary>
        public string MiddleName
        {
            get { return this.GetString(65, 40); }
            set { this.SetString(value, 65, 40); }
        }
        ///  <summary>
        /// Danish: MELNVN_MRK
        /// Middle name marker
        ///  </summary>
        public char MiddleNameMarker
        {
            get { return this.GetChar(105); }
            set { this.SetChar(value, 105); }
        }
        ///  <summary>
        /// Danish: EFTERNVN
        /// Last name
        ///  </summary>
        public string LastName
        {
            get { return this.GetString(106, 40); }
            set { this.SetString(value, 106, 40); }
        }
        ///  <summary>
        /// Danish: EFTERNVN_MRK
        /// Last name marker
        ///  </summary>
        public char LastNameMarker
        {
            get { return this.GetChar(146); }
            set { this.SetChar(value, 146); }
        }
        ///  <summary>
        /// Danish: NVNHAENSTART
        /// Name start date yyyyMMddTTMM
        ///  </summary>
        public DateTime? NameStartDate
        {
            get { return this.GetDateTime(147, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 147, 12, "yyyyMMddHHmm"); }
        }

        public Decimal NameStartDateDecimal
        {
            get { return this.GetDecimal(147, 12); }
            set { this.SetDecimal(value, 147, 12); }
        }

        ///  <summary>
        /// Danish: HAENSTART_UMRK-NAVNE
        /// Name start date uncertainty marker
        ///  </summary>
        public char NameStartDateUncertainty
        {
            get { return this.GetChar(159); }
            set { this.SetChar(value, 159); }
        }
        ///  <summary>
        /// Danish: ADRNVN
        /// AddressingName
        ///  </summary>
        public string AddressingName
        {
            get { return this.GetString(160, 34); }
            set { this.SetString(value, 160, 34); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("FirstName_s", 14, 50),
                    new Tuple<string, int, int>("FirstNameMarker", 64, 1),
                    new Tuple<string, int, int>("MiddleName", 65, 40),
                    new Tuple<string, int, int>("MiddleNameMarker", 105, 1),
                    new Tuple<string, int, int>("LastName", 106, 40),
                    new Tuple<string, int, int>("LastNameMarker", 146, 1),
                    new Tuple<string, int, int>("NameStartDate", 147, 12),
                    new Tuple<string, int, int>("NameStartDateUncertainty", 159, 1),
                    new Tuple<string, int, int>("AddressingName", 160, 34)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class BirthRegistrationInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 37; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: START_MYNKOD-FØDESTED
        /// Birth registration authority code
        ///  </summary>
        public string BirthRegistrationAuthorityCode
        {
            get { return this.GetString(14, 4); }
            set { this.SetString(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: MYNTXT-FØDESTED
        /// Additional birth registration text
        ///  </summary>
        public string AdditionalBirthRegistrationText
        {
            get { return this.GetString(18, 20); }
            set { this.SetString(value, 18, 20); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("BirthRegistrationAuthorityCode", 14, 4),
                    new Tuple<string, int, int>("AdditionalBirthRegistrationText", 18, 20)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentCitizenshipType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 30; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: LANDEKOD
        /// Country code
        ///  </summary>
        public decimal CountryCode
        {
            get { return this.GetDecimal(14, 4); }
            set { this.SetDecimal(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: HAENSTART-STATSBORGERSKAB
        /// Citizenship start date yyyyMMddTTMM
        ///  </summary>
        public DateTime? CitizenshipStartDate
        {
            get { return this.GetDateTime(18, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 18, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CitizenshipStartDateDecimal
        {
            get { return this.GetDecimal(18, 12); }
            set { this.SetDecimal(value, 18, 12); }
        }

        ///  <summary>
        /// Danish: HAENSTART_UMRK-STATSBORGERSKAB
        /// Citizenship start date uncertainty marker
        ///  </summary>
        public char CitizenshipStartDateUncertainty
        {
            get { return this.GetChar(30); }
            set { this.SetChar(value, 30); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CountryCode", 14, 4),
                    new Tuple<string, int, int>("CitizenshipStartDate", 18, 12),
                    new Tuple<string, int, int>("CitizenshipStartDateUncertainty", 30, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ChurchInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 25; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: FKIRK
        /// church Relationship
        ///  </summary>
        public char ChurchRelationship
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: START_DT-FOLKEKIRKE
        /// Start date yyyy-MM-dd
        ///  </summary>
        public DateTime? StartDate
        {
            get { return this.GetDateTime(15, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 15, 10, "yyyy-MM-dd"); }
        }

        public Decimal StartDateDecimal
        {
            get { return this.GetDecimal(15, 10); }
            set { this.SetDecimal(value, 15, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-FOLKEKIRKE
        /// Start date uncertainty marker
        ///  </summary>
        public char StartDateUncertainty
        {
            get { return this.GetChar(25); }
            set { this.SetChar(value, 25); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ChurchRelationship", 14, 1),
                    new Tuple<string, int, int>("StartDate", 15, 10),
                    new Tuple<string, int, int>("StartDateUncertainty", 25, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentCivilStatusType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 95; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: CIVST
        /// Civil status
        ///  </summary>
        public char CivilStatusCode
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: AEGTEPNR
        /// Spouse PNR
        ///  </summary>
        public string SpousePNR
        {
            get { return this.GetString(15, 10); }
            set { this.SetString(value, 15, 10); }
        }
        ///  <summary>
        /// Danish: AEGTEFOED_DT
        /// Spouse birth date yyyy-MM-dd
        ///  </summary>
        public DateTime? SpouseBirthDate
        {
            get { return this.GetDateTime(25, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 25, 10, "yyyy-MM-dd"); }
        }

        public Decimal SpouseBirthDateDecimal
        {
            get { return this.GetDecimal(25, 10); }
            set { this.SetDecimal(value, 25, 10); }
        }

        ///  <summary>
        /// Danish: AEGTEFOEddT_UMRK
        /// Spouse birth date uncertainty
        ///  </summary>
        public char SpouseBirthDateUncertainty
        {
            get { return this.GetChar(35); }
            set { this.SetChar(value, 35); }
        }
        ///  <summary>
        /// Danish: AEGTENVN
        /// Spouse name
        ///  </summary>
        public string SpouseName
        {
            get { return this.GetString(36, 34); }
            set { this.SetString(value, 36, 34); }
        }
        ///  <summary>
        /// Danish: AEGTENVN_MRK
        /// Spouse name marker
        ///  </summary>
        public char SpouseNameMarker
        {
            get { return this.GetChar(70); }
            set { this.SetChar(value, 70); }
        }
        ///  <summary>
        /// Danish: HAENSTART-CIVILSTAND
        /// Civil status start date
        ///  </summary>
        public DateTime? CivilStatusStartDate
        {
            get { return this.GetDateTime(71, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 71, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CivilStatusStartDateDecimal
        {
            get { return this.GetDecimal(71, 12); }
            set { this.SetDecimal(value, 71, 12); }
        }

        ///  <summary>
        /// Danish: HAENSTART_UMRK-CIVILSTAND
        /// Civil status start date uncertainty marker
        ///  </summary>
        public char CivilStatusStartDateUncertainty
        {
            get { return this.GetChar(83); }
            set { this.SetChar(value, 83); }
        }
        ///  <summary>
        /// Danish: SEP_HENVIS-CIVILSTAND
        /// Reference to any. separation yyyyMMddTTMM
        ///  </summary>
        public DateTime? ReferenceToAnySeparation
        {
            get { return this.GetDateTime(84, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 84, 12, "yyyyMMddHHmm"); }
        }

        public Decimal ReferenceToAnySeparationDecimal
        {
            get { return this.GetDecimal(84, 12); }
            set { this.SetDecimal(value, 84, 12); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CivilStatusCode", 14, 1),
                    new Tuple<string, int, int>("SpousePNR", 15, 10),
                    new Tuple<string, int, int>("SpouseBirthDate", 25, 10),
                    new Tuple<string, int, int>("SpouseBirthDateUncertainty", 35, 1),
                    new Tuple<string, int, int>("SpouseName", 36, 34),
                    new Tuple<string, int, int>("SpouseNameMarker", 70, 1),
                    new Tuple<string, int, int>("CivilStatusStartDate", 71, 12),
                    new Tuple<string, int, int>("CivilStatusStartDateUncertainty", 83, 1),
                    new Tuple<string, int, int>("ReferenceToAnySeparation", 84, 12)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class CurrentSeparationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 36; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: SEP_HENVIS-SEPARATION
        /// Reference to any. marital status yyyyMMddTTMM
        ///  </summary>
        public DateTime? ReferenceToAnyMaritalStatus
        {
            get { return this.GetDateTime(14, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 14, 12, "yyyyMMddHHmm"); }
        }

        public Decimal ReferenceToAnyMaritalStatusDecimal
        {
            get { return this.GetDecimal(14, 12); }
            set { this.SetDecimal(value, 14, 12); }
        }

        ///  <summary>
        /// Danish: START_DT-SEPARATION
        /// Separation start date yyyy-MM-dd
        ///  </summary>
        public DateTime? SeparationStartDate
        {
            get { return this.GetDateTime(26, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 26, 10, "yyyy-MM-dd"); }
        }

        public Decimal SeparationStartDateDecimal
        {
            get { return this.GetDecimal(26, 10); }
            set { this.SetDecimal(value, 26, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-SEPARATION
        /// Separation start date uncertainty marker
        ///  </summary>
        public char SeparationStartDateUncertainty
        {
            get { return this.GetChar(36); }
            set { this.SetChar(value, 36); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ReferenceToAnyMaritalStatus", 14, 12),
                    new Tuple<string, int, int>("SeparationStartDate", 26, 10),
                    new Tuple<string, int, int>("SeparationStartDateUncertainty", 36, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ChildType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 23; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: PNR
        /// Child PNR
        ///  </summary>
        public string ChildPNR
        {
            get { return this.GetString(14, 10); }
            set { this.SetString(value, 14, 10); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ChildPNR", 14, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ParentsInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 147; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: MOR_DT
        /// Mother date yyyy-MM-dd
        ///  </summary>
        public DateTime? MotherDate
        {
            get { return this.GetDateTime(14, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 14, 10, "yyyy-MM-dd"); }
        }

        public Decimal MotherDateDecimal
        {
            get { return this.GetDecimal(14, 10); }
            set { this.SetDecimal(value, 14, 10); }
        }

        ///  <summary>
        /// Danish: MOR_DT_UMRK
        /// Mother date uncertainty marker
        ///  </summary>
        public char MotherDateUncertainty
        {
            get { return this.GetChar(24); }
            set { this.SetChar(value, 24); }
        }
        ///  <summary>
        /// Danish: PNRMOR
        /// Mother PNR
        ///  </summary>
        public string MotherPNR
        {
            get { return this.GetString(25, 10); }
            set { this.SetString(value, 25, 10); }
        }
        ///  <summary>
        /// Danish: MOR_FOED_DT
        /// Mother birth date yyyy-MM-dd
        ///  </summary>
        public DateTime? MotherBirthDate
        {
            get { return this.GetDateTime(35, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 35, 10, "yyyy-MM-dd"); }
        }

        public Decimal MotherBirthDateDecimal
        {
            get { return this.GetDecimal(35, 10); }
            set { this.SetDecimal(value, 35, 10); }
        }

        ///  <summary>
        /// Danish: MOR_FOED_DT_UMRK
        /// Mother birth date uncertainty marker
        ///  </summary>
        public char MotherBirthDateUncertainty
        {
            get { return this.GetChar(45); }
            set { this.SetChar(value, 45); }
        }
        ///  <summary>
        /// Danish: MORNVN
        /// Mother name
        ///  </summary>
        public string MotherName
        {
            get { return this.GetString(46, 34); }
            set { this.SetString(value, 46, 34); }
        }
        ///  <summary>
        /// Danish: MORNVN_MRK
        /// Mother name marker
        ///  </summary>
        public char MotherNameUncertainty
        {
            get { return this.GetChar(80); }
            set { this.SetChar(value, 80); }
        }
        ///  <summary>
        /// Danish: FAR_DT
        /// Father date yyyy-MM-dd
        ///  </summary>
        public DateTime? FatherDate
        {
            get { return this.GetDateTime(81, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 81, 10, "yyyy-MM-dd"); }
        }

        public Decimal FatherDateDecimal
        {
            get { return this.GetDecimal(81, 10); }
            set { this.SetDecimal(value, 81, 10); }
        }

        ///  <summary>
        /// Danish: FAR_DT_UMRK
        /// Father date uncertainty marker
        ///  </summary>
        public char FatherDateUncertainty
        {
            get { return this.GetChar(91); }
            set { this.SetChar(value, 91); }
        }
        ///  <summary>
        /// Danish: PNRFAR
        /// Father PNR
        ///  </summary>
        public string FatherPNR
        {
            get { return this.GetString(92, 10); }
            set { this.SetString(value, 92, 10); }
        }
        ///  <summary>
        /// Danish: FAR_FOED_DT
        /// Father birth date yyyy-MM-dd
        ///  </summary>
        public DateTime? FatherBirthDate
        {
            get { return this.GetDateTime(102, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 102, 10, "yyyy-MM-dd"); }
        }

        public Decimal FatherBirthDateDecimal
        {
            get { return this.GetDecimal(102, 10); }
            set { this.SetDecimal(value, 102, 10); }
        }

        ///  <summary>
        /// Danish: FAR_FOED_DT_UMRK
        /// Father birth date uncertainty marker
        ///  </summary>
        public char FatherBirthDateUncertainty
        {
            get { return this.GetChar(112); }
            set { this.SetChar(value, 112); }
        }
        ///  <summary>
        /// Danish: FARNVN
        /// Father name
        ///  </summary>
        public string FatherName
        {
            get { return this.GetString(113, 34); }
            set { this.SetString(value, 113, 34); }
        }
        ///  <summary>
        /// Danish: FARNVN_MRK
        /// Father name marker
        ///  </summary>
        public char FatherNameUncertainty
        {
            get { return this.GetChar(147); }
            set { this.SetChar(value, 147); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("MotherDate", 14, 10),
                    new Tuple<string, int, int>("MotherDateUncertainty", 24, 1),
                    new Tuple<string, int, int>("MotherPNR", 25, 10),
                    new Tuple<string, int, int>("MotherBirthDate", 35, 10),
                    new Tuple<string, int, int>("MotherBirthDateUncertainty", 45, 1),
                    new Tuple<string, int, int>("MotherName", 46, 34),
                    new Tuple<string, int, int>("MotherNameUncertainty", 80, 1),
                    new Tuple<string, int, int>("FatherDate", 81, 10),
                    new Tuple<string, int, int>("FatherDateUncertainty", 91, 1),
                    new Tuple<string, int, int>("FatherPNR", 92, 10),
                    new Tuple<string, int, int>("FatherBirthDate", 102, 10),
                    new Tuple<string, int, int>("FatherBirthDateUncertainty", 112, 1),
                    new Tuple<string, int, int>("FatherName", 113, 34),
                    new Tuple<string, int, int>("FatherNameUncertainty", 147, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ParentalAuthorityType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 58; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: RELTYP-FORÆLDREMYN
        /// Relationship type
        ///  </summary>
        public decimal RelationshipType
        {
            get { return this.GetDecimal(14, 4); }
            set { this.SetDecimal(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: START_DT-FORÆLDREMYN
        /// Custody start date yyyy-MM-dd
        ///  </summary>
        public DateTime? CustodyStartDate
        {
            get { return this.GetDateTime(18, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 18, 10, "yyyy-MM-dd"); }
        }

        public Decimal CustodyStartDateDecimal
        {
            get { return this.GetDecimal(18, 10); }
            set { this.SetDecimal(value, 18, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-FORÆLDREMYN
        /// Custody start date uncertainty marker
        ///  </summary>
        public char CustodyStartDateUncertainty
        {
            get { return this.GetChar(28); }
            set { this.SetChar(value, 28); }
        }
        ///  <summary>
        /// Danish: SLET_DT-FORÆLDREMYN
        /// Custody end date yyyy-MM-dd
        ///  </summary>
        public DateTime? CustodyEndDate
        {
            get { return this.GetDateTime(29, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 29, 10, "yyyy-MM-dd"); }
        }

        public Decimal CustodyEndDateDecimal
        {
            get { return this.GetDecimal(29, 10); }
            set { this.SetDecimal(value, 29, 10); }
        }

        ///  <summary>
        /// Danish: RELPNR
        /// Relation PNR
        ///  </summary>
        public string RelationPNR
        {
            get { return this.GetString(39, 10); }
            set { this.SetString(value, 39, 10); }
        }
        ///  <summary>
        /// Danish: START_DT-RELPNR_PNR
        /// Relation PNR start date yyyy-MM-dd
        ///  </summary>
        public DateTime? RelationPNRStartDate
        {
            get { return this.GetDateTime(49, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 49, 10, "yyyy-MM-dd"); }
        }

        public Decimal RelationPNRStartDateDecimal
        {
            get { return this.GetDecimal(49, 10); }
            set { this.SetDecimal(value, 49, 10); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("RelationshipType", 14, 4),
                    new Tuple<string, int, int>("CustodyStartDate", 18, 10),
                    new Tuple<string, int, int>("CustodyStartDateUncertainty", 28, 1),
                    new Tuple<string, int, int>("CustodyEndDate", 29, 10),
                    new Tuple<string, int, int>("RelationPNR", 39, 10),
                    new Tuple<string, int, int>("RelationPNRStartDate", 49, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class DisempowermentType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 272; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: START_DT-UMYNDIG
        /// Disempowerment start date yyyy-MM-dd
        ///  </summary>
        public DateTime? DisempowermentStartDate
        {
            get { return this.GetDateTime(14, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 14, 10, "yyyy-MM-dd"); }
        }

        public Decimal DisempowermentStartDateDecimal
        {
            get { return this.GetDecimal(14, 10); }
            set { this.SetDecimal(value, 14, 10); }
        }

        ///  <summary>
        /// Danish: PNR
        /// Disempowerment start date uncertainty marker
        ///  </summary>
        public char DisempowermentStartDateUncertainty
        {
            get { return this.GetChar(24); }
            set { this.SetChar(value, 24); }
        }
        ///  <summary>
        /// Danish: SLET_DT-UMYNDIG
        /// Disempowerment end date yyyy-MM-dd
        ///  </summary>
        public DateTime? DisempowermentEndDate
        {
            get { return this.GetDateTime(25, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 25, 10, "yyyy-MM-dd"); }
        }

        public Decimal DisempowermentEndDateDecimal
        {
            get { return this.GetDecimal(25, 10); }
            set { this.SetDecimal(value, 25, 10); }
        }

        ///  <summary>
        /// Danish: UMYN_RELTYP
        /// Guardian relation type
        ///  </summary>
        public decimal GuardianRelationType
        {
            get { return this.GetDecimal(35, 4); }
            set { this.SetDecimal(value, 35, 4); }
        }
        ///  <summary>
        /// Danish: RELPNR
        /// Relation PNR
        ///  </summary>
        public string RelationPNR
        {
            get { return this.GetString(39, 10); }
            set { this.SetString(value, 39, 10); }
        }
        ///  <summary>
        /// Danish: START_DT-RELPNR_PNR
        /// Relation PNR start date yyyy-MM-dd
        ///  </summary>
        public DateTime? RelationPNRStartDate
        {
            get { return this.GetDateTime(49, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 49, 10, "yyyy-MM-dd"); }
        }

        public Decimal RelationPNRStartDateDecimal
        {
            get { return this.GetDecimal(49, 10); }
            set { this.SetDecimal(value, 49, 10); }
        }

        ///  <summary>
        /// Danish: RELADRSAT_RELPNR_TXT
        /// Guardian's name
        ///  </summary>
        public string GuardianName
        {
            get { return this.GetString(59, 34); }
            set { this.SetString(value, 59, 34); }
        }
        ///  <summary>
        /// Danish: START_DT-RELPNR_TXT
        /// Guardian's address start date yyyy-MM-dd
        ///  </summary>
        public DateTime? GuardianAddressStartDate
        {
            get { return this.GetDateTime(93, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 93, 10, "yyyy-MM-dd"); }
        }

        public Decimal GuardianAddressStartDateDecimal
        {
            get { return this.GetDecimal(93, 10); }
            set { this.SetDecimal(value, 93, 10); }
        }

        ///  <summary>
        /// Danish: RELTXT1
        /// Relation text 1
        ///  </summary>
        public string RelationText1
        {
            get { return this.GetString(103, 34); }
            set { this.SetString(value, 103, 34); }
        }
        ///  <summary>
        /// Danish: RELTXT2
        /// Relation text 2
        ///  </summary>
        public string RelationText2
        {
            get { return this.GetString(137, 34); }
            set { this.SetString(value, 137, 34); }
        }
        ///  <summary>
        /// Danish: RELTXT3
        /// Relation text 3
        ///  </summary>
        public string RelationText3
        {
            get { return this.GetString(171, 34); }
            set { this.SetString(value, 171, 34); }
        }
        ///  <summary>
        /// Danish: RELTXT4
        /// Relation text 4
        ///  </summary>
        public string RelationText4
        {
            get { return this.GetString(205, 34); }
            set { this.SetString(value, 205, 34); }
        }
        ///  <summary>
        /// Danish: RELTXT5
        /// Relation text 5
        ///  </summary>
        public string RelationText5
        {
            get { return this.GetString(239, 34); }
            set { this.SetString(value, 239, 34); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("DisempowermentStartDate", 14, 10),
                    new Tuple<string, int, int>("DisempowermentStartDateUncertainty", 24, 1),
                    new Tuple<string, int, int>("DisempowermentEndDate", 25, 10),
                    new Tuple<string, int, int>("GuardianRelationType", 35, 4),
                    new Tuple<string, int, int>("RelationPNR", 39, 10),
                    new Tuple<string, int, int>("RelationPNRStartDate", 49, 10),
                    new Tuple<string, int, int>("GuardianName", 59, 34),
                    new Tuple<string, int, int>("GuardianAddressStartDate", 93, 10),
                    new Tuple<string, int, int>("RelationText1", 103, 34),
                    new Tuple<string, int, int>("RelationText2", 137, 34),
                    new Tuple<string, int, int>("RelationText3", 171, 34),
                    new Tuple<string, int, int>("RelationText4", 205, 34),
                    new Tuple<string, int, int>("RelationText5", 239, 34)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class MunicipalConditionsType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 60; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: KOMFORHTYP
        /// Municipal condition type
        ///  </summary>
        public decimal MunicipalConditionType
        {
            get { return this.GetDecimal(14, 1); }
            set { this.SetDecimal(value, 14, 1); }
        }
        ///  <summary>
        /// Danish: KOMFORHKOD
        /// Municipal condition code
        ///  </summary>
        public string MunicipalConditionCode
        {
            get { return this.GetString(15, 5); }
            set { this.SetString(value, 15, 5); }
        }
        ///  <summary>
        /// Danish: START_DT-KOMMUNALE-FORHOLD
        /// Municipal condition start date yyyy-MM-dd
        ///  </summary>
        public DateTime? MunicipalConditionStartDate
        {
            get { return this.GetDateTime(20, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 20, 10, "yyyy-MM-dd"); }
        }

        public Decimal MunicipalConditionStartDateDecimal
        {
            get { return this.GetDecimal(20, 10); }
            set { this.SetDecimal(value, 20, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-KOMMUNALE-FORHOL
        /// Start date uncertainty marker
        ///  </summary>
        public char MunicipalConditionStartDateUncertaintyMarker
        {
            get { return this.GetChar(30); }
            set { this.SetChar(value, 30); }
        }
        ///  <summary>
        /// Danish: BEMAERK-KOMMUNALE-FORHOLD
        /// Municipal condition comment
        ///  </summary>
        public string MunicipalConditionComment
        {
            get { return this.GetString(31, 30); }
            set { this.SetString(value, 31, 30); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("MunicipalConditionType", 14, 1),
                    new Tuple<string, int, int>("MunicipalConditionCode", 15, 5),
                    new Tuple<string, int, int>("MunicipalConditionStartDate", 20, 10),
                    new Tuple<string, int, int>("MunicipalConditionStartDateUncertaintyMarker", 30, 1),
                    new Tuple<string, int, int>("MunicipalConditionComment", 31, 30)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class NotesType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 75; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: NOTATNR
        /// Note number
        ///  </summary>
        public decimal NoteNumber
        {
            get { return this.GetDecimal(14, 2); }
            set { this.SetDecimal(value, 14, 2); }
        }
        ///  <summary>
        /// Danish: NOTATLINIE
        /// Note text
        ///  </summary>
        public string NoteText
        {
            get { return this.GetString(16, 40); }
            set { this.SetString(value, 16, 40); }
        }
        ///  <summary>
        /// Danish: START_DT-NOTAT
        /// Note start date YYYY-MM-DD
        ///  </summary>
        public DateTime? StartDate
        {
            get { return this.GetDateTime(56, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 56, 10, "yyyy-MM-dd"); }
        }

        public Decimal StartDateDecimal
        {
            get { return this.GetDecimal(56, 10); }
            set { this.SetDecimal(value, 56, 10); }
        }

        ///  <summary>
        /// Danish: SLET_DT-NOTAT
        /// Note deletion date YYYY-MM-DD
        ///  </summary>
        public DateTime? EndDate
        {
            get { return this.GetDateTime(66, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 66, 10, "yyyy-MM-dd"); }
        }

        public Decimal EndDateDecimal
        {
            get { return this.GetDecimal(66, 10); }
            set { this.SetDecimal(value, 66, 10); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("NoteNumber", 14, 2),
                    new Tuple<string, int, int>("NoteText", 16, 40),
                    new Tuple<string, int, int>("StartDate", 56, 10),
                    new Tuple<string, int, int>("EndDate", 66, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ElectionInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 47; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public decimal PNR
        {
            get { return this.GetDecimal(4, 10); }
            set { this.SetDecimal(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: VALGKODE
        /// Election code
        ///  </summary>
        public decimal ElectionCode
        {
            get { return this.GetDecimal(14, 4); }
            set { this.SetDecimal(value, 14, 4); }
        }
        ///  <summary>
        /// Danish: VALGRET_DT
        /// Voting date YYYY-MM-DD
        ///  </summary>
        public DateTime? VotingDate
        {
            get { return this.GetDateTime(18, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 18, 10, "yyyy-MM-dd"); }
        }

        public Decimal VotingDateDecimal
        {
            get { return this.GetDecimal(18, 10); }
            set { this.SetDecimal(value, 18, 10); }
        }

        ///  <summary>
        /// Danish: START_DT-VALGOPLYSNINGER
        /// Election info start date YYYY-MM-DD
        ///  </summary>
        public DateTime? ElectionInfoStartDate
        {
            get { return this.GetDateTime(28, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 28, 10, "yyyy-MM-dd"); }
        }

        public Decimal ElectionInfoStartDateDecimal
        {
            get { return this.GetDecimal(28, 10); }
            set { this.SetDecimal(value, 28, 10); }
        }

        ///  <summary>
        /// Danish: SLET_DT-VALGOPLYSNINGER
        /// Election info deletion date YYYY-MM-DD
        ///  </summary>
        public DateTime? ElectionInfoDeletionDate
        {
            get { return this.GetDateTime(1, 3, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 1, 3, "yyyy-MM-dd"); }
        }

        public Decimal ElectionInfoDeletionDateDecimal
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ElectionCode", 14, 4),
                    new Tuple<string, int, int>("VotingDate", 18, 10),
                    new Tuple<string, int, int>("ElectionInfoStartDate", 28, 10),
                    new Tuple<string, int, int>("ElectionInfoDeletionDate", 1, 3)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class RelocationOrderType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 63; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalPNRType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 45; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNRGÆLD
        /// Current CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: PNR
        /// Old CPR Number
        ///  </summary>
        public string OldPNR
        {
            get { return this.GetString(14, 10); }
            set { this.SetString(value, 14, 10); }
        }
        ///  <summary>
        /// Danish: START_DT-PERSON
        /// Start date
        ///  </summary>
        public DateTime? OldPNRStartDate
        {
            get { return this.GetDateTime(24, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 24, 10, "yyyy-MM-dd"); }
        }

        public Decimal OldPNRStartDateDecimal
        {
            get { return this.GetDecimal(24, 10); }
            set { this.SetDecimal(value, 24, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-PERSON
        /// Start date uncertainty
        ///  </summary>
        public char OldPNRStartDateUncertainty
        {
            get { return this.GetChar(34); }
            set { this.SetChar(value, 34); }
        }
        ///  <summary>
        /// Danish: SLUT_DT-PERSON
        /// End date
        ///  </summary>
        public DateTime? OldPNREndDate
        {
            get { return this.GetDateTime(35, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 35, 10, "yyyy-MM-dd"); }
        }

        public Decimal OldPNREndDateDecimal
        {
            get { return this.GetDecimal(35, 10); }
            set { this.SetDecimal(value, 35, 10); }
        }

        ///  <summary>
        /// Danish: SLUT_DT_UMRK-PERSON
        /// Start date uncertainty
        ///  </summary>
        public char OldPNREndDateUncertainty
        {
            get { return this.GetChar(45); }
            set { this.SetChar(value, 45); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("OldPNR", 14, 10),
                    new Tuple<string, int, int>("OldPNRStartDate", 24, 10),
                    new Tuple<string, int, int>("OldPNRStartDateUncertainty", 34, 1),
                    new Tuple<string, int, int>("OldPNREndDate", 35, 10),
                    new Tuple<string, int, int>("OldPNREndDateUncertainty", 45, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalAddressType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 96; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Correction marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: KOMKOD
        /// Municipality
        ///  </summary>
        public decimal MunicipalityCode
        {
            get { return this.GetDecimal(15, 4); }
            set { this.SetDecimal(value, 15, 4); }
        }
        ///  <summary>
        /// Danish: VEJKOD
        /// Street
        ///  </summary>
        public decimal StreetCode
        {
            get { return this.GetDecimal(19, 4); }
            set { this.SetDecimal(value, 19, 4); }
        }
        ///  <summary>
        /// Danish: HUSNR
        /// House
        ///  </summary>
        public string HouseNumber
        {
            get { return this.GetString(23, 4); }
            set { this.SetString(value, 23, 4); }
        }
        ///  <summary>
        /// Danish: ETAGE
        /// Floor
        ///  </summary>
        public string Floor
        {
            get { return this.GetString(27, 2); }
            set { this.SetString(value, 27, 2); }
        }
        ///  <summary>
        /// Danish: SIDEDOER
        /// Door
        ///  </summary>
        public string Door
        {
            get { return this.GetString(29, 4); }
            set { this.SetString(value, 29, 4); }
        }
        ///  <summary>
        /// Danish: BNR
        /// Building number
        ///  </summary>
        public string BuildingNumber
        {
            get { return this.GetString(33, 4); }
            set { this.SetString(value, 33, 4); }
        }
        ///  <summary>
        /// Danish: CONVN
        /// C/O name
        ///  </summary>
        public string CareOfName
        {
            get { return this.GetString(37, 34); }
            set { this.SetString(value, 37, 34); }
        }
        ///  <summary>
        /// Danish: TILFLYDTO
        /// Relocation date yyyyMMddTTMM
        ///  </summary>
        public DateTime? RelocationDate
        {
            get { return this.GetDateTime(71, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 71, 12, "yyyyMMddHHmm"); }
        }

        public Decimal RelocationDateDecimal
        {
            get { return this.GetDecimal(71, 12); }
            set { this.SetDecimal(value, 71, 12); }
        }

        ///  <summary>
        /// Danish: TILFLYDT_UMRK
        /// Relocation date uncertainty marker
        ///  </summary>
        public char RelocationDateUncertainty
        {
            get { return this.GetChar(83); }
            set { this.SetChar(value, 83); }
        }
        ///  <summary>
        /// Danish: FRAFLYDTO
        /// Leaving date yyyyMMddTTMM
        ///  </summary>
        public DateTime? LeavingDate
        {
            get { return this.GetDateTime(84, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 84, 12, "yyyyMMddHHmm"); }
        }

        public Decimal LeavingDateDecimal
        {
            get { return this.GetDecimal(84, 12); }
            set { this.SetDecimal(value, 84, 12); }
        }

        ///  <summary>
        /// Danish: FRAFLYDTO_UMRK
        /// Leaving date uncertainty marker
        ///  </summary>
        public char LeavingDateUncertainty
        {
            get { return this.GetChar(96); }
            set { this.SetChar(value, 96); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("MunicipalityCode", 15, 4),
                    new Tuple<string, int, int>("StreetCode", 19, 4),
                    new Tuple<string, int, int>("HouseNumber", 23, 4),
                    new Tuple<string, int, int>("Floor", 27, 2),
                    new Tuple<string, int, int>("Door", 29, 4),
                    new Tuple<string, int, int>("BuildingNumber", 33, 4),
                    new Tuple<string, int, int>("CareOfName", 37, 34),
                    new Tuple<string, int, int>("RelocationDate", 71, 12),
                    new Tuple<string, int, int>("RelocationDateUncertainty", 83, 1),
                    new Tuple<string, int, int>("LeavingDate", 84, 12),
                    new Tuple<string, int, int>("LeavingDateUncertainty", 96, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalDepartureType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 218; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Correction marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: UDR_LANDEKOD
        /// Exit country code
        ///  </summary>
        public decimal ExitCountryCode
        {
            get { return this.GetDecimal(15, 4); }
            set { this.SetDecimal(value, 15, 4); }
        }
        ///  <summary>
        /// Danish: UDRDTO
        /// Exit date yyyyMMddTTMM
        ///  </summary>
        public DateTime? ExitDate
        {
            get { return this.GetDateTime(19, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 19, 12, "yyyyMMddHHmm"); }
        }

        public Decimal ExitDateDecimal
        {
            get { return this.GetDecimal(19, 12); }
            set { this.SetDecimal(value, 19, 12); }
        }

        ///  <summary>
        /// Danish: UDRDTO_UMRK
        /// Exit date uncertainty marker
        ///  </summary>
        public char ExitDateUncertainty
        {
            get { return this.GetChar(31); }
            set { this.SetChar(value, 31); }
        }
        ///  <summary>
        /// Danish: INDR_LANDEKOD
        /// Entry country code
        ///  </summary>
        public decimal EntryCountryCode
        {
            get { return this.GetDecimal(32, 4); }
            set { this.SetDecimal(value, 32, 4); }
        }
        ///  <summary>
        /// Danish: INDRDTO
        /// Entry date yyyyMMddTTMM
        ///  </summary>
        public DateTime? EntryDate
        {
            get { return this.GetDateTime(36, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 36, 12, "yyyyMMddHHmm"); }
        }

        public Decimal EntryDateDecimal
        {
            get { return this.GetDecimal(36, 12); }
            set { this.SetDecimal(value, 36, 12); }
        }

        ///  <summary>
        /// Danish: INDRDTO_UMRK
        /// Entry date uncertainty marker
        ///  </summary>
        public char EntryDateUncertainty
        {
            get { return this.GetChar(48); }
            set { this.SetChar(value, 48); }
        }
        ///  <summary>
        /// Danish: UDLANDADR1
        /// Foreign Address 1
        ///  </summary>
        public string ForeignAddress1
        {
            get { return this.GetString(49, 34); }
            set { this.SetString(value, 49, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR2
        /// Foreign Address 2
        ///  </summary>
        public string ForeignAddress2
        {
            get { return this.GetString(83, 34); }
            set { this.SetString(value, 83, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR3
        /// Foreign Address 3
        ///  </summary>
        public string ForeignAddress3
        {
            get { return this.GetString(117, 34); }
            set { this.SetString(value, 117, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR4
        /// Foreign Address 4
        ///  </summary>
        public string ForeignAddress4
        {
            get { return this.GetString(151, 34); }
            set { this.SetString(value, 151, 34); }
        }
        ///  <summary>
        /// Danish: UDLANDADR5
        /// Foreign Address 5
        ///  </summary>
        public string ForeignAddress5
        {
            get { return this.GetString(185, 34); }
            set { this.SetString(value, 185, 34); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("ExitCountryCode", 15, 4),
                    new Tuple<string, int, int>("ExitDate", 19, 12),
                    new Tuple<string, int, int>("ExitDateUncertainty", 31, 1),
                    new Tuple<string, int, int>("EntryCountryCode", 32, 4),
                    new Tuple<string, int, int>("EntryDate", 36, 12),
                    new Tuple<string, int, int>("EntryDateUncertainty", 48, 1),
                    new Tuple<string, int, int>("ForeignAddress1", 49, 34),
                    new Tuple<string, int, int>("ForeignAddress2", 83, 34),
                    new Tuple<string, int, int>("ForeignAddress3", 117, 34),
                    new Tuple<string, int, int>("ForeignAddress4", 151, 34),
                    new Tuple<string, int, int>("ForeignAddress5", 185, 34)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalDisappearanceType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 40; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Correction marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: FORSVINDDTO
        /// Disappearance date yyyyMMddTTMM
        ///  </summary>
        public DateTime? DisappearanceDate
        {
            get { return this.GetDateTime(15, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 15, 12, "yyyyMMddHHmm"); }
        }

        public Decimal DisappearanceDateDecimal
        {
            get { return this.GetDecimal(15, 12); }
            set { this.SetDecimal(value, 15, 12); }
        }

        ///  <summary>
        /// Danish: FORSVINDDTO_UMRK
        /// Disappearance date uncertainty marker
        ///  </summary>
        public char DisappearanceDateUncertainty
        {
            get { return this.GetChar(27); }
            set { this.SetChar(value, 27); }
        }
        ///  <summary>
        /// Danish: GENFINDDTO
        /// Retrieval date yyyyMMddTTMM
        ///  </summary>
        public DateTime? RetrievalDate
        {
            get { return this.GetDateTime(28, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 28, 12, "yyyyMMddHHmm"); }
        }

        public Decimal RetrievalDateDecimal
        {
            get { return this.GetDecimal(28, 12); }
            set { this.SetDecimal(value, 28, 12); }
        }

        ///  <summary>
        /// Danish: GENFINDDTO_UMRK
        /// Retrieval date uncertainty marker
        ///  </summary>
        public char RetrievalDateUncertainty
        {
            get { return this.GetChar(40); }
            set { this.SetChar(value, 40); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("DisappearanceDate", 15, 12),
                    new Tuple<string, int, int>("DisappearanceDateUncertainty", 27, 1),
                    new Tuple<string, int, int>("RetrievalDate", 28, 12),
                    new Tuple<string, int, int>("RetrievalDateUncertainty", 40, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalNameType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 173; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Correction marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: FORNVN
        /// First name(s)
        ///  </summary>
        public string FirstName_s
        {
            get { return this.GetString(15, 50); }
            set { this.SetString(value, 15, 50); }
        }
        ///  <summary>
        /// Danish: FORNVN_MRK
        /// First name marker
        ///  </summary>
        public char FirstNameMarker
        {
            get { return this.GetChar(65); }
            set { this.SetChar(value, 65); }
        }
        ///  <summary>
        /// Danish: MELNVN
        /// Middle name
        ///  </summary>
        public string MiddleName
        {
            get { return this.GetString(66, 40); }
            set { this.SetString(value, 66, 40); }
        }
        ///  <summary>
        /// Danish: MELNVN_MRK
        /// Middle name marker
        ///  </summary>
        public char MiddleNameMarker
        {
            get { return this.GetChar(106); }
            set { this.SetChar(value, 106); }
        }
        ///  <summary>
        /// Danish: EFTERNVN
        /// Last name
        ///  </summary>
        public string LastName
        {
            get { return this.GetString(107, 40); }
            set { this.SetString(value, 107, 40); }
        }
        ///  <summary>
        /// Danish: EFTERNVN_MRK
        /// Last name marker
        ///  </summary>
        public char LastNameMarker
        {
            get { return this.GetChar(147); }
            set { this.SetChar(value, 147); }
        }
        ///  <summary>
        /// Danish: NVNHAENSTART
        /// Name start date ÅÅÅÅMMDDTTMM
        ///  </summary>
        public DateTime? NameStartDate
        {
            get { return this.GetDateTime(148, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 148, 12, "yyyyMMddHHmm"); }
        }

        public Decimal NameStartDateDecimal
        {
            get { return this.GetDecimal(148, 12); }
            set { this.SetDecimal(value, 148, 12); }
        }

        ///  <summary>
        /// Danish: HAENSTART_UMRK-NAVNE
        /// Name start date uncertainty marker
        ///  </summary>
        public char NameStartDateUncertainty
        {
            get { return this.GetChar(160); }
            set { this.SetChar(value, 160); }
        }
        ///  <summary>
        /// Danish: NVNHAENSLUT
        /// Name end date ÅÅÅÅMMDDTTMM
        ///  </summary>
        public DateTime? NameEndDate
        {
            get { return this.GetDateTime(161, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 161, 12, "yyyyMMddHHmm"); }
        }

        public Decimal NameEndDateDecimal
        {
            get { return this.GetDecimal(161, 12); }
            set { this.SetDecimal(value, 161, 12); }
        }

        ///  <summary>
        /// Danish: HAENSLUT_UMRK-NAVNE
        /// Name end date uncertainty marker
        ///  </summary>
        public char NameEndDateUncertainty
        {
            get { return this.GetChar(173); }
            set { this.SetChar(value, 173); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("FirstName_s", 15, 50),
                    new Tuple<string, int, int>("FirstNameMarker", 65, 1),
                    new Tuple<string, int, int>("MiddleName", 66, 40),
                    new Tuple<string, int, int>("MiddleNameMarker", 106, 1),
                    new Tuple<string, int, int>("LastName", 107, 40),
                    new Tuple<string, int, int>("LastNameMarker", 147, 1),
                    new Tuple<string, int, int>("NameStartDate", 148, 12),
                    new Tuple<string, int, int>("NameStartDateUncertainty", 160, 1),
                    new Tuple<string, int, int>("NameEndDate", 161, 12),
                    new Tuple<string, int, int>("NameEndDateUncertainty", 173, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalCitizenshipType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 44; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Correction marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: LANDEKOD
        /// Country code
        ///  </summary>
        public decimal CountryCode
        {
            get { return this.GetDecimal(15, 4); }
            set { this.SetDecimal(value, 15, 4); }
        }
        ///  <summary>
        /// Danish: HAENSTART-STATSBORGERSKAB
        /// Citizenship start date yyyyMMddTTMM
        ///  </summary>
        public DateTime? CitizenshipStartDate
        {
            get { return this.GetDateTime(19, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 19, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CitizenshipStartDateDecimal
        {
            get { return this.GetDecimal(19, 12); }
            set { this.SetDecimal(value, 19, 12); }
        }

        ///  <summary>
        /// Danish: HAENSTART_UMRK-STATSBORGERSKAB
        /// Citizenship start date uncertainty marker
        ///  </summary>
        public char CitizenshipStartDateUncertainty
        {
            get { return this.GetChar(31); }
            set { this.SetChar(value, 31); }
        }
        ///  <summary>
        /// Danish: HAENSLUT-STATSBORGERSKAB
        /// Citizenship end date yyyyMMddTTMM
        ///  </summary>
        public DateTime? CitizenshipEndDate
        {
            get { return this.GetDateTime(32, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 32, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CitizenshipEndDateDecimal
        {
            get { return this.GetDecimal(32, 12); }
            set { this.SetDecimal(value, 32, 12); }
        }

        ///  <summary>
        /// Danish: HAENSLUT_UMRK-STATSBORGERSKAB
        /// Citizenship end date uncertainty marker
        ///  </summary>
        public char CitizenshipEndDateUncertainty
        {
            get { return this.GetChar(44); }
            set { this.SetChar(value, 44); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("CountryCode", 15, 4),
                    new Tuple<string, int, int>("CitizenshipStartDate", 19, 12),
                    new Tuple<string, int, int>("CitizenshipStartDateUncertainty", 31, 1),
                    new Tuple<string, int, int>("CitizenshipEndDate", 32, 12),
                    new Tuple<string, int, int>("CitizenshipEndDateUncertainty", 44, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalChurchInformationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 36; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: FKIRK
        /// church Relationship
        ///  </summary>
        public char ChurchRelationship
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: START_DT-FOLKEKIRKE
        /// Start date yyyy-MM-dd
        ///  </summary>
        public DateTime? StartDate
        {
            get { return this.GetDateTime(15, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 15, 10, "yyyy-MM-dd"); }
        }

        public Decimal StartDateDecimal
        {
            get { return this.GetDecimal(15, 10); }
            set { this.SetDecimal(value, 15, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-FOLKEKIRKE
        /// Start date uncertainty marker
        ///  </summary>
        public char StartDateUncertainty
        {
            get { return this.GetChar(25); }
            set { this.SetChar(value, 25); }
        }
        ///  <summary>
        /// Danish: SLUT_DT-FOLKEKIRKE
        /// End date yyyy-MM-dd
        ///  </summary>
        public DateTime? EndDate
        {
            get { return this.GetDateTime(26, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 26, 10, "yyyy-MM-dd"); }
        }

        public Decimal EndDateDecimal
        {
            get { return this.GetDecimal(26, 10); }
            set { this.SetDecimal(value, 26, 10); }
        }

        ///  <summary>
        /// Danish: SLUT_DT_UMRK-FOLKEKIRKE
        /// End date uncertainty marker
        ///  </summary>
        public char EndDateUncertainty
        {
            get { return this.GetChar(36); }
            set { this.SetChar(value, 36); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("ChurchRelationship", 14, 1),
                    new Tuple<string, int, int>("StartDate", 15, 10),
                    new Tuple<string, int, int>("StartDateUncertainty", 25, 1),
                    new Tuple<string, int, int>("EndDate", 26, 10),
                    new Tuple<string, int, int>("EndDateUncertainty", 36, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalCivilStatusType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 109; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Edit / Undo marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: CIVST
        /// Civil status
        ///  </summary>
        public char CivilStatusCode
        {
            get { return this.GetChar(15); }
            set { this.SetChar(value, 15); }
        }
        ///  <summary>
        /// Danish: AEGTEPNR
        /// Spouse PNR
        ///  </summary>
        public string SpousePNR
        {
            get { return this.GetString(16, 10); }
            set { this.SetString(value, 16, 10); }
        }
        ///  <summary>
        /// Danish: AEGTEFOED_DT
        /// Spouse Birth date ÅÅÅÅ-MM-DD
        ///  </summary>
        public DateTime? SpouseBirthdate
        {
            get { return this.GetDateTime(26, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 26, 10, "yyyy-MM-dd"); }
        }

        public Decimal SpouseBirthdateDecimal
        {
            get { return this.GetDecimal(26, 10); }
            set { this.SetDecimal(value, 26, 10); }
        }

        ///  <summary>
        /// Danish: AEGTEFOEDDT_UMRK
        /// Spouse birthdate uncertainty
        ///  </summary>
        public char SpouseBirthdateUncertainty
        {
            get { return this.GetChar(36); }
            set { this.SetChar(value, 36); }
        }
        ///  <summary>
        /// Danish: AEGTENVN
        /// Spouse name
        ///  </summary>
        public string SpouseName
        {
            get { return this.GetString(37, 34); }
            set { this.SetString(value, 37, 34); }
        }
        ///  <summary>
        /// Danish: AEGTENVN_MRK
        /// Spouse name marker
        ///  </summary>
        public char SpouseNameMarker
        {
            get { return this.GetChar(71); }
            set { this.SetChar(value, 71); }
        }
        ///  <summary>
        /// Danish: HAENSTART-CIVILSTAND
        /// Civil status start date ÅÅÅÅMMDDTTMM
        ///  </summary>
        public DateTime? CivilStatusStartDate
        {
            get { return this.GetDateTime(72, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 72, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CivilStatusStartDateDecimal
        {
            get { return this.GetDecimal(72, 12); }
            set { this.SetDecimal(value, 72, 12); }
        }

        ///  <summary>
        /// Danish: HAENSTART_UMRK-CIVILSTAND
        /// Civil status start date uncertainty
        ///  </summary>
        public char CivilStatusStartDateUncertainty
        {
            get { return this.GetChar(84); }
            set { this.SetChar(value, 84); }
        }
        ///  <summary>
        /// Danish: HAENSLUT-CIVILSTAND
        /// Civil status end date ÅÅÅÅMMDDTTMM
        ///  </summary>
        public DateTime? CivilStatusEndDate
        {
            get { return this.GetDateTime(85, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 85, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CivilStatusEndDateDecimal
        {
            get { return this.GetDecimal(85, 12); }
            set { this.SetDecimal(value, 85, 12); }
        }

        ///  <summary>
        /// Danish: HAENSLUT_UMRK-CIVILSTAND
        /// Civil status end date uncertainty
        ///  </summary>
        public char CivilStatusEndDateUncertainty
        {
            get { return this.GetChar(97); }
            set { this.SetChar(value, 97); }
        }
        ///  <summary>
        /// Danish: SEP_HENVIS-CIVILSTAND
        /// Reference to any separation ÅÅÅÅMMDDTTMM
        ///  </summary>
        public DateTime? ReferenceToAnySeparation
        {
            get { return this.GetDateTime(98, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 98, 12, "yyyyMMddHHmm"); }
        }

        public Decimal ReferenceToAnySeparationDecimal
        {
            get { return this.GetDecimal(98, 12); }
            set { this.SetDecimal(value, 98, 12); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("CivilStatusCode", 15, 1),
                    new Tuple<string, int, int>("SpousePNR", 16, 10),
                    new Tuple<string, int, int>("SpouseBirthdate", 26, 10),
                    new Tuple<string, int, int>("SpouseBirthdateUncertainty", 36, 1),
                    new Tuple<string, int, int>("SpouseName", 37, 34),
                    new Tuple<string, int, int>("SpouseNameMarker", 71, 1),
                    new Tuple<string, int, int>("CivilStatusStartDate", 72, 12),
                    new Tuple<string, int, int>("CivilStatusStartDateUncertainty", 84, 1),
                    new Tuple<string, int, int>("CivilStatusEndDate", 85, 12),
                    new Tuple<string, int, int>("CivilStatusEndDateUncertainty", 97, 1),
                    new Tuple<string, int, int>("ReferenceToAnySeparation", 98, 12)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class HistoricalSeparationType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 48; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ANNKOR
        /// Edit / Undo marker
        ///  </summary>
        public char CorrectionMarker
        {
            get { return this.GetChar(14); }
            set { this.SetChar(value, 14); }
        }
        ///  <summary>
        /// Danish: SEP_HENVIS-SEPARATION
        /// Reference to any. marital status yyyyMMddTTMM
        ///  </summary>
        public DateTime? ReferenceToAnyMaritalStatus
        {
            get { return this.GetDateTime(15, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 15, 12, "yyyyMMddHHmm"); }
        }

        public Decimal ReferenceToAnyMaritalStatusDecimal
        {
            get { return this.GetDecimal(15, 12); }
            set { this.SetDecimal(value, 15, 12); }
        }

        ///  <summary>
        /// Danish: START_DT-SEPARATION
        /// Separation start date yyyy-MM-dd
        ///  </summary>
        public DateTime? SeparationStartDate
        {
            get { return this.GetDateTime(27, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 27, 10, "yyyy-MM-dd"); }
        }

        public Decimal SeparationStartDateDecimal
        {
            get { return this.GetDecimal(27, 10); }
            set { this.SetDecimal(value, 27, 10); }
        }

        ///  <summary>
        /// Danish: START_DT_UMRK-SEPARATION
        /// Separation start date uncertainty marker
        ///  </summary>
        public char SeparationStartDateUncertainty
        {
            get { return this.GetChar(37); }
            set { this.SetChar(value, 37); }
        }
        ///  <summary>
        /// Danish: SLUT_DT-SEPARATION
        /// Separation end date yyyy-MM-dd
        ///  </summary>
        public DateTime? SeparationEndDate
        {
            get { return this.GetDateTime(38, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 38, 10, "yyyy-MM-dd"); }
        }

        public Decimal SeparationEndDateDecimal
        {
            get { return this.GetDecimal(38, 10); }
            set { this.SetDecimal(value, 38, 10); }
        }

        ///  <summary>
        /// Danish: SLUT_DT_UMRK-SEPARATION
        /// Separation end date uncertainty marker
        ///  </summary>
        public char SeparationEndDateUncertainty
        {
            get { return this.GetChar(48); }
            set { this.SetChar(value, 48); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CorrectionMarker", 14, 1),
                    new Tuple<string, int, int>("ReferenceToAnyMaritalStatus", 15, 12),
                    new Tuple<string, int, int>("SeparationStartDate", 27, 10),
                    new Tuple<string, int, int>("SeparationStartDateUncertainty", 37, 1),
                    new Tuple<string, int, int>("SeparationEndDate", 38, 10),
                    new Tuple<string, int, int>("SeparationEndDateUncertainty", 48, 1)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class _RelativeClearWrittenAddressType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 269; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: ADRNVN
        ///  </summary>
        public string AddressingName
        {
            get { return this.GetString(24, 34); }
            set { this.SetString(value, 24, 34); }
        }
        ///  <summary>
        /// Danish: CONVN
        ///  </summary>
        public string CareOfName
        {
            get { return this.GetString(58, 34); }
            set { this.SetString(value, 58, 34); }
        }
        ///  <summary>
        /// Danish: LOKALITET
        ///  </summary>
        public string LOKALITET
        {
            get { return this.GetString(92, 34); }
            set { this.SetString(value, 92, 34); }
        }
        ///  <summary>
        /// Danish: STANDARDADR
        /// Vejadrnvn,husnr,etage,sidedoer 
        ///  </summary>
        public string StandardAddress
        {
            get { return this.GetString(126, 34); }
            set { this.SetString(value, 126, 34); }
        }
        ///  <summary>
        /// Danish: BYNVN
        ///  </summary>
        public string CityName
        {
            get { return this.GetString(160, 34); }
            set { this.SetString(value, 160, 34); }
        }
        ///  <summary>
        /// Danish: POSTNR
        ///  </summary>
        public decimal PostCode
        {
            get { return this.GetDecimal(194, 4); }
            set { this.SetDecimal(value, 194, 4); }
        }
        ///  <summary>
        /// Danish: POSTDISTTXT
        ///  </summary>
        public string PostDistrict
        {
            get { return this.GetString(198, 20); }
            set { this.SetString(value, 198, 20); }
        }
        ///  <summary>
        /// Danish: KOMKOD
        ///  </summary>
        public decimal MunicipalityCode
        {
            get { return this.GetDecimal(218, 4); }
            set { this.SetDecimal(value, 218, 4); }
        }
        ///  <summary>
        /// Danish: VEJKOD
        ///  </summary>
        public decimal StreetCode
        {
            get { return this.GetDecimal(222, 4); }
            set { this.SetDecimal(value, 222, 4); }
        }
        ///  <summary>
        /// Danish: HUSNR
        ///  </summary>
        public string HouseNumber
        {
            get { return this.GetString(226, 4); }
            set { this.SetString(value, 226, 4); }
        }
        ///  <summary>
        /// Danish: ETAGE
        ///  </summary>
        public string Floor
        {
            get { return this.GetString(230, 2); }
            set { this.SetString(value, 230, 2); }
        }
        ///  <summary>
        /// Danish: SIDEDOER
        ///  </summary>
        public string Door
        {
            get { return this.GetString(232, 4); }
            set { this.SetString(value, 232, 4); }
        }
        ///  <summary>
        /// Danish: BNR
        ///  </summary>
        public string BuildingNumber
        {
            get { return this.GetString(236, 4); }
            set { this.SetString(value, 236, 4); }
        }
        ///  <summary>
        /// Danish: VEJADRNVN
        ///  </summary>
        public string StreetAddressingName
        {
            get { return this.GetString(240, 20); }
            set { this.SetString(value, 240, 20); }
        }
        ///  <summary>
        /// Danish: START_DT-BESKYTTELSE
        ///  </summary>
        public DateTime? ProtectionStartDate
        {
            get { return this.GetDateTime(260, 10, "yyyy-MM-dd"); }
            set { this.SetDateTime(value, 260, 10, "yyyy-MM-dd"); }
        }

        public Decimal ProtectionStartDateDecimal
        {
            get { return this.GetDecimal(260, 10); }
            set { this.SetDecimal(value, 260, 10); }
        }


        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("AddressingName", 24, 34),
                    new Tuple<string, int, int>("CareOfName", 58, 34),
                    new Tuple<string, int, int>("LOKALITET", 92, 34),
                    new Tuple<string, int, int>("StandardAddress", 126, 34),
                    new Tuple<string, int, int>("CityName", 160, 34),
                    new Tuple<string, int, int>("PostCode", 194, 4),
                    new Tuple<string, int, int>("PostDistrict", 198, 20),
                    new Tuple<string, int, int>("MunicipalityCode", 218, 4),
                    new Tuple<string, int, int>("StreetCode", 222, 4),
                    new Tuple<string, int, int>("HouseNumber", 226, 4),
                    new Tuple<string, int, int>("Floor", 230, 2),
                    new Tuple<string, int, int>("Door", 232, 4),
                    new Tuple<string, int, int>("BuildingNumber", 236, 4),
                    new Tuple<string, int, int>("StreetAddressingName", 240, 20),
                    new Tuple<string, int, int>("ProtectionStartDate", 260, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class MotherWithClearWrittenAddressType: _RelativeClearWrittenAddressType
    {
        #region Common

        public override int Length
        {
            get { return 269; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: PNRMOR
        /// Mother CPR Number
        ///  </summary>
        public string MotherPNR
        {
            get { return this.GetString(14, 10); }
            set { this.SetString(value, 14, 10); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("MotherPNR", 14, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class FatherWithClearWrittenAddressType: _RelativeClearWrittenAddressType
    {
        #region Common

        public override int Length
        {
            get { return 269; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: PNRMOR
        /// Mother CPR Number
        ///  </summary>
        public string FatherPNR
        {
            get { return this.GetString(14, 10); }
            set { this.SetString(value, 14, 10); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("FatherPNR", 14, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ChildWithClearWrittenAddressType: _RelativeClearWrittenAddressType
    {
        #region Common

        public override int Length
        {
            get { return 269; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: PNRMOR
        /// Mother CPR Number
        ///  </summary>
        public string MotherPNR
        {
            get { return this.GetString(14, 10); }
            set { this.SetString(value, 14, 10); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("MotherPNR", 14, 10)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class EventsType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 45; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: TIMESTAMPU
        /// Update date and time
        ///  </summary>
        public DateTime? CprUpdateDate
        {
            get { return this.GetDateTime(14, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 14, 12, "yyyyMMddHHmm"); }
        }

        public Decimal CprUpdateDateDecimal
        {
            get { return this.GetDecimal(14, 12); }
            set { this.SetDecimal(value, 14, 12); }
        }

        ///  <summary>
        /// Danish: HAENDELSE
        /// The event
        ///  </summary>
        public string Event_
        {
            get { return this.GetString(26, 3); }
            set { this.SetString(value, 26, 3); }
        }
        ///  <summary>
        /// Danish: AFLEDTMRK
        /// Derived mark
        ///  </summary>
        public string DerivedMark
        {
            get { return this.GetString(29, 2); }
            set { this.SetString(value, 29, 2); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("CprUpdateDate", 14, 12),
                    new Tuple<string, int, int>("Event_", 26, 3),
                    new Tuple<string, int, int>("DerivedMark", 29, 2)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class ErrorRecordType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 157; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: SORTFELT-10
        /// Can such. be PNR, FOEDDTO, KOEN (SEX) KOMKOD, VEJKOD
        ///  </summary>
        public string ErrorField
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: INDDATA
        /// Input
        ///  </summary>
        public string Input
        {
            get { return this.GetString(14, 75); }
            set { this.SetString(value, 14, 75); }
        }
        ///  <summary>
        /// Danish: FEJLNR
        /// Error number
        ///  </summary>
        public decimal ErrorNumber
        {
            get { return this.GetDecimal(89, 4); }
            set { this.SetDecimal(value, 89, 4); }
        }
        ///  <summary>
        /// Danish: FEJLTXT-UDTRÆK
        /// Error text
        ///  </summary>
        public string ErrorText
        {
            get { return this.GetString(93, 65); }
            set { this.SetString(value, 93, 65); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("ErrorField", 4, 10),
                    new Tuple<string, int, int>("Input", 14, 75),
                    new Tuple<string, int, int>("ErrorNumber", 89, 4),
                    new Tuple<string, int, int>("ErrorText", 93, 65)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class SubscriptionDeletionReceiptType: PersonRecordWrapper
    {
        #region Common

        public override int Length
        {
            get { return 43; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: PNR
        /// CPR Number
        ///  </summary>
        public string PNR
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: TIMESTAMPU
        /// Update date and time
        ///  </summary>
        public DateTime? UpdateTime
        {
            get { return this.GetDateTime(14, 12, "yyyyMMddHHmm"); }
            set { this.SetDateTime(value, 14, 12, "yyyyMMddHHmm"); }
        }

        public Decimal UpdateTimeDecimal
        {
            get { return this.GetDecimal(14, 12); }
            set { this.SetDecimal(value, 14, 12); }
        }

        ///  <summary>
        /// Danish: HAENDELSE
        /// Incident
        ///  </summary>
        public string Incident
        {
            get { return this.GetString(26, 3); }
            set { this.SetString(value, 26, 3); }
        }
        ///  <summary>
        /// Danish: NGLKONST
        /// Key Constant, blank on output if the field is not filled out input
        ///  </summary>
        public string KeyConstant
        {
            get { return this.GetString(29, 15); }
            set { this.SetString(value, 29, 15); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("PNR", 4, 10),
                    new Tuple<string, int, int>("UpdateTime", 14, 12),
                    new Tuple<string, int, int>("Incident", 26, 3),
                    new Tuple<string, int, int>("KeyConstant", 29, 15)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    public partial class EndRecordType: Wrapper
    {
        #region Common

        public override int Length
        {
            get { return 21; }
        }
        #endregion

        #region Properties

        ///  <summary>
        /// Danish: RECORDTYPE
        /// Equals last three digits of the record name
        ///  </summary>
        public decimal RecordType
        {
            get { return this.GetDecimal(1, 3); }
            set { this.SetDecimal(value, 1, 3); }
        }
        ///  <summary>
        /// Danish: TAELLER
        /// BLACK BOX 10
        ///  </summary>
        public string BlackBox10
        {
            get { return this.GetString(4, 10); }
            set { this.SetString(value, 4, 10); }
        }
        ///  <summary>
        /// Danish: TAELLER
        /// Counter
        ///  </summary>
        public string Counter
        {
            get { return this.GetString(14, 8); }
            set { this.SetString(value, 14, 8); }
        }

        #endregion
        public override Tuple<string, int, int>[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple<string, int, int>[]{
                    new Tuple<string, int, int>("RecordType", 1, 3),
                    new Tuple<string, int, int>("BlackBox10", 4, 10),
                    new Tuple<string, int, int>("Counter", 14, 8)
                });
                return ret.OrderBy(pd => pd.Item2).ToArray();
            }
        }

    }

  
    }
  