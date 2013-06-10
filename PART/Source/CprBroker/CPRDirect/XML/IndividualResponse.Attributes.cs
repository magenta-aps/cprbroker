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
using CprBroker.Schemas.Part;
using CprBroker.Engine;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = ToEgenskabType(),
                LokalUdvidelse = ToLokalUdvidelseType(),
                RegisterOplysning = ToRegisterOplysningType(),
                SundhedOplysning = ToSundhedOplysningType()
            };
        }

        public EgenskabInterval[] ToEgenskabIntervals()
        {
            var dataObjects = new List<ITimedType>();

            dataObjects.Add(this.CurrentNameInformation);
            dataObjects.AddRange(
                this.HistoricalName
                .Where(n => n.CorrectionMarker == CorrectionMarker.OK)
                .ToArray()
                );

            var ret = Interval.CreateFromData<EgenskabInterval>(dataObjects.Where(o => o != null).AsQueryable());
            Array.ForEach<EgenskabInterval>(ret, interval =>
                {
                    // TODO: Remove these commented lines
                    //interval.HistoricalNames = this.HistoricalName.ToArray();
                    //interval.BirthRegistrationInformation = this.BirthRegistrationInformation;
                    //interval.BasicInformation = this.PersonInformation;
                });
            return ret.ToArray();
        }

        public EgenskabType[] ToEgenskabType()
        {
            return ToEgenskabIntervals()
                .Select(e => e.ToEgenskabType())
                .ToArray();
        }

        public ITimedType[] ToRegisterOplysningIntervalObjects()
        {
            var dataObjects = new List<ITimedType>();

            dataObjects.Add(this.GetFolkeregisterAdresseSource(false));
            dataObjects.AddRange(
                this.HistoricalAddress
                .Where(a => a.CorrectionMarker == CorrectionMarker.OK)
                .ToArray());

            dataObjects.Add(this.CurrentDepartureData);
            dataObjects.AddRange(
                this.HistoricalDeparture
                .Where(d => d.CorrectionMarker == CorrectionMarker.OK)
                .ToArray()
                );

            dataObjects.Add(this.CurrentDisappearanceInformation);
            dataObjects.AddRange(
                this.HistoricalDisappearance
                .Where(d => d.CorrectionMarker == CorrectionMarker.OK)
                .ToArray()
                );

            dataObjects.Add(this.ChurchInformation);
            dataObjects.AddRange(
                this.HistoricalChurchInformation
                .ToArray());

            dataObjects.Add(new CurrentPnrTypeAdaptor(this.PersonInformation, this.HistoricalPNR));
            dataObjects.AddRange(
                this.HistoricalPNR
                .ToArray());

            dataObjects.Add(this.CurrentCitizenship);
            dataObjects.AddRange(
                this.HistoricalCitizenship
                .Where(c => c.CorrectionMarker == CorrectionMarker.OK)
                .ToArray());

            return dataObjects.Where(o => o != null).ToArray();
        }

        public RegisterOplysningInterval[] ToRegisterOplysningIntervalArray()
        {
            var dataObjects = ToRegisterOplysningIntervalObjects();
            return Interval.CreateFromData<RegisterOplysningInterval>(dataObjects.AsQueryable());
        }

        public RegisterOplysningType[] ToRegisterOplysningType()
        {
            return ToRegisterOplysningIntervalArray()
                .Select(ro => ro.ToRegisterOplysningType())
                .ToArray();
        }

        public IAddressSource GetFolkeregisterAdresseSource(bool putDummy)
        {
            if (this.CurrentAddressInformation != null && !this.ClearWrittenAddress.IsEmpty) // Both conditions are technically the same
            {
                return new CurrentAddressWrapper(this.CurrentAddressInformation, this.ClearWrittenAddress);
            }
            else if (this.CurrentDepartureData != null && !this.CurrentDepartureData.IsEmpty)
            {
                return CurrentDepartureData;
            }
            else if (this.CurrentDisappearanceInformation != null)
            {
                return CurrentDisappearanceInformation;
            }
            else if (putDummy)
            {
                return new DummyAddressSource();
            }
            else
            {
                return null;
            }
        }

    }

}
