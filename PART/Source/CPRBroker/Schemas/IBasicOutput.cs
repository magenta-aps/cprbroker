using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public interface IBasicOutput<T>
    {
        StandardReturType StandardRetur { get; set; }
        void SetMainItem(T mainItem);
    }

    public class BasicOutputType<T> : BasicOutputType, IBasicOutput<T>
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

        public void SetMainItem(T mainItem)
        {
            Item = mainItem;
        }
    }

    public partial class SoegOutputType : IBasicOutput<string[]>
    {
        public void SetMainItem(string[] mainItem)
        {
            Idliste = mainItem;
        }
    }

    public partial class ListOutputType1 : IBasicOutput<LaesResultatType[]>
    {
        public void SetMainItem(LaesResultatType[] mainItem)
        {
            LaesResultat = mainItem;
        }        
    }

    public partial class GetUuidOutputType : IBasicOutput<Guid>
    {
        public void SetMainItem(Guid mainItem)
        {
            UUID= mainItem.ToString();
        }        
    }

    public partial class LaesOutputType : IBasicOutput<LaesResultatType>
    {
        public void SetMainItem(LaesResultatType mainItem)
        {
            LaesResultat = mainItem;
        }        
    }
}
