using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GKApp2010.RTE;

namespace UpdateLib
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

}
