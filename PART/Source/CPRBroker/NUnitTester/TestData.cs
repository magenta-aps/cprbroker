using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.NUnitTester.Part;

namespace CprBroker.NUnitTester
{
    /// <summary>
    /// Contains the data to be used to test the system
    /// </summary>
    public class TestData
    {

        public static string BaseAppToken = "5f8b7af5-422e-46bb-9273-5e244dc37505" ;//"07059250-E448-4040-B695-9C03F9E59E38";
        public static string userToken = "testUser";
        public static string AppToken = "";
        public static string AppNamePrefix = "Unit test app";
        public static readonly string[] cprNumbers = new string[] { 
            "2608450478", // 01   KMD 01    DPR 01
            "2904432108", // 01   KMD 01    DPR 01
            "2301813506", // 03   KMD 01    DPR 03
            "1805812172", // 05   KMD 01    DPR 01     Greenland
            "2811781323", // 05   KMD 01    DPR 05
            "2901763006", // 05                                    number invalid, former double number 
            "2112562489", // 20
            "1209971948", // 30  KMD 01     DPR 01
            "2802813581", // 60
            "1204463081", // 70 disappeared
            "1410733141", // 80 
            "2304230889", // 90
            //"1234123412", //not found in DPR
            "2101773607",
            //"1101812134",// laesResultat with multiple marriages
            //"2711640463", // has multiple names
            //"1403412399" // has no DTNAVNE in DPR
            //"502261703" //// has no DTNAVNE in DPR
        };
        public const string CprNumbersFieldName = "cprNumbers";

        public static string[] cprNumbersWithChildren = new string[] { "1305763892" };
        public const string CprNumbersWithChildrenFieldName = "cprNumbersWithChildren";

        public static string[] correctMethodNames = new string[] { "GetCitizenFull" };
        public const string CorrectMethodNamesFieldName = "correctMethodNames";

        public static string[] incorrectMethodNames = new string[] { "sadaklsjfkl", "sadfkafka" };
        public const string IncorrectMethodNamesFieldName = "incorrectMethodNames";

        public static string[][] cprNumbersToSubscribe = new string[][] { cprNumbers, null };
        public const string CprNumbersToSubscribeFieldName = "cprNumbersToSubscribe";

        public static List<Subscriptions.BirthdateSubscriptionType> birthdateSubscriptions = new List<NUnitTester.Subscriptions.BirthdateSubscriptionType>();
        public static Func<Subscriptions.BirthdateSubscriptionType>[] birthdateSubscriptionFunctions = new Func<NUnitTester.Subscriptions.BirthdateSubscriptionType>[]
            {
                ()=>birthdateSubscriptions[0],
                ()=>birthdateSubscriptions[1],
            };
        public const string birthdateSubscriptionFunctionsFieldName = "birthdateSubscriptionFunctions";

        public static List<Subscriptions.ChangeSubscriptionType> changeSubscriptions = new List<NUnitTester.Subscriptions.ChangeSubscriptionType>();
        public static Func<Subscriptions.ChangeSubscriptionType>[] changeSubscriptionFunctions = new Func<NUnitTester.Subscriptions.ChangeSubscriptionType>[]
            {
                ()=>changeSubscriptions[0],
                ()=>changeSubscriptions[1],
            };
        public const string changeSubscriptionFunctionsFieldName = "changeSubscriptionFunctions";

        public static int? birthdateYears = 10;
        public static int birthdateDays = 0;

        public static Subscriptions.FileShareChannelType fileShareChannel = new NUnitTester.Subscriptions.FileShareChannelType() { Path = "C:\\Notif" };
        public static Subscriptions.WebServiceChannelType webServiceChannel = new NUnitTester.Subscriptions.WebServiceChannelType() { WebServiceUrl = "http://cprbroker.beta/Services/Notification.asmx" };
        public static Subscriptions.ChannelBaseType[] Channels = new NUnitTester.Subscriptions.ChannelBaseType[] 
            {
                fileShareChannel,
                webServiceChannel
            };
        public const string ChannelsFieldName = "Channels";

        public static string serviceVersion = "1.0";

        public static string[] LogText
        {
            get
            {
                return new string[] { 
                    "adfadfdafa" + Guid.NewGuid().ToString(),
                    "45ikjgherio"+Guid.NewGuid().ToString()
                };
            }
        }
        public const string LogTextFieldName = "LogText";

        public static readonly string testPersonNumber = new Random().Next(10001, 99999).ToString() + new Random().Next(10001, 99999).ToString();
        public static Admin.PersonFullStructureType testFullPerson;

        public static void Initialize()
        {
            birthdateSubscriptions.Clear();
            changeSubscriptions.Clear();


            // Create test laesResultat
            testFullPerson = new NUnitTester.Admin.PersonFullStructureType()
            {
                Item = new

            NUnitTester.Admin.DanishAddressStructureType()
                {
                    AddressStatusCode = NUnitTester.Admin.AddressStatusCodeType.Item1,
                    CareOfName = "DDD ",
                    CompletePostalLabel = new NUnitTester.Admin.CompletePostalLabelType()
                    {
                        AddresseeName = "SSS",
                        PostalAddressFirstLineText = "Line1",
                        PostalAddressSecondLineText = "Line2",
                        PostalAddressThirdLineText = "Line3",
                        PostalAddressFourthLineText = "Line4",
                        PostalAddressFifthLineText = "Line5"
                    },
                    Item = new NUnitTester.Admin.AddressCompleteType()
                    {
                        AddressAccess = new NUnitTester.Admin.AddressAccessType(),
                        AddressPostal = new NUnitTester.Admin.AddressPostalType()
                    }
                },
                MaritalStatusCode = NUnitTester.Admin.MaritalStatusCodeType.unmarried,
                //PersonInformationProtectionIndicator = true,
                PersonNationalityCode = "DK",
                RegularCPRPerson = new NUnitTester.Admin.RegularCPRPersonType()
                {
                    PersonBirthDateStructure = new NUnitTester.Admin.PersonBirthDateStructureType()
                    {
                        BirthDate = new DateTime(2000, 1, 1),
                        BirthDateUncertaintyIndicator = false
                    },

                    PersonGenderCode = NUnitTester.Admin.PersonGenderCodeType.male,
                    PersonInformationProtectionIndicator = true,
                    PersonNameForAddressingName = "DDD FFF",
                    SimpleCPRPerson = new NUnitTester.Admin.SimpleCPRPersonType()
                    {
                        PersonCivilRegistrationIdentifier = testPersonNumber,
                        PersonNameStructure = new NUnitTester.Admin.PersonNameStructureType()
                        {
                            PersonGivenName = "KLjlksadfj",
                            PersonMiddleName = "DD",
                            PersonSurnameName = "KLJklj"
                        }
                    }
                }
            };
        }

        public LaesInputType[] EmptyReadInput = new LaesInputType[] { null, new LaesInputType() };
        public const string EmptyReadInputFieldName = "EmptyReadInput";

        public string[] InvalidReadInput = new string[] { null, "", Guid.NewGuid().ToString().Substring(10) };
        public const string InvalidReadInputFieldName = "InvalidReadInput";

        public string[] NonExistingUuidReadInput = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
        public const string NonExistingUuidReadInputFieldName = "NonExistingUuidReadInput";
    }
}
