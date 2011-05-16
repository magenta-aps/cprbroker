//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
