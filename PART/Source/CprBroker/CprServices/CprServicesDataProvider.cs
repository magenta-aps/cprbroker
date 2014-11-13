using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider : IPartSearchListDataProvider, IExternalDataProvider, IPerCallDataProvider, IPartReadDataProvider
    {
        public CprServicesDataProvider()
        {
            this.ConfigurationProperties = new Dictionary<string, string>();
        }

        public string[] OperationKeys
        {
            get
            {
                return new string[] { 
                    Constants.OperationKeys.signon,
                    Constants.OperationKeys.newpass,
                    Constants.OperationKeys.ADRSOG1,
                    Constants.OperationKeys.NVNSOG2,
                };
            }
        }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.Address, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.UserId, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.Password, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},                    
                };
            }
        }

        public string Address
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.Address]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.Address] = value; }
        }

        public string UserId
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.UserId]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.UserId] = value; }
        }

        public string Password
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.Password]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.Password] = value; }
        }

        public Version Version
        {
            get { return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor); }
        }

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public bool IsAlive()
        {
            var token = SignonAndGetToken();
            return !string.IsNullOrEmpty(token);
        }

        public LaesResultatType[] Search(SoegInputType1 input)
        {
            var request = new SearchRequest(input.SoegObjekt.SoegAttributListe);
            var availableMethods = new List<SearchMethod>();
            availableMethods.Add(new SearchMethod(Properties.Resources.ADRSOG1));
            availableMethods.Add(new SearchMethod(Properties.Resources.NVNSOG2));

            var plan = new SearchPlan(request, availableMethods.ToArray());

            List<SearchPerson> ret = null;

            if (plan.IsSatisfactory)
            {
                bool searchOk = true;
                // TODO: See if tokens could be saved an reused
                string token = this.SignonAndGetToken();

                foreach (var call in plan.PlannedCalls)
                {
                    if (ret != null && ret.Count == 0)// Discontinue search if a previous search returned zero results
                    {
                        searchOk = false;
                        break;
                    }
                    var xml = call.ToRequestXml(Properties.Resources.SearchTemplate);

                    var xmlOut = "";
                    var kvit = Send(xml, ref token, out xmlOut);
                    if (kvit.OK)
                    {
                        var persons = call.ParseResponse(xmlOut, true);
                        if (ret == null)
                            ret = persons;
                        else
                            ret = ret.Intersect(persons).ToList();
                    }
                    else
                    {
                        searchOk = false;
                    }
                }

                if (searchOk)
                {
                    // TODO: Can this break the result? is UUID assignment necessary?
                    var cache = new UuidCache();
                    var pnrs = ret.Select(p => p.PNR).ToArray();
                    cache.FillCache(pnrs);

                    return ret.Select(p => p.ToLaesResultatType(cache.GetUuid)).ToArray();
                }
                else
                {
                    // TODO: What to do if search fails??
                }
            }
            return null;
        }

        public RegistreringType1 Read(Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out Schemas.QualityLevel? ql)
        {
            var method = new SearchMethod(Properties.Resources.ADRESSE3);
            var request = new SearchRequest(uuid.CprNumber);
            var call = new SearchMethodCall(method, request);

            var xml = call.ToRequestXml(Properties.Resources.SearchTemplate);

            var xmlOut = "";
            string token = this.SignonAndGetToken();
            var kvit = Send(xml, ref token, out xmlOut);
            ql = Schemas.QualityLevel.Cpr;
            if (kvit.OK)
            {
                var persons = call.ParseResponse(xmlOut, false);
                var cache = new UuidCache();
                cache.FillCache(persons.Select(p => p.PNR).ToArray());
                return persons[0].ToRegistreringType1(cache.GetUuid);
            }
            return null;
        }
    }
}
