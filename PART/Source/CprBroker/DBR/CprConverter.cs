using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Engine.Local;

namespace CprBroker.DBR
{
    public class CprConverter
    {

        public static void DeletePersonRecords(string cprNumber, DPRDataContext dataContext)
        {
            decimal pnr = decimal.Parse(cprNumber);

            dataContext.PersonTotals.DeleteAllOnSubmit(dataContext.PersonTotals.Where(t => t.PNR == pnr));
            dataContext.Persons.DeleteAllOnSubmit(dataContext.Persons.Where(t => t.PNR == pnr));
            dataContext.Childs.DeleteAllOnSubmit(dataContext.Childs.Where(t => t.ParentPNR == pnr));
            dataContext.PersonNames.DeleteAllOnSubmit(dataContext.PersonNames.Where(t => t.PNR == pnr));
            dataContext.CivilStatus.DeleteAllOnSubmit(dataContext.CivilStatus.Where(t => t.PNR == pnr));
            dataContext.Separations.DeleteAllOnSubmit(dataContext.Separations.Where(t => t.PNR == pnr));
            dataContext.Nationalities.DeleteAllOnSubmit(dataContext.Nationalities.Where(t => t.PNR == pnr));
            dataContext.Departures.DeleteAllOnSubmit(dataContext.Departures.Where(t => t.PNR == pnr));
            dataContext.ContactAddresses.DeleteAllOnSubmit(dataContext.ContactAddresses.Where(t => t.PNR == pnr));
            dataContext.PersonAddresses.DeleteAllOnSubmit(dataContext.PersonAddresses.Where(t => t.PNR == pnr));
            dataContext.Protections.DeleteAllOnSubmit(dataContext.Protections.Where(t => t.PNR == pnr));
            dataContext.Disappearances.DeleteAllOnSubmit(dataContext.Disappearances.Where(t => t.PNR == pnr));
            dataContext.Events.DeleteAllOnSubmit(dataContext.Events.Where(t => t.PNR == pnr));
            dataContext.Notes.DeleteAllOnSubmit(dataContext.Notes.Where(t => t.PNR == pnr));
            dataContext.MunicipalConditions.DeleteAllOnSubmit(dataContext.MunicipalConditions.Where(t => t.PNR == pnr));
            dataContext.ParentalAuthorities.DeleteAllOnSubmit(dataContext.ParentalAuthorities.Where(t => t.ChildPNR == pnr));
            dataContext.GuardianAndParentalAuthorityRelations.DeleteAllOnSubmit(dataContext.GuardianAndParentalAuthorityRelations.Where(t => t.PNR == pnr));
            dataContext.GuardianAddresses.DeleteAllOnSubmit(dataContext.GuardianAddresses.Where(t => t.PNR == pnr));
        }

        /*
        public static void AppendPerson(IndividualResponseType person, DPRDataContext dataContext)
        {
            dataContext.PersonTotals.InsertOnSubmit(person.ToPersonTotal());

            dataContext.Persons.InsertOnSubmit(person.ToPerson());

            dataContext.Childs.InsertAllOnSubmit(person.Child.Select(c => c.ToDpr()));

            dataContext.PersonNames.InsertOnSubmit(person.CurrentNameInformation.ToDpr());
            dataContext.PersonNames.InsertAllOnSubmit(person.HistoricalName.Select(n => n.ToDpr()));

            dataContext.CivilStatus.InsertOnSubmit(person.CurrentCivilStatus.ToDpr());
            dataContext.CivilStatus.InsertAllOnSubmit(person.HistoricalCivilStatus.Select(c => c.ToDpr()));

            dataContext.Separations.InsertOnSubmit(person.CurrentSeparation.ToDpr());
            dataContext.Separations.InsertAllOnSubmit(person.HistoricalSeparation.Select(c => c.ToDpr()));

            dataContext.Nationalities.InsertOnSubmit(person.CurrentCitizenship.ToDpr());
            dataContext.Nationalities.InsertAllOnSubmit(person.HistoricalCitizenship.Select(c => c.ToDpr()));

            dataContext.Departures.InsertOnSubmit(person.CurrentDepartureData.ToDpr());
            dataContext.Departures.InsertAllOnSubmit(person.HistoricalDeparture.Select(c => c.ToDpr()));

            dataContext.ContactAddresses.InsertOnSubmit(person.ContactAddress.ToDpr());

            var currenrAddress = person.GetFolkeregisterAdresseSource(false) as CurrentAddressWrapper;
            if (currenrAddress != null)
                dataContext.PersonAddresses.InsertOnSubmit(currenrAddress.ToDpr());
            dataContext.PersonAddresses.InsertAllOnSubmit(person.HistoricalAddress.Select(c => c.ToDpr()));

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
                dataContext.GuardianAndParentalAuthorityRelations.InsertOnSubmit(person.Disempowerment.ToDpr());
                // TODO: Shall we also create records from ParentalAuthorityType??            
                dataContext.GuardianAddresses.InsertOnSubmit(person.Disempowerment.ToDprAddress());
            }
        }
         */

        public static void ImportGeoInformationFileInSteps(Stream dataStream, int batchSize, Encoding encoding, String connectionString)
        {
            var allPnrs = new List<string>();
            using (var file = new StreamReader(dataStream, encoding))
            {
                var extractResult = new ExtractParseResult();

                long totalReadLinesCount = 0;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var dataContext = new LookupDataContext(conn))
                    {
                        // Start reading the file
                        while (!file.EndOfStream)
                        {
                            var wrappers = CompositeWrapper.Parse(file, CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P05780, batchSize);
                            var batchReadLinesCount = wrappers.Count;
                            totalReadLinesCount += batchReadLinesCount;

                            using (var transactionScope = ExtractManager.CreateTransactionScope())
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
                                //Admin.LogFormattedSuccess("Geo Information was updated");
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
                var extractResult = new ExtractParseResult();

                long totalReadLinesCount = 0;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var dataContext = new LookupDataContext(conn))
                    {
                        // Start reading the file
                        while (!file.EndOfStream)
                        {
                            var wrappers = CompositeWrapper.Parse(file, CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P11980, batchSize);
                            var batchReadLinesCount = wrappers.Count;
                            totalReadLinesCount += batchReadLinesCount;

                            Admin.LogFormattedSuccess("Batch read, records found <{0}>, total so far <{1}>", batchReadLinesCount, totalReadLinesCount);

                            using (var transactionScope = ExtractManager.CreateTransactionScope())
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
                                Admin.LogFormattedSuccess("Geo Information was updated");
                                transactionScope.Complete();
                            }
                        }
                    }
                }
            }
        }
    
    }
}
