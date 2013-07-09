using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class RelationListeType
    {

        public void AssignUuids(Func<string, Guid> cprToUuidFunc)
        {
            foreach (var rels in new IPersonRelationType[][] { 
                this.Aegtefaelle, 
                this.Boern, 
                this.Bopaelssamling, 
                this.ErstatningAf, 
                this.ErstatningFor, 
                this.Fader, 
                this.Foraeldremyndighedsboern, 
                this.Foraeldremyndighedsindehaver, 
                this.Moder, 
                this.RegistreretPartner,
                this.RetligHandleevneVaergeForPersonen, 
                this.RetligHandleevneVaergemaalsindehaver })
            {
                if (rels != null)
                {
                    foreach (var rel in rels)
                    {
                        if (rel != null)
                        {
                            rel.ReferenceID = UnikIdType.Create(cprToUuidFunc(rel.CprNumber));
                        }
                    }
                }
            }
        }
    }
}
