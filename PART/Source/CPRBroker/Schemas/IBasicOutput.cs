using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public interface IBasicOutput
    {
        StandardReturType StandardRetur { get; set; }
    }

    public class BasicOutputType<T> : BasicOutputType
    {
        public T Item { get; set; }
        public static BasicOutputType<T> CreateAsOK(T item)
        {
            return new BasicOutputType<T>()
            {
                Item = item,
                StandardRetur = StandardReturType.OK()
            };
        }

        public static BasicOutputType<T> CreateAsOKFromFirstResult(object[] result)
        {
            return CreateAsOK((T)result[0]);
        }
    }

    public partial class SoegOutputType : IBasicOutput
    { }


    public partial class BasicOutputType : IBasicOutput
    { }

    public partial class ListOutputType : IBasicOutput
    { }
}
