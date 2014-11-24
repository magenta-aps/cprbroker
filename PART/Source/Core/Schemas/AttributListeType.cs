using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class AttributListeType
    {
        public string GetPnr()
        {
            if(this.RegisterOplysning !=null)
            {
                foreach (var ro in this.RegisterOplysning)
                {
                    if (ro != null)
                    {
                        if (ro.Item is CprBorgerType)
                        {
                            if (!string.IsNullOrEmpty((ro.Item as CprBorgerType).PersonCivilRegistrationIdentifier))
                                return (ro.Item as CprBorgerType).PersonCivilRegistrationIdentifier;
                        }
                        else if (ro.Item is UdenlandskBorgerType )
                        {
                            if (!string.IsNullOrEmpty((ro.Item as UdenlandskBorgerType).PersonCivilRegistrationReplacementIdentifier))
                                return (ro.Item as UdenlandskBorgerType).PersonCivilRegistrationReplacementIdentifier;
                        }
                        else if (ro.Item is UkendtBorgerType)
                        {
                            if (!string.IsNullOrEmpty((ro.Item as UkendtBorgerType).PersonCivilRegistrationReplacementIdentifier))
                                return (ro.Item as UkendtBorgerType).PersonCivilRegistrationReplacementIdentifier;
                        }
                    }
                }
            }
            return null;
        }
    }
}
