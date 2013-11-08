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

using System.Data;

using System.Runtime.Serialization;
using GKApp2010.DB;

namespace GKApp2010.GKS.OU
{
    // ================================================================================
    [DataContract]
    public class OUType
    {
        [DataMember]
        int id;

        [DataMember]
        string description;

        [DataMember]
        int refCnt;

        // --------------------------------------------------------------------------------
        public OUType()
        {
        }

        // --------------------------------------------------------------------------------
        public OUType(int aId, string aDescription, int aRefCnt)
        {
            id = aId;
            description = aDescription;
            refCnt = aRefCnt;
        }

        // --------------------------------------------------------------------------------
        public int Id
        {
            get { return id; }
        }

        // --------------------------------------------------------------------------------
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        // --------------------------------------------------------------------------------
        public int ReferenceCount
        {
            get { return refCnt; }
        }

        // --------------------------------------------------------------------------------
        // TODO Consider caching of OUType list
        public static OUTypeList GetOUTypes(string context, ref string aux)
        {
            OUTypeList ouTypeList = new OUTypeList();
            ouTypeList.Load(context, ref aux);
            return ouTypeList;
        }
    }

    // ================================================================================
    //[DataContract]
    public class OUTypeList : List<OUType>
    {
        // --------------------------------------------------------------------------------
        public void Load(string context, ref string aux)
        {
            //CREATE PROCEDURE spGKv2_OU_GetOUTypes
            //    @context            VARCHAR(1020),
            //    @aux                VARCHAR(1020)       OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);
            StoredProcedureCallContext spContext = new StoredProcedureCallContext("GKSDB", "spGKv2_OU_GetOUTypes");
            DataSet ds = spContext.ExecuteDataSet();

            this.Clear();

            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    int idxId = dt.Columns.IndexOf("Id");
                    int idxDescription = dt.Columns.IndexOf("Description");
                    int idxRefCnt = dt.Columns.IndexOf("RefCount");

                    foreach (DataRow dr in dt.Rows)
                    {
                        OUType ouType = new OUType((int)dr[idxId], (string)dr[idxDescription], (int)dr[idxRefCnt]);
                        this.Add(ouType);
                    }
                }
            }

            aux = spContext.Aux;
        }
    }
}
