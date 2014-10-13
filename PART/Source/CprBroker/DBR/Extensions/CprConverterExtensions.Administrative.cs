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
 * Dennis Amdi Skov Isaksen
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
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static Event ToDpr(this EventsType events)
        {
            Event e = new Event();
            e.PNR = decimal.Parse(events.PNR);
            if (events.CprUpdateDate.HasValue)
                e.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(events.CprUpdateDate.Value, 12);
            e.Event_ = events.Event_;
            e.AFLMRK = events.DerivedMark;
            return e;
        }

        public static Note ToDpr(this NotesType notes)
        {
            Note n = new Note();
            n.PNR = decimal.Parse(notes.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(notes.Registration.RegistrationDate, 12);

            if (notes.StartDate.HasValue)
                n.NationalRegisterMemoDate = CprBroker.Utilities.Dates.DateToDecimal(notes.StartDate.Value, 12);

            if (notes.EndDate.HasValue)
                n.DeletionDate = CprBroker.Utilities.Dates.DateToDecimal(notes.EndDate.Value, 8);

            n.NoteNumber = notes.NoteNumber;
            n.NationalRegisterNoteLine = notes.NoteText;
            n.MunicipalityCode = 0; //TODO: Can be fetched in CPR Services, komkod
            return n;
        }

        public static MunicipalCondition ToDpr(this MunicipalConditionsType condition)
        {
            MunicipalCondition m = new MunicipalCondition();
            m.PNR = decimal.Parse(condition.PNR);
            m.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(condition.Registration.RegistrationDate, 12);
            m.ConditionType = condition.MunicipalConditionType;
            m.ConditionMarker = condition.MunicipalConditionCode;

            if (condition.MunicipalConditionStartDate.HasValue)
                m.ConditionDate = CprBroker.Utilities.Dates.DateToDecimal(condition.MunicipalConditionStartDate.Value, 8);

            m.ConditionComments = condition.MunicipalConditionComment;
            return m;
        }
    }
}
