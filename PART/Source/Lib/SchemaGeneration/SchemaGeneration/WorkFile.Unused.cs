using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Net;

namespace SchemaGeneration
{
    public partial class WorkFile
    {
        public string[] IncludedUrls { get; private set; }

        private string[] GetIncludedUrls(XmlDocument doc)
        {
            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var nodes = doc.SelectNodes("//xsd:import[@schemaLocation]", nsMgr).OfType<XmlElement>()
                .Union(doc.SelectNodes("//xsd:include[@schemaLocation]", nsMgr).OfType<XmlElement>())
                .ToArray();
            var locations = nodes.Select(nd => nd.Attributes["schemaLocation"].Value).ToArray();
            return locations;
        }

        [Obsolete]
        public void DownloadIncludes(string originalLocationDir)
        {
            var cl = new WebClient();
            foreach (var inc in IncludedUrls)
            {
                string localName = "";
                string includeFilePath = null;

                Action copier;

                if (IsUrl(inc))
                {
                    // Web file
                    var arr = inc.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    localName = arr.Last();

                    copier = () =>
                    {
                        var fileBytes = cl.DownloadData(inc);
                        File.WriteAllBytes(includeFilePath, fileBytes);
                    };
                }
                else
                {
                    // local file
                    var path = inc;
                    localName = path.Split('\\').Last();

                    copier = () =>
                    {
                        File.Copy(originalLocationDir + inc, includeFilePath);
                    };
                }
                includeFilePath = new FileInfo(this.XsdFullPath).Directory + localName;
                copier();
                var incFile = new WorkFile(includeFilePath);
                //incFile.DownloadIncludes();
            }
        }

        public bool IsUrl(string location)
        {
            return location.Contains('/');
        }
    }



}
