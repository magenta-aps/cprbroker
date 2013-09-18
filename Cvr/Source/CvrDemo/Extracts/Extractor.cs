using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using CvrDemo.Data;
namespace CvrDemo.Extracts
{
    public class Extractor
    {
        public void ImportUnits(string sourceFile, string connectionString)
        {
            var model = CreateUnitModelTable();
            Import(sourceFile, model, connectionString);
            
        }

        public void ImportOwners(string sourceFile, string connectionString)
        {
            var model = CreateOwnerModelTable();
            Import(sourceFile, model, connectionString);
        }

        public static DataTable CreateOwnerModelTable()
        {
            DataTable ret = new DataTable("Owner");
            ret.Columns.Add("Ajourføringsmarkering", typeof(int));
            ret.Columns.Add("OwnedLegalUnitIdentifier", typeof(int));
            ret.Columns.Add("ParticipantType", typeof(int));
            ret.Columns.Add("ParticipantvalidFromDate", typeof(decimal));
            ret.Columns.Add("ParticipantIdentifier", typeof(long));
            ret.Columns.Add("ParticipantName", typeof(string));
            ret.Columns.Add("LegalUnitIdentifier", typeof(int));

            return ret;
        }

        public static DataTable CreateUnitModelTable()
        {
            DataTable ret = new DataTable("Unit");
            ret.Columns.Add("Ajourfoeringsmarkering", typeof(int));
            ret.Columns.Add("LegalUnitIdentifier", typeof(int));
            ret.Columns.Add("ProductionUnitIdentifier", typeof(int));
            ret.Columns.Add("NameValidFromDate", typeof(decimal));
            ret.Columns.Add("Name", typeof(string));
            ret.Columns.Add("AdvertisingProtectionIndicator", typeof(bool));
            ret.Columns.Add("MainDivisionIndicator", typeof(bool));
            ret.Columns.Add("StartDate", typeof(decimal));
            ret.Columns.Add("CessationDate", typeof(decimal));
            ret.Columns.Add("AddressOfficialValidFromDate", typeof(decimal));
            ret.Columns.Add("AddressOfficialMunicipalityCode", typeof(int));
            ret.Columns.Add("AddressOfficialStreetCode", typeof(int));
            ret.Columns.Add("AddressOfficialStreetBuildingIdentifier", typeof(string));
            ret.Columns.Add("AddressOfficialStreetName", typeof(string));
            ret.Columns.Add("AddressOfficialFloorIdentifier", typeof(string));
            ret.Columns.Add("AddressOfficialSuiteIdentifier", typeof(string));

            ret.Columns.Add("AddressOfficialDistrictSubdivisionIdentifier", typeof(string));
            ret.Columns.Add("AddressOfficialPostCodeIdentifier", typeof(int));
            ret.Columns.Add("AddressOfficialDistrictName", typeof(string));
            ret.Columns.Add("AddressOfficialCareOfName", typeof(string));
            ret.Columns.Add("AddressOfficialStreetBuildingIdentifierTo", typeof(string));
            ret.Columns.Add("AddressOfficialRegionCode", typeof(int));
            ret.Columns.Add("AddressOfficialAddressLineText", typeof(string));
            ret.Columns.Add("AddressPostalValidFromDate", typeof(decimal));
            ret.Columns.Add("AddressPostalMunicipalityCode", typeof(int));
            ret.Columns.Add("AddressPostalStreetCode", typeof(int));
            ret.Columns.Add("AddressPostalStreetBuildingIdentifier", typeof(string));
            ret.Columns.Add("AddressPostalStreetName", typeof(string));
            ret.Columns.Add("AddressPostalFloorIdentifier", typeof(string));
            ret.Columns.Add("AddressPostalSuiteIdentifier", typeof(string));
            ret.Columns.Add("AddressPostalDistrictSubdivisionIdentifier", typeof(string));
            ret.Columns.Add("AddressPostalPostOfficeBoxIdentifier", typeof(string));
            ret.Columns.Add("AddressPostalPostCodeIdentifier", typeof(int));
            ret.Columns.Add("AddressPostalDistrictName", typeof(string));
            ret.Columns.Add("AddressPostalStreetBuildingIdentifierTo", typeof(string));
            ret.Columns.Add("AddressPostalRegionCode", typeof(int));
            ret.Columns.Add("AddressPostalAddressLineText", typeof(string));
            ret.Columns.Add("MainActivityValidFromDate", typeof(decimal));
            ret.Columns.Add("MainActivityActivityCode", typeof(int));
            ret.Columns.Add("MainActivityActivityDescription", typeof(string));
            ret.Columns.Add("SecondaryActivity1ValidFromDate", typeof(decimal));
            ret.Columns.Add("SecondaryActivity1ActivityCode", typeof(int));
            ret.Columns.Add("SecondaryActivity2ValidFromDate", typeof(decimal));

            ret.Columns.Add("SecondaryActivity2ActivityCode", typeof(int));
            ret.Columns.Add("SecondaryActivity3ValidFromDate", typeof(decimal));
            ret.Columns.Add("SecondaryActivity3ActivityCode", typeof(int));
            ret.Columns.Add("BusinessFormatBusinessFormatCode", typeof(int));
            ret.Columns.Add("BusinessFormatDescription", typeof(string));
            ret.Columns.Add("BusinessFormatDataSupplierIdentifier", typeof(string));
            ret.Columns.Add("EmploymentQuarterReferenceYear", typeof(int));
            ret.Columns.Add("EmploymentQuarterReferenceQuarter", typeof(int));
            ret.Columns.Add("EmploymentQuarterEmploymentIntervalCode", typeof(string));
            ret.Columns.Add("TelephoneNumberIdentifier", typeof(string));
            ret.Columns.Add("FaxNumberIdentifier", typeof(string));
            ret.Columns.Add("EmailAdressIdentifier", typeof(string));
            ret.Columns.Add("ObligationValidFromDate", typeof(decimal));
            ret.Columns.Add("ObligationCode", typeof(string));
            ret.Columns.Add("CreditorInformationValidFromDate", typeof(decimal));
            ret.Columns.Add("CreditorStatusInformationCode", typeof(int));

            return ret;
        }

