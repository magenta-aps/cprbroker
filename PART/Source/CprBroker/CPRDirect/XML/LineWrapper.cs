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
    }
}
