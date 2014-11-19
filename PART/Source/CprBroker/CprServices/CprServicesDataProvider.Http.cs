using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider
    {
        public Kvit Send(string op, string input, ref string token, out string ret)
        {
            if (string.IsNullOrEmpty(token))
                token = Constants.DefaultToken;

            //var baseUrl = "";
            //baseUrl = "https://gctp-demo.cpr.dk/cpr-online-gctp/gctp";
            //var logOnPath = "/cics/dmwg/cscwbsgn";
            //var cprApplicationPath = "/cpcacpra/ajou/xyz";

            using (var callContext = this.BeginCall(op, op))
            {
                // Prepare the request object
                HttpWebRequest req = WebRequest.Create(this.Address) as HttpWebRequest;
                req.Method = WebRequestMethods.Http.Post;
                req.UserAgent = Constants.UserAgent;
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(new Uri(this.Address), new Cookie(Constants.TokenCookieName, token));
                req.ContentLength = input.Length;

                // Fill the input and get response
                using (var s = req.GetRequestStream())
                {
                    // Fill the posted data
                    StreamWriter w = new StreamWriter(s, Constants.XmlEncoding);
                    w.Write(input);
                    w.Flush();

                    // Get the response
                    var resp = req.GetResponse();
                    using (var rd = new StreamReader(resp.GetResponseStream(), Constants.XmlEncoding))
                    {
                        ret = rd.ReadToEnd();

                        // Update token if possible
                        var t = Utils.GetToken(resp.Headers);
                        if (!string.IsNullOrEmpty(t))
                            token = t;

                        // Final return
                        var kvit = Kvit.FromResponseXml(ret);
                        if (kvit.OK)
                        {
                            callContext.Succeed();
                        }
                        else
                        {
                            callContext.Fail();
                        }
                        return kvit;
                    }
                }
            }
        }

        public string SignonAndGetToken()
        {
            var inp = Properties.Resources.OrdinaryLogon;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("cpr", Constants.XmlNamespace);
            var node = doc.SelectSingleNode("//cpr:Sik", mgr);
            node.Attributes["userid"].Value = this.UserId;
            node.Attributes["password"].Value = this.Password;

            string token = null;
            string ret;
            var kvit = Send(Constants.OperationKeys.signon, doc.OuterXml, ref token, out ret);
            if (kvit.OK)
            {
                return token;
            }
            return null;
        }

        public Kvit ChangePassword(string newPassword)
        {
            var inp = Properties.Resources.ChangePassword;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("cpr", Constants.XmlNamespace);
            var node = doc.SelectSingleNode("//cpr:Sik", mgr);
            node.Attributes["userid"].Value = this.UserId;
            node.Attributes["password"].Value = this.Password;
            node.Attributes["newpass1"].Value = newPassword;


            string token = null;
            var ret = "";
            return Send(Constants.OperationKeys.newpass, doc.OuterXml, ref token, out ret);
        }


    }
}
