using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CprBroker.Providers.CprServices
{
    public class SearchMethod
    {
        public SearchMethod()
        {

        }

        public SearchMethod(string xmlTemplate)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlTemplate);

            Name = doc.SelectSingleNode("//Service").Attributes["r"].Value;

            var fieldNodes = doc.SelectNodes("//Field");
            InputFields.AddRange(
                fieldNodes.OfType<XmlNode>()
                .Select(n => new InputField(n))
                );
        }

        public bool CanBeUsedFor(SearchRequest request)
        {
            var empty = default(KeyValuePair<string, string>);
            // Filter out any empty inputs
            var filledInputs = request.CriteriaFields.Where(cf => !string.IsNullOrEmpty(cf.Value));

            // Filter and group required fields for this method
            var requiredGroups = InputFields
                .Where(f => f.Required && f.EnumValues.Count == 0)
                .GroupBy(f => f.RequiredGroup);

            // Loop over groups, fail if any group cannot be satisfied
            foreach (var gr in requiredGroups)
            {
                if (string.IsNullOrEmpty(gr.Key)) // No group key, ALL fields are required
                {
                    foreach (var f in gr.ToArray())
                    {
                        var fieldInput = filledInputs.FirstOrDefault(cf => cf.Key == f.Name);
                        if (empty.Equals(fieldInput))
                            return false;
                    }
                }
                else // Group key exists, ANY field is required
                {
                    var groupFieldNames = gr.ToArray().Select(f => f.Name).ToArray();
                    var groupInput = request.CriteriaFields.FirstOrDefault(f => groupFieldNames.Contains(f.Key));
                    if (empty.Equals(groupInput))
                        return false;
                }
            }
            return true;
        }

        public System.Collections.Generic.List<InputField> InputFields = new List<InputField>();
        public string Name;
    }
}
