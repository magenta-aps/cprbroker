using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.DBR;
using CprBroker.Providers.DPR;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Utilities;
using System.Reflection;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using CprBroker.Tests.DBR;
using CprBroker.Tests.DBR.Comparison;
using CprBroker.Tests.DBR.Comparison.Person;
using CprBroker.Providers.CPRDirect;

namespace BatchClient
{
    /// <summary>
    /// Compares DPR and DBR databases, after (optionally) converting persons from a CPR broker database
    /// Paramatares
    /// /brokerDb "connection string to CPR broker database", will be used to update current config
    /// /otherDb "connection string to real DPR database"
    /// /otherDb2 "connection string to the generated DBR database"
    /// </summary>
    public class CompareDbr : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            // Set connection string
            if (!string.IsNullOrEmpty(BrokerConnectionString))
            {
                Utilities.UpdateConnectionString(BrokerConnectionString);
            }

            // Load CPR numbers
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
                return dataContext.ExecuteQuery<decimal>("select PNR from DTTOTAL WHERE INDLAESDTO IS NOT NULL")
                    .ToArray()
                    .Select(p => p.ToPnrDecimalString())
                    .ToArray();
            }
        }

        public class TestInfo
        {
            public Type TestType;
            public Type DataType;
            public PropertyInfo[] Properties;
            public object TestInstance;
            public MethodInfo CompareCountMethod;
            public MethodInfo CompareContentsMethod;
            public MethodInfo GetObjectMethod;

            static TestInfo[] _All;
            public static TestInfo[] All()
            {
                if (_All == null)
                {
                    var baseType = typeof(CprBroker.Tests.DBR.Comparison.Person.PersonComparisonTest<>);
                    _All = baseType
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsGenericType && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition().Equals(baseType))
                        .Select(t => new TestInfo()
                    {
                        TestType = t,
                        TestInstance = t.InvokeMember(null, BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null),
                        Properties = null
                    })
                    .ToArray();

                    Array.ForEach<TestInfo>(
                        _All,
                        t =>
                        {
                            t.DataType = t.TestType.BaseType.GetGenericArguments()[0];

                            t.Properties = t.TestType.InvokeMember(
                                "GetProperties",
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                null,
                                t.TestInstance,
                                null
                            ) as PropertyInfo[];

                            t.CompareCountMethod = t.TestType.GetMethod("CompareCount");

                            t.CompareContentsMethod = t.TestType.GetMethod("CompareContents");
                            t.GetObjectMethod = t.TestType.GetMethod("Get");

                        }
                    );
                }
                return _All;
            }
        }

        public override void ProcessPerson(string pnr)
        {
            bool pass = true;

            var tests = TestInfo.All();

            using (var dbrDataContext = new DPRDataContext(OtherConnectionString2))
            {
                // Convert
                if (!string.IsNullOrEmpty(BrokerConnectionString))
                {
                    CprConverter.DeletePersonRecords(pnr, dbrDataContext);
                    dbrDataContext.SubmitChanges();
                    var person = ExtractManager.GetPerson(pnr);
                    CprConverter.AppendPerson(person, dbrDataContext);
                    dbrDataContext.SubmitChanges();
                }

                // Compare
                using (var realDataContext = new DPRDataContext(OtherConnectionString))
                {
                    foreach (var test in tests)
                    {
                        // Compare counts
                        try
                        {
                            test.CompareCountMethod.Invoke(
                                test.TestInstance,
                                new object[] { pnr, realDataContext, dbrDataContext });
                        }
                        catch (Exception ex)
                        {
                            pass = false;
                            while (ex.InnerException != null)
                                ex = ex.InnerException;

                            Fail(
                                string.Format("{0} Count mismatch in <{1}>\r\n{2}", pnr, test.DataType.Name, ex.Message),
                                ex.ToString());
                        }

                        // Compare contents
                        var realTable = (test.GetObjectMethod.Invoke(test.TestInstance, new object[] { realDataContext, pnr }) as IQueryable).Cast<object>().ToArray();
                        var dbrTable = (test.GetObjectMethod.Invoke(test.TestInstance, new object[] { dbrDataContext, pnr }) as IQueryable).Cast<object>().ToArray();


                        foreach (var prop in test.Properties)
                        {
                            for (int i = 0; i < realTable.Length; i++)
                            {
                                var r = prop.GetValue(realTable[i], null);
                                var d = prop.GetValue(dbrTable[i], null);

                                if (!object.Equals(r, d))
                                {
                                    pass = false;
                                    Fail(
                                        string.Format("{0} {1}.{2}[{3}]\r\n   {4}",
                                            pnr, test.DataType.Name, prop.Name, i,
                                            ExpectedActual(r, d))
                                        , "");
                                }
                            }
                        }
                    }
                }
            }
            if (!pass)
                throw new Exception("Failed");
        }

        string ExpectedActual(object r, object d)
        {
            if (r == null)
                r = "NULL";
            if (d == null)
                d = "NULL";
            return string.Format("Expected <{0}>, Actual <{1}>", r, d);
        }

        TableInfo[] _DprDataContextProperties;
        TableInfo[] DprDataContextProperties
        {
            get
            {
                if (_DprDataContextProperties == null)
                {
                    _DprDataContextProperties = typeof(DPRDataContext)
                    .GetProperties(System.Reflection.BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(System.Data.Linq.Table<>)))
                    .Select(p => new TableInfo() { Prop = p, Attr = p.PropertyType.GetGenericArguments()[0].GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute })
                    .ToArray();
                }
                return _DprDataContextProperties;
            }
        }

        public class TableInfo
        {
            public PropertyInfo Prop;
            public Attribute Attr;
            public string TableName
            {
                get
                {
                    return CprBroker.Utilities.DataLinq.GetTableName(
                        Prop.PropertyType.GetGenericArguments().Single()
                        );
                }
            }

            public static DataSet GetDataSetObjects(TableInfo[] tables, DPRDataContext dataContext, string pnr)
            {
                string cmd = string.Join("",
                    tables.Select(t => string.Format("select * FROM {0} WHERE PNR=@PNR;", t.TableName)).ToArray());

                using (var adpt = new SqlDataAdapter(cmd, dataContext.Connection.ConnectionString))
                {
                    var ret = new DataSet();
                    adpt.SelectCommand.Parameters.Add(new SqlParameter("@PNR", pnr));
                    adpt.Fill(ret);
                    return ret;
                }
            }
        }

        private void CompareAdo(string pnr)
        {
            using (var realDataContext = new DPRDataContext(OtherConnectionString))
            {
                var real = TableInfo.GetDataSetObjects(this.DprDataContextProperties, realDataContext, pnr);

                using (var dbrDataContext = new DPRDataContext(OtherConnectionString2))
                {
                    var dbr = TableInfo.GetDataSetObjects(this.DprDataContextProperties, dbrDataContext, pnr);
                    foreach (DataTable realTable in real.Tables)
                    {
                        var dbrTable = dbr.Tables[realTable.TableName];

                        // Compare counts
                        if (dbrTable.Rows.Count != realTable.Rows.Count)
                        {
                            Fail(pnr, string.Format("<{0}> Count mismatch. Expected <{1}>, found <{2}>", realTable.TableName, realTable.Rows.Count, dbrTable.Rows.Count));
                        }
                    }
                }
            }
        }
    }
}