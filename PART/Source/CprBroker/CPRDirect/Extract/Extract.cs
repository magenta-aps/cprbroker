using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    partial class Extract
    {
        public Extract(string batchFileText, Dictionary<string, Type> typeMap, Dictionary<string, bool> reverseRelationMap)
            : this()
        {
            var dataLines = new List<LineWrapper>(LineWrapper.ParseBatch(batchFileText));

            var startLine = dataLines.First();
            var endLine = dataLines.Last();

            dataLines.Remove(startLine);
            dataLines.Remove(endLine);

            this.ExtractId = Guid.NewGuid();
            this.ExtractDate = (startLine.ToWrapper(typeMap) as StartRecordType).ProductionDate.Value;
            this.ImportDate = DateTime.Now;
            this.StartRecord = startLine.Contents;
            this.EndRecord = endLine.Contents;

            this.ExtractItems.AddRange(
                dataLines
                .Select(line =>
                    {
                        var ret = line.ToExtractItem(typeMap, reverseRelationMap);
                        ret.ExtractId = this.ExtractId;
                        return ret;
                    }
                ));
        }

        public static IndividualResponseType GetPerson(string pnr, IQueryable<ExtractItem> extractItems, Dictionary<string, Type> typeMap)
        {
            var found = extractItems
                .Where(item => item.PNR == pnr)
                .GroupBy(item => item.Extract)
                .OrderByDescending(g => g.Key.ExtractDate)
                .FirstOrDefault();

            if (found != null)
            {
                var individualResponse = new IndividualResponseType();

                var linewWappers = found
                    .Select(item => new LineWrapper(item.Contents).ToWrapper(typeMap))
                    .ToArray();

                // TODO: Add reversible relationship support after finding a good indexing solution

                var startWrapper = new LineWrapper(found.Key.StartRecord).ToWrapper(typeMap);
                var endWrapper = new LineWrapper(found.Key.EndRecord).ToWrapper(typeMap);
                individualResponse.FillFrom(linewWappers, startWrapper, endWrapper);

                return individualResponse;
            }

            return null;
        }
    }
}