using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public interface ICprDirectPersonDataProvider : CprBroker.Engine.IPartReadDataProvider
    {
        IndividualResponseType GetPerson(string cprNumber);
    }
}
