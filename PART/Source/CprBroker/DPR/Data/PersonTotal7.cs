using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;
using System.Data.Linq.Mapping;

namespace CprBroker.Providers.DPR
{
    [Table(Name = "dbo.DTTOTAL")]
    public partial class PersonTotal7 : PersonTotal
    {
        
        private System.Nullable<System.DateTime> _DprLoadDate;

        private System.Nullable<char> _ApplicationCode;

        private System.Nullable<char> _DataRetrievalType;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnDprLoadDateChanging(System.Nullable<System.DateTime> value);
        partial void OnDprLoadDateChanged();
        partial void OnApplicationCodeChanging(System.Nullable<char> value);
        partial void OnApplicationCodeChanged();
        partial void OnDataRetrievalTypeChanging(System.Nullable<char> value);
        partial void OnDataRetrievalTypeChanged();
        #endregion

        public PersonTotal7()
        {
            OnCreated();
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "INDLAESDTO", Storage = "_DprLoadDate", DbType = "DateTime")]
        public System.Nullable<System.DateTime> DprLoadDate
        {
            get
            {
                return this._DprLoadDate;
            }
            set
            {
                if ((this._DprLoadDate != value))
                {
                    this.OnDprLoadDateChanging(value);
                    this.SendPropertyChanging();
                    this._DprLoadDate = value;
                    this.SendPropertyChanged("DprLoadDate");
                    this.OnDprLoadDateChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "INDLAESPGM", Storage = "_ApplicationCode", DbType = "Char(1)")]
        public System.Nullable<char> ApplicationCode
        {
            get
            {
                return this._ApplicationCode;
            }
            set
            {
                if ((this._ApplicationCode != value))
                {
                    this.OnApplicationCodeChanging(value);
                    this.SendPropertyChanging();
                    this._ApplicationCode = value;
                    this.SendPropertyChanged("ApplicationCode");
                    this.OnApplicationCodeChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "HENTTYP", Storage = "_DataRetrievalType", DbType = "Char(1)")]
        public System.Nullable<char> DataRetrievalType
        {
            get
            {
                return this._DataRetrievalType;
            }
            set
            {
                if ((this._DataRetrievalType != value))
                {
                    this.OnDataRetrievalTypeChanging(value);
                    this.SendPropertyChanging();
                    this._DataRetrievalType = value;
                    this.SendPropertyChanged("DataRetrievalType");
                    this.OnDataRetrievalTypeChanged();
                }
            }
        }
    }
}
