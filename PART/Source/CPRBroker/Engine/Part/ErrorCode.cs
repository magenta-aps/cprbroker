using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part {

    public class ErrorCode
    {



        public virtual StandardReturType ToStandardReturn()
        {
            return StandardReturType.Create(HttpErrorCode.UNSPECIFIED);
        }

        public class NullInputErrorCode : ErrorCode
        {
            public override StandardReturType ToStandardReturn()
            {
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Input cannot be null");
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
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid UUID: {0}", Value));
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
                return StandardReturType.Create(HttpErrorCode.NOT_FOUND, string.Format("UUID valid but not found : {0}", TextMessages.InvalidInput, Value));
            }
        }


    }
}
