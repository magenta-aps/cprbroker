using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration.Install;
using System.Data.OleDb;
using System.Data.SqlClient;
using CprBroker.Utilities;

namespace CprBroker.Installers
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
                return string.Format("{0}\\{1}\\{2}", Installer.GetInstallerAssemblyFolderPath(), FolderName, ShortFileName);
            }
        }

        public string SchemaFilePath
        {
            get
            {
                return string.Format("{0}\\{1}\\Schema.ini", Installer.GetInstallerAssemblyFolderPath(), FolderName);
            }
        }

        public string FolderPath
        {
            get
            {
                return string.Format("{0}\\{1}", Installer.GetInstallerAssemblyFolderPath(), FolderName);
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
