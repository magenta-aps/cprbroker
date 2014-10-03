/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Dennis Amdi Skov Isaksen
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
