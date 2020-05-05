// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLogger : ILogger
    {
        private readonly ILogFormatter _defaultFormatter = new DefaultLogFormatter();
        private readonly ILogFormatter _systemdFormatter = new SystemdLogFormatter();

        private readonly string _name;
        private ILogFormatter _consoleLogFormatter;
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

        internal IExternalScopeProvider ScopeProvider { get; set; }

        internal ConsoleLoggerOptions Options { get; set; }
        
        internal ILogFormatter Formatter
        {
            get { return _consoleLogFormatter; }
            set
            {
                if (value == null)
                {
                    _consoleLogFormatter = _defaultFormatter;
                }
                else
                {
                    _consoleLogFormatter = value;
                }
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
                WriteMessage(logLevel, _name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            var format = Options.Format;
            Debug.Assert(format >= ConsoleLoggerFormat.Default && format <= ConsoleLoggerFormat.Custom);
            ILogFormatter formatter;
            if (format == ConsoleLoggerFormat.Default)
            {
                // use default if we cant find loggerformatter as we shouldnt throw
                formatter = _defaultFormatter;
            }
            else if (format == ConsoleLoggerFormat.Systemd)
            {
                formatter = _systemdFormatter;
            }
            else // custom
            {
                Debug.Assert(format == ConsoleLoggerFormat.Custom);
                formatter = Formatter;
            }
            var entry = formatter.Format(logLevel, logName, eventId, message, exception, Options);
            _queueProcessor.EnqueueMessage(entry);
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
