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
        // public static Microsoft.Extensions.Logging.ILoggingBuilder AddFormatter(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.Console.ILogFormatter> configure) { throw null; }
    }
    // public partial class DefaultConsoleLogFormatterConfigurationFactory : Microsoft.Extensions.Logging.IConsoleLogFormatterConfigurationFactory
    // {
    //     public DefaultConsoleLogFormatterConfigurationFactory() { }
    //     public Microsoft.Extensions.Logging.Console.ILogFormatter CreateJsonFormatter(System.Action<Microsoft.Extensions.Logging.Console.ILogFormatter> setup) { throw null; }
    // }
    // public static partial class FileLoggerExtensions
    // {
    //     public static Microsoft.Extensions.Logging.ILoggingBuilder AddConsoleLoggerX(this Microsoft.Extensions.Logging.ILoggingBuilder builder) { throw null; }
    //     public static Microsoft.Extensions.Logging.ILoggingBuilder AddFileLogger(this Microsoft.Extensions.Logging.ILoggingBuilder builder) { throw null; }
    //     public static Microsoft.Extensions.Logging.ILoggingBuilder AddFileLogger(this Microsoft.Extensions.Logging.ILoggingBuilder builder, System.Action<Microsoft.Extensions.Logging.FileLoggerOptions> configure) { throw null; }
    // }
    // public partial class FileLoggerOptions
    // {
    //     public FileLoggerOptions() { }
    //     public string Folder { get { throw null; } set { } }
    //     public Microsoft.Extensions.Logging.LogLevel LogLevel { get { throw null; } set { } }
    //     public int MaxFileSizeInMB { get { throw null; } set { } }
    //     public int RetainPolicyFileCount { get { throw null; } set { } }
    // }
    // public partial class FileLoggerProvider : Microsoft.Extensions.Logging.LoggerProvider
    // {
    //     public FileLoggerProvider() { }
    //     public FileLoggerProvider(Microsoft.Extensions.Logging.FileLoggerOptions Settings) { }
    //     public FileLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.FileLoggerOptions> Settings) { }
    //     protected override void Dispose(bool disposing) { }
    //     public override bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) { throw null; }
    //     public override void WriteLog(Microsoft.Extensions.Logging.LogEntry Info) { }
    // }
    // public partial interface IConsoleLogFormatterConfigurationFactory
    // {
    //     Microsoft.Extensions.Logging.Console.ILogFormatter CreateJsonFormatter(System.Action<Microsoft.Extensions.Logging.Console.ILogFormatter> setup);
    // }
    // public partial class LogEntry
    // {
    //     public static readonly string StaticHostName;
    //     public LogEntry() { }
    //     public string Category { get { throw null; } set { } }
    //     public Microsoft.Extensions.Logging.EventId EventId { get { throw null; } set { } }
    //     public System.Exception Exception { get { throw null; } set { } }
    //     public string HostName { get { throw null; } }
    //     public Microsoft.Extensions.Logging.LogLevel Level { get { throw null; } set { } }
    //     public System.Collections.Generic.List<Microsoft.Extensions.Logging.LogScopeInfo> Scopes { get { throw null; } set { } }
    //     public object State { get { throw null; } set { } }
    //     public System.Collections.Generic.Dictionary<string, object> StateProperties { get { throw null; } set { } }
    //     public string StateText { get { throw null; } set { } }
    //     public string Text { get { throw null; } set { } }
    //     public System.DateTime TimeStampUtc { get { throw null; } }
    //     public string UserName { get { throw null; } }
    // }
    // public abstract partial class LoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    // {
    //     protected System.IDisposable SettingsChangeToken;
    //     public LoggerProvider() { }
    //     public bool IsDisposed { get { throw null; } protected set { } }
    //     protected virtual void Dispose(bool disposing) { }
    //     ~LoggerProvider() { }
    //     public abstract bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel);
    //     Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerProvider.CreateLogger(string Category) { throw null; }
    //     void Microsoft.Extensions.Logging.ISupportExternalScope.SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    //     void System.IDisposable.Dispose() { }
    //     public abstract void WriteLog(Microsoft.Extensions.Logging.LogEntry Info);
    // }
    // public partial class LogScopeInfo
    // {
    //     public LogScopeInfo() { }
    //     public System.Collections.Generic.Dictionary<string, object> Properties { get { throw null; } set { } }
    //     public string Text { get { throw null; } set { } }
    // }
}
namespace Microsoft.Extensions.Logging.Console
{
    public enum ConsoleLoggerFormat
    {
        Default = 0,
        Systemd = 1,
        Compact = 2,
        Json = 3,
        Custom = 4,
    }
    public partial class ConsoleLoggerOptions
    {
        public ConsoleLoggerOptions() { }
        public bool DisableColors { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat Format { get { throw null; } set { } }
        public virtual string Formatter { get { throw null; } set { } }
        public bool IncludeScopes { get { throw null; } set { } }
        public System.Text.Json.JsonWriterOptions JsonWriterOptions { get { throw null; } set { } }
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get { throw null; } set { } }
        public string TimestampFormat { get { throw null; } set { } }
        public bool UseUtcTimestamp { get { throw null; } set { } }
    }
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("Console")]
    public partial class ConsoleLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider, Microsoft.Extensions.Logging.ISupportExternalScope, System.IDisposable
    {
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options, System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Console.ILogFormatter> formatters) { } //, Microsoft.Extensions.Logging.IConsoleLogFormatterConfigurationFactory formatterFactory) { }
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) { throw null; }
        public void Dispose() { }
        public void SetScopeProvider(Microsoft.Extensions.Logging.IExternalScopeProvider scopeProvider) { }
    }
    public partial interface ILogFormatter
    {
        string Name { get; }
        Microsoft.Extensions.Logging.Console.LogMessageEntry Format(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, string message, System.Exception exception, Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions options);
    }
    public partial class JsonConsoleLogFormatter : Microsoft.Extensions.Logging.Console.ILogFormatter
    {
        internal JsonConsoleLogFormatter() { }
        public string Name { get { throw null; } }
        public Microsoft.Extensions.Logging.Console.LogMessageEntry Format(Microsoft.Extensions.Logging.LogLevel logLevel, string logName, int eventId, string message, System.Exception exception, Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions options) { throw null; }
    }
    public readonly partial struct LogMessageEntry
    {
        // also control over how this gets written
        public readonly System.ConsoleColor? LevelBackground; //
        public readonly System.ConsoleColor? LevelForeground; //
        public readonly string LevelString; //
        public readonly bool LogAsError; //
        public readonly string Message; //
        public readonly System.ConsoleColor? MessageColor; //
        public readonly string TimeStamp; //
        public LogMessageEntry(string message, string timeStamp = null, string levelString = null, System.ConsoleColor? levelBackground = default(System.ConsoleColor?), System.ConsoleColor? levelForeground = default(System.ConsoleColor?), System.ConsoleColor? messageColor = default(System.ConsoleColor?), bool logAsError = false) { throw null; }
    }
}
