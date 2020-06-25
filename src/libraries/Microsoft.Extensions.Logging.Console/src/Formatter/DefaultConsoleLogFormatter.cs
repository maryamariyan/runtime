// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Console
{
    internal class DefaultConsoleLogFormatter : IConsoleLogFormatter, IDisposable
    {
        private static readonly string _loglevelPadding = ": ";
        private static readonly string _messagePadding;
        private static readonly string _newLineWithMessagePadding;
        private IDisposable _optionsReloadToken;

        // ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? DefaultConsoleColor = null;

        [ThreadStatic]
        private static StringBuilder _logBuilder;

        static DefaultConsoleLogFormatter()
        {
            var logLevelString = GetLogLevelString(LogLevel.Information);
            _messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
            _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        }

        public DefaultConsoleLogFormatter(IOptionsMonitor<DefaultConsoleLogFormatterOptions> options)
        {
            FormatterOptions = options.CurrentValue;
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        private void ReloadLoggerOptions(DefaultConsoleLogFormatterOptions options)
        {
            FormatterOptions = options;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }

        public string Name => ConsoleLogFormatterNames.Default;

        internal DefaultConsoleLogFormatterOptions FormatterOptions { get; set; }

        public void Write<TState>(LogLevel logLevel, string category, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var stringWriter = new StringWriter();
            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                if (!FormatterOptions.MultiLine)
                {
                    stringWriter = FormatHelperCompact(logLevel, category, eventId.Id, message, exception, scopeProvider, state, stringWriter);
                    textWriter.Write(stringWriter.Text);
                    return;
                }
                stringWriter = Format(logLevel, category, eventId.Id, message, exception, scopeProvider, stringWriter);
                textWriter.Write(stringWriter.Text);
            }
        }

        private StringWriter FormatHelperCompact<TState>(LogLevel logLevel, string category, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, TState scope, StringWriter textWriter)
        {
            var stringWriter = new StringWriter();
            var logLevelColors = GetLogLevelConsoleColors(logLevel);
            var logLevelString = GetLogLevelString(logLevel);

            string timestamp = null;
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                var dateTime = GetCurrentDateTime();
                timestamp = dateTime.ToString(timestampFormat);
            }
            stringWriter.Clear();
            if (timestamp != null)
            {
                stringWriter.Write(timestamp + " ", null, null);
            }
            if (logLevelString != null)
            {
                stringWriter.Write(logLevelString + " ", logLevelColors.Background, logLevelColors.Foreground);
            }

            stringWriter.Write($"{category}[{eventId}] ", null, null);
            string originalFormat = null;
            int count = 0;

            if (scope != null)
            {
                if (scope is IReadOnlyList<KeyValuePair<string, object>> kvpsx)
                {
                    var strings = new List<KeyValuePair<string, object>>();
                    foreach (var kvp in kvpsx)
                    {
                        if (kvp.Key.Contains("{OriginalFormat}"))
                        {
                            originalFormat = kvp.Value.ToString();
                        }
                        else
                        {
                            strings.Add(kvp);
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
                                    stringWriter.Write(curString, null, null);
                                    // TODO: when DisableColors is true, also uncolor the inner var colors
                                    stringWriter.Write(strings.ElementAt(count).Value.ToString(), null, ConsoleColor.Cyan);
                                    prevIndex += curIndex + strings.ElementAt(count).Key.Length + 2;
                                    count++;
                                }
                            }
                        }
                    }
                }
                else if (scope is IReadOnlyList<KeyValuePair<string, string>> kvps)
                {
                    var strings = new List<KeyValuePair<string, string>>();
                    foreach (var kvp in kvps)
                    {
                        if (kvp.Key.Contains("{OriginalFormat}"))
                        {
                            originalFormat = kvp.Value;
                        }
                        else
                        {
                            strings.Add(kvp);
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
                                    stringWriter.Write(curString, null, null);
                                    stringWriter.Write(strings.ElementAt(count).Value, null, ConsoleColor.Cyan);
                                    prevIndex += curIndex + strings.ElementAt(count).Key.Length + 2;
                                    count++;
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                if (originalFormat == null)
                {
                    stringWriter.Write(message, null, null);
                }
                else if (count == 0)
                {
                    stringWriter.Write(originalFormat, null, null);
                }
            }

            stringWriter.Write(" ", null, null);
            GetScopeInformation(scopeProvider, stringWriter);

            if (exception != null)
            {
                // exception message
                stringWriter.Write(" ", null, null);
                stringWriter.Write(exception.ToString().Replace(Environment.NewLine, " "), null, null);
                // TODO: try to improve readability for exception message.
                // TODO: maybe use Compact as default?
            }
            stringWriter.Write(Environment.NewLine, null, null);
            return stringWriter;
        }

        // IConsoleMessageBuilder // allocates a string 
        // Append(string messagee)
        // SetColor(xx)
        // ToString()

        private StringWriter Format(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, StringWriter stringWriter)
        {
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
            logBuilder.AppendLine("]");

            // scope information
            GetScopeInformation(logBuilder, scopeProvider);

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(_messagePadding);

                var len = logBuilder.Length;
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
            
            stringWriter.Clear();
            if (timestamp != null)
            {
                stringWriter.Write(timestamp, DefaultConsoleColor, DefaultConsoleColor);
            }
            if (logLevelString != null)
            {
                stringWriter.Write(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
            }
            stringWriter.Write(formattedMessage, DefaultConsoleColor, DefaultConsoleColor);

            return stringWriter;
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

        private void GetScopeInformation(IExternalScopeProvider scopeProvider, StringWriter stringWriter)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
                scopeProvider.ForEachScope((scope, writer) =>
                {
                    writer.Write("=> ", null, null);
                    writer.Write(scope.ToString(), null, ConsoleColor.DarkGray);
                    writer.Write(" ", null, null);

                }, stringWriter);
            }
        }

        private void GetScopeInformation(StringBuilder stringBuilder, IExternalScopeProvider scopeProvider)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
                var initialLength = stringBuilder.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    var (builder, paddAt) = state;
                    var padd = paddAt == builder.Length;
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
                }, (stringBuilder, initialLength));

                if (stringBuilder.Length > initialLength)
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
