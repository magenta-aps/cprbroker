using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static Street ToDprStreet(this StreetType s)
        {
            Street st = new Street();
            st.KOMKOD = s.MunicipalityCode;
            st.SVEJADRNVN = s.StreetAddressingName
                .ToUpper()
                .Replace('É', 'E')
                .Replace('Ä', 'Æ')
                .Replace('Ö', 'Ø')
                .Replace('Ÿ', 'Y')
                //.Replace('Ü', 'Y') causes more failures than solutions
                ;
            st.VEJKOD = s.StreetCode;
            st.VEJADNVN = s.StreetAddressingName;
            return st;
        }

        public static City ToDprCity(this CityType city)
        {
            City c = new City();
            c.AJFDTO = city.Timestamp;
            c.BYNVN = city.CityName;
            c.HUSNRFRA = city.HouseNumberFrom;
            c.HUSNRTIL = city.HouseNumberTo;
            c.KOMKOD = city.MunicipalityCode;
            c.LIGEULIGE = city.EvenOrOdd;
            c.VEJKOD = city.StreetCode;
            return c;
        }

        public static PostDistrict ToDprPostDistrict(this PostDistrictType pd)
        {
            PostDistrict p = new PostDistrict();
            p.AJFDTO = pd.Timestamp;
            p.DISTTXT = pd.PostDistrictText;
            p.HUSNRFRA = pd.HouseNumberFrom;
            p.HUSNRTIL = pd.HouseNumberTo;
            p.KOMKOD = pd.MunicipalityCode;
            p.LIGEULIGE = pd.EvenOrOdd;
            p.VEJKOD = pd.StreetCode;
            p.POSTNR = pd.PostNumber;
            return p;
        }

        public static AreaRestorationDistrict ToDprAreaRestorationDistrict(this AreaRestorationDistrictType ardt)
        {
            AreaRestorationDistrict a = new AreaRestorationDistrict();
            a.AJFDTO = ardt.Timestamp;
            a.BYFORNYKOD = ardt.AreaRestorationCode;
            a.DISTTXT = ardt.DistrictText;
            a.HUSNRFRA = ardt.HouseNumberFrom;
            a.HUSNRTIL = ardt.HouseNumberTo;
            a.KOMKOD = ardt.MunicipalityCode;
            a.LIGEULIGE = ardt.EvenOrOdd;
            a.VEJKOD = ardt.StreetCode;
            return a;
        }

        public static DiverseDistrict ToDprDiverseDistrict(this DiverseDistrictType ddt)
        {
            DiverseDistrict d = new DiverseDistrict();
            d.AJFDTO = ddt.Timestamp;
            d.DISTTXT = ddt.DistrictText;
            d.DISTTYP = ddt.DistrictType;
            d.DIVDISTKOD = ddt.DivDistrictCode;
            d.HUSNRFRA = ddt.HouseNumberFrom;
            d.HUSNRTIL = ddt.HouseNumberTo;
            d.KOMKOD = ddt.MunicipalityCode;
            d.LIGEULIGE = ddt.EvenOrOdd;
            d.VEJKOD = ddt.StreetCode;
            return d;
        }

        public static EvacuationDistrict ToDprEvacuationDistrict(this EvacuationDistrictType edt)
        {
            EvacuationDistrict e = new EvacuationDistrict();
            e.AJFDTO = edt.Timestamp;
            e.DISTTXT = edt.DistrictText;
            e.EVAKUERKOD = edt.EvacuationCode;
            e.HUSNRFRA = edt.HouseNumberFrom;
            e.HUSNRTIL = edt.HouseNumberTo;
            e.KOMKOD = edt.MunicipalityCode;
            e.LIGEULIGE = edt.EvenOrOdd;
            e.VEJKOD = edt.StreetCode;
            return e;
        }

        public static ChurchDistrict ToDprChurchDistrict(this ChurchDistrictType cdt)
        {
            ChurchDistrict c = new ChurchDistrict();
            c.AJFDTO = cdt.Timestamp;
            c.DISTTXT = cdt.DistrictText;
            c.HUSNRFRA = cdt.HouseNumberFrom;
            c.HUSNRTIL = cdt.HouseNumberTo;
            c.KIRKEKOD = cdt.ChurchDistrictCode;
            c.KOMKOD = cdt.MunicipalityCode;
            c.LIGEULIGE = cdt.EvenOrOdd;
            c.VEJKOD = cdt.StreetCode;
            return c;
        }

        public static SchoolDistrict ToDprSchoolDistrict(this SchoolDistrictType sdt)
        {
            SchoolDistrict s = new SchoolDistrict();
            s.AJFDTO = sdt.Timestamp;
            s.DISTTXT = sdt.DistrictText;
            s.HUSNRFRA = sdt.HouseNumberFrom;
            s.HUSNRTIL = sdt.HouseNumberTo;
            s.KOMKOD = sdt.MunicipalityCode;
            s.LIGEULIGE = sdt.EvenOrOdd;
            s.SKOLEKOD = sdt.SchoolCode;
            s.VEJKOD = sdt.StreetCode;
            return s;
        }

        public static PopulationDistrict ToDprPopulationDistrict(this PopulationDistrictType pdt)
        {
            PopulationDistrict p = new PopulationDistrict();
            p.AJFDTO = pdt.Timestamp;
            p.BEFOLKKOD = pdt.PopulationDistrictCode;
            p.DISTTXT = pdt.DistrictText;
            p.HUSNRFRA = pdt.HouseNumberFrom;
            p.HUSNRTIL = pdt.HouseNumberTo;
            p.KOMKOD = pdt.MunicipalityCode;
            p.LIGEULIGE = pdt.EvenOrOdd;
            p.VEJKOD = pdt.StreetCode;
            return p;
        }

        public static SocialDistrict ToDprSocialDistrict(this SocialDistrictType sdt)
        {
            SocialDistrict s = new SocialDistrict();
            s.AJFDTO = sdt.Timestamp;
            s.DISTTXT = sdt.DistrictText;
            s.HUSNRFRA = sdt.HouseNumberFrom;
            s.HUSNRTIL = sdt.HouseNumberTo;
            s.KOMKOD = sdt.MunicipalityCode;
            s.LIGEULIGE = sdt.EvenOrOdd;
            s.SOCIALKOD = sdt.SocialCode;
            s.VEJKOD = sdt.StreetCode;
            return s;
        }

        public static ChurchAdministrationDistrict ToDprChurchAdministrationDistrict(this ChurchAdministrationDistrictType cadt)
        {
            ChurchAdministrationDistrict c = new ChurchAdministrationDistrict();
            c.AJFDTO = cadt.Timestamp;
            c.HUSNRFRA = cadt.HouseNumberFrom;
            c.HUSNRTIL = cadt.HouseNumberTo;
            c.KOMKOD = cadt.MunicipalityCode;
            c.LIGEULIGE = cadt.EvenOrOdd;
            c.MYNKOD = cadt.AuthorityAndChurchCode;
            c.VEJKOD = cadt.StreetCode;
            return c;
        }

        public static ElectionDistrict ToDprElectionDistrict(this ElectionDistrictType edt)
        {
            ElectionDistrict e = new ElectionDistrict();
            e.AJFDTO = edt.Timestamp;
            e.DISTTXT = edt.DistrictText;
            e.HUSNRFRA = edt.HouseNumberFrom;
            e.HUSNRTIL = edt.HouseNumberTo;
            e.KOMKOD = edt.MunicipalityCode;
            e.LIGEULIGE = edt.EvenOrOdd;
            e.VALGKOD = edt.ElectionCode;
            e.VEJKOD = edt.StreetCode;
            return e;
        }

        public static HeatingDistrict ToDprHeatingDistrict(this HeatingDistrictType hdt)
        {
            HeatingDistrict h = new HeatingDistrict();
            h.AJFDTO = hdt.Timestamp;
            h.DISTTXT = hdt.DistrictText;
            h.HUSNRFRA = hdt.HouseNumberFrom;
            h.HUSNRTIL = hdt.HouseNumberTo;
            h.KOMKOD = hdt.MunicipalityCode;
            h.LIGEULIGE = hdt.EvenOrOdd;
            h.VARMEKOD = hdt.HeatingDistrictCode;
            h.VEJKOD = hdt.StreetCode;
            return h;
        }

        public static PostNumber ToDprPostNumber(this PostNumberType pnt)
        {
            PostNumber p = new PostNumber();
            p.POSTNR = pnt.PostCode;
            p.POSTTXT = pnt.PostText;
            return p;
        }
    }
}
