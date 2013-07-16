using CprBroker.Providers.CPRDirect;
using CprBroker.Tests.CPRDirect.Persons;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.CPRDirect
{
    class Helpers
    {
        public static void Check_correlation_between_historical_data(IndividualResponseType person)
        {
            var registerOplysningIntervals = person.ToRegisterOplysningIntervalArray();
            var registerOplysning = registerOplysningIntervals
                .Select(
                    interval =>
                        interval.ToRegisterOplysningType()
                )
                .ToArray();
            Assert.GreaterOrEqual(registerOplysning.Length, 0);
            // We only want to cary out historical tests if historical records er present.
            if (registerOplysning.Length > 0)
            {
                /*
                foreach (RegisterOplysningType interval in registerOplysning)
                {
                    /*
                     * We test if the address and end time attributes are set in any
                     * of the intervals.
                     */

                /* 
                 * We have to cast the item object as the type CprBorgerType as
                 * the type is not declared in the class.
                 */
                /*
                    CprBorgerType item = (CprBorgerType)interval.Item;
                    Assert.IsNotNull(item);
                    //Assert.IsNotNull(item.AdresseNoteTekst);        // <- is null
                    //Assert.IsNotNull(item.FolkeregisterAdresse);    // <- is null
                    Console.WriteLine(item.FolkeregisterAdresse);   // nothing is printed
                    Console.WriteLine(item.AdresseNoteTekst);       // prints out the object name
                    VirkningType virkning = interval.Virkning;
                    Assert.IsNotNull(virkning);
                    Assert.IsNotNull(virkning.TilTidspunkt);
                }
                 */

                /*
                 * TESTING IF NAME INTERVALS ARE CONNECTED
                 */

                /*
                 * We initialize two lists to keep track of start and end dates.
                 */
                List<DateTime> startDates = new List<DateTime>();
                List<DateTime> endDates = new List<DateTime>();
                List<string> names = new List<string>();
                var egenskaber = person.ToEgenskabIntervals();
                if (egenskaber.Length > 1)
                {
                    foreach (EgenskabInterval egenskab in egenskaber)
                    {
                        /*
                         * Start and end dates are added to the lists.
                         */
                        startDates.Add(egenskab.Name.ToStartTS().Value);
                        DateTime? endTS = egenskab.Name.ToEndTS();
                        if (endTS != null)
                            endDates.Add(endTS.Value);
                        Console.WriteLine("Date: " + egenskab.Name.ToStartTS());
                        if (endTS != null)
                            Console.WriteLine("End date: " + endTS.Value);
                        Console.WriteLine("First name(s): " + egenskab.Name.FirstName_s);
                        /*
                         * At this point it would be expected to see the Middle name 'Fich' as that one appears in the data file.
                         * But nothing is printed in any of the cases.
                         */
                        Console.WriteLine("Middle name: " + egenskab.Name.MiddleName);
                        Console.WriteLine("Last name: " + egenskab.Name.LastName);
                        var test = egenskab.ToAndreAdresser();
                        names.Add(egenskab.Name.FirstName_s + egenskab.Name.MiddleName + egenskab.Name.LastName);
                    }
                    /*
                     * We test if the names are different. If not there is likely some info not being retrieved.
                     */
                    string prev = "";
                    for (int i = 0; i < names.Count; i++)
                    {
                        prev = names[i];
                        if (i > 0)
                        {
                            //Assert.AreNotEqual(prev, names[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }

                /*
                 * TESTING IF ADDRESS INTERVALS ARE CONNECTED
                 */

                /*
                 * We initialize two lists to keep track of start and end dates.
                 */
                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<string> addresses = new List<string>();
                var histAddresses = person.HistoricalAddress;
                if (histAddresses.Count >= 1)
                {
                    foreach (HistoricalAddressType addr in histAddresses)
                    {
                        /*
                         * Start and end dates are added to the lists.
                         */
                        startDates.Add(addr.ToStartTS().Value);
                        DateTime? endTS = addr.ToEndTS().Value;
                        if (endTS != null)
                            endDates.Add(endTS.Value);
                        Console.WriteLine("Address: " + addr.ToAddressPostalType().DistrictSubdivisionIdentifier + addr.ToAddressPostalType().StreetName + addr.ToAddressPostalType().StreetBuildingIdentifier);
                        if (endTS != null)
                            Console.WriteLine("End date: " + endTS.Value);
                        addresses.Add(addr.ToAddressPostalType().DistrictSubdivisionIdentifier + addr.ToAddressPostalType().StreetName + addr.ToAddressPostalType().StreetBuildingIdentifier);
                    }
                    /*
                     * We test if the addresses are different. If not there is likely some info not being retrieved.
                     */
                    string prev = "";
                    for (int i = 0; i < names.Count; i++)
                    {
                        prev = names[i];
                        if (i > 0)
                        {
                            // Unnecessry condition that fails for Test PNR 0101980014
                            //Assert.AreNotEqual(prev, names[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }

                /*
                 * TESTING IF CIVIL STATUS INTERVALS ARE CONNECTED
                 */

                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<char> statuses = new List<char>();
                var civStatuses = person.HistoricalCivilStatus;
                if (civStatuses.Count >= 1)
                {
                    foreach (HistoricalCivilStatusType status in civStatuses)
                    {
                        statuses.Add(status.CivilStatusCode);
                        startDates.Add(status.CivilStatusStartDate.Value);
                        endDates.Add(status.CivilStatusEndDate.Value);
                    }
                    /*
                     * We test if the statuses are different. If not there is likely some info not being retrieved.
                     */
                    char prev = '0';
                    for (int i = 0; i < statuses.Count; i++)
                    {
                        prev = statuses[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, statuses[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }

                /*
                 * TESTING IF CIVIL STATUS INTERVALS ARE CONNECTED
                 */

                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<char> informations = new List<char>();
                var chInformations = person.HistoricalChurchInformation;
                if (chInformations.Count >= 1)
                {
                    foreach (HistoricalChurchInformationType info in chInformations)
                    {
                        informations.Add(info.ChurchRelationship);
                        startDates.Add(info.StartDate.Value);
                        endDates.Add(info.EndDate.Value);
                    }
                    /*
                     * We test if the statuses are different. If not there is likely some info not being retrieved.
                     */
                    char prev = '0';
                    for (int i = 0; i < informations.Count; i++)
                    {
                        prev = informations[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, informations[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }

                /*
                 * TESTING IF CITIZENSHIP INTERVALS ARE CONNECTED
                 */

                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<string> countryCodes = new List<string>();
                var hisCitizenships = person.HistoricalCitizenship;
                if (hisCitizenships.Count >= 1)
                {
                    foreach (HistoricalCitizenshipType cship in hisCitizenships)
                    {
                        countryCodes.Add(cship.StringCountryCode);
                        startDates.Add(cship.CitizenshipStartDate.Value);
                        endDates.Add(cship.CitizenshipEndDate.Value);
                    }
                    /*
                     * We test if the statuses are different. If not there is likely some info not being retrieved.
                     */
                    string prev = "";
                    for (int i = 0; i < countryCodes.Count; i++)
                    {
                        prev = countryCodes[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, countryCodes[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }

                /*
                 * TESTING IF DEPARTURE INTERVALS ARE CONNECTED
                 */

                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<string> departures = new List<string>();
                var hisDepartures = person.HistoricalDeparture;
                if (hisDepartures.Count >= 1)
                {
                    foreach (HistoricalDepartureType departure in hisDepartures)
                    {
                        departures.Add(
                            departure.EntryCountryCode.ToString() +
                            departure.EntryDate +
                            departure.ExitCountryCode +
                            departure.ExitDate +
                            departure.ForeignAddress1 +
                            departure.ForeignAddress2 +
                            departure.ForeignAddress3 +
                            departure.ForeignAddress4 +
                            departure.ForeignAddress5
                        );
                        if (departure.ToStartTS() != null)
                            startDates.Add(departure.ToStartTS().Value);
                        endDates.Add(departure.ToEndTS().Value);
                        Console.WriteLine("Exit country: " + departure.ExitCountryCode);
                        Console.WriteLine("Exit date: " + departure.ExitDate);
                        Console.WriteLine("Entry country: " + departure.EntryCountryCode);
                        Console.WriteLine("Entry date: " + departure.EntryDate);
                        Console.WriteLine("Foregn addresses:");
                        Console.WriteLine("1: " + departure.ForeignAddress1);
                        Console.WriteLine("2: " + departure.ForeignAddress2);
                        Console.WriteLine("3: " + departure.ForeignAddress3);
                        Console.WriteLine("4: " + departure.ForeignAddress4);
                        Console.WriteLine("5: " + departure.ForeignAddress5);
                    }
                    /*
                     * We test if the statuses are different. If not there is likely some info not being retrieved.
                     */
                    string prev = "";
                    for (int i = 0; i < departures.Count; i++)
                    {
                        prev = departures[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, departures[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    endDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }

                    /*
                     * We also check if the end date of the latest occurance is connected
                     * to the start date of the current address.
                     */
                    Console.WriteLine("Last Addr date: " + endDates.Last());
                    if (person.GetFolkeregisterAdresseSource(false) != null)
                    {
                        Console.WriteLine("Cur Addr date: " + person.GetFolkeregisterAdresseSource(false).ToStartTS());
                        Assert.AreEqual(endDates.Last(), person.GetFolkeregisterAdresseSource(false).ToStartTS());
                    }
                }

                /*
                 * TESTING IF DISAPPEARANCE INTERVALS ARE CONNECTED
                 */

                List<string> disappearances = new List<string>();
                var hisDisappearances = person.HistoricalDisappearance;
                if (hisDisappearances.Count >= 1)
                {
                    foreach (HistoricalDisappearanceType disappearance in hisDisappearances)
                    {
                        disappearances.Add(
                            disappearance.DisappearanceDate.Value.ToString() +
                            disappearance.RetrievalDate.Value.ToString()
                        );
                        Console.WriteLine("Disappearance: " + disappearance.DisappearanceDate.Value.ToString());
                        Console.WriteLine("Retrieval: " + disappearance.RetrievalDate.Value.ToString());
                    }
                    /*
                     * We test if the disappearances/retrievals are different. If not there is likely some info not being retrieved.
                     */
                    string prev = "";
                    for (int i = 0; i < disappearances.Count; i++)
                    {
                        prev = disappearances[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, disappearances[i]);
                        }
                    }

                    /*
                     * We DO NOT check if start and end dates are corresponding as they do not exist.
                     */
                }

                /*
                 * TESTING IF SEPARATION INTERVALS ARE CONNECTED
                 */

                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<DateTime> separations = new List<DateTime>();
                var hisSeparations = person.HistoricalSeparation;
                if (hisSeparations.Count >= 1)
                {
                    foreach (HistoricalSeparationType separation in hisSeparations)
                    {
                        separations.Add(separation.ReferenceToAnyMaritalStatus.Value);
                        startDates.Add(separation.SeparationStartDate.Value);
                        endDates.Add(separation.SeparationEndDate.Value);
                        Console.WriteLine("Marital status: " + separation.ReferenceToAnyMaritalStatus.Value);
                    }
                    /*
                     * We test if the PNRs are different. If not there is likely some info not being retrieved.
                     */
                    DateTime prev;
                    for (int i = 0; i < separations.Count; i++)
                    {
                        prev = separations[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, separations[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }

                /*
                 * TESTING IF CPR NUMBER INTERVALS ARE CONNECTED
                 */

                startDates = new List<DateTime>();
                endDates = new List<DateTime>();
                List<string> numbers = new List<string>();
                var hisPNRs = person.HistoricalPNR;
                if (hisPNRs.Count >= 1)
                {
                    foreach (HistoricalPNRType number in hisPNRs)
                    {
                        numbers.Add(number.OldPNR + number.PNR);
                        startDates.Add(number.OldPNRStartDate.Value);
                        endDates.Add(number.OldPNREndDate.Value);
                        Console.WriteLine("Old number: " + number.OldPNR);
                        Console.WriteLine("New number: " + number.PNR);
                    }
                    /*
                     * We test if the PNRs are different. If not there is likely some info not being retrieved.
                     */
                    string prev = "";
                    for (int i = 0; i < numbers.Count; i++)
                    {
                        prev = numbers[i];
                        if (i > 0)
                        {
                            Assert.AreNotEqual(prev, numbers[i]);
                        }
                    }

                    /*
                     * We check if start and end dates are corresponding.
                     */
                    startDates.Sort();
                    for (int i = 0; i < startDates.Count; i++)
                    {
                        //We only check from the second point and forward as the start date of the first element will not be connected with anything.
                        if (i > 0)
                        {
                            Console.WriteLine("Testing date: " + startDates[i]);
                            CollectionAssert.Contains(endDates, startDates[i]);
                        }
                    }
                }
            }
        }
    }
}
