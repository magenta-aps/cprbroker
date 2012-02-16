﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CprBroker.Data.Events
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
	
	
	public partial class DataChangeEventDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertDataChangeEvent(DataChangeEvent instance);
    partial void UpdateDataChangeEvent(DataChangeEvent instance);
    partial void DeleteDataChangeEvent(DataChangeEvent instance);
    #endregion
		
		public DataChangeEventDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataChangeEventDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataChangeEventDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataChangeEventDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<DataChangeEvent> DataChangeEvents
		{
			get
			{
				return this.GetTable<DataChangeEvent>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DataChangeEvent")]
	public partial class DataChangeEvent : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _DataChangeEventId;
		
		private System.Guid _PersonUuid;
		
		private System.DateTime _ReceivedData;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnDataChangeEventIdChanging(System.Guid value);
    partial void OnDataChangeEventIdChanged();
    partial void OnPersonUuidChanging(System.Guid value);
    partial void OnPersonUuidChanged();
    partial void OnReceivedDateChanging(System.DateTime value);
    partial void OnReceivedDateChanged();
    #endregion
		
		public DataChangeEvent()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DataChangeEventId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid DataChangeEventId
		{
			get
			{
				return this._DataChangeEventId;
			}
			set
			{
				if ((this._DataChangeEventId != value))
				{
					this.OnDataChangeEventIdChanging(value);
					this.SendPropertyChanging();
					this._DataChangeEventId = value;
					this.SendPropertyChanged("DataChangeEventId");
					this.OnDataChangeEventIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PersonUuid", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid PersonUuid
		{
			get
			{
				return this._PersonUuid;
			}
			set
			{
				if ((this._PersonUuid != value))
				{
					this.OnPersonUuidChanging(value);
					this.SendPropertyChanging();
					this._PersonUuid = value;
					this.SendPropertyChanged("PersonUuid");
					this.OnPersonUuidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReceivedData", DbType="DateTime NOT NULL")]
		public System.DateTime ReceivedDate
		{
			get
			{
				return this._ReceivedData;
			}
			set
			{
				if ((this._ReceivedData != value))
				{
					this.OnReceivedDateChanging(value);
					this.SendPropertyChanging();
					this._ReceivedData = value;
					this.SendPropertyChanged("ReceivedDate");
					this.OnReceivedDateChanged();
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
