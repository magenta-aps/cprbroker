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
            var rd = new System.IO.StringReader(batchFileText);
            var lines = rd.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return ParseBatch(lines, Constants.DataObjectMap);
        }

        public static new IList<IndividualResponseType> ParseBatch(string[] dataLines, Dictionary<string, Type> typeMap)
        {
            var allLineWrappers = Wrapper.ParseBatch(dataLines, typeMap);

            var startRecord = allLineWrappers.Where(w => w is StartRecordType).FirstOrDefault() as StartRecordType;
            var endRecord = allLineWrappers.Where(w => w is EndRecordType).FirstOrDefault() as EndRecordType;

            var relevantLines = allLineWrappers
                .Where(lineWrapper => !new Wrapper[] { startRecord, endRecord }.Contains(lineWrapper));

            var groupedWrapers = relevantLines
                .GroupBy(w => new LineWrapper(w.Contents).PNR);

            var ret = groupedWrapers
                .Select(individualWrappersGrouping =>
                    {
                        var individualLines = individualWrappersGrouping.ToList();
                        var individual = new IndividualResponseType();
                        individual.FillFrom(individualLines, startRecord, endRecord);
                        return individual;
                    }
            );
            return ret.ToList();
        }

        

    }
}

