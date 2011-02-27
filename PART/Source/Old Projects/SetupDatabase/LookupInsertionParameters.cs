using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration.Install;
using System.Data.OleDb;
using CprBroker.Engine.Util;
using CprBroker.DAL;
using CprBroker.DAL.Applications;
using CprBroker.DAL.Part;
using CprBroker.DAL.DataProviders;
using CprBroker.EventBroker.DAL;
using System.Data.SqlClient;

namespace CprBroker.SetupDatabase
{
    public class LookupInsertionParameters
    {
        public Installer Installer;
        public Type TableType;
        public string Contents;
        public string FileName;
        public ColumnType[] ColumnTypes;
        public DataTable LookupDataTable;

        private string FolderName = Guid.NewGuid().ToString();
        private string ShortFileName = Guid.NewGuid().ToString() + ".csv";

        public LookupInsertionParameters(Installer installer, Type tableType, string contents, params ColumnType[] columnTypes)
        {
            Installer = installer;
            TableType = tableType;
            Contents = contents;
            ColumnTypes = columnTypes;
            ReadLookupData();

        }

        public string DataFilePath
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", Installer.GetAssemblyFolderPath(), FolderName, ShortFileName);
            }
        }

        public string SchemaFilePath
        {
            get
            {
                return string.Format("{0}\\{1}\\Schema.ini", Installer.GetAssemblyFolderPath(), FolderName);
            }
        }

        public string FolderPath
        {
            get
            {
                return string.Format("{0}\\{1}", Installer.GetAssemblyFolderPath(), FolderName);
            }
        }

        private void ReadLookupData()
        {
            string tableName = TableType.Name;

            FileInfo targetFileInfo = new FileInfo(DataFilePath);

            LookupDataTable = new DataTable(tableName);
            // Write contents to physical file
            Directory.CreateDirectory(FolderPath);
            File.WriteAllText(targetFileInfo.FullName, Contents, System.Text.Encoding.Unicode);

            // Write Schema.ini file
            string schemaIniContents = string.Format("[{0}]\r\nFormat={1}\r\nColNameHeader={2}\r\nCharacterSet={3}",
                    targetFileInfo.Name,
                    "Delimited(;)",
                    true,
                    "Unicode"
                    );

            foreach (ColumnType colType in ColumnTypes)
            {
                if (colType.Index.HasValue && ColumnType.TypeMap.ContainsKey(colType.Type))
                {
                    schemaIniContents += string.Format("\r\nCol{0}={1} {2}",
                        colType.Index.Value,
                        colType.Name,
                        ColumnType.TypeMap[colType.Type]
                        );
                }
            }

            File.WriteAllText(SchemaFilePath, schemaIniContents);

            // Read data table
            OleDbConnectionStringBuilder lookupCsvConnectionStringBuilder = new OleDbConnectionStringBuilder();
            lookupCsvConnectionStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            lookupCsvConnectionStringBuilder.DataSource = targetFileInfo.Directory.FullName;
            lookupCsvConnectionStringBuilder.Add("Extended Properties", "text;");

            string readLookupDataSql = string.Format("select * from [{0}]", targetFileInfo.Name);
            OleDbDataAdapter oleAdpt = new OleDbDataAdapter(readLookupDataSql, lookupCsvConnectionStringBuilder.ConnectionString);

            foreach (ColumnType colType in ColumnTypes)
            {
                LookupDataTable.Columns.Add(colType.Name, colType.Type);
            }
            oleAdpt.Fill(LookupDataTable);

            // Delete temp files
            File.Delete(SchemaFilePath);
            File.Delete(DataFilePath);
            Directory.Delete(FolderPath);
        }

        public static LookupInsertionParameters[] InitializeInsertionParameters(Installer installer)
        {
            List<LookupInsertionParameters> ret = new List<LookupInsertionParameters>();

            //ret.Add(new LookupInsertionParameters(installer, typeof(AddressStatus), Properties.Resources.AddressStatus));
            ret.Add(new LookupInsertionParameters(installer, typeof(CprBroker.DAL.Applications.Application), Properties.Resources.Application,
                new ColumnType() { Name = "ApplicationId", Type = typeof(Guid) }
                ));
            ret.Add(new LookupInsertionParameters(installer, typeof(ChannelType), Properties.Resources.ChannelType));
            ret.Add(new LookupInsertionParameters(installer, typeof(DataProvider), Properties.Resources.DataProvider));
            ret.Add(new LookupInsertionParameters(installer, typeof(Gender), Properties.Resources.Gender));
            ret.Add(new LookupInsertionParameters(installer, typeof(LogType), Properties.Resources.LogType));
            ret.Add(new LookupInsertionParameters(installer, typeof(MaritalStatusType), Properties.Resources.MaritalStatusType));
            ret.Add(new LookupInsertionParameters(installer, typeof(Municipality), Properties.Resources.Municipality,
                new ColumnType() { Name = "MunicipalityCode", Type = typeof(string) }
                ));
            ret.Add(new LookupInsertionParameters(installer, typeof(RelationshipType), Properties.Resources.RelationshipType));
            ret.Add(new LookupInsertionParameters(installer, typeof(SubscriptionType), Properties.Resources.SubscriptionType));

            return ret.ToArray();
        }

        public static void InsertLookups(LookupInsertionParameters[] parameters, string connectionString)
        {
            foreach (var insertParam in parameters)
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);
                bulkCopy.DestinationTableName = insertParam.TableType.Name;
                bulkCopy.WriteToServer(insertParam.LookupDataTable);
            }
        }

        public class ColumnType
        {
            public string Name;
            public Type Type;
            public int? Index = null;

            public static readonly Dictionary<Type, string> TypeMap = new Dictionary<Type, string>();

            static ColumnType()
            {
                TypeMap[typeof(String)] = "Text Width 4000";
            }
        }
    }
}
