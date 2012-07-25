using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public static IList<IndividualResponseType> ParseBatch(string batchFileText)
        {
            var lines = LineWrapper.ParseBatch(batchFileText);
            return ParseBatch(lines, Constants.DataObjectMap);
        }

        public static IList<IndividualResponseType> ParseBatch(LineWrapper[] dataLines, Dictionary<string, Type> typeMap)
        {
            var allWrappers = dataLines.Select(lw => lw.ToWrapper(typeMap)).ToList();

            var startRecord = allWrappers.First() as StartRecordType;
            var endRecord = allWrappers.Last() as EndRecordType;

            allWrappers.Remove(startRecord);
            allWrappers.Remove(endRecord);

            var groupedWrapers = allWrappers
                .Where(w => w != null)
                .GroupBy(w => new LineWrapper(w.Contents).PNR)
                .ToList();

            var ret = groupedWrapers
                .Select(individualWrappersGrouping =>
                    {
                        var individualLines = individualWrappersGrouping.ToList();
                        var individual = new IndividualResponseType();
                        individual.FillFrom(individualLines, startRecord, endRecord);
                        return individual;
                    })
                .ToList();
            return ret;
        }



    }
}

