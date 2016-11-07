﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CprBroker.Providers.Local.Search.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CprBroker.Providers.Local.Search.Properties.Resources", typeof(Resources).Assembly);
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
        public static string InitializePersonSearchCache_Sql {
            get {
                return ResourceManager.GetString("InitializePersonSearchCache_Sql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- =============================================
        ///-- Author:		Beemen Beshara
        ///-- Description:	Trigger for record deletion in PersonRegistration, 
        ///--  Deleted the associated cache records if no other records are available
        ///-- =======================================================================
        ///
        ///IF EXISTS (SELECT * FROM sys.triggers where name=&apos;PersonRegistration_DeleteSearchCache&apos;)
        ///BEGIN
        ///	DROP TRIGGER dbo.PersonRegistration_DeleteSearchCache
        ///END
        ///GO
        ///
        ///CREATE TRIGGER dbo.PersonRegistration_DeleteSea [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PersonRegistration_DeleteSearchCache {
            get {
                return ResourceManager.GetString("PersonRegistration_DeleteSearchCache", resourceCulture);
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
        public static string PersonRegistration_PopulateSearchCache_Sql {
            get {
                return ResourceManager.GetString("PersonRegistration_PopulateSearchCache_Sql", resourceCulture);
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
        public static string PersonSearchCache_Sql {
            get {
                return ResourceManager.GetString("PersonSearchCache_Sql", resourceCulture);
            }
        }
    }
}