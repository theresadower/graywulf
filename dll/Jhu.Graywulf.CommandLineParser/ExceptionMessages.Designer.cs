﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jhu.Graywulf.CommandLineParser {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Jhu.Graywulf.CommandLineParser.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to Option type is not bool: &apos;{0}&apos; on type &apos;{1}&apos;..
        /// </summary>
        internal static string OptionNotBool {
            get {
                return ResourceManager.GetString("OptionNotBool", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambigous parameter or option &apos;{0}&apos;..
        /// </summary>
        internal static string ParameterAmbigous {
            get {
                return ResourceManager.GetString("ParameterAmbigous", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate parameter or option: &apos;{0}&apos;..
        /// </summary>
        internal static string ParameterDuplicate {
            get {
                return ResourceManager.GetString("ParameterDuplicate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate parameter or option &apos;{0}&apos; specified on type &apos;{1}&apos;..
        /// </summary>
        internal static string ParameterDuplicateName {
            get {
                return ResourceManager.GetString("ParameterDuplicateName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter or option expected instead of &apos;{0}&apos;..
        /// </summary>
        internal static string ParameterExpected {
            get {
                return ResourceManager.GetString("ParameterExpected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid parameter: &apos;{0}&apos;..
        /// </summary>
        internal static string ParameterInvalid {
            get {
                return ResourceManager.GetString("ParameterInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter or option required: &apos;{0}&apos;..
        /// </summary>
        internal static string ParameterRequired {
            get {
                return ResourceManager.GetString("ParameterRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value of parameter &apos;{0}&apos; required..
        /// </summary>
        internal static string ParameterValueRequired {
            get {
                return ResourceManager.GetString("ParameterValueRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported type &apos;{0}&apos; of parameter &apos;{1}&apos; on type &apos;{2}&apos;. Only Enums and types implementing the Parse method are supported..
        /// </summary>
        internal static string UnsupportedType {
            get {
                return ResourceManager.GetString("UnsupportedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verb attribute specified more than once or not specified at all on type &apos;{0}&apos;..
        /// </summary>
        internal static string VerbAttributeError {
            get {
                return ResourceManager.GetString("VerbAttributeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate verb specified on type &apos;{0}&apos;..
        /// </summary>
        internal static string VerbDuplicate {
            get {
                return ResourceManager.GetString("VerbDuplicate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid verb: &apos;{0}&apos;..
        /// </summary>
        internal static string VerbInvalid {
            get {
                return ResourceManager.GetString("VerbInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verb required..
        /// </summary>
        internal static string VerbRequired {
            get {
                return ResourceManager.GetString("VerbRequired", resourceCulture);
            }
        }
    }
}
