﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Extensions.Logging.Generators {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Extensions.Logging.Generators.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Argument {0} is not referenced from the logging message.
        /// </summary>
        internal static string ArgumentHasNoCorrespondingTemplateMessage {
            get {
                return ResourceManager.GetString("ArgumentHasNoCorrespondingTemplateMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument is not referenced from the logging message.
        /// </summary>
        internal static string ArgumentHasNoCorrespondingTemplateTitle {
            get {
                return ResourceManager.GetString("ArgumentHasNoCorrespondingTemplateTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t include a template for {0} in the logging message since it is implicitly taken care.
        /// </summary>
        internal static string DontMentionExceptionInMessageMessage {
            get {
                return ResourceManager.GetString("DontMentionExceptionInMessageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t include exception parameters as templates in the logging message.
        /// </summary>
        internal static string DontMentionExceptionInMessageTitle {
            get {
                return ResourceManager.GetString("DontMentionExceptionInMessageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t include a template for {0} in the logging message since it is implicitly taken care of.
        /// </summary>
        internal static string DontMentionLogLevelInMessageMessage {
            get {
                return ResourceManager.GetString("DontMentionLogLevelInMessageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t include log level parameters as templates in the logging message..
        /// </summary>
        internal static string DontMentionLogLevelInMessageTitle {
            get {
                return ResourceManager.GetString("DontMentionLogLevelInMessageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple logging messages are using event id {0}.
        /// </summary>
        internal static string ErrorEventIdReuseMessage {
            get {
                return ResourceManager.GetString("ErrorEventIdReuseMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple logging messages cannot use the same event id.
        /// </summary>
        internal static string ErrorEventIdReuseTitle {
            get {
                return ResourceManager.GetString("ErrorEventIdReuseTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The first argument to a logging method must implement the Microsoft.Extensions.Logging.ILogger interface.
        /// </summary>
        internal static string ErrorFirstArgMustBeILoggerMessage {
            get {
                return ResourceManager.GetString("ErrorFirstArgMustBeILoggerMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The first argument to a logging method must implement the Microsoft.Extensions.Logging.ILogger interface.
        /// </summary>
        internal static string ErrorFirstArgMustBeILoggerTitle {
            get {
                return ResourceManager.GetString("ErrorFirstArgMustBeILoggerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing message for logging method {0}.
        /// </summary>
        internal static string ErrorInvalidMessageMessage {
            get {
                return ResourceManager.GetString("ErrorInvalidMessageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing message for logging method.
        /// </summary>
        internal static string ErrorInvalidMessageTitle {
            get {
                return ResourceManager.GetString("ErrorInvalidMessageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging method names cannot start with _.
        /// </summary>
        internal static string ErrorInvalidMethodNameMessage {
            get {
                return ResourceManager.GetString("ErrorInvalidMethodNameMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging method names cannot start with _.
        /// </summary>
        internal static string ErrorInvalidMethodNameTitle {
            get {
                return ResourceManager.GetString("ErrorInvalidMethodNameTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods must return void.
        /// </summary>
        internal static string ErrorInvalidMethodReturnTypeMessage {
            get {
                return ResourceManager.GetString("ErrorInvalidMethodReturnTypeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods must return void.
        /// </summary>
        internal static string ErrorInvalidMethodReturnTypeTitle {
            get {
                return ResourceManager.GetString("ErrorInvalidMethodReturnTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging method parameter names cannot start with _.
        /// </summary>
        internal static string ErrorInvalidParameterNameMessage {
            get {
                return ResourceManager.GetString("ErrorInvalidParameterNameMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging method parameter names cannot start with _.
        /// </summary>
        internal static string ErrorInvalidParameterNameTitle {
            get {
                return ResourceManager.GetString("ErrorInvalidParameterNameTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods cannot have a body.
        /// </summary>
        internal static string ErrorMethodHasBodyMessage {
            get {
                return ResourceManager.GetString("ErrorMethodHasBodyMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods cannot have a body.
        /// </summary>
        internal static string ErrorMethodHasBodyTitle {
            get {
                return ResourceManager.GetString("ErrorMethodHasBodyTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods cannot be generic.
        /// </summary>
        internal static string ErrorMethodIsGenericMessage {
            get {
                return ResourceManager.GetString("ErrorMethodIsGenericMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods cannot be generic.
        /// </summary>
        internal static string ErrorMethodIsGenericTitle {
            get {
                return ResourceManager.GetString("ErrorMethodIsGenericTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A LogLevel value must be supplied in the LoggerMessage attribute or as a parameter to the logging method.
        /// </summary>
        internal static string ErrorMissingLogLevelMessage {
            get {
                return ResourceManager.GetString("ErrorMissingLogLevelMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A LogLevel value must be supplied in the LoggerMessage attribute or as a parameter to the logging method.
        /// </summary>
        internal static string ErrorMissingLogLevelTitle {
            get {
                return ResourceManager.GetString("ErrorMissingLogLevelTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find definition for type {0}.
        /// </summary>
        internal static string ErrorMissingRequiredTypeMessage {
            get {
                return ResourceManager.GetString("ErrorMissingRequiredTypeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a required type definition.
        /// </summary>
        internal static string ErrorMissingRequiredTypeTitle {
            get {
                return ResourceManager.GetString("ErrorMissingRequiredTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging class cannot be in nested types.
        /// </summary>
        internal static string ErrorNestedTypeMessage {
            get {
                return ResourceManager.GetString("ErrorNestedTypeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging class cannot be in nested types.
        /// </summary>
        internal static string ErrorNestedTypeTitle {
            get {
                return ResourceManager.GetString("ErrorNestedTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods must be partial.
        /// </summary>
        internal static string ErrorNotPartialMethodMessage {
            get {
                return ResourceManager.GetString("ErrorNotPartialMethodMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods must be partial.
        /// </summary>
        internal static string ErrorNotPartialMethodTitle {
            get {
                return ResourceManager.GetString("ErrorNotPartialMethodTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods must be static.
        /// </summary>
        internal static string ErrorNotStaticMethodMessage {
            get {
                return ResourceManager.GetString("ErrorNotStaticMethodMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging methods must be static.
        /// </summary>
        internal static string ErrorNotStaticMethodTitle {
            get {
                return ResourceManager.GetString("ErrorNotStaticMethodTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No need to supply a DateTime value as argument since the logging infrastructure inserts one implicitly.
        /// </summary>
        internal static string PassingDateTimeMessage {
            get {
                return ResourceManager.GetString("PassingDateTimeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No need to supply a DateTime value as argument since the logging infrastructure inserts one implicitly.
        /// </summary>
        internal static string PassingDateTimeTitle {
            get {
                return ResourceManager.GetString("PassingDateTimeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove redundant qualifier (Info:, Warning:, Error:, etc) from the logging message since it is implicit in the specified log level..
        /// </summary>
        internal static string RedundantQualifierInMessageMessage {
            get {
                return ResourceManager.GetString("RedundantQualifierInMessageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Redundant qualifier in logging message.
        /// </summary>
        internal static string RedundantQualifierInMessageTitle {
            get {
                return ResourceManager.GetString("RedundantQualifierInMessageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Template {0} is not provided as argument to the logging method.
        /// </summary>
        internal static string TemplateHasNoCorrespondingArgumentMessage {
            get {
                return ResourceManager.GetString("TemplateHasNoCorrespondingArgumentMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging template has no corresponding method argument.
        /// </summary>
        internal static string TemplateHasNoCorrespondingArgumentTitle {
            get {
                return ResourceManager.GetString("TemplateHasNoCorrespondingArgumentTitle", resourceCulture);
            }
        }
    }
}
