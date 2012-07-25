using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CprBroker.Providers.CPRDirect
{
    public static class ExtractManager
    {
        public static void ImportFile(string path)
        {
            var text = File.ReadAllText(path, Constants.DefaultEncoding);
            ImportText(text);
        }

        public static void ImportText(string text)
        {
            using (var dataContext = new ExtractDataContext())
            {
                var extract = new Extract(text, Constants.DataObjectMap);
                dataContext.Extracts.InsertOnSubmit(extract);
                dataContext.SubmitChanges();
            }
        }

        public static IndividualResponseType GetPerson(string pnr)
        {
            using (var dataContext = new ExtractDataContext())
            {
                return Extract.GetPerson(pnr, dataContext.ExtractItems, Constants.DataObjectMap);
            }

        }
    }
}
