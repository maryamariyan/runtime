// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class OptionsBuilderDataAnnotationsExtensions
    {
        public static Microsoft.Extensions.Options.OptionsBuilder<TOptions> ValidateDataAnnotations<TOptions>(this Microsoft.Extensions.Options.OptionsBuilder<TOptions> optionsBuilder) where TOptions : class { throw null; }
    }
    public static partial class OptionsBuilderValidationExtensions
    {
        public static Microsoft.Extensions.Options.OptionsBuilder<TOptions> ValidateDataAnnotationsEagerly<TOptions>(this Microsoft.Extensions.Options.OptionsBuilder<TOptions> optionsBuilder) where TOptions : class, new() { throw null; }
        public static Microsoft.Extensions.Options.OptionsBuilder<TOptions> ValidateEagerly<TOptions>(this Microsoft.Extensions.Options.OptionsBuilder<TOptions> optionsBuilder) where TOptions : class, new() { throw null; }
    }
    public partial class ReadableOptionsValidationException : System.Exception
    {
        public ReadableOptionsValidationException(Microsoft.Extensions.Options.OptionsValidationException e) { }
        public System.Collections.Generic.IReadOnlyCollection<string> Failures { get { throw null; } }
        public override string Message { get { throw null; } }
        public System.Type OptionsType { get { throw null; } }
    }
}
namespace Microsoft.Extensions.Options
{
    public partial class DataAnnotationValidateOptions<TOptions> : Microsoft.Extensions.Options.IValidateOptions<TOptions> where TOptions : class
    {
        public DataAnnotationValidateOptions(string name) { }
        public string Name { get { throw null; } }
        public Microsoft.Extensions.Options.ValidateOptionsResult Validate(string name, TOptions options) { throw null; }
    }
}
