using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CprServices
{
    public class InputField
    {
        public string Name;
        public bool Required = false;
        public string RequiredGroup;
        public List<string> EnumValues = new List<string>();

        public InputField()
        {
        }

        public InputField(System.Xml.XmlNode n)
        {
            this.Name = n.Attributes["r"].Value;

            if (n.Attributes["required"] != null)
                this.Required = Convert.ToBoolean(n.Attributes["required"].Value);

            if (n.Attributes["requiredGroup"] != null)
                this.RequiredGroup = n.Attributes["requiredGroup"].Value;

            if (n.Attributes["enumValues"] != null)
                this.EnumValues.AddRange(n.Attributes["enumValues"].Value.Split(','));
        }

    }
}
