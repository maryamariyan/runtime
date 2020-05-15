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
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder) where TFormatter : class, Microsoft.Extensions.Logging.Console.ILogFormatter where TOptions : class { throw null; }
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddLogFormatter<TFormatter, TOptions>(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<TOptions> configure) where TFormatter : class, Microsoft.Extensions.Logging.Console.ILogFormatter where TOptions : class { throw null; }
    }
}
namespace Microsoft.Extensions.Logging.Console
{
    public partial class CompactLogFormatterOptions
    {
        public CompactLogFormatterOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    public enum ConsoleLoggerFormat
    {
        Default = 0,
        Systemd = 1,
    }
    public partial class ConsoleLoggerOptions
    {
        public ConsoleLoggerOptions() { }
        public Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat Format { get { throw null; } set { } }
        public string Formatter { get { throw null; } set { } }
    }
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("Console")]
    public partial class ConsoleLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    {
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options, System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Console.ILogFormatter> formatters) { }
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) { throw null; }
        public void Dispose() { }
        public void SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    }
    public readonly partial struct ConsoleMessage
    {
        public readonly System.ConsoleColor? Background;
        public readonly string Content;
        public readonly System.ConsoleColor? Foreground;
        public ConsoleMessage(string message, System.ConsoleColor? background = default(System.ConsoleColor?), System.ConsoleColor? foreground = default(System.ConsoleColor?)) { throw null; }
    }
    public partial class DefaultLogFormatter : Microsoft.Extensions.Logging.Console.ILogFormatter, System.IDisposable
    {
        public DefaultLogFormatter(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.DefaultLogFormatterOptions> options) { }
        public Microsoft.Extensions.Logging.Console.DefaultLogFormatterOptions FormatterOptions { get { throw null; } set { } }
        public string Name { get { throw null; } }
        public void Dispose() { }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, string message, System.Exception exception, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { throw null; }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { throw null; }
    }
    public partial class DefaultLogFormatterOptions
    {
        public DefaultLogFormatterOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    public partial interface IConsole
    {
        void Flush();
        void Write(string message, System.ConsoleColor? background, System.ConsoleColor? foreground);
        void WriteLine(string message, System.ConsoleColor? background, System.ConsoleColor? foreground);
    }
    public partial interface ILogFormatter
    {
        string Name { get; }
        Microsoft.Extensions.Logging.Console.LogMessageEntry Format(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, string message, System.Exception exception, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider);
        Microsoft.Extensions.Logging.Console.LogMessageEntry Format<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider);
    }
    public partial class JsonConsoleLogFormatter : Microsoft.Extensions.Logging.Console.ILogFormatter, System.IDisposable
    {
        public JsonConsoleLogFormatter(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.JsonLogFormatterOptions> options) { }
        public Microsoft.Extensions.Logging.Console.JsonLogFormatterOptions FormatterOptions { get { throw null; } set { } }
        public string Name { get { throw null; } }
        public void Dispose() { }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, string message, System.Exception exception, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { throw null; }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { throw null; }
    }
    public partial class JsonLogFormatterOptions
    {
        public JsonLogFormatterOptions() { }
        public bool IncludeScopes { get { throw null; } set { } }
        public System.Text.Json.JsonSerializerOptions JsonSerializerOptions { get { throw null; } set { } }
        public System.Text.Json.JsonWriterOptions JsonWriterOptions { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    public readonly partial struct LogMessageEntry
    {
        public readonly bool LogAsError;
        public readonly Microsoft.Extensions.Logging.Console.ConsoleMessage[] Messages;
        public LogMessageEntry(Microsoft.Extensions.Logging.Console.ConsoleMessage[] messages, bool logAsError = false) { throw null; }
    }
    public partial class SystemdLogFormatter : Microsoft.Extensions.Logging.Console.ILogFormatter, System.IDisposable
    {
        public SystemdLogFormatter(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.SystemdLogFormatterOptions> options) { }
        public Microsoft.Extensions.Logging.Console.SystemdLogFormatterOptions FormatterOptions { get { throw null; } set { } }
        public string Name { get { throw null; } }
        public void Dispose() { }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, string message, System.Exception exception, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { throw null; }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter, Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { throw null; }
    }
    public partial class SystemdLogFormatterOptions
    {
        public SystemdLogFormatterOptions() { }
        public bool IncludeScopes { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
}
