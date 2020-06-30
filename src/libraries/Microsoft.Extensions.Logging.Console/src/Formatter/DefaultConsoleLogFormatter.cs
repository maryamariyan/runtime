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
        private const string LoglevelPadding = ": ";
        private static readonly string _messagePadding = new string(' ', GetLogLevelString(LogLevel.Information).Length + LoglevelPadding.Length);
        private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        private IDisposable _optionsReloadToken;

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
            {
                string message = formatter(state, exception);
                if (!string.IsNullOrEmpty(message) || exception != null)
                {
                    WriteSingleLine(logLevel, category, eventId.Id, message, exception, scopeProvider, state, stringWriter);
                }
            }
        }

        private void WriteSingleLine<TState>(LogLevel logLevel, string category, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, TState scope, StringWriter stringWriter)
        {
            bool singleLine = FormatterOptions.SingleLine;
            ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
            string logLevelString = GetLogLevelString(logLevel);

            string timestamp = null;
            string timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                DateTime dateTime = GetCurrentDateTime();
                timestamp = dateTime.ToString(timestampFormat);
            }
            stringWriter.DisableColors = FormatterOptions.DisableColors;
            if (timestamp != null)
            {
                stringWriter.Write(singleLine ? timestamp + ' ' : timestamp);
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
            if (!singleLine)
            {
                stringWriter.Write(Environment.NewLine);
            }

            // scope information
            GetScopeInformation(stringWriter, scopeProvider, singleLine);
            if (singleLine)
            {
                stringWriter.Write(' ');
            }

            if (!string.IsNullOrEmpty(message))
            {
                if (singleLine)
                {
                    stringWriter.WriteReplacing(Environment.NewLine, " ", message);
                }
                else
                {
                    stringWriter.Write(_messagePadding);
                    stringWriter.WriteReplacing(Environment.NewLine, _newLineWithMessagePadding, message);
                    stringWriter.Write(Environment.NewLine);
                }
            }

            if (exception != null)
            {
                if (singleLine)
                {
                    stringWriter.Write(' ');
                    stringWriter.WriteReplacing(Environment.NewLine, " ", exception.ToString());
                }
                else
                {
                    stringWriter.Write(_messagePadding);
                    stringWriter.WriteReplacing(Environment.NewLine, _newLineWithMessagePadding, exception.ToString());
                    stringWriter.Write(Environment.NewLine);
                }
            }
            if (singleLine)
            {
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
            if (!FormatterOptions.DisableColors)
            {
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
                }
            }

            return new ConsoleColors(null, null);
        }

        private void GetScopeInformation(StringWriter stringWriter, IExternalScopeProvider scopeProvider, bool singleLine)
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
                        writer.Write(_messagePadding);
                        writer.Write("=> ");
                    }
                    else
                    {
                        writer.Write(" => ");
                    }
                    writer.Write(scope.ToString());
                }, (stringWriter, singleLine ? -1 : initialLength));

                if (stringWriter.Length > initialLength && !singleLine)
                {
                    stringWriter.Write(Environment.NewLine);
                }
            }
        }
    }

    internal readonly struct ConsoleColors
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
