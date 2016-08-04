﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jhu.Graywulf.Install {
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
    internal class Scripts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Scripts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Jhu.Graywulf.Install.Scripts", typeof(Scripts).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --USE [Graywulf_Log]
        ///GO
        ////****** Object:  Schema [dev]    Script Date: 09/25/2013 17:23:22 ******/
        ///CREATE SCHEMA [dev] AUTHORIZATION [dbo]
        ///GO
        ////****** Object:  Table [dbo].[EventData]    Script Date: 09/25/2013 17:23:24 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///CREATE TABLE [dbo].[EventData](
        ///	[EventId] [bigint] NOT NULL,
        ///	[Key] [nvarchar](50) NOT NULL,
        ///	[Data] [sql_variant] NOT NULL,
        /// CONSTRAINT [PK_EventLogData] PRIMARY KEY CLUSTERED 
        ///(
        ///	[EventId] ASC,
        ///	[Key] ASC
        ///)WITH (PAD_ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Jhu_Graywulf_Logging {
            get {
                return ResourceManager.GetString("Jhu_Graywulf_Logging", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE ASSEMBLY [Jhu.Graywulf.Registry.Enum]
        ///AUTHORIZATION [dbo]
        ///FROM [$Hex]
        ///WITH PERMISSION_SET = SAFE
        ///
        ///GO
        ///.
        /// </summary>
        internal static string Jhu_Graywulf_Registry_Assembly {
            get {
                return ResourceManager.GetString("Jhu_Graywulf_Registry_Assembly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- ENABLE CLR
        ///
        ///-- sp_configure &apos;show advanced options&apos;, 1;
        ///-- GO
        ///-- RECONFIGURE;
        ///-- GO
        ///-- sp_configure &apos;clr enabled&apos;, 1;
        ///-- GO
        ///-- RECONFIGURE;
        ///-- GO
        ///
        ///-- USER DEFINE TYPES --
        ///
        ///CREATE TYPE [dbo].[DeploymentState]
        ///EXTERNAL NAME [Jhu.Graywulf.Registry.Enum].[Jhu.Graywulf.Registry.Sql.DeploymentState]
        ///
        ///GO
        ///
        ///
        ///CREATE TYPE [dbo].[JobExecutionState]
        ///EXTERNAL NAME [Jhu.Graywulf.Registry.Enum].[Jhu.Graywulf.Registry.Sql.JobExecutionState]
        ///
        ///GO
        ///
        ///
        ///CREATE TYPE [dbo].[RunningState]
        ///EXTERNAL NAME [J [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Jhu_Graywulf_Registry_Logic {
            get {
                return ResourceManager.GetString("Jhu_Graywulf_Registry_Logic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF (OBJECT_ID(&apos;[dbo].[Entity]&apos;) IS NOT NULL)
        ///DROP TABLE [dbo].[Entity]
        ///
        ///GO
        ///
        ///CREATE TABLE [dbo].[Entity]
        ///(
        ///	[Guid] [uniqueidentifier] NOT NULL,
        ///	[ConcurrencyVersion] [timestamp] NOT NULL,
        ///	[ParentGuid] [uniqueidentifier] NOT NULL,
        ///	[EntityType] [int] NOT NULL,
        ///	[Number] [int] NOT NULL,
        ///	[Name] [nvarchar](128) NOT NULL,
        ///	[Version] [nvarchar](25) NOT NULL,
        ///	[System] [bit] NOT NULL,
        ///	[Hidden] [bit] NOT NULL,
        ///	[ReadOnly] [bit] NOT NULL,
        ///	[Primary] [bit] NOT NULL,
        ///	[Deleted] [bit] NOT NULL,
        ///	[L [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Jhu_Graywulf_Registry_Tables {
            get {
                return ResourceManager.GetString("Jhu_Graywulf_Registry_Tables", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;configuration&gt;
        ///  &lt;system.web&gt;
        ///    &lt;authentication mode=&quot;Forms&quot;&gt;
        ///      &lt;forms name=&quot;.ASPXFORMSAUTH&quot; loginUrl=&quot;/gwauth/SignIn.aspx&quot; enableCrossAppRedirects=&quot;true&quot; path=&quot;/&quot; /&gt;
        ///    &lt;/authentication&gt;
        ///    &lt;machineKey validation=&quot;SHA1&quot; decryption=&quot;AES&quot; /&gt;
        ///  &lt;/system.web&gt;
        ///&lt;/configuration&gt;.
        /// </summary>
        internal static string web_config {
            get {
                return ResourceManager.GetString("web.config", resourceCulture);
            }
        }
    }
}
