// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Microsoft.Extensions.Logging.Console
{
    internal class CompactLogFormatter : ILogFormatter, IDisposable
    {
        private IDisposable _optionsReloadToken;
        private static readonly string _loglevelPadding = ": ";
        private static readonly string _messagePadding;
        private static readonly string _newLineWithMessagePadding;

        // ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? DefaultConsoleColor = null;

        [ThreadStatic]
        private static StringBuilder _logBuilder;

        static CompactLogFormatter()
        {
            var logLevelString = GetLogLevelString(LogLevel.Information);
            _messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
            _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        }

        public CompactLogFormatter(IOptionsMonitor<CompactLogFormatterOptions> options)
        {
            FormatterOptions = options.CurrentValue;
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        private void ReloadLoggerOptions(CompactLogFormatterOptions options)
        {
            FormatterOptions = options;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }

        public CompactLogFormatterOptions FormatterOptions { get; set; }

        public string Name => "Compact";

        public LogMessageEntry Format<TState>(LogLevel logLevel, string logName, int eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider)
        {
            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                return FormatHelper(logLevel, logName, eventId, message, exception, scopeProvider, state);
            }
            return default;
        }

        public LogMessageEntry Format(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider)
        {
            return FormatHelper<object>(logLevel, logName, eventId, message, exception, scopeProvider, null);
        }

        private LogMessageEntry FormatHelper<TState>(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, TState scope)
        {
            List<Message> msgs = new List<Message>();
            // todo fix later:
            var logBuilder = _logBuilder;
            _logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received

            var logLevelColors = GetLogLevelConsoleColors(logLevel);
            var logLevelString = GetLogLevelString(logLevel);
            // category and event id
            logBuilder.Append(_loglevelPadding);
            logBuilder.Append(logName);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.Append("]");
            // logBuilder.AppendLine("]");

            msgs.Add(new Message($"{logName}[{eventId}] ", null, null));
            string originalFormat = null;
            int count = 0;

            if (scope != null)
            {
                logBuilder.Append(_messagePadding);
                if (scope is IReadOnlyList<KeyValuePair<string, object>> kvpsx)
                {
                    var strings = new List<KeyValuePair<string, object>>();
                    logBuilder.Append(" -> ");
                    foreach (var kvp in kvpsx)
                    {
                        if (kvp.Key.Contains("{OriginalFormat}"))
                        {
                            originalFormat = kvp.Value.ToString();
                        }
                        else
                        {
                            //count++;
                            strings.Add(kvp);
                            logBuilder.Append(kvp.Value.ToString());
                            logBuilder.Append(", ");
                        }
                    }
                    int prevIndex = 0;
                    if (originalFormat != null)
                    {
                        foreach (var kvp in kvpsx)
                        {
                            if (!kvp.Key.Contains("{OriginalFormat}"))
                            {
                                var curIndex = originalFormat.IndexOf("{" + strings.ElementAt(count).Key + "}");
                                if (curIndex != -1)
                                {
                                    var curString = originalFormat.Substring(prevIndex, curIndex - prevIndex);
                                    msgs.Add(new Message(curString, null, null));
                                    msgs.Add(new Message(strings.ElementAt(count).Value.ToString(), null, ConsoleColor.Cyan));
                                    prevIndex += curIndex + strings.ElementAt(count).Key.Length + 2;
                                    count++;
                                    //strings.Add(kvp.Value.ToString());
                                    logBuilder.Append(kvp.Value.ToString());
                                    logBuilder.Append(", ");
                                }
                            }
                        }
                    }
                }
                else if (scope is IReadOnlyList<KeyValuePair<string, string>> kvps)
                {
                    var strings = new List<KeyValuePair<string, string>>();
                    logBuilder.Append(" -> ");
                    foreach (var kvp in kvps)
                    {
                        if (kvp.Key.Contains("{OriginalFormat}"))
                        {
                            originalFormat = kvp.Value;
                        }
                        else
                        {
                            //count++;
                            strings.Add(kvp);
                            logBuilder.Append(kvp.Value);
                            logBuilder.Append(", ");
                        }
                    }
                    int prevIndex = 0;
                    if (originalFormat != null)
                    {
                        foreach (var kvp in kvps)
                        {
                            if (!kvp.Key.Contains("{OriginalFormat}"))
                            {
                                var curIndex = originalFormat.IndexOf("{" + strings.ElementAt(count).Key + "}");
                                if (curIndex != -1)
                                {
                                    var curString = originalFormat.Substring(prevIndex, curIndex - prevIndex);
                                    msgs.Add(new Message(curString, null, null));
                                    msgs.Add(new Message(strings.ElementAt(count).Value, null, ConsoleColor.Cyan));
                                    prevIndex += curIndex + strings.ElementAt(count).Key.Length + 2;
                                    count++;
                                    logBuilder.Append(kvp.Value);
                                    logBuilder.Append(", ");
                                }
                            }
                        }
                    }
                }
                else
                {
                    logBuilder.Append("-> ");
                    logBuilder.Append(scope.ToString());
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(_messagePadding);

                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                if (originalFormat == null)
                {
                    msgs.Add(new Message(message, null, null));
                }
                else if (count == 0)
                {
                    msgs.Add(new Message(originalFormat, null, null));
                }
            }

            // scope information
            msgs.Add(new Message(" ", null, null));
            GetScopeInformation(logBuilder, scopeProvider, msgs);

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.Append(exception.ToString());
                msgs.Add(new Message(" ", null, null));
                msgs.Add(new Message(exception.ToString(), null, null));
                // logBuilder.AppendLine(exception.ToString());
            }

            string timestamp = null;
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                var dateTime = GetCurrentDateTime();
                timestamp = dateTime.ToString(timestampFormat);
            }

            var formattedMessage = logBuilder.ToString();
            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _logBuilder = logBuilder;

            return new LogMessageEntry(
                message: formattedMessage,
                timeStamp: timestamp,
                levelString: logLevelString,
                levelBackground: logLevelColors.Background,
                levelForeground: logLevelColors.Foreground,
                messageColor: DefaultConsoleColor,
                logAsError: logLevel >= FormatterOptions.LogToStandardErrorThreshold,
                writeCallback : console =>
                {
                    if (timestamp != null || logLevelString != null)
                    {
                        console.Write("[", null, null);

                        if (timestamp != null)
                        {
                            console.Write(timestamp, null, null);
                            console.Write(" ", null, null);
                        }
                        if (logLevelString != null)
                        {
                            console.Write(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
                            console.Write("", null, null);
                        }
                        console.Write("] ", null, null);
                    }

                    if (msgs.Count > 0)
                    {
                        foreach (var item in msgs)
                        {
                            console.Write(item.Content, item.Background, item.Foreground);
                        }
                    }
                    
                    //console.Write(formattedMessage, DefaultConsoleColor, DefaultConsoleColor);
                    console.WriteLine(string.Empty, DefaultConsoleColor, DefaultConsoleColor);
                    console.Flush();
                }
            );
        }

        internal struct LogEntry
        {
            internal LogEntry(Action<IConsole> action, Message[] messages, bool logAsError)
            {
                Messages = messages;
                WriteCallback = action;
                LogAsError = logAsError;
            }

            internal Message[] Messages;
            internal Action<IConsole> WriteCallback;
            internal bool LogAsError;
        }

        internal struct Message
        {
            internal Message(string message, ConsoleColor? background, ConsoleColor? foreground)
            {
                Content = message;
                Background = background;
                Foreground = foreground;
            }
            internal string Content;
            internal ConsoleColor? Background;
            internal ConsoleColor? Foreground;
        }

        private DateTime GetCurrentDateTime()
        {
            return FormatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "compact_trce";
                case LogLevel.Debug:
                    return "compact_dbug";
                case LogLevel.Information:
                    return "compact_info";
                case LogLevel.Warning:
                    return "compact_warn";
                case LogLevel.Error:
                    return "compact_fail";
                case LogLevel.Critical:
                    return "compact_crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            if (FormatterOptions.DisableColors)
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

        private void GetScopeInformation(StringBuilder stringBuilder, IExternalScopeProvider scopeProvider, List<Message> msgs)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
                var initialLength = stringBuilder.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    var (builder, paddAt, messages) = state;
                    var padd = paddAt == builder.Length;
                    if (padd)
                    {
                        //messages.Add(new Message(_messagePadding, null, null));
                        builder.Append(_messagePadding);
                    }
                    messages.Add(new Message("=> ", null, null));
                    messages.Add(new Message(scope.ToString(), null, ConsoleColor.DarkGray));
                    messages.Add(new Message(" ", null, null));
                    builder.Append("=> ");
                    builder.Append(scope);
                    builder.Append(" ");

                }, (stringBuilder, initialLength, msgs));

                if (stringBuilder.Length > initialLength)
                {
                    // stringBuilder.AppendLine();
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
