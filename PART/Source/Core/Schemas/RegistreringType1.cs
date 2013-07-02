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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using System.Reflection;

namespace CprBroker.Schemas.Part
{
    public partial class RegistreringType1
    {
        [System.Xml.Serialization.XmlIgnore]
        public string SourceObjectsXml { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public DateTime BrokerUpdateDate { get; set; }

        public void CalculateVirkning()
        {
            this.Virkning = null;
            var partialVirkning = GetPropertyValuesOfType<VirkningType>(this);
            var partialVirkning2 = GetPropertyValuesOfType<TilstandVirkningType>(this)
                .Select(tv => tv.ToVirkningType());
            var allVirknings = partialVirkning.Concat(partialVirkning2)
                .Where(v => !VirkningType.IsDoubleOpen(v));
            // TODO: Check this
            this.Virkning = allVirknings.ToArray();
        }

        public static T[] GetPropertyValuesOfType<T>(object root) where T : class
        {
            List<T> ret = new List<T>();
            List<object> scannedObjects = new List<object>();
            Type targetType = typeof(T);

            Action<object> method = null;

            method = (subRoot) =>
            {
                if (subRoot == null || scannedObjects.Contains(subRoot))
                    return;

                scannedObjects.Add(subRoot);
                Type subRootType = subRoot.GetType();

                var props = subRootType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var prop in props)
                {
                    if (prop.PropertyType.IsClass)
                    {
                        if (targetType.IsAssignableFrom(prop.PropertyType))
                        {
                            var ff = prop.GetValue(subRoot, null);
                            ret.Add(ff as T);
                        }
                        else
                        {
                            object value = prop.GetValue(subRoot, null);
                            if (prop.PropertyType.IsArray)
                            {
                                Array arr = value as Array;
                                if (arr != null)
                                {
                                    for (int i = 0; i < arr.Length; i++)
                                    {
                                        method(arr.GetValue(i));
                                    }
                                }
                            }
                            else
                            {
                                method(value);
                            }
                        }
                    }
                }
            };

            method(root);
            return ret.ToArray();
        }

        public static T[] MergeIntervals<T>(RegistreringType1[] personRegistrations, VirkningType targetInterval, Func<RegistreringType1, IEnumerable<T>> populator)
            where T : ITypeWithVirkning
        {
            var matchesByDate = personRegistrations
                    .SelectMany(oio =>
                    {
                        var pop = populator(oio);
                        if (pop == null)
                            pop = new T[0].AsEnumerable();
                        return pop.Select(ro => new RegisteredIntervalVirkningWrapper<T>(ro, oio.Tidspunkt.ToDateTime(), oio.BrokerUpdateDate));
                    })
                    .Where(ro => targetInterval.Intersects(ro.Item.Virkning))
                    .OrderBy(ro => ro.EffectiveStartTS)
                    .ThenBy(ro => ro.RegistrationDate)
                    .ThenBy(ro => ro.BrokerUpdateDate)
                    .ToArray();

            var ret = new List<RegisteredIntervalVirkningWrapper<T>>();
            var isIMultipleInstanceTypeWithVirkning = typeof(IMultipleInstanceTypeWithVirkning).IsAssignableFrom(typeof(T));

            // Now scan the effect periods from latest to first
            foreach (var currentInterval in matchesByDate.Reverse())
            {
                var nextInterval = ret
                    .Where(o =>
                        {
                            if (isIMultipleInstanceTypeWithVirkning)
                            {
                                var oT = o.Item as IMultipleInstanceTypeWithVirkning;
                                var currentT = currentInterval.Item as IMultipleInstanceTypeWithVirkning;
                                return oT.HasSameIntervalGroup(currentT);
                            }
                            else
                            {
                                return true;
                            }

                        }
                        )
                    .FirstOrDefault();
                if (nextInterval == null)
                {
                    ret.Insert(0, currentInterval);
                }
                else
                {
                    if (currentInterval.EffectiveStartTS < nextInterval.EffectiveStartTS)
                    {
                        if ( // Handling of a current married status followed by a dead marital status
                            isIMultipleInstanceTypeWithVirkning
                            && !currentInterval.StartTS.HasValue && currentInterval.EndTS.HasValue
                            && nextInterval.StartTS.HasValue && !nextInterval.EndTS.HasValue
                            && currentInterval.EndTS.Value > nextInterval.StartTS.Value)
                        {
                            nextInterval.EndTS = currentInterval.EndTS;
                        }
                        else
                        {
                            if (currentInterval.EffectiveEndTS > nextInterval.EffectiveStartTS)
                                currentInterval.EffectiveEndTS = nextInterval.EffectiveStartTS;
                            ret.Insert(0, currentInterval);
                        }
                    }
                }
            }
            return ret.Select(o => o.Item).ToArray();
        }

