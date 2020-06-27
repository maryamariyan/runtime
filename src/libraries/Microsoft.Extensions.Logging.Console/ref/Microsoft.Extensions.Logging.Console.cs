// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Extensions.Logging
{
    public static partial class ConsoleLoggerExtensions
    {
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsole(this Microsoft.Extensions.Logging.ILoggingBuilder builder) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsole(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder) where TFormatter : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatter where TOptions : Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<TOptions> configure) where TFormatter : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatter where TOptions : Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddDefaultConsoleLogFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.DefaultConsoleLogFormatterOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddJsonConsoleLogFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.JsonConsoleLogFormatterOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddSystemdConsoleLogFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions> configure) { throw null; }
    }
}
namespace Microsoft.Extensions.Logging.Console
{
    public static partial class ConsoleLogFormatterNames
    {
        public const string Default = "default";
        public const string Json = "json";
        public const string Systemd = "systemd";
    }
    [System.ObsoleteAttribute("ConsoleLoggerFormat has been deprecated.", false)]
    public enum ConsoleLoggerFormat
    {
        Default = 0,
        Systemd = 1,
    }
    public partial class ConsoleLoggerOptions
    {
        public ConsoleLoggerOptions() { }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.DisableColors has been deprecated. Please use ColoredConsoleLogFormatterOptions.DisableColors instead.", false)]
        public bool DisableColors { get { throw null; } set { } }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.Format has been deprecated. Please use ConsoleLoggerOptions.FormatterName instead.", false)]
        public Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat Format { get { throw null; } set { } }
        public string FormatterName { get { throw null; } set { } }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.IncludeScopes has been deprecated..", false)]
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.TimestampFormat has been deprecated..", false)]
        public string TimestampFormat { get { throw null; } set { } }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.UseUtcTimestamp has been deprecated..", false)]
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("Console")]
    public partial class ConsoleLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    {
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options) { }
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options, System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Console.IConsoleLogFormatter> formatters) { }
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) { throw null; }
        public void Dispose() { }
        public void SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    }
    public partial class DefaultConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions
    {
        public DefaultConsoleLogFormatterOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public bool MultiLine { get { throw null; } set { } }
    }
    public partial interface IConsoleLogFormatter
    {
        string Name { get; }
        void Write<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string category, Microsoft.Extensions.Logging.EventId eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider, System.IO.TextWriter textWriter);
    }
    public partial class JsonConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions
    {
        public JsonConsoleLogFormatterOptions() { }
        public System.Text.Json.JsonWriterOptions JsonWriterOptions { get { throw null; } set { } }
    }
    public partial class SystemdConsoleLogFormatterOptions
    {
        public SystemdConsoleLogFormatterOptions() { }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
        public bool FindKeyValuePairsInLog { get { throw null; } set { } }
    }
}