        public static void Import(string sourceFile, DataTable model, string connectionString)
        {
            SqlBulkCopy bulk = new SqlBulkCopy(connectionString);
            bulk.DestinationTableName = model.TableName;
            bulk.BatchSize = 10000;

            var boolColumns = new List<int>();
            var decimalColumns = new List<int>();
            var intColumns = new List<int>();
            var longColumns = new List<int>();

            for (int col = 0; col < model.Columns.Count; col++)
            {
                if (model.Columns[col].DataType == typeof(bool))
                {
                    boolColumns.Add(col);
                }
                else if (model.Columns[col].DataType == typeof(decimal))
                {
                    decimalColumns.Add(col);
                }
                else if (model.Columns[col].DataType == typeof(int))
                {
                    intColumns.Add(col);
                }
                else if (model.Columns[col].DataType == typeof(long))
                {
                    longColumns.Add(col);
                }

            }

            using (var source = new StreamReader(sourceFile, Encoding.GetEncoding(1252)))
            {

                int i = 0;
                while (!source.EndOfStream)
                {
                    string line = source.ReadLine();

                    if (i > 0)
                    {
                        var values = line.Split(';').Select(o => o as object).ToArray();

                        foreach (var col in boolColumns)
                        {
                            string val = values[col] as string;
                            switch (val)
                            {
                                case "1":
                                    values[col] = true;
                                    break;
                                case "0":
                                    values[col] = false;
                                    break;
                                default:
                                    values[col] = DBNull.Value;
                                    break;
                            }
                        }

                        foreach (var col in decimalColumns)
                        {
                            string val = values[col] as string;
                            decimal decimalVal;
                            if (decimal.TryParse(val, out decimalVal))
                                values[col] = decimalVal;
                            else
                                values[col] = DBNull.Value;
                        }

                        foreach (var col in intColumns)
                        {
                            string val = values[col] as string;
                            int intVal;
                            if (int.TryParse(val, out intVal))
                                values[col] = intVal;
                            else
                                values[col] = DBNull.Value;
                        }

                        model.Rows.Add(values);
                    }
                    i++;

                }
                bulk.WriteToServer(model);
            }
        }
    }
}