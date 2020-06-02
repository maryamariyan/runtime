// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Extensions.Logging.Console
{
    internal class FormattedConsoleLogger : ILogger
    {

        private readonly string _name;
        private readonly FormattedConsoleLoggerProcessor _queueProcessor;
        private readonly ConcurrentDictionary<string, IConsoleLogFormatter> _formatters;
        private readonly IConsoleMessageBuilder _consoleMessageBuilder;

        internal FormattedConsoleLogger(string name, FormattedConsoleLoggerProcessor loggerProcessor, ConcurrentDictionary<string, IConsoleLogFormatter> formatters, IConsoleMessageBuilder consoleMessageBuilder)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
            _queueProcessor = loggerProcessor;
            _formatters = formatters;
            _consoleMessageBuilder = consoleMessageBuilder;
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        internal IConsoleLogFormatterOptions FormatterOptions { get; set; }
        internal IConsoleLogFormatter Formatter { get; set; }

        private FormattedConsoleLoggerOptions _formattedConsoleLoggerOptions;

        internal FormattedConsoleLoggerOptions Options {
            get 
            {
                return _formattedConsoleLoggerOptions;
            } 
            set 
            {
                if (
                    value.FormatterName == null ||
                    (
                        !_formatters.TryGetValue(value.FormatterName, out IConsoleLogFormatter logFormatter) &&
                        !_formatters.TryGetValue(value.FormatterName?.ToLower(), out logFormatter))
                    )
                {
                    logFormatter = _formatters[ConsoleLogFormatterNames.Default];
                }
                // update Formatter using FormatterName when Options changes
                Formatter = logFormatter;

                // update Formatter.FormatterOptions when options.FormatterOptions changes
                FormatterOptions = Formatter.UpdateWith(value.FormatterOptions);

                _formattedConsoleLoggerOptions = value;
            }
        }

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

            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                Formatter.Format(logLevel, _name, eventId.Id, state, exception, formatter, ScopeProvider, _consoleMessageBuilder);
                _consoleMessageBuilder.Clear();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
