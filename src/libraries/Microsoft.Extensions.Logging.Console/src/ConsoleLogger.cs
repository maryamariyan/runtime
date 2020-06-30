// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ConsoleLoggerProcessor _queueProcessor;

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

            if (_stringWriter == null)
            {
                _stringWriter = new StringWriter();
            }

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                Formatter.Write(logLevel, _name, eventId.Id, state, exception, formatter, _stringWriter);
                _queueProcessor.EnqueueMessage(new LogMessageEntry(_stringWriter.ComputeAnsiString(), logAsError: logLevel >= Options.LogToStandardErrorThreshold));
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
