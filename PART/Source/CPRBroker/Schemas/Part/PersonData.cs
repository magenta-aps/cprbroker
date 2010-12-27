using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    [System.Xml.Serialization.XmlInclude(typeof(CprData))]
    [System.Xml.Serialization.XmlInclude(typeof(ForeignCitizenData))]
    [System.Xml.Serialization.XmlInclude(typeof(UnknownCitizenData))]
    public abstract class PersonData
    {

    }
}
