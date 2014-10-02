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

namespace BatchClient
{
    public class CompareDbr : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
                return dataContext
                    .PersonTotals
                    .Select(p => p.PNR)
                    .ToArray()
                    .Select(p => p.ToPnrDecimalString())
                    .ToArray();
            }
        }

        class TestInfo
        {
            public Type TestType;
            public Type DataType;
            public PropertyInfo[] Properties;
            public object TestInstance;

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

            using (var realDataContext = new DPRDataContext(OtherConnectionString))
            {
                using (var dbrDataContext = new DPRDataContext(OtherConnectionString2))
                {
                    foreach (var test in tests)
                    {
                        // Compare counts
                        try
                        {
                            test.TestType.InvokeMember(
                                "CompareCount",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                                null,
                                test.TestInstance,
                                new object[] { pnr, realDataContext, dbrDataContext });
                        }
                        catch (Exception ex)
                        {
                            pass = false;
                            Fail(
                                    string.Format("{0} Count mismatch in <{1}>\r\n{2}", pnr, test.DataType.Name, ex.InnerException.Message),
                                    ex.ToString());
                        }
                        
                        // Compare contents
                        foreach (var prop in test.Properties)
                        {
                            try
                            {
                                test.TestType.InvokeMember(
                                    "CompareContents",
                                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                    null,
                                    test.TestInstance,
                                    new object[] { prop, pnr, realDataContext, dbrDataContext }
                                    );
                            }
                            catch (Exception ex)
                            {
                                pass = false;
                                Fail(string.Format("{0} {1}.{2}\r\n----------------------------------\r\n{3}\r\n", pnr, test.DataType.Name, prop.Name, ex.InnerException.Message), "");
                            }
                            
                        }
                    }
                }
            }
            if (!pass)
                throw new Exception("Failed");


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