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
                n.NationalRegisterMemoDate = CprBroker.Utilities.Dates.DateToDecimal(notes.StartDate.Value, 8);

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
