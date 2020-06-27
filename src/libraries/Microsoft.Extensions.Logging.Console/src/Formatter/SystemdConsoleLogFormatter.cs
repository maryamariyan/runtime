// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Console
{
    internal class SystemdConsoleLogFormatter : IConsoleLogFormatter, IDisposable
    {
        private IDisposable _optionsReloadToken;

        private const string LoglevelPadding = ": ";
        // TODO: remove +1 below
        private static readonly string _messagePadding = new string(' ', GetSyslogSeverityString(LogLevel.Information).Length + 1 + LoglevelPadding.Length);

        public SystemdConsoleLogFormatter(IOptionsMonitor<SystemdConsoleLogFormatterOptions> options)
        {
            FormatterOptions = options.CurrentValue;
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        private void ReloadLoggerOptions(SystemdConsoleLogFormatterOptions options)
        {
            FormatterOptions = options;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }

        public string Name => ConsoleLogFormatterNames.Systemd;

        internal SystemdConsoleLogFormatterOptions FormatterOptions { get; set; }

        public void Write<TState>(LogLevel logLevel, string category, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            if (textWriter is StringWriter stringWriter)
            {
                var message = formatter(state, exception);
                if (!string.IsNullOrEmpty(message) || exception != null)
                {
                    Format(logLevel, category, eventId.Id, message, exception, scopeProvider, stringWriter);
                }
            }
        }

        private void Format(LogLevel logLevel, string category, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, StringWriter textWriter)
        {
            // systemd reads messages from standard out line-by-line in a '<pri>message' format.
            // newline characters are treated as message delimiters, so we must replace them.
            // Messages longer than the journal LineMax setting (default: 48KB) are cropped.
            // Example:
            // <6>ConsoleApp.Program[10] Request received

            // loglevel
            var logLevelString = GetSyslogSeverityString(logLevel);
            textWriter.Write(logLevelString);

            // timestamp
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                var dateTime = GetCurrentDateTime();
                textWriter.Write(dateTime.ToString(timestampFormat));
            }

            // category and event id
            textWriter.Write(category + "[" + eventId + "]");

            // scope information
            GetScopeInformation(textWriter, scopeProvider);

            // message
            if (!string.IsNullOrEmpty(message))
            {
                textWriter.Write(' ');
                // message
                textWriter.WriteReplacing(Environment.NewLine, " ", message);
            }

            // exception
            // System.InvalidOperationException at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                textWriter.Write(' ');
                textWriter.WriteReplacing(Environment.NewLine, " ", exception.ToString());
            }

            // newline delimiter
            textWriter.Write(Environment.NewLine);
        }

        private DateTime GetCurrentDateTime()
        {
            return FormatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
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

        private void GetScopeInformation(StringWriter stringWriter, IExternalScopeProvider scopeProvider)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
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
                    writer.Write(scope);
                }, (stringWriter, -1));
            }
        }
    }
}
