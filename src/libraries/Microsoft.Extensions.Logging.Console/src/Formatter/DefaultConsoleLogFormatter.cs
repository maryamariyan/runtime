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
            if (textWriter is StringWriter stringWriter)
            // TODO: minor issue but ok
            {
                var message = formatter(state, exception);
                if (!string.IsNullOrEmpty(message) || exception != null)
                {
                    if (!FormatterOptions.MultiLine)
                    {
                        FormatHelperCompact(logLevel, category, eventId.Id, message, exception, scopeProvider, state, stringWriter);
                        return;
                    }
                    Format(logLevel, category, eventId.Id, message, exception, scopeProvider, stringWriter);
                }
            }
        }

        private void FormatHelperCompact<TState>(LogLevel logLevel, string category, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, TState scope, StringWriter stringWriter)
        {
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
            stringWriter.DisableColors = FormatterOptions.DisableColors;
            // stringWriter.ResetColor();
            if (timestamp != null)
            {
                stringWriter.Write(timestamp + " ");
            }
            if (logLevelString != null)
            {
                stringWriter.WriteAndReset(logLevelString + " ", logLevelColors.Background, logLevelColors.Foreground);
            }

            // category and event id
            stringWriter.Write(category + "[" + eventId + "] ");

            GetScopeInformation(stringWriter, scopeProvider);
            stringWriter.Write(" ");

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
                                    stringWriter.Write(curString);
                                    stringWriter.WriteAndReset(strings.ElementAt(count).Value.ToString(), null, ConsoleColor.Yellow);
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
                                    stringWriter.Write(curString);
                                    stringWriter.WriteAndReset(strings.ElementAt(count).Value, null, ConsoleColor.Yellow);
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
                    stringWriter.Write(message.Replace(Environment.NewLine, " "));
                }
                else if (count == 0)
                {
                    stringWriter.Write(originalFormat.Replace(Environment.NewLine, " "));
                }
            }

            if (exception != null)
            {
                // exception message
                stringWriter.Write(" ", null, null);
                stringWriter.Write(exception.ToString().Replace(Environment.NewLine, " "));
            }
            stringWriter.Write(Environment.NewLine, null, null);
        }

        private void Format(LogLevel logLevel, string category, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, StringWriter stringWriter)
        {
            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received
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
            stringWriter.DisableColors = FormatterOptions.DisableColors;
            stringWriter.ResetColor();
            if (timestamp != null)
            {
                stringWriter.Write(timestamp);
            }
            if (logLevelString != null)
            {
                stringWriter.WriteAndReset(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
            }
            // category and event id
            stringWriter.Write(_loglevelPadding + category + "[" + eventId + "]");
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
                stringWriter.Write(_messagePadding);
                stringWriter.WriteReplacing(Environment.NewLine, _newLineWithMessagePadding, exception.ToString());
                stringWriter.Write(Environment.NewLine);
            }
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

        private void GetScopeInformation(StringWriter stringWriter, IExternalScopeProvider scopeProvider)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
                int initialLength = stringWriter.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    (StringWriter writer, int paddAt) = state;
                    bool padd = paddAt == writer.Length;
                    if (padd)
                    {
                        // writer.ResetColor();
                        writer.Write(_messagePadding);
                        writer.Write("=> ");
                    }
                    else
                    {
                        writer.Write(" => ");
                    }
                    writer.WriteAndReset(scope.ToString(), null, ConsoleColor.White);
                }, (stringWriter, FormatterOptions.MultiLine ? initialLength : -1));

                if (stringWriter.Length > initialLength && FormatterOptions.MultiLine)
                {
                    stringWriter.Write(Environment.NewLine);
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
