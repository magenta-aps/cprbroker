using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;

namespace PersonMasterTestClient
{
    [TestFixture]
    class Tests
    {

        public static string[] RandomCprNumbers(int count)
        {
            var cprNumbers = new List<string>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                var day = random.Next(1, 29).ToString("00");
                var month = random.Next(1, 13).ToString("00");
                var year = random.Next(1, 100).ToString("00");
                var part1 = random.Next(1000, 9999).ToString();
                cprNumbers.Add(day + month + year + part1);
            }
            return cprNumbers.ToArray();
        }

        private const string PersonMasterConnectionString = "";

        [Test]
        public void TestGetUuidArray(
            [Random(0, 100, 10)] int count)
        {
            var cprNumbers = RandomCprNumbers(count);
            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;

            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);

            Assert.IsNotNull(ret, "Return value is null");
            Assert.AreEqual(cprNumbers.Length, ret.Length, "Return value length does not equal input length");
            foreach (var uuid in ret)
            {
                Assert.AreNotEqual(Guid.Empty, uuid, "UUID is empty");
                Assert.AreEqual(1, ret.Where((id) => id == uuid).Count(), "Repeated UUID : " + uuid.ToString());
            }
        }

        public void TestGetUUIDStoredProcedure(int count)
        {
            var cprNumbers = RandomCprNumbers(count);
            using (var connection = new SqlConnection(PersonMasterConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("spGK_PM_GetObjectIDsFromCPRArray", connection))
                {
                    //CREATE PROCEDURE spGK_PM_GetObjectIDFromCPR
                    //    @context            VARCHAR(120),
                    //    @cprNo              VARCHAR(10),
                    //    @objectOwnerID      uniqueidentifier,
                    //    @objectID           uniqueidentifier    OUTPUT,
                    //    @aux                VARCHAR(1020)       OUTPUT
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("context", SqlDbType.VarChar).Value = "";
                    command.Parameters.Add("cprNo", SqlDbType.VarChar).Value = "";
                    command.Parameters.Add("objectOwnerID", SqlDbType.UniqueIdentifier).Value = "";
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var data = new DataTable();
                        adapter.Fill(data);

                        Assert.AreEqual(cprNumbers.Length, data.Rows.Count);
                    }
                }
            }
        }

    }
}
