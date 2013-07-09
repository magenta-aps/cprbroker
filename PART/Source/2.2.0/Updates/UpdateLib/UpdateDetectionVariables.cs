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
        public abstract string ServiceDisplayName { get; }

        // From CustomActions
        public abstract string ServiceExeFileName { get; }
        public abstract string DatabaseFeatureName { get; }

        // From SQL
        public abstract string[] TrackedTableNames { get; }
        public abstract string TablesTableName { get; }
        public abstract string SystemName { get; }

        // Control parameters
        public abstract bool TriggersEnabled { get; }
        public virtual string ExtraDatabaseCreateDDL { get { return null; } }
        public virtual string ExtraDatabaseDropDDL { get { return null; } }

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
