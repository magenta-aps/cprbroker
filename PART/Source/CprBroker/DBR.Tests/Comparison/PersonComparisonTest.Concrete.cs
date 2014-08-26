using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison
{
    [TestFixture]
    public class PersonTotalComparisonTest : PersonComparisonTest<PersonTotal> { }

    [TestFixture]
    public class PersonComparisonTest : PersonComparisonTest<Person> { }

    [TestFixture]
    public class ChildComparisonTests : PersonComparisonTest<Child> { }

    [TestFixture]
    public class PersonNameComparisonTests : PersonComparisonTest<PersonName> { }

    [TestFixture]
    public class CivilStatusComparisonTests : PersonComparisonTest<CivilStatus> { }

    [TestFixture]
    public class SeparationComparisonTests : PersonComparisonTest<Separation> { }

    [TestFixture]
    public class NationalityComparisonTests : PersonComparisonTest<Nationality> { }

    [TestFixture]
    public class DepartureComparisonTests : PersonComparisonTest<Departure> { }

    [TestFixture]
    public class ContactAddressComparisonTests : PersonComparisonTest<ContactAddress> { }

    [TestFixture]
    public class PersonAddressComparisonTests : PersonComparisonTest<PersonAddress> { }

    [TestFixture]
    public class ProtectionComparisonTests : PersonComparisonTest<Protection> { }

    [TestFixture]
    public class DisappearanceComparisonTests : PersonComparisonTest<Disappearance> { }

    [TestFixture]
    public class EventComparisonTests : PersonComparisonTest<Event> { }

    [TestFixture]
    public class NoteComparisonTests : PersonComparisonTest<Note> { }

    [TestFixture]
    public class MunicipalConditionComparisonTests : PersonComparisonTest<MunicipalCondition> { }

    [TestFixture]
    public class ParentalAuthorityConditionComparisonTests : PersonComparisonTest<ParentalAuthority> { }

    [TestFixture]
    public class GuardianAndParentalAuthorityRelationComparisonTests : PersonComparisonTest<GuardianAndParentalAuthorityRelation> { }

    [TestFixture]
    public class GuardianAddressComparisonTests : PersonComparisonTest<GuardianAddress> { }

}