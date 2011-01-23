using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.DAL;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IDataProvider
    {
        #region IDataProvider Members

        bool IDataProvider.IsAlive()
        {
            return true;
        }

        Version IDataProvider.Version
        {
            get { return new Version(Versioning.Major, Versioning.Minor); }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Converts a database address to an OIO address
        /// </summary>
        /// <param name="personAddress">Database address</param>
        /// <returns></returns>
        private object GetAddress(DAL.Address personAddress)
        {
            /*
             * The idea here is to fill the Schemas.UtilAddress fields and then return its ToOioAddress() method
            */

            Schemas.Util.Address address = new CprBroker.Schemas.Util.Address();
            address[AddressField.Building] = personAddress.StreetBuildingIdentifier;
            address[AddressField.CareOfName] = personAddress.CareOfName;
            address[AddressField.Door] = personAddress.SuiteIdentifier;
            address[AddressField.Floor] = personAddress.FloorIdentifier;
            address[AddressField.HouseNumber] = personAddress.StreetBuildingIdentifier;
            address[AddressField.Line1] = personAddress.Line1;
            address[AddressField.Line2] = personAddress.Line2;
            address[AddressField.Line3] = personAddress.Line3;
            address[AddressField.Line4] = personAddress.Line4;
            address[AddressField.Line5] = personAddress.Line5;
            address[AddressField.Line6] = personAddress.Line6;


            address[AddressField.MunicipalityCode] = personAddress.MunicipalityCode;
            if (personAddress.Municipality != null)
            {
                address[AddressField.MunicipalityName] = personAddress.Municipality.MunicipalityName;
            }
            address[AddressField.StreetCode] = personAddress.StreetCode;
            address[AddressField.StreetName] = personAddress.StreetName;
            address[AddressField.StreetNameForAddressing] = personAddress.StreetNameForAddressing;

            address[AddressField.PostCode] = personAddress.PostCode;
            address[AddressField.PostDistrictName] = personAddress.PostDistrictName;

            return address.ToOioAddress(PersonCivilRegistrationStatusCodeType.Item01);
        }

        /// <summary>
        /// Retruns a Person object with at least the given detail level from the database
        /// </summary>
        /// <param name="cprNumber">CPR number</param>
        /// <param name="detailLevel">Minimum detail level</param>
        /// <param name="context">Database context</param>
        /// <returns></returns>
        private Person GetPersonFromDatabase(string cprNumber, DetailLevel.DetailLevelType detailLevel, CPRBrokerDALDataContext context)
        {
            System.Data.Linq.DataLoadOptions loadOptios = new System.Data.Linq.DataLoadOptions();
            loadOptios.LoadWith<DAL.Person>((per) => per.Address);
            loadOptios.LoadWith<DAL.Address>((adr) => adr.Municipality);
            context.LoadOptions = loadOptios;

            var person = context.Persons.FirstOrDefault(
                per =>
                    per.PersonNumber == cprNumber
                    && per.DetailLevelId >= (int)detailLevel
            );

            return person;
        }

        #endregion

    }
}
