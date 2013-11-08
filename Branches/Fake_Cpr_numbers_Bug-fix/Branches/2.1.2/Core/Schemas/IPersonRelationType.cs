using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public interface IPersonRelationType
    {
        string CprNumber { get; set; }
        UnikIdType ReferenceID { get; set; }
        string CommentText { get; set; }
        VirkningType Virkning { get; set; }
    }

    
}
