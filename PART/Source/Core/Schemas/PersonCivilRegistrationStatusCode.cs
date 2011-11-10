using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas
{
    /// <summary>
    /// Enumerates the possible civil registration states as of http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/11/24/cpr_personcivilregistrationstatuscode.xsd.meta.xml
    /// </summary>
    public enum PersonCivilRegistrationStatusCode
    {
        RegisteredWithResidenceInDanishPopulationRegister = 1,
        RegisteredWithHighStreetcodeInDanishPopulationRegister = 3,
        RegisteredWithResidenceInGreenlandicPopulationRegister = 5,
        RegisteredWithHighStreetcodeIn_GreenlandicPopulationRegister = 7,
        RegisteredWithoutResidenceInDanishOrGreenlandicPopulationRegisterAndAdministrativeCivilRegistrationNumbers = 20,
        CancelledCivilRegistrationNumbers = 30,
        DeletedCivilRegistrationNumbers = 50,
        ChangedCivilRegistrationNumbers = 60,
        Disappeared = 70,
        Emigrated = 80,
        dead = 90 //(dead or dead as emigrated or disappeared)=90
    }
}
