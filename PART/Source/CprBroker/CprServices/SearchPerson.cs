using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    /// <summary>
    /// Data tuple representing a person that is returned from a search operation
    /// </summary>
    public class SearchPerson : IEquatable<SearchPerson>
    {
        public string PNR;
        public NavnStrukturType Name;
        public AdresseType Address;

        public bool Equals(SearchPerson other)
        {
            return string.Equals(other.PNR, PNR);
        }

        public AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[]{
                    new EgenskabType(){ NavnStruktur = Name}
                },
                RegisterOplysning = new RegisterOplysningType[]{
                    new RegisterOplysningType() { Item = new CprBorgerType(){ 
                        PersonCivilRegistrationIdentifier = PNR,
                        FolkeregisterAdresse =  Address
                    }}
                }
            };
        }
    }
}
