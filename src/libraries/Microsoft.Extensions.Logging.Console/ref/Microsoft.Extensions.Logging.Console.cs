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
        [System.ObsoleteAttribute("deprecated.", false)]
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsole(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsole(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.FormattedConsoleLoggerOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsole(this Microsoft.Extensions.Logging.ILoggingBuilder builder, string formatterName) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder) where TFormatter : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatter where TOptions : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<TOptions> configure) where TFormatter : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatter where TOptions : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions { throw null; }
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
    [System.ObsoleteAttribute("deprecated.", false)]
    public partial class ConsoleLoggerOptions
    {
        public ConsoleLoggerOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat Format { get { throw null; } set { } }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("Console")]
    [System.ObsoleteAttribute("deprecated.", false)]
    public partial class ConsoleLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    {
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options) { }
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) { throw null; }
        public void Dispose() { }
        public void SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    }
    public partial class DefaultConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions
    {
        public DefaultConsoleLogFormatterOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public bool MultiLine { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    public partial class FormattedConsoleLoggerOptions
    {
        public FormattedConsoleLoggerOptions() { }
        public string FormatterName { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions FormatterOptions { get { throw null; } set { } }
    }
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("FormattedConsole")]
    public partial class FormattedConsoleLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    {
        public FormattedConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.FormattedConsoleLoggerOptions> options, System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Console.IConsoleLogFormatter> formatters) { }
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) { throw null; }
        public void Dispose() { }
        public void SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    }
    public partial interface IConsoleLogFormatter
    {
        string Name { get; }
        void Format<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, Microsoft.Extensions.Logging.EventId eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider, Microsoft.Extensions.Logging.Console.IConsoleMessageBuilder consoleMessageBuilder);
        Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions UpdateWith(Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions options);
    }
    public partial interface IConsoleLogFormatterOptions
    {
        bool IncludeScopes { get; set; }
        Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get; set; }
        string TimestampFormat { get; set; }
        bool UseUtcTimestamp { get; set; }
    }
    public partial interface IConsoleMessageBuilder
    {
        bool LogAsError { get; set; }
        Microsoft.Extensions.Logging.Console.IConsoleMessageBuilder Append(string message);
        Microsoft.Extensions.Logging.Console.IConsoleMessageBuilder Build();
        void Clear();
        Microsoft.Extensions.Logging.Console.IConsoleMessageBuilder ResetColor();
        Microsoft.Extensions.Logging.Console.IConsoleMessageBuilder SetColor(System.ConsoleColor? background, System.ConsoleColor? foreground);
    }
    public partial class JsonConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions
    {
        public JsonConsoleLogFormatterOptions() { }
        public bool IncludeScopes { get { throw null; } set { } }
        public System.Text.Json.JsonWriterOptions JsonWriterOptions { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    public partial class SystemdConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.IConsoleLogFormatterOptions
    {
        public SystemdConsoleLogFormatterOptions() { }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
}
