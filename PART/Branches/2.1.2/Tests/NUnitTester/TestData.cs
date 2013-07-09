/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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

        public static string BaseAppToken = "5f8b7af5-422e-46bb-9273-5e244dc37505";//"07059250-E448-4040-B695-9C03F9E59E38";
        public static string userToken = "testUser";
        public static string AppToken = "";
        public static string AppNamePrefix = "Unit test app";
        public static readonly string[] cprNumbers = new string[] { 
           
        };
        public const string CprNumbersFieldName = "cprNumbers";

        public static readonly string[] invalidCprNumbers = new string[]{
            null,
            "",
            "adsa",
            "1234567894342324",
            "jkash77",
            "          ",
            "ddddd",
            "  12345678",
            "    56    ",
        };
        public const string InvalidCprNumbersFieldName = "invalidCprNumbers";

        public static readonly Part.SoegInputType1[] InvalidSearchCriteria = new Part.SoegInputType1[]
        {
            null,
            new Part.SoegInputType1(),
            new Part.SoegInputType1()
            {
                SoegObjekt=new Part.SoegObjektType(){ UUID=Guid.NewGuid().ToString()+"D"}
            },
            new Part.SoegInputType1()
            {
                SoegObjekt=new Part.SoegObjektType(),
                FoersteResultatReference="dd"
            },
            new Part.SoegInputType1()
            {
                SoegObjekt=new Part.SoegObjektType(),
                MaksimalAntalKvantitet="-1"
            },
        };
        public const string InvalidSearchCriteriaFieldName = "InvalidSearchCriteria";
                
        public static string[] correctMethodNames = new string[] { "GetUuid" };
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

        public static void Initialize()
        {
            birthdateSubscriptions.Clear();
            changeSubscriptions.Clear();
        }

        public Part.LaesInputType[] EmptyReadInput = new Part.LaesInputType[] { null, new Part.LaesInputType() };
        public const string EmptyReadInputFieldName = "EmptyReadInput";

        public string[] InvalidReadInput = new string[] { null, "", Guid.NewGuid().ToString().Substring(10) };
        public const string InvalidReadInputFieldName = "InvalidReadInput";

        public string[] NonExistingUuidReadInput = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        public const string NonExistingUuidReadInputFieldName = "NonExistingUuidReadInput";
    }
}
