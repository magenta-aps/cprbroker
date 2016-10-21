﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CprBroker.CustomActions.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CprBroker.CustomActions.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ////****** Object:  Table [dbo].[Authority]    Script Date: 11/21/2013 10:16:51 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[Authority]&apos;) AND type in (N&apos;U&apos;))
        ///BEGIN
        ///CREATE TABLE [dbo].[Authority](
        ///	[AuthorityCode] [varchar](4) NOT NULL
        ///		CONSTRAINT [PK_Authority] PRIMARY KEY CLUSTERED (	[AuthorityCode] ASC),
        ///	[AuthorityType] [varchar](2) NOT NULL,
        ///	[AuthorityGroup] [char](10) NOT NULL,
        ///	[UpdateTime] [datetime [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Authority {
            get {
                return ResourceManager.GetString("Authority", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 00037071620120702
        ///001000000019910923120000000000199109231200000000000000Ukendt Myndighed                                                                                                                                                                                      Ukendt Myndighed                                                                                                                                                     000
        ///001000139020110819105772269735201107010000000000000000Cpr-Kontoret      [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Authority_4357 {
            get {
                return ResourceManager.GetString("Authority_4357", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ////****** Object:  Table [dbo].[Country]    Script Date: 11/21/2013 10:16:51 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[Country]&apos;) AND type in (N&apos;U&apos;))
        ///BEGIN
        ///CREATE TABLE [dbo].[Country](
        ///	[Alpha2Code] [varchar](2) NOT NULL
        ///		CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED (	[Alpha2Code] ASC),
        ///	[Alpha3Code] [varchar](3) NOT NULL,
        ///	[NumericCode] [int] NOT NULL,
        ///	[CountryName] [nvarchar](60) NOT NULL,
        ///	[Descr [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Country {
            get {
                return ResourceManager.GetString("Country", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ////****** Object:  Table [dbo].[ActorRef]    Script Date: 11/21/2013 10:16:51 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[ActorRef]&apos;) AND type in (N&apos;U&apos;))
        ///BEGIN
        ///CREATE TABLE [dbo].[ActorRef](
        ///	[ActorRefId] [uniqueidentifier] NOT NULL,
        ///	[Type] [int] NOT NULL,
        ///	[Value] [varchar](50) NULL,
        /// CONSTRAINT [PK_ActorRef] PRIMARY KEY CLUSTERED 
        ///(
        ///	[ActorRefId] ASC
        ///)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  =  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string CreatePartDatabaseObjects {
            get {
                return ResourceManager.GetString("CreatePartDatabaseObjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  Table [dbo].[DataChangeEvent]    Script Date: 11/21/2013 10:16:51 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[DataChangeEvent]&apos;) AND type in (N&apos;U&apos;))
        ///BEGIN
        ///CREATE TABLE [dbo].[DataChangeEvent](
        ///	[DataChangeEventId] [uniqueidentifier] NOT NULL
        ///		DEFAULT NEWID()
        ///		CONSTRAINT [PK_DataChangeEvent] PRIMARY KEY CLUSTERED (	[DataChangeEventId] ASC),
        ///	[PersonUuid] [uniqueidentifier] NOT NULL,
        ///	[Per [rest of string was truncated]&quot;;.
        /// </summary>
        public static string DataChangeEvent {
            get {
                return ResourceManager.GetString("DataChangeEvent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /* 
        ///  ===========================================================================
        ///    Procedure:   InitializePersonSearchCache
        ///    Author:		 Beemen Beshara
        ///    Create date: 24-Jan-2014
        ///    Description: Initializes the cashed version of persons&apos; searchable fields
        /// ============================================================================
        ///*/
        ///
        ///IF EXISTS (SELECT * FROM sys.procedures WHERE name = &apos;InitializePersonSearchCache&apos;)
        ///    DROP PROCEDURE dbo.InitializePersonSearchCache
        ///GO
        ///
        ///CREATE PROCEDUR [rest of string was truncated]&quot;;.
        /// </summary>
        public static string InitializePersonSearchCache {
            get {
                return ResourceManager.GetString("InitializePersonSearchCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LifecycleStatusId;LifecycleStatusName
        ///0;Created
        ///1;Imported
        ///2;Deactivated
        ///3;Deleted
        ///4;Updated
        ///.
        /// </summary>
        public static string LifecycleStatus {
            get {
                return ResourceManager.GetString("LifecycleStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;configuration&gt;
        ///
        ///    &lt;configSections&gt;
        ///
        ///        &lt;!-- PART--&gt;
        ///        &lt;sectionGroup name=&quot;system.web.webPages.razor&quot; type=&quot;System.Web.WebPages.Razor.Configuration.RazorWebSectionGroup, System.Web.WebPages.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35&quot;&gt;
        ///            &lt;section name=&quot;host&quot; type=&quot;System.Web.WebPages.Razor.Configuration.HostSection, System.Web.WebPages.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35&quot; r [rest of string was truncated]&quot;;.
        /// </summary>
        public static string MvcWebConfig {
            get {
                return ResourceManager.GetString("MvcWebConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*
        ///	This file patches the CPR broker database to version 1.3
        ///	Creates tables Extract, ExtractItem and Authority
        ///	SQL 9.xxx (2005) is a minimum because it makes use of INCLUDE in index for ExtractItem
        ///*/
        ///
        ////****** Object:  Column [dbo].[PersonRegistration].[Table]    Script Date: 23/08/2012 18:36:34 ******/
        ///IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id= object_id(&apos;PersonRegistration&apos;) AND name = &apos;SourceObjects&apos;)
        ///BEGIN
        ///	ALTER TABLE [dbo].[PersonRegistration] ADD [SourceObjects] [xml] NULL
        ///E [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_1_3 {
            get {
                return ResourceManager.GetString("PatchDatabase_1_3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  Table [dbo].[Extract].[Ready]     ******/
        ///IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N&apos;[dbo].[Extract]&apos;) AND name = &apos;Ready&apos;)
        ///BEGIN
        ///	ALTER TABLE dbo.Extract ADD
        ///		Ready bit NOT NULL CONSTRAINT DF_Extract_Ready DEFAULT 0 
        ///END
        ///GO
        ///
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///
        ///SET ANSI_PADDING ON
        ///GO
        ///
        ////****** Object:  Table [dbo].[ExtractPersonStaging]    Script Date: 10/17/2012 19:15:13 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///SET ANSI_PADDING ON
        ///GO
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_1_3_2 {
            get {
                return ResourceManager.GetString("PatchDatabase_1_3_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///ALTER TABLE [dbo].[PersonProperties] ALTER COLUMN [BirthRegistrationAuthority] varchar(60) 
        ///GO.
        /// </summary>
        public static string PatchDatabase_1_4 {
            get {
                return ResourceManager.GetString("PatchDatabase_1_4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -------------------------------------
        ///-----  Column width expansion  ------
        ///-------------------------------------
        ///ALTER TABLE [dbo].[PersonProperties] ALTER COLUMN [BirthPlace] varchar(132) 
        ///GO
        ///
        ///
        ///---------------------------------------
        ///-----  Table for extract errors  ------
        ///---------------------------------------
        ///
        ///IF NOT EXISTS (SELECT * FROM sys.tables WHERE name=&apos;ExtractError&apos;)
        ///CREATE TABLE dbo.ExtractError(
        ///    ExtractErrorId uniqueidentifier NOT NULL DEFAULT(newid()),
        ///    ExtractId unique [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_2_1 {
            get {
                return ResourceManager.GetString("PatchDatabase_2_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ----------------------------------------------------------------------------
        ///------------  Fixes to error in installer of previous version  -------------
        ///----------------------------------------------------------------------------
        ///
        ///IF EXISTS (SELECT * FROM sys.columns c WHERE name = &apos;EffectId&apos; and object_id=object_id(&apos;PersonProperties&apos;))
        ///	ALTER TABLE PersonProperties DROP COLUMN EffectId
        ///GO
        ///
        ///-----------------------------------------------------------------
        ///------------  Enables parameterized subscr [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_2_2 {
            get {
                return ResourceManager.GetString("PatchDatabase_2_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Create table DataProviderCall to keep track of calls made to dataproviders
        ////****** Object:  Table [dbo].[DataProviderCall]    Script Date: 11/12/2013 14:07:02 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///SET ANSI_PADDING ON
        ///GO
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[DataProviderCall]&apos;) AND type in (N&apos;U&apos;))
        ///BEGIN
        ///CREATE TABLE [dbo].[DataProviderCall](
        ///	[DataProviderCallId] [uniqueidentifier] NOT NULL,
        ///	[ActivityId] [uniqueidentifier] NOT NULL,
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_2_2_1 {
            get {
                return ResourceManager.GetString("PatchDatabase_2_2_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER TABLE DataProviderCall ALTER COLUMN Success bit NULL
        ///
        ////****** Object:  Table [dbo].[BudgetInterval]    Script Date: 11/28/2013 16:33:20 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///
        ///SET ANSI_PADDING ON
        ///GO
        ///
        ///IF NOT EXISTS (SELECT * FROM sys.tables WHERE Name = &apos;BudgetInterval&apos;)
        ///BEGIN
        ///	CREATE TABLE [dbo].[BudgetInterval](
        ///		[IntervalMilliseconds] [bigint] NOT NULL,
        ///		[Name] [varchar](50) NOT NULL,
        ///		[CallThreshold] [int] NULL,
        ///		[CostThreshold] [decimal](18, 4) NULL,
        ///		[La [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_2_2_2 {
            get {
                return ResourceManager.GetString("PatchDatabase_2_2_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- If the new columns are not there, drop the cache table
        ///IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(&apos;PersonSearchCache&apos;) AND name = &apos;MunicipalityCode&apos; )
        ///BEGIN
        ///    -- Only if the table exists
        ///    IF EXISTS (SELECT * FROM sys.tables WHERE name = &apos;PersonSearchCache&apos;)
        ///        DROP TABLE PersonSearchCache
        ///END
        ///
        ///GO
        ///
        ////*
        ///   ==============================
        ///   ====      Semaphores      ====
        ///   ==============================
        /// */
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE obj [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatchDatabase_2_2_3 {
            get {
                return ResourceManager.GetString("PatchDatabase_2_2_3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(&apos;Semaphore&apos;) AND name = &apos;WaitCount&apos; )
        ///ALTER TABLE [dbo].[Semaphore]
        ///    ADD [WaitCount] INT DEFAULT 1 NULL;.
        /// </summary>
        public static string PatchDatabase_2_2_6 {
            get {
                return ResourceManager.GetString("PatchDatabase_2_2_6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF NOT EXISTS (SELECT * FROM sys.indexes si WHERE si.name = N&apos;IX_QueueItem_QueueId&apos;)
        ///	CREATE NONCLUSTERED INDEX [IX_QueueItem_QueueId] ON [dbo].[QueueItem] 
        ///	(
        ///		[QueueId] ASC
        ///	)
        ///	INCLUDE ( [AttemptCount],[SemaphoreId]) 
        ///GO
        ///.
        /// </summary>
        public static string PatchDatabase_DbrTest {
            get {
                return ResourceManager.GetString("PatchDatabase_DbrTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*
        ///    =========================
        ///    Table: PersonRegistration
        ///    =========================
        ///*/
        ///IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[PersonRegistration]&apos;) AND type in (N&apos;U&apos;))
        ///BEGIN
        ///    CREATE TABLE [dbo].[PersonRegistration](
        ///        [PersonRegistrationId] [uniqueidentifier] NOT NULL,
        ///        [UUID] [uniqueidentifier] NOT NULL,
        ///        [ActorRefId] [uniqueidentifier] NULL,
        ///        [RegistrationDate] [datetime] NOT NULL,
        ///        [BrokerUpdateDate] [datetim [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PersonRegistration {
            get {
                return ResourceManager.GetString("PersonRegistration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- =============================================
        ///-- Author:		Beemen Beshara
        ///-- Description:	Trigger for changes in PersonRegistration, 
        ///--  refreshes the cached serach table by calling InitializePersonSearchCache for 
        ///--  each record being inserted or updated
        ///-- =============================================
        ///
        ///IF EXISTS (SELECT * FROM sys.triggers where name=&apos;PersonRegistration_PopulateSearchCache&apos;)
        ///BEGIN
        ///	DROP TRIGGER dbo.PersonRegistration_PopulateSearchCache
        ///END
        ///GO
        ///
        ///CREATE TRIGGER dbo.PersonRe [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PersonRegistration_PopulateSearchCache {
            get {
                return ResourceManager.GetString("PersonRegistration_PopulateSearchCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- ========================================================
        ///-- Table  : PersonSearchCache
        ///-- ========================================================
        ///IF NOT EXISTS (SELECT * FROM sys.tables WHERE Name = &apos;PersonSearchCache&apos;)
        ///BEGIN
        ///    CREATE TABLE [dbo].[PersonSearchCache](
        ///	    -- Root fields
        ///        [PersonRegistrationId] [uniqueidentifier] NULL,
        ///	    [UUID] [uniqueidentifier] NOT NULL,		
        ///        LivscyklusKode NVARCHAR(MAX),
        ///            
        ///        -- Egenskab fields
        ///        AddressingName NVARCH [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PersonSearchCache {
            get {
                return ResourceManager.GetString("PersonSearchCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TypeId;TypeName;BatchSize;MaxRetry
        ///100;CprBroker.Providers.CPRDirect.ExtractStagingQueue, CprBroker.Providers.CPRDirect;1000;100
        ///101;CprBroker.Providers.CPRDirect.PartConversionQueue, CprBroker.Providers.CPRDirect;100;100
        ///200;CprBroker.Providers.CPRDirect.DbrBaseQueue, CprBroker.Providers.CPRDirect;100;100
        ///300;CprBroker.Providers.DPR.Queues.DprUpdateQueue, CprBroker.Providers.DPR;100;100
        ///.
        /// </summary>
        public static string Queue_Csv {
            get {
                return ResourceManager.GetString("Queue_Csv", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*
        ///    Trims strings that are used in address numbers
        ///    - Removed spaces and zeros from the left
        ///    - Removes spaces from the right
        ///*/
        ///
        ///IF EXISTS (SELECT * FROM sys.objects WHERE type = &apos;FN&apos; AND name = &apos;TrimAddressString&apos;)
        ///	DROP FUNCTION TrimAddressString
        ///GO
        ///
        ///CREATE FUNCTION TrimAddressString(@s VARCHAR(MAX))
        ///    RETURNS VARCHAR(MAX)
        ///AS
        ///BEGIN
        ///    DECLARE @i int, @l INT
        ///    SET @i = 0;
        ///	SET @l = LEN(@s)
        ///
        ///    WHILE SUBSTRING(@s,@i+1,1) in (&apos;0&apos;, &apos; &apos;) AND @i &lt; @l
        ///	    SET @i = @i + 1
        ///
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        public static string TrimAddressString {
            get {
                return ResourceManager.GetString("TrimAddressString", resourceCulture);
            }
        }
    }
}
