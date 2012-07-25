using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    partial class Extract
    {
        public Extract(string batchFileText, Dictionary<string, Type> typeMap)
            : this()
        {
            var rd = new System.IO.StringReader(batchFileText);
            var dataLines = rd
                .ReadToEnd()
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => new LineWrapper(l))
                .ToList();

            var startLine = dataLines.First();
            dataLines.RemoveAt(0);

            var endLine = dataLines.Last();
            dataLines.RemoveAt(dataLines.Count - 1);

            this.ExtractDate = (startLine.ToWrapper(typeMap) as StartRecordType).ProductionDate.Value;
            this.StartRecord = startLine.Contents;
            this.EndRecord = endLine.Contents;

            this.ExtractItems.AddRange(
                dataLines
                .Select(line => line.ToExtractItem())
                );
        }
    }
}