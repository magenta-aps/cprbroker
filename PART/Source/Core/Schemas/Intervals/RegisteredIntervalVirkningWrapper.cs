using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public interface ITypeWithVirkning
    {
        VirkningType Virkning { get; set; }
    }

    public class RegisteredIntervalVirkningWrapper<T> : IRegisteredInterval
        where T : ITypeWithVirkning
    {
        public T Item { get; set; }

        public DateTime? StartTS
        {
            get
            {
                return this.Item.Virkning.FraTidspunkt.ToDateTime();
            }
            set
            {
                this.Item.Virkning.FraTidspunkt = TidspunktType.Create(value);
            }
        }

        public DateTime? EndTS
        {
            get
            {
                return this.Item.Virkning.TilTidspunkt.ToDateTime();
            }
            set
            {
                this.Item.Virkning.TilTidspunkt = TidspunktType.Create(value);
            }
        }

        public DateTime? RegistrationDate { get; set; }
    }
}
