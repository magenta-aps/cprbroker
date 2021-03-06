﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CprBroker.Providers.CPRDirect
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="DPR")]
	public partial class LookupDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertAuthority(Authority instance);
    partial void UpdateAuthority(Authority instance);
    partial void DeleteAuthority(Authority instance);
    #endregion
		
		public LookupDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LookupDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LookupDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LookupDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Authority> Authorities
		{
			get
			{
				return this.GetTable<Authority>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Authority")]
	public partial class Authority : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _AuthorityCode;
		
		private string _AuthorityType;
		
		private string _AuthorityGroup;
		
		private System.DateTime _UpdateTime;
		
		private string _AuthorityPhone;
		
		private System.DateTime _StartDate;
		
		private System.Nullable<System.DateTime> _EndDate;
		
		private string _AuthorityName;
		
		private string _Address;
		
		private string _AddressLine1;
		
		private string _AddressLine2;
		
		private string _AddressLine3;
		
		private string _AddressLine4;
		
		private string _Telefax;
		
		private string _FullName;
		
		private string _Email;
		
		private string _Alpha2CountryCode;
		
		private string _Alpha3CountryCode;
		
		private string _NumericCountryCode;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnAuthorityCodeChanging(string value);
    partial void OnAuthorityCodeChanged();
    partial void OnAuthorityTypeChanging(string value);
    partial void OnAuthorityTypeChanged();
    partial void OnAuthorityGroupChanging(string value);
    partial void OnAuthorityGroupChanged();
    partial void OnUpdateTimeChanging(System.DateTime value);
    partial void OnUpdateTimeChanged();
    partial void OnAuthorityPhoneChanging(string value);
    partial void OnAuthorityPhoneChanged();
    partial void OnStartDateChanging(System.DateTime value);
    partial void OnStartDateChanged();
    partial void OnEndDateChanging(System.Nullable<System.DateTime> value);
    partial void OnEndDateChanged();
    partial void OnAuthorityNameChanging(string value);
    partial void OnAuthorityNameChanged();
    partial void OnAddressChanging(string value);
    partial void OnAddressChanged();
    partial void OnAddressLine1Changing(string value);
    partial void OnAddressLine1Changed();
    partial void OnAddressLine2Changing(string value);
    partial void OnAddressLine2Changed();
    partial void OnAddressLine3Changing(string value);
    partial void OnAddressLine3Changed();
    partial void OnAddressLine4Changing(string value);
    partial void OnAddressLine4Changed();
    partial void OnTelefaxChanging(string value);
    partial void OnTelefaxChanged();
    partial void OnFullNameChanging(string value);
    partial void OnFullNameChanged();
    partial void OnEmailChanging(string value);
    partial void OnEmailChanged();
    partial void OnAlpha2CountryCodeChanging(string value);
    partial void OnAlpha2CountryCodeChanged();
    partial void OnAlpha3CountryCodeChanging(string value);
    partial void OnAlpha3CountryCodeChanged();
    partial void OnNumericCountryCodeChanging(string value);
    partial void OnNumericCountryCodeChanged();
    #endregion
		
		public Authority()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuthorityCode", DbType="VarChar(4) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string AuthorityCode
		{
			get
			{
				return this._AuthorityCode;
			}
			set
			{
				if ((this._AuthorityCode != value))
				{
					this.OnAuthorityCodeChanging(value);
					this.SendPropertyChanging();
					this._AuthorityCode = value;
					this.SendPropertyChanged("AuthorityCode");
					this.OnAuthorityCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuthorityType", DbType="VarChar(2) NOT NULL", CanBeNull=false)]
		public string AuthorityType
		{
			get
			{
				return this._AuthorityType;
			}
			set
			{
				if ((this._AuthorityType != value))
				{
					this.OnAuthorityTypeChanging(value);
					this.SendPropertyChanging();
					this._AuthorityType = value;
					this.SendPropertyChanged("AuthorityType");
					this.OnAuthorityTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuthorityGroup", DbType="Char(10) NOT NULL", CanBeNull=false)]
		public string AuthorityGroup
		{
			get
			{
				return this._AuthorityGroup;
			}
			set
			{
				if ((this._AuthorityGroup != value))
				{
					this.OnAuthorityGroupChanging(value);
					this.SendPropertyChanging();
					this._AuthorityGroup = value;
					this.SendPropertyChanged("AuthorityGroup");
					this.OnAuthorityGroupChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UpdateTime", DbType="DateTime NOT NULL")]
		public System.DateTime UpdateTime
		{
			get
			{
				return this._UpdateTime;
			}
			set
			{
				if ((this._UpdateTime != value))
				{
					this.OnUpdateTimeChanging(value);
					this.SendPropertyChanging();
					this._UpdateTime = value;
					this.SendPropertyChanged("UpdateTime");
					this.OnUpdateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuthorityPhone", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string AuthorityPhone
		{
			get
			{
				return this._AuthorityPhone;
			}
			set
			{
				if ((this._AuthorityPhone != value))
				{
					this.OnAuthorityPhoneChanging(value);
					this.SendPropertyChanging();
					this._AuthorityPhone = value;
					this.SendPropertyChanged("AuthorityPhone");
					this.OnAuthorityPhoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StartDate", DbType="DateTime NOT NULL")]
		public System.DateTime StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				if ((this._StartDate != value))
				{
					this.OnStartDateChanging(value);
					this.SendPropertyChanging();
					this._StartDate = value;
					this.SendPropertyChanged("StartDate");
					this.OnStartDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EndDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				if ((this._EndDate != value))
				{
					this.OnEndDateChanging(value);
					this.SendPropertyChanging();
					this._EndDate = value;
					this.SendPropertyChanged("EndDate");
					this.OnEndDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuthorityName", DbType="NVarChar(20)")]
		public string AuthorityName
		{
			get
			{
				return this._AuthorityName;
			}
			set
			{
				if ((this._AuthorityName != value))
				{
					this.OnAuthorityNameChanging(value);
					this.SendPropertyChanging();
					this._AuthorityName = value;
					this.SendPropertyChanged("AuthorityName");
					this.OnAuthorityNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Address", DbType="NVarChar(34)")]
		public string Address
		{
			get
			{
				return this._Address;
			}
			set
			{
				if ((this._Address != value))
				{
					this.OnAddressChanging(value);
					this.SendPropertyChanging();
					this._Address = value;
					this.SendPropertyChanged("Address");
					this.OnAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddressLine1", DbType="NVarChar(34)")]
		public string AddressLine1
		{
			get
			{
				return this._AddressLine1;
			}
			set
			{
				if ((this._AddressLine1 != value))
				{
					this.OnAddressLine1Changing(value);
					this.SendPropertyChanging();
					this._AddressLine1 = value;
					this.SendPropertyChanged("AddressLine1");
					this.OnAddressLine1Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddressLine2", DbType="NVarChar(34)")]
		public string AddressLine2
		{
			get
			{
				return this._AddressLine2;
			}
			set
			{
				if ((this._AddressLine2 != value))
				{
					this.OnAddressLine2Changing(value);
					this.SendPropertyChanging();
					this._AddressLine2 = value;
					this.SendPropertyChanged("AddressLine2");
					this.OnAddressLine2Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddressLine3", DbType="NVarChar(34)")]
		public string AddressLine3
		{
			get
			{
				return this._AddressLine3;
			}
			set
			{
				if ((this._AddressLine3 != value))
				{
					this.OnAddressLine3Changing(value);
					this.SendPropertyChanging();
					this._AddressLine3 = value;
					this.SendPropertyChanged("AddressLine3");
					this.OnAddressLine3Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddressLine4", DbType="NVarChar(34)")]
		public string AddressLine4
		{
			get
			{
				return this._AddressLine4;
			}
			set
			{
				if ((this._AddressLine4 != value))
				{
					this.OnAddressLine4Changing(value);
					this.SendPropertyChanging();
					this._AddressLine4 = value;
					this.SendPropertyChanged("AddressLine4");
					this.OnAddressLine4Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Telefax", DbType="VarChar(8)")]
		public string Telefax
		{
			get
			{
				return this._Telefax;
			}
			set
			{
				if ((this._Telefax != value))
				{
					this.OnTelefaxChanging(value);
					this.SendPropertyChanging();
					this._Telefax = value;
					this.SendPropertyChanged("Telefax");
					this.OnTelefaxChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FullName", DbType="NVarChar(60)")]
		public string FullName
		{
			get
			{
				return this._FullName;
			}
			set
			{
				if ((this._FullName != value))
				{
					this.OnFullNameChanging(value);
					this.SendPropertyChanging();
					this._FullName = value;
					this.SendPropertyChanged("FullName");
					this.OnFullNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Email", DbType="NVarChar(100)")]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Alpha2CountryCode", DbType="Char(2) NOT NULL", CanBeNull=false)]
		public string Alpha2CountryCode
		{
			get
			{
				return this._Alpha2CountryCode;
			}
			set
			{
				if ((this._Alpha2CountryCode != value))
				{
					this.OnAlpha2CountryCodeChanging(value);
					this.SendPropertyChanging();
					this._Alpha2CountryCode = value;
					this.SendPropertyChanged("Alpha2CountryCode");
					this.OnAlpha2CountryCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Alpha3CountryCode", DbType="Char(3) NOT NULL", CanBeNull=false)]
		public string Alpha3CountryCode
		{
			get
			{
				return this._Alpha3CountryCode;
			}
			set
			{
				if ((this._Alpha3CountryCode != value))
				{
					this.OnAlpha3CountryCodeChanging(value);
					this.SendPropertyChanging();
					this._Alpha3CountryCode = value;
					this.SendPropertyChanged("Alpha3CountryCode");
					this.OnAlpha3CountryCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NumericCountryCode", DbType="Char(3) NOT NULL", CanBeNull=false)]
		public string NumericCountryCode
		{
			get
			{
				return this._NumericCountryCode;
			}
			set
			{
				if ((this._NumericCountryCode != value))
				{
					this.OnNumericCountryCodeChanging(value);
					this.SendPropertyChanging();
					this._NumericCountryCode = value;
					this.SendPropertyChanged("NumericCountryCode");
					this.OnNumericCountryCodeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
