using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GKApp2010.RTE;

namespace DPRUpdateLib
{
    public abstract class UpdateDetectionVariables
    {
        // From Runner
        public abstract string Tag { get; }
        public abstract string ServiceName { get; }

        // From UpdateStagingBatch
        public abstract string ConnectionStringName { get; }
        public abstract string PnrColumnName { get; }
        public abstract string IdColumnName { get; }
        public abstract string StagingTableName { get; }
        public abstract string TableColumnName { get; }
        public abstract string TimestampColumnName { get; }
        public abstract string SchemaName { get; }

        // From UpdatesNotificationService

        // From ProjectInstaller
        public abstract string ServiceDescription { get; }

        // From CustomActions
        public abstract string ServiceExeFileName { get; }
        public abstract string DatabaseFeatureName { get; }

        // From SQL
        public abstract string[] TrackedTableNames { get; }
        public abstract string TablesTableName { get; }
        public abstract string SystemName { get; }

        public string SubstituteDDL(string ddl)
        {
            ddl = ddl.Replace(
                "<InsertTableNames>",
                string.Join(
                    Environment.NewLine,
                    this.TrackedTableNames.Select(
                        t => "INSERT INTO " + this.TablesTableName + " (" + this.TableColumnName + ") VALUES ('" + t + "')"
                        )
                    )
                );
            ddl = ddl.Replace("<StagingTableName>", this.StagingTableName);
            ddl = ddl.Replace("<PnrColumnName>", this.PnrColumnName);
            ddl = ddl.Replace("<TableColumnName>", this.TableColumnName);
            ddl = ddl.Replace("<TimestampColumnName>", this.TimestampColumnName);
            ddl = ddl.Replace("<TablesTableName>", this.TablesTableName);
            ddl = ddl.Replace("<SystemName>", this.SystemName);
            ddl = ddl.Replace("<IdColumnName>", this.IdColumnName);
            return ddl;
        }
    }

    public class DPRUpdateDetectionVariables : UpdateDetectionVariables
    {
        // From Runner        
        public override string Tag
        {
            get { return "DPRUpdateLib.Runner"; }
        }
        public override string ServiceName
        {
            get { return "DPR Updates Notification Service"; }
        }

        // From UpdateStagingBatch
        public override string ConnectionStringName
        {
            get { return "DPRUpdates"; }
        }
        public override string PnrColumnName
        {
            get { return "PNR"; }
        }
        public override string IdColumnName
        {
            get { return "Id"; }
        }
        public override string StagingTableName
        {
            get { return "T_DPRUpdateStaging"; }
        }
        public override string TableColumnName
        {
            get { return "DPRTable"; }
        }
        public override string TimestampColumnName
        {
            get { return "CreateTS"; }
        }
        public override string SchemaName
        {
            get { return "dbo"; }
        }

        // From UpdatesNotificationService

        // From ProjectInstaller
        public override string ServiceDescription
        {
            get
            {
                string s = "";
                s += "Collect changes made to persons in the DPR database and notify target systems such as CPRBroker of these changes. ";
                s += "In this version of the service, collection occurs at predefined intervals (polling the DB). Poll interval is defined in configuration. ";
                s += "Version=[" + Info.GetRuntimeVersion() + "]. ";
                s += "20110329/SDE";
                return s;
            }
        }

        // From CustomActions
        public override string ServiceExeFileName
        {
            get { return "DPRUpdatesNotificationService.exe"; }
        }
        public override string DatabaseFeatureName
        {
            get { return "DPRN"; }
        }

        // From SQL
        public override string[] TrackedTableNames
        {
            get
            {
                return new string[] { "DTBESKYT", "DTBOERN", "DTCIV", "DTFORALDREMYND", "DTFORSV", "DTKADR", "DTNAVNE", "DTPERS", "DTSEPARATION", "DTSTAT", "DTTOTAL", "DTUDRIND" };
            }
        }
        public override string TablesTableName
        {
            get { return "T_DPRUpdateStaging_DPRTable"; }
        }
        public override string SystemName
        {
            get { return "DPR"; }
        }
    }
}
