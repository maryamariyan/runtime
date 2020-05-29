// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// A provider of <see cref="ConsoleLogger"/> instances.
    /// </summary>
    [ProviderAlias("Console")]
    public class ConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly IOptionsMonitor<ConsoleLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers;
        private readonly ConcurrentDictionary<string, IConsoleLogFormatter> _formatters;
        private readonly ConsoleLoggerProcessor _messageQueue;

        private IDisposable _optionsReloadToken;
        private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

        /// <summary>
        /// Creates an instance of <see cref="ConsoleLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to create <see cref="ConsoleLogger"/> instances with.</param>
        public ConsoleLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.Extensions.Logging.Console.ConsoleLoggerOptions> options) 
            : this(options, Enumerable.Empty<IConsoleLogFormatter>())
        {
            ; // todo: check workflow. maybe we should always have 4 formatters prepped instead?
            // current implementation wont work well with empty formatters.
            // use IServiceLocator or IServiceCollection? to locate Default formatter and add as formatters?
        }

        /// <summary>
        /// Creates an instance of <see cref="ConsoleLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to create <see cref="ConsoleLogger"/> instances with.</param>
        /// <param name="formatters">Log formatters added for <see cref="ConsoleLogger"/> insteaces.</param>
        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options, IEnumerable<IConsoleLogFormatter> formatters)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, ConsoleLogger>();
            _formatters = new ConcurrentDictionary<string, IConsoleLogFormatter>(formatters.ToDictionary(f => f.Name)); 

            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

            _messageQueue = new ConsoleLoggerProcessor();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _messageQueue.Console = new WindowsLogConsole();
                _messageQueue.ErrorConsole = new WindowsLogConsole(stdErr: true);
            }
            else
            {
                _messageQueue.Console = new AnsiLogConsole(new AnsiSystemConsole());
                _messageQueue.ErrorConsole = new AnsiLogConsole(new AnsiSystemConsole(stdErr: true));
            }
        }

        // warning:  ReloadLoggerOptions can be called before the ctor completed,... before registering all of the state used in this method need to be initialized
        private void ReloadLoggerOptions(ConsoleLoggerOptions options)
        {
            if (
                options.FormatterName == null ||
                !_formatters.TryGetValue(options.FormatterName, out IConsoleLogFormatter logFormatter) ||
                !_formatters.TryGetValue(options.FormatterName?.ToLower(), out logFormatter)
                )
            {
                switch (options.Format)
                {
                    case ConsoleLoggerFormat.Systemd:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Systemd];
                        break;
                    default:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Default];
                        break;
                }
            }
            UpdateFormatterOptions(logFormatter, options);

            foreach (var logger in _loggers)
            {
                logger.Value.Formatter = logFormatter;
            }
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            if (
                _options.CurrentValue.FormatterName == null ||
                !_formatters.TryGetValue(_options.CurrentValue.FormatterName, out IConsoleLogFormatter logFormatter) ||
                !_formatters.TryGetValue(_options.CurrentValue.FormatterName?.ToLower(), out logFormatter)
                )
            {
                switch (_options.CurrentValue.Format)
                {
                    case ConsoleLoggerFormat.Systemd:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Systemd];
                        break;
                    default:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Default];
                        break;
                }
            }
            UpdateFormatterOptions(logFormatter, _options.CurrentValue);

            return _loggers.GetOrAdd(name, loggerName => new ConsoleLogger(name, _messageQueue)
            {
                ScopeProvider = _scopeProvider,
                Formatter = logFormatter
            });
        }

        private void UpdateFormatterOptions(IConsoleLogFormatter formatter, ConsoleLoggerOptions deprecatedFromOptions)
        {
            if (deprecatedFromOptions.FormatterName != null)
                return;
            // kept for deprecated apis:
            if (formatter is DefaultConsoleLogFormatter defaultFormatter)
            {
                defaultFormatter.FormatterOptions.DisableColors = deprecatedFromOptions.DisableColors;
                defaultFormatter.FormatterOptions.IncludeScopes = deprecatedFromOptions.IncludeScopes;
                defaultFormatter.FormatterOptions.LogToStandardErrorThreshold = deprecatedFromOptions.LogToStandardErrorThreshold;
                defaultFormatter.FormatterOptions.TimestampFormat = deprecatedFromOptions.TimestampFormat;
                defaultFormatter.FormatterOptions.UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp;
            }
            else 
            if (formatter is SystemdConsoleLogFormatter systemdFormatter)
            {
                systemdFormatter.FormatterOptions.IncludeScopes = deprecatedFromOptions.IncludeScopes;
                systemdFormatter.FormatterOptions.LogToStandardErrorThreshold = deprecatedFromOptions.LogToStandardErrorThreshold;
                systemdFormatter.FormatterOptions.TimestampFormat = deprecatedFromOptions.TimestampFormat;
                systemdFormatter.FormatterOptions.UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
            _messageQueue.Dispose();
        }

        /// <inheritdoc />
        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;

            foreach (var logger in _loggers)
            {
                logger.Value.ScopeProvider = _scopeProvider;
            }

        }
    }
}
