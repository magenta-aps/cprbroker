using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public partial class CprConverterExtensions
    {
        public static Person ToPerson(this IndividualResponseType person)
        {
            Person p = new Person();
            p.PNR = decimal.Parse(person.PersonInformation.PNR);
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(person.RegistrationDate, 12);
            p.Birthdate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.Birthdate.Value, 8);
            p.Gender = person.PersonInformation.Gender.ToString();
            p.CustomerNumber = null; //DPR SPECIFIC
            /*
             * Birth date related
             */
            p.BirthRegistrationAuthorityCode = decimal.Parse(person.BirthRegistrationInformation.BirthRegistrationAuthorityCode);
            p.BirthRegistrationDate = CprBroker.Utilities.Dates.DateToDecimal(person.BirthRegistrationInformation.Registration.RegistrationDate, 12);
            p.BirthRegistrationPlaceUpdateDate = 0; //TODO: Can be retrieved from CPR Services: foedmynhaenstart
            p.BirthplaceTextUpdateDate = null; //TODO: Can be retrieved from CPR Services: foedtxttimestamp
            p.BirthplaceText = person.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate that this is correct
            /*
             * Religious related
             */
            p.ChristianMark = person.ChurchInformation.ChurchRelationship.ToString();
            p.ChurchRelationUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(person.ChurchInformation.Registration.RegistrationDate, 12);
            p.ChurchAuthorityCode = 0; //TODO: Can be retrieved from CPR Services: fkirkmynkod
            p.ChurchDate = CprBroker.Utilities.Dates.DateToDecimal(person.ChurchInformation.StartDate.Value, 8);
            /*
             * Guardianship related
             */
            p.UnderGuardianshipAuthprityCode = 0; //TODO: Can be retrieved from CPR Services: mynkod-ctumyndig
            p.GuardianshipUpdateDate = null; //TODO: Can be fetched in CPR Services: timestamp
            if (person.Disempowerment != null)
            {
                p.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(person.Disempowerment.DisempowermentStartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.Disempowerment was NULL");
                p.UnderGuardianshipDate = null;
            }
            /*
             * PNR related
             */
            p.PnrMarkingDate = null; //TODO: Can be fetched in CPR Services: pnrhaenstart
            p.PnrDate = 0; //TODO: Can be fetched in CPR Services: pnrmrkhaenstart 
            p.CurrentPnrUpdateDate = null; //TODO: Can be fetched in CPR Services: timestamp
            if (!string.IsNullOrEmpty(person.PersonInformation.CurrentCprNumber))
            {
                p.CurrentPnr = decimal.Parse(person.PersonInformation.CurrentCprNumber);
            }
            else
            {
                Console.WriteLine("person.PersonInformation.CurrentCprNumber was NULL or empty");
                p.CurrentPnr = null;
            }
            if (person.PersonInformation.PersonEndDate != null)
            {
                p.PnrDeletionDate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.PersonEndDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.PersonInformation.PersonEndDate was NULL");
                p.PnrDeletionDate = null;
            }
            /*
             * Position related
             */
            p.JobDate = null; //TODO: Can be fetched in CPR Services: stillingsdato
            p.Job = person.PersonInformation.Job;
            /*
             * Relations related
             */
            p.KinshipUpdateDate = 0; //TODO: Can be fetched in CPR Services: timestamp
            if (!string.IsNullOrEmpty(person.ParentsInformation.MotherPNR))
            {
                p.MotherPnr = decimal.Parse(person.ParentsInformation.MotherPNR);
            }
            else
            {
                Console.WriteLine("person.ParentsInformation.MotherPNR was NULL or empty");
                p.MotherPnr = 0;
            }
            p.KinshipUpdateDate = 0; //TODO: Can be fetched in CPR Services: timestamp
            if (person.ParentsInformation.MotherBirthDate != null)
            {
                p.MotherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.MotherBirthDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.ParentsInformation.MotherBirthDate was NULL or empty");
                p.MotherBirthdate = null;
            }
            p.MotherDocumentation = null; //TODO: Can be fetched in CPR Services: mor_dok
            p.FatherPnr = decimal.Parse(person.ParentsInformation.FatherPNR);
            if (person.ParentsInformation.FatherBirthDate != null)
            {
                p.FatherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.FatherBirthDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.ParentsInformation.FatherBirthDate was NULL or empty");
                p.FatherBirthdate = null;
            }
            p.FatherDocumentation = null; //TODO: Can be fetched in CPR Services: far_dok
            p.PaternityDate = null; //TODO: Can be fetched in CPR Services: farhaenstart
            p.PaternityAuthorityCode = null; //TODO: Can be fetched in CPR Services: far_mynkod
            p.MotherName = person.ParentsInformation.MotherName;
            p.FatherName = person.ParentsInformation.FatherName;
            if (person.Disempowerment != null)
            {
                if (person.Disempowerment.DisempowermentEndDate.HasValue)
                    p.UnderGuardianshipDeleteDate = person.Disempowerment.DisempowermentEndDate.Value;
                p.UnderGuardianshipRelationType = person.Disempowerment.GuardianRelationType;
            }
            else
            {
                Console.WriteLine("person.Disempowerment was NULL");
                p.UnderGuardianshipDeleteDate = null;
                p.UnderGuardianshipRelationType = null;
            }
            return p;
        }

    }
}
