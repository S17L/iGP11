﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iGP11.Tool.Application.Bootstrapper {
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
    internal class Configuration {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Configuration() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iGP11.Tool.Application.Bootstrapper.Configuration", typeof(Configuration).Assembly);
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
        ///   Looks up a localized string similar to {
        ///  &quot;applicationListenerUri&quot;: &quot;http://{0}:{1}&quot;,
        ///  &quot;databaseFilePath&quot;: &quot;db\\iGP11.db&quot;,
        ///  &quot;encryptionKey&quot;: &quot;tyUid7QsdmoLa31d&quot;,
        ///  &quot;feedbackEmailAddress&quot;: &quot;igp11.feedback@gmail.com&quot;,
        ///  &quot;iconsUri&quot;: &quot;https://icons8.com/&quot;,
        ///  &quot;maxDegreeOfParallelism&quot;: &quot;8&quot;,
        ///  &quot;proxySettingsFileName&quot;: &quot;settings.igp&quot;,
        ///  &quot;systemIpAddress&quot;: &quot;127.0.0.1&quot;,
        ///  &quot;modules&quot;: {
        ///    &quot;proxy&quot;: &quot;iGP11.dll&quot;,
        ///    &quot;direct3D11&quot;: &quot;iGP11.Direct3D11.dll&quot;
        ///  }
        ///}
        ///.
        /// </summary>
        internal static string ConstantSettings {
            get {
                return ResourceManager.GetString("ConstantSettings", resourceCulture);
            }
        }
    }
}
