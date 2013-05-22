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

    public partial class EgenskabType : ITypeWithVirkning
    {
    }

    public partial class RegisterOplysningType : ITypeWithVirkning
    {
    }

    public partial class SundhedOplysningType : ITypeWithVirkning
    {
    }


}
