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
using System.Configuration;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.PartInterface
{
    public static class Utilities
    {
        public static readonly Random Random = new Random();
        public static string RandomCprNumber()
        {
            var day = Random.Next(1, 29).ToString("00");
            var month = Random.Next(1, 13).ToString("00");
            var year = Random.Next(1, 100).ToString("00");
            var part1 = Random.Next(1000, 9980).ToString();
            var ret = day + month + year + part1;
            while (!CprBroker.PartInterface.Strings.IsModulus11OK(ret))
            {
                part1 = (int.Parse(part1) + 1).ToString();
                ret = day + month + year + part1;
            }
            return ret;
        }

        public static string[] RandomCprNumbers2()
        {
            return RandomCprNumbers(2);
        }

        public static string[] RandomCprNumbers5()
        {
            return RandomCprNumbers(5);
        }

        public static string[] RandomCprNumbers10()
        {
            return RandomCprNumbers(10);
        }

        public static string[] RandomCprNumbers(int count)
        {
            var ret = new string[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = RandomCprNumber();
            }
            return ret;
        }

        public static RegistreringType1 CreateFakePerson()
        {
            return CreateFakePerson(false);
        }

        public static RegistreringType1 CreateFakePerson(bool addSourceObject)
        {
            var ret = new RegistreringType1()
            {
                AttributListe = new AttributListeType()
                {
                    Egenskab = new EgenskabType[] { },
                    LokalUdvidelse = null,
                    RegisterOplysning = new RegisterOplysningType[] { },
                    SundhedOplysning = null,
                },
                RelationListe = new RelationListeType(),
                TilstandListe = new TilstandListeType(),
                AktoerRef = UnikIdType.Create(Guid.NewGuid()),
                LivscyklusKode = LivscyklusKodeType.Rettet,
                CommentText = "",
                Virkning = new VirkningType[] { },
                SourceObjectsXml = null,
                Tidspunkt = TidspunktType.Create(DateTime.Today)
            };
            if (addSourceObject)
            {
                var o = new object[] { Guid.NewGuid(), Guid.NewGuid() };
                ret.SourceObjectsXml = CprBroker.Utilities.Strings.SerializeObject(o);
            }
            return ret;
        }

        public static Guid[] RandomGuids(int count)
        {
            var ret = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = Guid.NewGuid();
            }
            return ret;
        }

        public static string[] RandomGuidStrings(int count)
        {
            return Utilities.RandomGuids(count).Select(id => id.ToString()).ToArray();
        }

        public static string[] RandomGuids5
        {
            get
            {
                return Utilities.RandomGuidStrings(5);
            }
        }

        public static string AppToken
        {
            get
            {
                return CprBroker.Utilities.Constants.BaseApplicationToken.ToString();
            }
        }


        private static bool _UpdateConnectionString = false;

        public static void UpdateConnectionString(string brokerConnectionString)
        {
            if (_UpdateConnectionString)
                return;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
            Console.WriteLine("Setting connection string to : {0}", brokerConnectionString);
            if (section == null)
            {
                section = new ConnectionStringsSection();
                config.Sections.Add("connectionStrings", section);
                config.Save();
            }
            var connStr = section.ConnectionStrings["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"];
            if (connStr == null)
            {
                connStr = new ConnectionStringSettings("CprBroker.Config.Properties.Settings.CprBrokerConnectionString", brokerConnectionString);
                section.ConnectionStrings.Add(connStr);
            }
            else
            {
                connStr.ConnectionString = brokerConnectionString;
            }
            config.Save();
            Console.WriteLine("Setting connection saved");

            _UpdateConnectionString = true;
        }

        private static bool? _IsConsole = null;
        public static bool IsConsole
        {
            get
            {
                if (!_IsConsole.HasValue)
                {
                    _IsConsole = false;
                    try
                    {
                        var d = Console.WindowHeight;
                        _IsConsole = true;
                    }
                    catch
                    {

                    }
                }
                return _IsConsole.Value;
            }
        }
    }
}
