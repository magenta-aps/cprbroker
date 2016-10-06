﻿using CprBroker.Providers.DPR;
using CprBroker.Schemas.Part;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public class PersonInfoExtended : PersonInfo
    {
        public Person Person { get; set; }
        public Disappearance Disappearance { get; set; }

        private static T[] Get<T>(IList<object> objects)
            where T : class
        {
            return objects.Select(o => o as T).Where(o => o != null).ToArray();
        }

        public static PersonInfoExtended FromObjects(IList<object> objects)
        {
            Func<IHasNullableCorrectionMarker, bool> isActive = (o) =>
                VirkningType.DateRangeIncludes(o.ToStartTS(), o.ToEndTS(), DateTime.Now)
                && o.IsOk();

            return new PersonInfoExtended()
            {
                // Main object
                PersonTotal = Get<PersonTotal>(objects).First(),

                // Get the latest active nationality (if possible)
                Nationality = Get<Nationality>(objects).Where(pn => pn.CorrectionMarker == null && pn.NationalityEndDate == null).OrderByDescending(pn => pn.NationalityStartDate).FirstOrDefault(),

                // Get the latest valid address (if possible)
                Address = Get<PersonAddress>(objects)
                .Where(pa =>
                    pa.CorrectionMarker == null
                    && VirkningType.DateRangeIncludes(pa.ToStartTS(), pa.ToEndTS(), DateTime.Now))
                .OrderByDescending(pa => pa.AddressStartDate).FirstOrDefault(),

                // Get the current departure record (if possible)
                Departure = Get<Departure>(objects).Where(d => isActive(d)).OrderByDescending(d => d.ForeignAddressDate).FirstOrDefault(),

                // No need to filtrate by null NameTerminationDate because we are sorting by NameStartDate
                PersonName = Get<PersonName>(objects).FirstOrDefault(pn => isActive(pn)),

                // No need to filtrate by null EndDate bacause this is done in PersonTotal.ToCivilStatusCodeType
                Separation = Get<Separation>(objects).FirstOrDefault(s => isActive(s)),

                // Get all valid (active and inactive) civil states
                CivilStates = Get<CivilStatus>(objects).Where(civ => civ.CorrectionMarker == null).OrderBy(civ => civ.MaritalStatusDate).ToArray(),

                // Get All Children
                Children = Get<Child>(objects).ToArray(),
                ChildrenInCustodyRelations = null, //this.ChildrenInCustody_Relations.Where(r => relationTypes.Contains(r.RelationType)).ToArray(),

                // Parental authority
                ParentalAuthority = Get<ParentalAuthority>(objects).ToArray(),
                CustodyHolderRelations = null, //this.ParentalAuthorityHolders_Relations.Where(p => relationTypes.Contains(p.RelationType)).ToArray(),

                Person = Get<Person>(objects).First(),

                Disappearance = Get<Disappearance>(objects).FirstOrDefault(d => isActive(d))
            };
        }
    }
}