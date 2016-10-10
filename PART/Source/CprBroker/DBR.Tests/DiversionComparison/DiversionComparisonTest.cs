using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Tests.DBR.Properties;
using CprBroker.Utilities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.DiversionComparison
{
    [TestFixture]
    public class DiversionComparisonTest
    {
        static DiversionComparisonTest()
        {
            if (!Directory.Exists(Settings.Default.DprDiversionCacheDirectory))
                Directory.CreateDirectory(Settings.Default.DprDiversionCacheDirectory);

            PartInterface.Utilities.UpdateConnectionString(Settings.Default.CprBrokerConnectionString);
        }

        [TestFixtureSetUp]
        public void InitBrokerContext()
        {
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
        }

        public string GetRealResponse(string request)
        {
            var fileName = string.Format(@"{0}Response.{1}.txt",
                Settings.Default.DprDiversionCacheDirectory,
                request);
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                Console.WriteLine("{0}:\"{1}\"", nameof(request), request);

                var inputData = Providers.DPR.Constants.DiversionEncoding.GetBytes(request);

                using (TcpClient client = new TcpClient(Settings.Default.RealDprDiversionAddress, Settings.Default.RealDprDiversionPort))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(inputData, 0, inputData.Length);
                        stream.ReadTimeout = 1000;

                        var retData = new Byte[3500];
                        var retBytes = stream.Read(retData, 0, retData.Length);
                        var retText = Providers.DPR.Constants.DiversionEncoding.GetString(retData.Take(retBytes).ToArray());
                        Console.WriteLine("Response:");
                        Console.WriteLine(retText);

                        File.WriteAllText(fileName, retText);
                        return retText;
                    }
                }
            }
        }

        public DiversionRequest ParseRequest(string request)
        {
            var ret = DiversionRequest.Parse(request);

            if (ret is ErrorRequestType)
            {
                //ret = ret;
            }
            else if (ret is ClassicRequestType)
            {
                ret = new ClassicRequestTypeStub(ret.Contents);
            }
            else if (ret is NewRequestType)
            {
                ret = new NewRequestTypeStub(ret.Contents);
            }
            else
            {
                throw new Exception("Unknown request type created");
            }

            return ret;
        }

        public string GetEmulatedResponse(string request, string dprConnectionString)
        {
            var req = ParseRequest(request);
            var resp = req.Process(dprConnectionString);
            return resp.ToString();
        }

        public void Compare(string request, Func<string, string> preprocessor = null)
        {
            if (preprocessor == null)
            {
                preprocessor = (s) => s;
            }


            var realResponse = preprocessor(GetRealResponse(request).Trim());
            var emulatedResponse = preprocessor(GetEmulatedResponse(request, Settings.Default.ImitatedDprConnectionString).Trim());
            Assert.AreEqual(realResponse, emulatedResponse);
        }

        #region CPR Numbers
        public string[] GetCprNumbers(int count)
        {
            var minPnr = decimal.Parse("0103000000");
            using (var context = new DPRDataContext(Settings.Default.RealDprConnectionString))
            {
                return context.ExecuteQuery<decimal>(""
                    + "SELECT PNR FROM DTTOTAL "
                    + "WHERE PNR > {0} "
                    + "AND (INDLAESDTO IS NULL OR INDLAESDTO < {1}) "
                    + "AND (HENTTYP IS NULL OR HENTTYP IN ({2},{3})) "
                    + "AND (INDLAESPGM IS NULL OR INDLAESPGM = {4}) "
                    + "ORDER BY PNR"
                    ,
                    minPnr,
                    new DateTime(2016, 10, 1),
                    DataRetrievalTypes.Extract, DataRetrievalTypes.Extract2,
                    UpdatingProgram.DprUpdate
                    )
                    .Take(count)
                    .ToArray()
                    .Select(p => p.ToPnrDecimalString())
                    .ToArray();
            }
        }

        public string[] CprNumbers2 { get { return GetCprNumbers(2); } }
        public string[] CprNumbers5 { get { return GetCprNumbers(5); } }
        public string[] CprNumbers10 { get { return GetCprNumbers(10); } }
        public string[] CprNumbers100 { get { return GetCprNumbers(100); } }
        public string[] CprNumbers400 { get { return GetCprNumbers(400); } }
        #endregion

        [Test]
        public void InvalidRequest_Old12Char(
            [Values("44", "46", "")]string start,
            [ValueSource(nameof(CprNumbers2))]string pnr)
        {
            var request = start + pnr;
            Compare(request);
        }

        [Test]
        public void NonNumericRequest_Old12Char(
            [Values("aa", "bb")]string start,
            [ValueSource(nameof(CprNumbers2))]string pnr)
        {
            var request = start + pnr;
            Compare(request);
        }

        [Test]
        public void RealRequest_Old12Char(
            [Values("1")]string type,
            [Values("0")]string largeData,
            [ValueSource(nameof(CprNumbers5))]string pnr)
        {
            var request = type + largeData + pnr;
            Compare(request);
        }

        public void CompareNewRequest(
            char type,
            char largeData,
            string pnr,
            char responseData,
            Func<string, string> preprocessor = null)
        {
            var request = ""
                + type
                + largeData
                + pnr
                + "MMXIII"
                + "0" // Do not force diversion
                + responseData
                + "" // no user
                ;
            Compare(request, preprocessor);
        }

        [Test]
        public void RealRequest_New40Char_Ingen(
            [Values('1')]char type,
            [Values('0')]char largeData,
            [ValueSource(nameof(CprNumbers5))]string pnr)
        {
            CompareNewRequest(type, largeData, pnr, 'I');
        }

        [Test]
        public void RealRequest_New40Char_Stam(
            [Values('1')]char type,
            [Values('0')]char largeData,
            [ValueSource(nameof(CprNumbers10))]string pnr)
        {
            CompareNewRequest(type, largeData, pnr, 'S');
        }

        [Test]
        public void RealRequest_New40Char_Udvidet(
            [Values('1')]char type,
            [Values('0')]char largeData,
            [ValueSource(nameof(CprNumbers10))]string pnr)
        {
            CompareNewRequest(type, largeData, pnr, 'U', (s) =>
            {
                var values = s.Substring(7).Split(';');

                var valuesAndProps = new NewResponseFullDataType()
                    .PropertyDefinitions
                    .Zip(values, (p, v) => new { Prop = p, Value = v });

                var status = valuesAndProps.Single(p => p.Prop.Item1 == "STATUS").Value;

                var newValues = valuesAndProps
                    .Select(p =>
                    {
                        var name = p.Prop.Item1.ToUpper();
                        var value = p.Value;

                        value = value.TrimStart('0');

                        if (
                            name.Contains("AJF") ||
                            name.Contains("MYNKOD") ||
                            value.Equals("0") || //STATUSHAENSTART is null in the database but emulated data has 0 as value.
                            name.Contains("ADRNVN") ||
                            name.Contains("INDRAP") ||
                            name.Contains("FOEDMYNHAENSTART") ||
                            name.Contains("KUNDENR") ||
                            name.Contains("FARSKABHAENSTART") ||
                            name.Contains("AEGTEMRK") ||
                            name.Contains("FARSKABMYNNVN") ||
                            name.Contains("TIDLKOMNVN") ||
                            name.Contains("CIVMYN") ||
                            name.Contains("STILLINGDTO") ||                            
                            name.Contains("dummy 1293810")
                            )
                        {
                            value = "";
                        }

                        if (name.Contains("START") && value.Length >= 12 && value.EndsWith("99"))
                        {
                            value = value.Substring(0, value.Length - 2) + "00";
                        }

                        if (status == "90")
                        {
                            var excluded90 = new string[] {
                                "POSTDISTTXT",
                                "POSTNR",
                                "BYNVN",
                                "STANDARDADR",
                                "KOMKOD",
                                "KOMNVN",
                                "VEJKOD",
                                "KOMKOD",
                                "VEJADRNVN",
                                "HUSNR",
                                "ETAGE",
                                "SIDEDOER",
                                "TILFLYDTO",
                                "TILFLYDTOMRK",
                                "TILFLYKOMDTO"

                            };
                            if (excluded90.Contains(name))
                            {
                                value = "";
                            }
                        }



                        return string.Format("{0}={1}",
                            p.Prop.Item1,
                            value);
                    }
                    ).ToArray();
                ;

                return s.Substring(0, 7) + string.Join("u;", newValues);
            });
        }
        
    }
}
