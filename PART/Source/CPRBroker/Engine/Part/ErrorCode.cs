using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class ErrorCode
    {
        public virtual StandardReturType ToStandardReturn()
        {
            return StandardReturType.Create("1000", "Unspecified error");
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
                return StandardReturType.Create("2", string.Format("Invalid UUID: {0}", Value));
            }
        }

        public class UnknownUuidErrorCode : ErrorCode
        {
            public string Value;

            public UnknownUuidErrorCode(string value)
            {
                Value = value;
            }

            public override StandardReturType ToStandardReturn()
            {
                return StandardReturType.Create("3", string.Format("UUID not found : {0}", TextMessages.InvalidInput, Value));
            }
        }


    }
}
