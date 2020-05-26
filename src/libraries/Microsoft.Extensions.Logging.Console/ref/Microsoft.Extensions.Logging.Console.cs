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
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsole(this Microsoft.Extensions.Logging.ILoggingBuilder builder, string formatterName) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder) where TFormatter : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatter where TOptions : class { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<TOptions> configure) where TFormatter : class, Microsoft.Extensions.Logging.Console.IConsoleLogFormatter where TOptions : class { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder UseColoredConsoleLogFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.ColoredConsoleLogFormatterOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder UseJsonConsoleLogFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.JsonConsoleLogFormatterOptions> configure) { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder UseSystemdConsoleLogFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions> configure) { throw null; }
    }
}
namespace Microsoft.Extensions.Logging.Console
{
    public partial class ColoredConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions
    {
        public ColoredConsoleLogFormatterOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public bool MultiLine { get { throw null; } set { } }
    }
    public static partial class ConsoleLogFormatterNames
    {
        public const string Colored = "colored";
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
        [System.ObsoleteAttribute("ConsoleLoggerOptions.LogToStandardErrorThreshold has been deprecated..", false)]
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.TimestampFormat has been deprecated..", false)]
        public string TimestampFormat { get { throw null; } set { } }
        [System.ObsoleteAttribute("ConsoleLoggerOptions.UseUtcTimestamp has been deprecated..", false)]
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("Console")]
    public partial class ConsoleLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    {
        [System.ObsoleteAttribute("ConsoleLoggerProvider.ctor has been deprecated.", false)]
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options) { }
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options, System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Console.IConsoleLogFormatter> formatters) { }
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) { throw null; }
        public void Dispose() { }
        public void SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    }
    public readonly partial struct ConsoleMessage
    {
        public readonly System.ConsoleColor? Background;
        public readonly System.ConsoleColor? Foreground;
        public readonly string Message;
        public ConsoleMessage(string message, System.ConsoleColor? background = default(System.ConsoleColor?), System.ConsoleColor? foreground = default(System.ConsoleColor?)) { throw null; }
    }
    public partial interface IConsoleLogFormatter
    {
        string Name { get; }
        Microsoft.Extensions.Logging.Console.LogMessageEntry Format<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider);
    }
    public partial class JsonConsoleLogFormatterOptions : Microsoft.Extensions.Logging.Console.SystemdConsoleLogFormatterOptions
    {
        public JsonConsoleLogFormatterOptions() { }
        public System.Text.Json.JsonWriterOptions JsonWriterOptions { get { throw null; } set { } }
    }
    public readonly partial struct LogMessageEntry
    {
        public readonly bool LogAsError;
        public readonly Microsoft.Extensions.Logging.Console.ConsoleMessage[] Messages;
        public LogMessageEntry(Microsoft.Extensions.Logging.Console.ConsoleMessage[] messages, bool logAsError = false) { throw null; }
    }
    public partial class SystemdConsoleLogFormatterOptions
    {
        public SystemdConsoleLogFormatterOptions() { }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
}
