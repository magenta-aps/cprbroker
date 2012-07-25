using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public class LineWrapper
    {
        public string Contents { get; set; }

        public LineWrapper(string contents)
        {
            Contents = contents;
        }

        public string Code
        {
            get { return Contents.Substring(0, 3); }
        }

        public int IntCode
        {
            get { return int.Parse(Code); }
        }

        public string PNR
        {
            get { return Contents.Substring(3, 10); }
        }

        public bool IsValid
        {
            get
            {
                return Contents.Trim().Length > 0
                    && Contents.Length >= Constants.DataObjectCodeLength;
            }
        }

        public Wrapper ToWrapper(Dictionary<string, Type> typeMap)
        {
            if (typeMap.ContainsKey(this.Code))
            {
                Type type = typeMap[this.Code];
                var wrapper = Utilities.Reflection.CreateInstance(type) as Wrapper;
                this.Contents = this.Contents.PadRight(wrapper.Length);
                wrapper.Contents = this.Contents;
                return wrapper;
            }
            return null;
        }

        public ExtractItem ToExtractItem()
        {
            return new ExtractItem()
            {
                CprNumber = this.PNR,
                Contents = this.Contents,
                DataTypeCode = this.Code
            };
        }

        public static LineWrapper[] ParseBatch(string batchFileText)
        {
            var rd = new System.IO.StringReader(batchFileText);
            var dataLines = rd
                .ReadToEnd()
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => new LineWrapper(l))
                .Where(w => w.IsValid)
                .ToArray();
            return dataLines;
        }
    }
}
