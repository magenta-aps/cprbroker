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
 * Dennis Amdi Skov Isaksen
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
using System.IO;
using System.Transactions;
using System.Data.SqlClient;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Engine.Local;
using CprBroker.DBR.Extensions;

namespace CprBroker.DBR
{
    public partial class CprConverter
    {

        public static TransactionScope CreateTransactionScope()
        {
            return new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Required,
                new System.Transactions.TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = System.Transactions.TransactionManager.MaximumTimeout
                });
        }

        public static int DeletePersonRecords(string cprNumber, DPRDataContext dataContext)
        {
            return DeletePersonRecords(new string[] { cprNumber }, dataContext);
        }

        public static int DeletePersonRecords(string[] cprNumbers, DPRDataContext dataContext)
        {
            decimal[] pnrs = cprNumbers.Select(cpr => decimal.Parse(cpr)).ToArray();

            var types = new Type[]{
                typeof(PersonTotal),
                typeof(Person),
                typeof(Child),
                typeof(PersonName),
                typeof(CivilStatus),
                typeof(Separation),
                typeof(Nationality),
                typeof(Departure),
                typeof(ContactAddress),
                typeof(PersonAddress),
                typeof(Protection),
                typeof(Disappearance),
                typeof(Event),
                typeof(Note),
                typeof(MunicipalCondition),
                typeof(ParentalAuthority),
                typeof(GuardianAndParentalAuthorityRelation),
                typeof(GuardianAddress),
            };
            string paramArray = string.Join(", ", Enumerable.Range(0, pnrs.Length).Select(i => string.Format("{{{0}}}", i)).ToArray());
            var cmd =
                string.Join(
                    Environment.NewLine,
                    types.Select(t => string.Format("DELETE {0} WHERE PNR IN ({1});", CprBroker.Utilities.DataLinq.GetTableName(t), paramArray)).ToArray()
                    );
            var ret = dataContext.ExecuteCommand(cmd, pnrs.Select(p => p as object).ToArray());
            return ret;
        }

        public static void AppendPerson(IndividualResponseType person, DPRDataContext dataContext, char dataRetrievalType = CprBroker.Providers.DPR.DataRetrievalTypes.Extract, char? updatingProgram = null)
        {
            dataContext.PersonTotal7s.InsertOnSubmit(person.ToPersonTotal(dataContext, dataRetrievalType, updatingProgram));

            dataContext.Persons.InsertOnSubmit(person.ToPerson(dataContext));

            dataContext.Childs.InsertAllOnSubmit(person.Child.Select(c => c.ToDpr()));

            PersonInformationType pit = person.PersonInformation;
            var lst = new List<PersonName>();

            lst.Add(person.CurrentNameInformation.ToDpr(pit));
            lst.AddRange(person.HistoricalName.Select(n => n.ToDpr()));
            dataContext.PersonNames.InsertAllOnSubmit(
                CprBroker.Utilities.DataLinq.Distinct<PersonName>(lst)
                );

            // TODO: We cannot directly map civil status records here, because CPR direct has 2 records for married->divorecd, while DPR represents this as one record
            // Refer to DPR provider for a similar solution
            dataContext.CivilStatus.InsertOnSubmit(person.CurrentCivilStatus.ToDpr());
            dataContext.CivilStatus.InsertAllOnSubmit(person.HistoricalCivilStatus.Select(c => c.ToDpr()));

            if (person.CurrentSeparation != null)
                dataContext.Separations.InsertOnSubmit(person.CurrentSeparation.ToDpr());

            dataContext.Separations.InsertAllOnSubmit(person.HistoricalSeparation.Select(c => c.ToDpr()));

            dataContext.Nationalities.InsertOnSubmit(person.CurrentCitizenship.ToDpr());
            dataContext.Nationalities.InsertAllOnSubmit(person.HistoricalCitizenship.Select(c => c.ToDpr()));

            if (person.CurrentDepartureData != null)
            {
                ElectionInformationType eit = person.ElectionInformation.OrderByDescending(i => i.VotingDate).FirstOrDefault() as ElectionInformationType;
                dataContext.Departures.InsertOnSubmit(person.CurrentDepartureData.ToDpr(eit));
            }
            dataContext.Departures.InsertAllOnSubmit(person.HistoricalDeparture.Select(c => c.ToDpr()));

            if (person.ContactAddress != null)
                dataContext.ContactAddresses.InsertOnSubmit(person.ContactAddress.ToDpr());

            // Addresses
            var personAddresses = new List<PersonAddress>();

            foreach (var adr in person.HistoricalAddress.OrderBy(a => a.RelocationDate.Value))
            {
                personAddresses.Add(adr.ToDpr(dataContext));
            }

            var currentAddress = person.GetFolkeregisterAdresseSource(false) as CurrentAddressWrapper;
            if (currentAddress != null)
                personAddresses.Add(currentAddress.ToDpr(dataContext, person.PersonInformation));

            CprConverterExtensions.ClearPreviousAddressData(personAddresses.ToArray());
            dataContext.PersonAddresses.InsertAllOnSubmit(personAddresses);
            // End addresses
            dataContext.Protections.InsertAllOnSubmit(person.Protection.Select(p => p.ToDpr()));

            if (person.CurrentDisappearanceInformation != null)
                dataContext.Disappearances.InsertOnSubmit(person.CurrentDisappearanceInformation.ToDpr());
            dataContext.Disappearances.InsertAllOnSubmit(person.HistoricalDisappearance.Select(d => d.ToDpr()));

            dataContext.Events.InsertAllOnSubmit(person.Events.Select(ev => ev.ToDpr()));

            dataContext.Notes.InsertAllOnSubmit(person.Notes.Select(n => n.ToDpr()));

            dataContext.MunicipalConditions.InsertAllOnSubmit(person.MunicipalConditions.Select(c => c.ToDpr()));

            dataContext.ParentalAuthorities.InsertAllOnSubmit(person.ParentalAuthority.Select(p => p.ToDpr()));

            if (person.Disempowerment != null)
            {
                // TODO: Shall we also create records from ParentalAuthorityType??            
                var gpar = person.Disempowerment.ToDpr();
                if (gpar != null)
                    dataContext.GuardianAndParentalAuthorityRelations.InsertOnSubmit(gpar);

                // TODO: Shall we also create records from ParentalAuthorityType??            
                var ga = person.Disempowerment.ToDprAddress();
                if (ga != null)
                    dataContext.GuardianAddresses.InsertOnSubmit(ga);
            }
        }

        public static void ImportGeoInformationFileInSteps(Stream dataStream, int batchSize, Encoding encoding, String connectionString)
        {
            using (var file = new StreamReader(dataStream, encoding))
            {
                var extractResult = new ExtractParseSession();

                long totalReadLinesCount = 0;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(conn))
                    {
                        // Start reading the file
                        while (!file.EndOfStream)
                        {
                            var wrappers = CompositeWrapper.Parse(file, CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P05780, batchSize);
                            var batchReadLinesCount = wrappers.Count;
                            totalReadLinesCount += batchReadLinesCount;

                            using (var transactionScope = CreateTransactionScope())
                            {
                                foreach (var w in wrappers)
                                {
                                    if (w is StreetType)
                                    {
                                        var s = (w as StreetType).ToDprStreet();
                                        dataContext.Streets.InsertOnSubmit(s);
                                    }
                                    else if (w is CityType)
                                    {
                                        var c = (w as CityType).ToDprCity();
                                        dataContext.Cities.InsertOnSubmit(c);
                                    }
                                    else if (w is PostDistrictType)
                                    {
                                        var p = (w as PostDistrictType).ToDprPostDistrict();
                                        dataContext.PostDistricts.InsertOnSubmit(p);
                                    }
                                    else if (w is AreaRestorationDistrictType)
                                    {
                                        var a = (w as AreaRestorationDistrictType).ToDprAreaRestorationDistrict();
                                        dataContext.AreaRestorationDistricts.InsertOnSubmit(a);
                                    }
                                    else if (w is DiverseDistrictType)
                                    {
                                        var d = (w as DiverseDistrictType).ToDprDiverseDistrict();
                                        dataContext.DiverseDistricts.InsertOnSubmit(d);
                                    }
                                    else if (w is EvacuationDistrictType)
                                    {
                                        var e = (w as EvacuationDistrictType).ToDprEvacuationDistrict();
                                        dataContext.EvacuationDistricts.InsertOnSubmit(e);
                                    }
                                    else if (w is ChurchDistrictType)
                                    {
                                        var c = (w as ChurchDistrictType).ToDprChurchDistrict();
                                        dataContext.ChurchDistricts.InsertOnSubmit(c);
                                    }
                                    else if (w is SchoolDistrictType)
                                    {
                                        var s = (w as SchoolDistrictType).ToDprSchoolDistrict();
                                        dataContext.SchoolDistricts.InsertOnSubmit(s);
                                    }
                                    else if (w is PopulationDistrictType)
                                    {
                                        var p = (w as PopulationDistrictType).ToDprPopulationDistrict();
                                        dataContext.PopulationDistricts.InsertOnSubmit(p);
                                    }
                                    else if (w is SocialDistrictType)
                                    {
                                        var s = (w as SocialDistrictType).ToDprSocialDistrict();
                                        dataContext.SocialDistricts.InsertOnSubmit(s);
                                    }
                                    else if (w is ChurchAdministrationDistrictType)
                                    {
                                        var c = (w as ChurchAdministrationDistrictType).ToDprChurchAdministrationDistrict();
                                        dataContext.ChurchAdministrationDistricts.InsertOnSubmit(c);
                                    }
                                    else if (w is ElectionDistrictType)
                                    {
                                        var e = (w as ElectionDistrictType).ToDprElectionDistrict();
                                        dataContext.ElectionDistricts.InsertOnSubmit(e);
                                    }
                                    else if (w is HeatingDistrictType)
                                    {
                                        var h = (w as HeatingDistrictType).ToDprHeatingDistrict();
                                        dataContext.HeatingDistricts.InsertOnSubmit(h);
                                    }
                                    else if (w is PostNumberType)
                                    {
                                        var p = (w as PostNumberType).ToDprPostNumber();
                                        dataContext.PostNumbers.InsertOnSubmit(p);
                                    }
                                }
                                dataContext.SubmitChanges();
                                transactionScope.Complete();
                            }
                        }
                    }
                }
            }
        }

        public static void ImportPostCodeFileInSteps(Stream dataStream, int batchSize, Encoding encoding, String connectionString)
        {
            var allPnrs = new List<string>();
            using (var file = new StreamReader(dataStream, encoding))
            {
                var extractResult = new ExtractParseSession();

                long totalReadLinesCount = 0;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(conn))
                    {
                        // Start reading the file
                        while (!file.EndOfStream)
                        {
                            var wrappers = CompositeWrapper.Parse(file, CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P11980, batchSize);
                            var batchReadLinesCount = wrappers.Count;
                            totalReadLinesCount += batchReadLinesCount;

                            using (var transactionScope = CreateTransactionScope())
                            {
                                foreach (var w in wrappers)
                                {
                                    if (w is PostNumberType)
                                    {
                                        var p = (w as PostNumberType).ToDprPostNumber();
                                        dataContext.PostNumbers.InsertOnSubmit(p);
                                    }
                                }
                                dataContext.SubmitChanges();
                                transactionScope.Complete();
                            }
                        }
                    }
                }
            }
        }

        [Obsolete("This functionality should be done inside the appropriate queue")]
        public static void ImportCprDirectFileInSteps(Stream dataStream, String filePath, int batchSize, Encoding encoding, String connectionString)
        {
            var allPnrs = new List<string>();
            using (var file = new StreamReader(dataStream, encoding))
            {
                // TODO: This code reads the whole file at once, which could be extremely heavy for a computer (or even impossible),
                // Code should utilise batchSize parameter
                var extractResult = new ExtractParseSession(file.ReadToEnd(), CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                var extract = extractResult.ToExtract(filePath);
                var extractItems = extractResult.ToExtractItems(
                    extract.ExtractId,
                    CprBroker.Providers.CPRDirect.Constants.DataObjectMap,
                    CprBroker.Providers.CPRDirect.Constants.RelationshipMap,
                    CprBroker.Providers.CPRDirect.Constants.MultiRelationshipMap
                );
                using (var transactionScope = CreateTransactionScope())
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                        {
                            conn.BulkInsertAll<Extract>(new Extract[] { extract });
                            conn.BulkInsertAll<ExtractItem>(extractItems);
                            trans.Commit();
                        }
                    }
                    transactionScope.Complete();
                }
                using (var dataContext = new ExtractDataContext())
                {
                    extract = dataContext.Extracts.Where(ex => ex.ExtractId == extract.ExtractId).First();
                    extract.Ready = true;
                    dataContext.SubmitChanges();
                }
                var extractDataContext = new ExtractDataContext();
                using (var dbrConn = new SqlConnection(connectionString))
                {
                    dbrConn.Open();
                    using (var dbrDataContext = new CprBroker.Providers.DPR.LookupDataContext(dbrConn))
                    {
                        // TODO: This should only loop over PNR's, not all items
                        foreach (ExtractItem item in extractItems)
                        {
                            Console.WriteLine("ITEM: " + item.PNR);
                            var pnr = item.PNR;
                            IndividualResponseType individual = Extract.GetPersonFromLatestExtract(
                                pnr,
                                extractDataContext.ExtractItems,
                            CprBroker.Providers.CPRDirect.Constants.DataObjectMap
                            );
                            AppendPerson(individual, new DPRDataContext(dbrConn));
                        }
                    }
                }
            }
        }
    }
}