        public static FiltreretOejebliksbilledeType Merge(PersonIdentifier pId, VirkningType targetVirkning, RegistreringType1[] oioRegs)
        {
            return new FiltreretOejebliksbilledeType()
                {
                    AttributListe = new AttributListeType()
                    {
                        Egenskab = RegistreringType1.MergeIntervals<EgenskabType>(oioRegs, targetVirkning, oio => oio.AttributListe.Egenskab),
                        RegisterOplysning = RegistreringType1.MergeIntervals<RegisterOplysningType>(oioRegs, targetVirkning, oio => oio.AttributListe.RegisterOplysning),
                        SundhedOplysning = RegistreringType1.MergeIntervals<SundhedOplysningType>(oioRegs, targetVirkning, oio => oio.AttributListe.SundhedOplysning),
                        LokalUdvidelse = null
                    },

                    RelationListe = new RelationListeType()
                    {
                        Aegtefaelle = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Aegtefaelle),
                        Boern = MergeIntervals<PersonFlerRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Boern),
                        Bopaelssamling = MergeIntervals<PersonFlerRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Bopaelssamling),
                        ErstatningAf = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.ErstatningAf),
                        ErstatningFor = MergeIntervals<PersonFlerRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.ErstatningFor),
                        Fader = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Fader),
                        Foraeldremyndighedsboern = MergeIntervals<PersonFlerRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Foraeldremyndighedsboern),
                        Foraeldremyndighedsindehaver = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Foraeldremyndighedsindehaver),
                        LokalUdvidelse = null,
                        Moder = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.Moder),
                        RegistreretPartner = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.RegistreretPartner),
                        RetligHandleevneVaergeForPersonen = MergeIntervals<PersonRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.RetligHandleevneVaergeForPersonen),
                        RetligHandleevneVaergemaalsindehaver = MergeIntervals<PersonFlerRelationType>(oioRegs, targetVirkning, oio => oio.RelationListe.RetligHandleevneVaergemaalsindehaver),
                    },
                    TilstandListe = new TilstandListeType() { },

                    BrugervendtNoegleTekst = pId.CprNumber,

                    UUID = pId.UUID.Value.ToString()
                };
        }

        public void OrderByStartDate(bool ascending)
        {
            if (AttributListe != null)
            {
                AttributListe.Egenskab = VirkningType.OrderByStartDate<EgenskabType>(AttributListe.Egenskab, ascending);
                AttributListe.RegisterOplysning = VirkningType.OrderByStartDate<RegisterOplysningType>(AttributListe.RegisterOplysning, ascending);
                AttributListe.SundhedOplysning = VirkningType.OrderByStartDate<SundhedOplysningType>(AttributListe.SundhedOplysning, ascending);
            }
            if (RelationListe != null)
            {
                RelationListe.Aegtefaelle = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.Aegtefaelle, ascending);
                RelationListe.Boern = VirkningType.OrderByStartDate<PersonFlerRelationType>(RelationListe.Boern, ascending);
                RelationListe.Bopaelssamling = VirkningType.OrderByStartDate<PersonFlerRelationType>(RelationListe.Bopaelssamling, ascending);
                RelationListe.ErstatningAf = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.ErstatningAf, ascending);
                RelationListe.ErstatningFor = VirkningType.OrderByStartDate<PersonFlerRelationType>(RelationListe.ErstatningFor, ascending);
                RelationListe.Fader = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.Fader, ascending);
                RelationListe.Foraeldremyndighedsboern = VirkningType.OrderByStartDate<PersonFlerRelationType>(RelationListe.Foraeldremyndighedsboern, ascending);
                RelationListe.Foraeldremyndighedsindehaver = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.Foraeldremyndighedsindehaver, ascending);
                RelationListe.Moder = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.Moder, ascending);
                RelationListe.RegistreretPartner = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.RegistreretPartner, ascending);
                RelationListe.RetligHandleevneVaergeForPersonen = VirkningType.OrderByStartDate<PersonRelationType>(RelationListe.RetligHandleevneVaergeForPersonen, ascending);
                RelationListe.RetligHandleevneVaergemaalsindehaver = VirkningType.OrderByStartDate<PersonFlerRelationType>(RelationListe.RetligHandleevneVaergemaalsindehaver, ascending);
            }
        }
    }
}
