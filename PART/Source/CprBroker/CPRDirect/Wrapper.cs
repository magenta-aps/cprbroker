using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public abstract class Wrapper
    {
        private string _Contents;
        public string Contents
        {
            get { return _Contents; }
            set
            {
                int len = string.Format("{0}", value).Length;
                if (len != Length)
                {
                    throw new ArgumentOutOfRangeException(
                        "Contents",
                        value,
                        string.Format("Should be exactly {0} characters", Length)
                        );
                }
                _Contents = value;
            }
        }

        public abstract int Length { get; }

    }
}
