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

        public static string[] SerialCprNumbers(int count)
        {
            var cprNumbers = new List<string>();

            for (int iDay = 1; iDay <= 28; iDay++)
            {
                for (int iMonth = 1; iMonth <= 12; iMonth++)
                {
                    for (int iYear = 1; iYear <= 99; iYear++)
                    {
                        for (int iPart = 1; iPart <= 9999; iPart++)
                        {
                            var day = iDay.ToString("00");
                            var month = iMonth.ToString("00");
                            var year = iYear.ToString("00");
                            var part1 = iPart.ToString("0000");
                            cprNumbers.Add(day + month + year + part1);
                            if (cprNumbers.Count == count)
                            {
                                return cprNumbers.ToArray();
                            }
                        }
                    }
                }
            }
            return cprNumbers.ToArray();
        }

        private const string PersonMasterConnectionString = "";
        public int[] CprCounts = new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 /*, 1024, 2048, 4096, 8192, 16384, 32768, 65536*/ };

        [Test]
        public void TestGetUuidArrayOfRandomCpr(
            [ValueSource("CprCounts")] int count)
        {
            var cprNumbers = RandomCprNumbers(count);
            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;

            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);
            ValidateOutput(cprNumbers, ret);
        }

        [Test]
        public void TestGetUuidArrayOfCprSequence(
            [ValueSource("CprCounts")] int count)
        {
            var cprNumbers = SerialCprNumbers(count);
            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;

            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);
            ValidateOutput(cprNumbers, ret);
        }

        public void ValidateOutput(string[] cprNumbers, Guid[] objectIds)
        {
            Assert.IsNotNull(objectIds, "Return value is null");
            Assert.AreEqual(cprNumbers.Length, objectIds.Length, "Return value length does not equal input length");
            foreach (var uuid in objectIds)
            {
                Assert.AreNotEqual(Guid.Empty, uuid, "UUID is empty");
                Assert.AreEqual(1, objectIds.Where((id) => id == uuid).Count(), "Repeated UUID : " + uuid.ToString());
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
