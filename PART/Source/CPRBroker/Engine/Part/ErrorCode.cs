using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class ErrorCode
    {
        public string Code;
        public string Text;

        public virtual StandardReturType ToStandardReturn()
        {
            return StandardReturType.Create(Code, Text);
        }

        public static StandardReturType Create<T>(T[] errorCodes) where T : ErrorCode
        {
            string[] code = (from e in errorCodes select e.ToStandardReturn().StatuskodeKode).ToArray();
            string[] text = (from e in errorCodes select e.ToStandardReturn().FejlbeskedTekst).ToArray();

            return StandardReturType.Create(
                string.Join(Environment.NewLine, code),
                string.Join(Environment.NewLine, text)
            );
        }

        public class NullInputErrorCode : ErrorCode
        {
            public override StandardReturType ToStandardReturn()
            {
                return StandardReturType.Create("1", "Input cannot be null");
            }
        }


        public class InvalidUuidErrorCode : ErrorCode
        {
            public string Value;

            public InvalidUuidErrorCode(string value)
            {
                Value = value;
            }

            public override StandardReturType ToStandardReturn()
            {
                return StandardReturType.Create("2", string.Format("{0} : UUID: {1}", TextMessages.InvalidInput, Value));
            }
        }


    }
}
