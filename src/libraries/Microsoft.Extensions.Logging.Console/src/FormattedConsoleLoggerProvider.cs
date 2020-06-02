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
    /// A provider of <see cref="FormattedConsoleLogger"/> instances.
    /// </summary>
    [ProviderAlias("FormattedConsole")]
    public class FormattedConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private FormattedConsoleLoggerOptions _options;
        private readonly ConcurrentDictionary<string, FormattedConsoleLogger> _loggers;
        private readonly ConcurrentDictionary<string, IConsoleLogFormatter> _formatters;
        private readonly FormattedConsoleLoggerProcessor _messageQueue;

        private IDisposable _optionsReloadToken;
        private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;
        private readonly IConsoleMessageBuilder _consoleMessageBuilder;

        /// <summary>
        /// Creates an instance of <see cref="ConsoleLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to create <see cref="FormattedConsoleLogger"/> instances with.</param>
        /// <param name="formatters">Log formatters added for <see cref="FormattedConsoleLogger"/> insteaces.</param>
        public FormattedConsoleLoggerProvider(IOptionsMonitor<FormattedConsoleLoggerOptions> options, IEnumerable<IConsoleLogFormatter> formatters)
        {
            _options = options.CurrentValue;
            _loggers = new ConcurrentDictionary<string, FormattedConsoleLogger>();
            _formatters = new ConcurrentDictionary<string, IConsoleLogFormatter>(formatters.ToDictionary(f => f.Name)); 

            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);

            _messageQueue = new FormattedConsoleLoggerProcessor();
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
            _consoleMessageBuilder = new DummyConsoleBuilder(_messageQueue);
        }

        // warning:  ReloadLoggerOptions can be called before the ctor completed,... before registering all of the state used in this method need to be initialized
        private void ReloadLoggerOptions(FormattedConsoleLoggerOptions options)
        {
            foreach (var logger in _loggers)
            {
                logger.Value.Options = options;
                options.FormatterOptions = logger.Value.FormatterOptions;
                logger.Value.Options = options;
            }
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, loggerName => new FormattedConsoleLogger(name, _messageQueue, _formatters, _consoleMessageBuilder)
            {
                ScopeProvider = _scopeProvider,
                Options = _options
            });
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
