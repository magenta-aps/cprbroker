using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL
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

        public static string[] GetCountryEnglishAndDanishNamesByAlpha2Code(string alpha2CodeString)
        {
            if (!string.IsNullOrEmpty(alpha2CodeString))
            {
                using (CPRBrokerDALDataContext dataContext = new CPRBrokerDALDataContext())
                {
                    var countryNames = (from cn in dataContext.Countries
                                        where cn.Alpha2Code == alpha2CodeString
                                        select new string[]
                            {
                                cn.CountryName,
                                cn.DanishCountryName,
                                cn.DanishCountryName2,
                            }).FirstOrDefault();
                    if (countryNames != null)
                    {
                        countryNames = Array.FindAll<string>(countryNames, (n) => !String.IsNullOrEmpty(n));
                        return countryNames;
                    }
                }
            }
            return null;
        }
    }
}
