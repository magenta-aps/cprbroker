using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison
{
    [TestFixture]
    public class PersonTotalComparisonTest : ComparisonTest<PersonTotal>
    {
        public override IQueryable<PersonTotal> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.PersonTotals.Where(t => t.PNR == pnr);
        }
    }

    [TestFixture]
    public class PersonComparisonTest : ComparisonTest<Person>
    {
        public override IQueryable<Person> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Persons.Where(t => t.PNR == pnr);
        }
    }

    [TestFixture]
    public class ChildComparisonTests : ComparisonTest<Child>
    {
        public override IQueryable<Child> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Childs.Where(c => c.ParentPNR == pnr);
        }
    }

    [TestFixture]
    public class PersonNameComparisonTests : ComparisonTest<PersonName>
    {
        public override IQueryable<PersonName> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.PersonNames.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class CivilStatusComparisonTests : ComparisonTest<CivilStatus>
    {
        public override IQueryable<CivilStatus> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.CivilStatus.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class SeparationComparisonTests : ComparisonTest<Separation>
    {
        public override IQueryable<Separation> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Separations.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class NationalityComparisonTests : ComparisonTest<Nationality>
    {
        public override IQueryable<Nationality> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Nationalities.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class DepartureComparisonTests : ComparisonTest<Departure>
    {
        public override IQueryable<Departure> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Departures.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class ContactAddressComparisonTests : ComparisonTest<ContactAddress>
    {
        public override IQueryable<ContactAddress> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.ContactAddresses.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class PersonAddressComparisonTests : ComparisonTest<PersonAddress>
    {
        public override IQueryable<PersonAddress> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.PersonAddresses.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class ProtectionComparisonTests : ComparisonTest<Protection>
    {
        public override IQueryable<Protection> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Protections.Where(o => o.PNR == pnr);
        }
    }
    
    [TestFixture]
    public class DisappearanceComparisonTests : ComparisonTest<Disappearance>
    {
        public override IQueryable<Disappearance> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Disappearances.Where(o => o.PNR == pnr);
        }
    }
    
    [TestFixture]
    public class EventComparisonTests : ComparisonTest<Event>
    {
        public override IQueryable<Event> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Events.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class NoteComparisonTests : ComparisonTest<Note>
    {
        public override IQueryable<Note> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.Notes.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class MunicipalConditionComparisonTests : ComparisonTest<MunicipalCondition>
    {
        public override IQueryable<MunicipalCondition> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.MunicipalConditions.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class ParentalAuthorityConditionComparisonTests : ComparisonTest<ParentalAuthority>
    {
        public override IQueryable<ParentalAuthority> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.ParentalAuthorities.Where(o => o.ChildPNR == pnr);
        }
    }
    
    [TestFixture]
    public class GuardianAndParentalAuthorityRelationComparisonTests : ComparisonTest<GuardianAndParentalAuthorityRelation>
    {
        public override IQueryable<GuardianAndParentalAuthorityRelation> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.GuardianAndParentalAuthorityRelations.Where(o => o.PNR == pnr);
        }
    }

    [TestFixture]
    public class GuardianAddressComparisonTests : ComparisonTest<GuardianAddress>
    {
        public override IQueryable<GuardianAddress> Get(DPRDataContext dataContext, decimal pnr)
        {
            return dataContext.GuardianAddresses.Where(o => o.PNR == pnr);
        }
    }

    
}
