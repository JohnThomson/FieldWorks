﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LanguageExplorer.UtilityTools {
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
    internal class TransductionSampleStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TransductionSampleStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LanguageExplorer.UtilityTools.TransductionSampleStrings", typeof(TransductionSampleStrings).Assembly);
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
        ///   Looks up a localized string similar to Could not locate the writing system based on the locale &apos;{0}&apos;..
        /// </summary>
        internal static string ksCannotLocateWsForX {
            get {
                return ResourceManager.GetString("ksCannotLocateWsForX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot use &apos;Undo&apos; to cancel the effect of this utility. You would need to go back to a previously saved version of the database..
        /// </summary>
        internal static string ksCannotUndoTransducingCitForms {
            get {
                return ResourceManager.GetString("ksCannotUndoTransducingCitForms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is a trivial demonstration of using Python to generate the contents of a field. It is extremely limited.  It grabs the default vernacular alternative of the citation form of every entry, runs it through a Python script that you supply, and then places the output in a different writing system alternative of the citation form. It is currently implemented in a very inefficient way; it actually runs the Python script once for every entry in the database. This utility will run the Python script found at (yo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ksDemoOfUsingPython {
            get {
                return ResourceManager.GetString("ksDemoOfUsingPython", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was an error: {0}
        ///{1}.
        /// </summary>
        internal static string ksErrorMsgWithStackTrace {
            get {
                return ResourceManager.GetString("ksErrorMsgWithStackTrace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transduce Citation Forms.
        /// </summary>
        internal static string ksTransduceCitationForms {
            get {
                return ResourceManager.GetString("ksTransduceCitationForms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This can be used to convert Citation Forms into another writing system..
        /// </summary>
        internal static string ksWhenToTransduceCitForms {
            get {
                return ResourceManager.GetString("ksWhenToTransduceCitForms", resourceCulture);
            }
        }
    }
}