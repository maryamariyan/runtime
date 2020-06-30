// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Configuration;
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
        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options)
            : this(options, Enumerable.Empty<IConsoleLogFormatter>()) { }

        /// <summary>
        /// Creates an instance of <see cref="ConsoleLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to create <see cref="ConsoleLogger"/> instances with.</param>
        /// <param name="formatters">Log formatters added for <see cref="ConsoleLogger"/> insteaces.</param>
        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options, IEnumerable<IConsoleLogFormatter> formatters)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, ConsoleLogger>();

            _formatters = SetFormatters(formatters);

            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

            _messageQueue = new ConsoleLoggerProcessor();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // [TODO]: check if VT enabled on windows, console mode.
                _messageQueue.Console = new WindowsLogConsole();
                _messageQueue.ErrorConsole = new WindowsLogConsole(stdErr: true);
            }
            else
            {
                _messageQueue.Console = new AnsiLogConsole(new AnsiSystemConsole());
                _messageQueue.ErrorConsole = new AnsiLogConsole(new AnsiSystemConsole(stdErr: true));
            }
        }

        private ConcurrentDictionary<string, IConsoleLogFormatter> SetFormatters(IEnumerable<IConsoleLogFormatter> fromSetup = null)
        {
            IEnumerable<IConsoleLogFormatter> formatters = fromSetup;
            if (formatters == null || formatters.ToList().Count == 0)
            {
                var defaultMonitor = new FormatterOptionsMonitor<DefaultConsoleLogFormatterOptions>(new DefaultConsoleLogFormatterOptions());
                var systemdMonitor = new FormatterOptionsMonitor<SystemdConsoleLogFormatterOptions>(new SystemdConsoleLogFormatterOptions());
                formatters = new List<IConsoleLogFormatter>()
                {
                    new DefaultConsoleLogFormatter(defaultMonitor, _scopeProvider),
                    new SystemdConsoleLogFormatter(systemdMonitor, _scopeProvider)
                };
            }
            return new ConcurrentDictionary<string, IConsoleLogFormatter>(formatters.ToDictionary(f => f.Name));
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
#pragma warning disable CS0618
                switch (options.Format)
                {
                    case ConsoleLoggerFormat.Systemd:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Systemd];
                        break;
                    default:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Default];
                        break;
                }
                UpdateFormatterOptions(logFormatter, options);
#pragma warning restore CS0618
            }

            foreach (KeyValuePair<string, ConsoleLogger> logger in _loggers)
            {
                logger.Value.Options = options;
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
#pragma warning disable CS0618
                switch (_options.CurrentValue.Format)
                {
                    case ConsoleLoggerFormat.Systemd:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Systemd];
                        break;
                    default:
                        logFormatter = _formatters[ConsoleLogFormatterNames.Default];
                        break;
                }
                UpdateFormatterOptions(logFormatter, _options.CurrentValue);
#pragma warning disable CS0618
            }

            return _loggers.GetOrAdd(name, loggerName => new ConsoleLogger(name, _messageQueue)
            {
                Options = _options.CurrentValue,
                ScopeProvider = _scopeProvider,
                Formatter = logFormatter,
            });
        }
#pragma warning disable CS0618
        private void UpdateFormatterOptions(IConsoleLogFormatter formatter, ConsoleLoggerOptions deprecatedFromOptions)
        {
            // kept for deprecated apis:
            if (formatter is DefaultConsoleLogFormatter defaultFormatter)
            {
                defaultFormatter.FormatterOptions.DisableColors = deprecatedFromOptions.DisableColors;
                defaultFormatter.FormatterOptions.IncludeScopes = deprecatedFromOptions.IncludeScopes;
                defaultFormatter.FormatterOptions.TimestampFormat = deprecatedFromOptions.TimestampFormat;
                defaultFormatter.FormatterOptions.UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp;
            }
            else
            if (formatter is SystemdConsoleLogFormatter systemdFormatter)
            {
                systemdFormatter.FormatterOptions.IncludeScopes = deprecatedFromOptions.IncludeScopes;
                systemdFormatter.FormatterOptions.TimestampFormat = deprecatedFromOptions.TimestampFormat;
                systemdFormatter.FormatterOptions.UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp;
            }
        }
#pragma warning restore CS0618

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

            foreach (System.Collections.Generic.KeyValuePair<string, ConsoleLogger> logger in _loggers)
            {
                logger.Value.ScopeProvider = _scopeProvider;
            }

            foreach (System.Collections.Generic.KeyValuePair<string, IConsoleLogFormatter> formatter in _formatters)
            {
                formatter.Value.ScopeProvider = _scopeProvider;
            }

        }
    }
}
