using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL
{
    public partial class Country
    {
        public static string GetCountryAlpha2CodeByEnglishName(string englishCountryName)
        {
            using (CPRBrokerDALDataContext dataContext = new CPRBrokerDALDataContext())
            {
                return (from cn in dataContext.Countries
                        where cn.CountryName == englishCountryName
                        select cn.Alpha2Code).FirstOrDefault();
            }
        }

        public static string GetCountryAlpha2CodeByDanishName(string danishCountryName)
        {
            using (CPRBrokerDALDataContext dataContext = new CPRBrokerDALDataContext())
            {
                return (from cn in dataContext.Countries
                        where cn.DanishCountryName == danishCountryName || cn.DanishCountryName2 == danishCountryName
                        select cn.Alpha2Code).FirstOrDefault();
            }
        }

        public static string GetCountryAlpha2CodeByKmdCode(string kmdCodeString)
        {
            int kmdCode = 0;
            if (!string.IsNullOrEmpty(kmdCodeString) && int.TryParse(kmdCodeString, out kmdCode))
            {
                using (CPRBrokerDALDataContext dataContext = new CPRBrokerDALDataContext())
                {
                    return (from cn in dataContext.Countries
                            where cn.KmdCode == kmdCode || cn.KmdCode2 == kmdCode || cn.KmdCode3 == kmdCode
                            select cn.Alpha2Code).FirstOrDefault();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
