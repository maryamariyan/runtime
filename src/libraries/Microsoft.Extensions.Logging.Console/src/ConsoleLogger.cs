// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text;
#pragma warning disable CS0618

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLogger : ILogger
    {
        private const string LoglevelPadding = ": ";
        private static readonly string _messagePadding = new string(' ', GetLogLevelString(LogLevel.Information).Length + LoglevelPadding.Length);
        private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;

        // ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? DefaultConsoleColor = null;

        private readonly string _name;
        private readonly ConsoleLoggerProcessor _queueProcessor;

        [ThreadStatic]
        private static StringBuilder _logBuilder;

        internal ConsoleLogger(string name, ConsoleLoggerProcessor loggerProcessor)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
            _queueProcessor = loggerProcessor;
        }

        internal IConsoleLogFormatter Formatter { get; set; }
        internal IExternalScopeProvider ScopeProvider { get; set; }

        internal ConsoleLoggerOptions Options { get; set; }

        [ThreadStatic]
        internal static StringWriter _stringWriter;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                WriteMessage(logLevel, _name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            ConsoleLoggerFormat format = Options.Format;
            Debug.Assert(format >= ConsoleLoggerFormat.Default && format <= ConsoleLoggerFormat.Systemd);

            StringBuilder logBuilder = _logBuilder;
            _logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            if (_stringWriter == null)
            {
                _stringWriter = new StringWriter();
            }

            LogMessageEntry entry;
            if (format == ConsoleLoggerFormat.Default)
            {
                Format(logLevel, logName, eventId, message, exception, ScopeProvider, _stringWriter);
                entry = new LogMessageEntry(_stringWriter.ComputeAnsiString(), logAsError: logLevel >= Options.LogToStandardErrorThreshold);
            }
            else if (format == ConsoleLoggerFormat.Systemd)
            {
                entry = CreateSystemdLogMessage(logBuilder, logLevel, logName, eventId, message, exception);
            }
            else
            {
                entry = default;
            }
            _queueProcessor.EnqueueMessage(entry);

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _logBuilder = logBuilder;
        }

        private void Format(LogLevel logLevel, string category, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, StringWriter stringWriter, bool bugFixBetterFormatExceptionMessage = false)
        {
            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received
            ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
            string logLevelString = GetLogLevelString(logLevel);

            string timestamp = null;
            string timestampFormat = Options.TimestampFormat;
            if (timestampFormat != null)
            {
                DateTime dateTime = GetCurrentDateTime();
                timestamp = dateTime.ToString(timestampFormat);
            }
            stringWriter.DisableColors = Options.DisableColors;
            if (timestamp != null)
            {
                stringWriter.Write(timestamp);
            }
            if (logLevelString != null)
            {
                stringWriter.SetBackgroundColor(logLevelColors.Background);
                stringWriter.SetForegroundColor(logLevelColors.Foreground);
                stringWriter.Write(logLevelString);
                stringWriter.ResetColor();
            }
            // category and event id
            stringWriter.Write(LoglevelPadding + category + "[" + eventId + "]");
            stringWriter.Write(Environment.NewLine);

            // scope information
            GetScopeInformation(stringWriter, scopeProvider);

            if (!string.IsNullOrEmpty(message))
            {
                // message
                stringWriter.Write(_messagePadding);
                stringWriter.WriteReplacing(Environment.NewLine, _newLineWithMessagePadding, message);
                stringWriter.Write(Environment.NewLine);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                if (bugFixBetterFormatExceptionMessage)
                {
                    stringWriter.Write(_messagePadding);
                    stringWriter.WriteReplacing(Environment.NewLine, _newLineWithMessagePadding, exception.ToString());
                }
                else
                {
                    stringWriter.Write(exception.ToString());
                }
                stringWriter.Write(Environment.NewLine);
            }
        }

        private void GetScopeInformation(StringWriter stringWriter, IExternalScopeProvider scopeProvider, bool multiLine = true, bool setColorForScope = false)
        {
            if (Options.IncludeScopes && scopeProvider != null)
            {
                int initialLength = stringWriter.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    (StringWriter writer, int paddAt) = state;
                    bool padd = paddAt == writer.Length;
                    if (padd)
                    {
                        writer.Write(_messagePadding);
                        writer.Write("=> ");
                    }
                    else
                    {
                        writer.Write(" => ");
                    }
                    if (setColorForScope)
                    {
                        writer.SetForegroundColor(ConsoleColor.White);
                        writer.Write(scope.ToString());
                        writer.SetForegroundColor(null);
                    }
                    else
                    {
                        writer.Write(scope.ToString());
                    }
                }, (stringWriter, multiLine ? initialLength : -1));

                if (stringWriter.Length > initialLength && multiLine)
                {
                    stringWriter.Write(Environment.NewLine);
                }
            }
        }

        private LogMessageEntry CreateDefaultLogMessage(StringBuilder logBuilder, LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            // Example:
            // info: ConsoleApp.Program[10]
            //       Request received

            ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);

            // timestamp
            string timestamp = null;
            string timestampFormat = Options.TimestampFormat;
            if (timestampFormat != null)
            {
                DateTime dateTime = GetCurrentDateTime();
                timestamp = dateTime.ToString(timestampFormat);
                logBuilder.Append(timestamp);
            }
            string logLevelString = GetLogLevelString(logLevel);
            logBuilder.Append(logLevelString);
            // category and event id
            logBuilder.Append(LoglevelPadding);
            logBuilder.Append(logName);
            logBuilder.Append('[');
            logBuilder.Append(eventId);
            logBuilder.AppendLine("]");

            // scope information
            GetScopeInformation(logBuilder, multiLine: true);

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(_messagePadding);

                int len = logBuilder.Length;
                logBuilder.AppendLine(message);
                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine(exception.ToString());
            }

            return new LogMessageEntry(
                message: logBuilder.ToString(),
                logAsError: logLevel >= Options.LogToStandardErrorThreshold
            );
        }

        private LogMessageEntry CreateSystemdLogMessage(StringBuilder logBuilder, LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            // systemd reads messages from standard out line-by-line in a '<pri>message' format.
            // newline characters are treated as message delimiters, so we must replace them.
            // Messages longer than the journal LineMax setting (default: 48KB) are cropped.
            // Example:
            // <6>ConsoleApp.Program[10] Request received

            // loglevel
            string logLevelString = GetSyslogSeverityString(logLevel);
            logBuilder.Append(logLevelString);

            // timestamp
            string timestampFormat = Options.TimestampFormat;
            if (timestampFormat != null)
            {
                DateTime dateTime = GetCurrentDateTime();
                logBuilder.Append(dateTime.ToString(timestampFormat));
            }

            // category and event id
            logBuilder.Append(logName);
            logBuilder.Append('[');
            logBuilder.Append(eventId);
            logBuilder.Append(']');

            // scope information
            GetScopeInformation(logBuilder, multiLine: false);

            // message
            if (!string.IsNullOrEmpty(message))
            {
                logBuilder.Append(' ');
                // message
                AppendAndReplaceNewLine(logBuilder, message);
            }

            // exception
            // System.InvalidOperationException at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                logBuilder.Append(' ');
                AppendAndReplaceNewLine(logBuilder, exception.ToString());
            }

            // newline delimiter
            logBuilder.Append(Environment.NewLine);

            return new LogMessageEntry(
                message: logBuilder.ToString(),
                logAsError: logLevel >= Options.LogToStandardErrorThreshold
            );

            static void AppendAndReplaceNewLine(StringBuilder sb, string message)
            {
                int len = sb.Length;
                sb.Append(message);
                sb.Replace(Environment.NewLine, " ", len, message.Length);
            }
        }

        private DateTime GetCurrentDateTime()
        {
            return Options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private static string GetSyslogSeverityString(LogLevel logLevel)
        {
            // 'Syslog Message Severities' from https://tools.ietf.org/html/rfc5424.
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return "<7>"; // debug-level messages
                case LogLevel.Information:
                    return "<6>"; // informational messages
                case LogLevel.Warning:
                    return "<4>"; // warning conditions
                case LogLevel.Error:
                    return "<3>"; // error conditions
                case LogLevel.Critical:
                    return "<2>"; // critical conditions
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            if (Options.DisableColors)
            {
                return new ConsoleColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);
                case LogLevel.Error:
                    return new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red);
                case LogLevel.Warning:
                    return new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black);
                case LogLevel.Information:
                    return new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
                case LogLevel.Debug:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                case LogLevel.Trace:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                default:
                    return new ConsoleColors(DefaultConsoleColor, DefaultConsoleColor);
            }
        }

        private void GetScopeInformation(StringBuilder stringBuilder, bool multiLine)
        {
            IExternalScopeProvider scopeProvider = ScopeProvider;
            if (Options.IncludeScopes && scopeProvider != null)
            {
                int initialLength = stringBuilder.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    (StringBuilder builder, int paddAt) = state;
                    bool padd = paddAt == builder.Length;
                    if (padd)
                    {
                        builder.Append(_messagePadding);
                        builder.Append("=> ");
                    }
                    else
                    {
                        builder.Append(" => ");
                    }
                    builder.Append(scope);
                }, (stringBuilder, multiLine ? initialLength : -1));

                if (stringBuilder.Length > initialLength && multiLine)
                {
                    stringBuilder.AppendLine();
                }
            }
        }

        private readonly struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }
    }
}
#pragma warning restore CS0618
