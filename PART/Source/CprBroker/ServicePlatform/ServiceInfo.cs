/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.ServicePlatform
{
    public class ServiceInfo
    {
        public string Name { get; private set; }
        public string UUID { get; private set; }
        public string Path { get; private set; }

        private ServiceInfo()
        { }

        public CprServices.SearchMethod ToSearchMethod()
        {
            return new CprServices.SearchMethod(Properties.Resources.PnrLookup) { Name = Name };
        }

        public static readonly ServiceInfo CPRSubscription = new ServiceInfo()
        {
            Name = "CPR Subscription",
            UUID = "9cdccc2f-3243-11e2-8fef-d4bed98c5934",
            Path = "/service/CPRSubscription/CPRSubscription/1/"
        };

        public static readonly ServiceInfo ADRSOG1 = new ServiceInfo() 
        {
            Name = "ADRSOG1",
            UUID = "6538f113-b1e6-4b21-8fe1-28b7bf78a1bd",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo ForwardToCprService = new ServiceInfo()
        {
            Name = "CPR Service",
            UUID = "8ee213c7-25a9-11e2-8770-d4bed98c63db",
            Path = "/service/CPRService/CPRService/1"
        };

        public static readonly ServiceInfo StamPlus_Local = new ServiceInfo()
        {
            Name = "Stam+",
            UUID = "7baec3a5-26a5-4922-80fd-37f3bad3dfcb",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo NAVNE3_Local = new ServiceInfo()
        {
            Name = "navne3",
            UUID = "e3f26293-803e-4b8f-8a15-674a4f8abaad",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo FamilyPlus_Local = new ServiceInfo()
        {
            Name = "Familie+",
            UUID = "a34cf599-26e4-4609-bfbd-2490d3d1c3ab",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo Adresse4_Local = new ServiceInfo()
        {
            Name = "Adresse4",
            UUID = "b7e5ee9c-1acd-41e6-b117-08a11dee6862",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

    }
}
