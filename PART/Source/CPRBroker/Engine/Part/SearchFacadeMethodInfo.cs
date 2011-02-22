using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    public class SearchFacadeMethodInfo : FacadeMethodInfo<SoegOutputType, string[]>
    {
        SoegInputType1 Input;

        private SearchFacadeMethodInfo()
        { }

        public SearchFacadeMethodInfo(SoegInputType1 input, string appToken, string userToken)
            : base(appToken, userToken)
        {
            Input = input;
        }

        public override void Initialize()
        {
            this.SubMethodInfos = new SubMethodInfo[] { new SearchSubMethodInfo(Input) };
        }

        public override StandardReturType ValidateInput()
        {
            if (Input == null)
            {
                return StandardReturType.NullInput();
            }

            if (Input.SoegObjekt == null)
            {
                return StandardReturType.NullInput("SoegObjekt");
            }

            if (!string.IsNullOrEmpty(Input.SoegObjekt.UUID) && !Util.Strings.IsGuid(Input.SoegObjekt.UUID))
            {
                return StandardReturType.InvalidUuid(Input.SoegObjekt.UUID);
            }
            // Start index & ax results
            if (!string.IsNullOrEmpty(Input.FoersteResultatReference))
            {
                int startResult;
                if (!int.TryParse(Input.FoersteResultatReference, out startResult))
                {
                    return StandardReturType.InvalidValue("FoersteResultatReference", Input.FoersteResultatReference);
                }
                if (startResult < 0)
                {
                    return StandardReturType.ValueOutOfRange("FoersteResultatReference", Input.FoersteResultatReference);
                }
            }

            if (!string.IsNullOrEmpty(Input.MaksimalAntalKvantitet))
            {
                int maxResults;
                if (!int.TryParse(Input.MaksimalAntalKvantitet, out maxResults))
                {
                    return StandardReturType.InvalidValue("MaksimalAntalKvantitet", Input.MaksimalAntalKvantitet);
                }
                if (maxResults < 0)
                {
                    return StandardReturType.ValueOutOfRange("MaksimalAntalKvantitet", Input.MaksimalAntalKvantitet);
                }
            }

            // Not implemented criteria
            if (Input.SoegObjekt.SoegRegistrering != null)
            {
                return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegRegistrering");
            }
            if (Input.SoegObjekt.SoegRelationListe != null)
            {
                return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegRelationListe");
            }

            if (Input.SoegObjekt.SoegTilstandListe != null)
            {
                return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegTilstandListe");
            }

            if (Input.SoegObjekt.SoegVirkning != null)
            {
                return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegVirkning");
            }

            // Now validate attribute lists
            if (Input.SoegObjekt.SoegAttributListe != null)
            {
                if (Input.SoegObjekt.SoegAttributListe.LokalUdvidelse != null)
                {
                    return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegAttributListe.LokalUdvidelse");
                }

                if (Input.SoegObjekt.SoegAttributListe.SoegRegisterOplysning != null)
                {
                    return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegAttributListe.SoegRegisterOplysning");
                }

                if (Input.SoegObjekt.SoegAttributListe.SoegSundhedOplysning != null)
                {
                    return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegAttributListe.SoegSundhedOplysning");
                }

                if (Input.SoegObjekt.SoegAttributListe.SoegEgenskab != null)
                {
                    foreach (var egen in Input.SoegObjekt.SoegAttributListe.SoegEgenskab)
                    {
                        if (egen.AndreAdresser != null)
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "AndreAdresser");
                        }
                        if (!string.IsNullOrEmpty(egen.FoedestedNavn))
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "FoedestedNavn");
                        }

                        if (!string.IsNullOrEmpty(egen.FoedselsregistreringMyndighedNavn))
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "FoedselsregistreringMyndighedNavn");
                        }
                        if (egen.KontaktKanal != null)
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "KontaktKanal");
                        }
                        if (egen.NaermestePaaroerende != null)
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "NaermestePaaroerende");
                        }
                        if (egen.SoegVirkning != null)
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegVirkning");
                        }
                        if (egen.SoegVirkning != null)
                        {
                            return StandardReturType.Create(HttpErrorCode.NOT_IMPLEMENTED, "SoegVirkning");
                        }
                    }
                }
            }
            return StandardReturType.OK();
        }

        public override string[] Aggregate(object[] results)
        {
            var foundIds = results[0] as Guid[];
            return (from id in foundIds select id.ToString()).ToArray();
        }
    }
}
