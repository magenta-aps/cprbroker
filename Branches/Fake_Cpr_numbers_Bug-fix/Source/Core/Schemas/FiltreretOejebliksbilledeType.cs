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

namespace CprBroker.Schemas.Part
{
    public partial class FiltreretOejebliksbilledeType
    {
        public FiltreretOejebliksbilledeType Filter(VirkningType targetInterval)
        {
            return new FiltreretOejebliksbilledeType()
            {
                AttributListe = new AttributListeType()
                {
                    Egenskab = Filter<EgenskabType>(this.AttributListe.Egenskab, targetInterval),
                    RegisterOplysning = Filter<RegisterOplysningType>(this.AttributListe.RegisterOplysning, targetInterval),
                    LokalUdvidelse = null,
                    SundhedOplysning = Filter<SundhedOplysningType>(this.AttributListe.SundhedOplysning, targetInterval),
                },
                TilstandListe = this.TilstandListe,
                BrugervendtNoegleTekst = this.BrugervendtNoegleTekst,
                UUID = this.UUID,
                RelationListe = new RelationListeType()
                {
                    Aegtefaelle = Filter<PersonRelationType>(this.RelationListe.Aegtefaelle, targetInterval),
                    RegistreretPartner = Filter<PersonRelationType>(this.RelationListe.RegistreretPartner, targetInterval),
                    Boern = Filter<PersonFlerRelationType>(this.RelationListe.Boern, targetInterval),
                    Bopaelssamling = Filter<PersonFlerRelationType>(this.RelationListe.Bopaelssamling, targetInterval),
                    ErstatningAf = Filter<PersonRelationType>(this.RelationListe.ErstatningAf, targetInterval),
                    ErstatningFor = Filter<PersonFlerRelationType>(this.RelationListe.ErstatningFor, targetInterval),
                    Fader = Filter<PersonRelationType>(this.RelationListe.Fader, targetInterval),
                    Moder = Filter<PersonRelationType>(this.RelationListe.Moder, targetInterval),
                    Foraeldremyndighedsboern = Filter<PersonFlerRelationType>(this.RelationListe.Foraeldremyndighedsboern, targetInterval),
                    Foraeldremyndighedsindehaver = Filter<PersonRelationType>(this.RelationListe.Foraeldremyndighedsindehaver, targetInterval),
                    LokalUdvidelse = this.RelationListe.LokalUdvidelse,
                    RetligHandleevneVaergeForPersonen = Filter<PersonRelationType>(this.RelationListe.RetligHandleevneVaergeForPersonen, targetInterval),
                    RetligHandleevneVaergemaalsindehaver = Filter<PersonFlerRelationType>(this.RelationListe.RetligHandleevneVaergemaalsindehaver, targetInterval)
                }
            };
        }

        public static T[] Filter<T>(T[] dataArray, VirkningType targetInterval)
            where T : ITypeWithVirkning
        {
            if (dataArray != null)
                return dataArray.Where(dataElement => dataElement.Virkning.Intersects(targetInterval)).ToArray();
            else
                return null;
        }
    }
}
