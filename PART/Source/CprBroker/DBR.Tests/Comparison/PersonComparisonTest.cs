using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using System.Reflection;
using CprBroker.DBR;
using CprBroker.Utilities;
using System.IO;

namespace CprBroker.Tests.DBR.Comparison.Person
{
    [Category("PersonComparison")]
    public abstract class PersonComparisonTest<TObject> : ComparisonTest<TObject, DPRDataContext>
        where TObject : new()
    {
        public override string[] LoadKeys()
        {
            if (KeysHolder._Keys == null)
            {
                if (CprBroker.Tests.PartInterface.Utilities.IsConsole)
                {
                    Console.WriteLine("Loading PNRS");
                    using (var dataContext = new DPRDataContext(Properties.Settings.Default.RealDprConnectionString))
                    {
                        KeysHolder._Keys = dataContext
                            .PersonTotals
                            .Select(p => p.PNR)
                            .ToArray()
                            .Select(p => p.ToPnrDecimalString())
                            .ToArray();
                        Console.WriteLine("Found PNRs <{0}>", KeysHolder._Keys.Count());
                    }
                }
                else
                {
                    var r = new Random();
                    if (!Directory.Exists("c:\\logs\\"))
                        Directory.CreateDirectory("c:\\logs\\");
                    using (var w = new System.IO.StreamWriter(string.Format("c:\\logs\\Compare-{0}-{1}-{2}.log", GetType().Name, DateTime.Now.ToString("yyyyMMdd HHmmss"), r.Next())))
                    {
                        w.AutoFlush = true;

                        var excludedPnrs0 = File.Exists("ExcludedPNR.txt") ?
                            System.IO.File.ReadAllLines("ExcludedPNR.txt")
                            .Select(l => l.Trim())
                            .Where(l => l.Length > 0 && !l.StartsWith("#"))
                            .Select(l => decimal.Parse(l))
                            .ToArray()
                            :
                            new decimal[] { };

                        var excludedPnrs1 = new decimal[] { };
                        var excludedPnrs2 = new decimal[] { };

                        using (var dataContext = new DPRDataContext(Properties.Settings.Default.RealDprConnectionString))
                        {
                            dataContext.Log = w;
                            excludedPnrs1 = dataContext.PersonAddresses
                                .Where(pa => pa.AddressStartDateMarker == 'U')
                                .Select(pa => pa.PNR)
                                .ToArray();
                            excludedPnrs2 = dataContext.ExecuteQuery<decimal>("SELECT PNR FROM DTTOTAL WHERE INDLAESDTO IS NULL")
                                .ToArray();
                        }

                        var excludedPnrs = excludedPnrs0.Union(excludedPnrs1).Union(excludedPnrs2).Distinct().ToArray();

                        try
                        {
                            using (var dataContext = new System.Data.Linq.DataContext(Properties.Settings.Default.ImitatedDprConnectionString))
                            {
                                dataContext.Log = w;
                                KeysHolder._Keys = dataContext.ExecuteQuery<decimal>("select PNR FROM DTTOTAL ORDER BY PNR")
                                    .Where(pnr => !excludedPnrs.Contains(pnr))
                                    .Skip(Properties.Settings.Default.PersonComparisonSampleSkip)
                                    .Take(Properties.Settings.Default.PersonComparisonSampleSize)
                                    .ToArray()
                                    .Select(pnr => pnr.ToPnrDecimalString())
                                    .ToArray();
                                w.WriteLine("Loaded <{0}> PNRs", KeysHolder._Keys.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            w.WriteLine(ex.ToString());
                            throw;
                        }
                    }
                }
            }
            return KeysHolder._Keys;
        }

        public override IQueryable<TObject> Get(DPRDataContext dataContext, string key)
        {
            var tableName = Utilities.DataLinq.GetTableName<TObject>();

            var cacheKey = string.Format("{0}.{1}", tableName, key);
            return DatabaseLoadCache.Root.GetOrLoad<DPRDataContext, TObject>(dataContext,
                cacheKey,
                dc =>
                {
                    var orderBy = string.Join(", ", GetOrderByColumnNames());
                    var whereAnnKorr = "";
                    if (!string.IsNullOrEmpty(GetCorrectionMarkerColumnName()))
                        whereAnnKorr = string.Format(" AND ({0} IS NULL OR {0} = ' ')", GetCorrectionMarkerColumnName());

                    return dataContext.Fill<TObject>(
                        string.Format(
                            "select * from {0} WHERE PNR={1} {2} ORDER BY {3}",
                            tableName,
                            key,
                            whereAnnKorr,
                            orderBy)
                        ).AsQueryable();
                });
        }

        public override DPRDataContext CreateDataContext(string connectionString)
        {
            return new DPRDataContext(connectionString);
        }
    }

}
