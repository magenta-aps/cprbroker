using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Installers;

namespace CprBroker.Tests.Installers
{
    [TestFixture]
    public class WebsiteCustomActionHelpersTests
    {
        string[] ValidUrlPatterns = new string[] { "http://PersonMaster:80" };
        string[] RandomUrls
        {
            get
            {
                List<string> ret = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    ret.Add(string.Format("http://ss{0}/asde", Guid.NewGuid().ToString().Substring(0, 10)));
                }
                return ret.ToArray();
            }
        }

        [Test]
        public void AddUrlToLocalIntranet_Valid_Passes(
            [ValueSource("RandomUrls")] string url)
        {
            WebsiteCustomAction.AddUrlToLocalIntranet(url);
        }


    }
}
