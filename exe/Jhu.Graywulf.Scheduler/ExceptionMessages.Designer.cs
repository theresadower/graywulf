﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jhu.Graywulf.Scheduler {
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
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Jhu.Graywulf.Scheduler.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to Error loading workflow type. The workflow assembly might be at an inaccessible location..
        /// </summary>
        internal static string ErrorLoadingWorkflowType {
            get {
                return ResourceManager.GetString("ErrorLoadingWorkflowType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error parsing assembly name from type name, type cannot be loaded: &apos;{0}&apos;..
        /// </summary>
        internal static string ErrorParsingTypeName {
            get {
                return ResourceManager.GetString("ErrorParsingTypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No server with the required database found..
        /// </summary>
        internal static string NoServerForDatabaseFound {
            get {
                return ResourceManager.GetString("NoServerForDatabaseFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Poller already running..
        /// </summary>
        internal static string PollerHasAlreadyStarted {
            get {
                return ResourceManager.GetString("PollerHasAlreadyStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Poller has not been started yet..
        /// </summary>
        internal static string PollerHasNotStarted {
            get {
                return ResourceManager.GetString("PollerHasNotStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sanity check failed. Check configuration settings..
        /// </summary>
        internal static string SanityCheckFailed {
            get {
                return ResourceManager.GetString("SanityCheckFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Workflow host has been already started..
        /// </summary>
        internal static string WorkflowHostHasAlreadyStarted {
            get {
                return ResourceManager.GetString("WorkflowHostHasAlreadyStarted", resourceCulture);
            }
        }
    }
}
