// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
#pragma warning disable CS0618

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
                // [TODO]: how to check if VT enabled on windows, console mode.
                _messageQueue.Console = new WindowsLogConsole();
                _messageQueue.ErrorConsole = new WindowsLogConsole(stdErr: true);
            }
            else
            {
                _messageQueue.Console = new AnsiLogConsole(new AnsiSystemConsole());
                _messageQueue.ErrorConsole = new AnsiLogConsole(new AnsiSystemConsole(stdErr: true));
            }
        }

        private static IEnumerable<IConsoleLogFormatter> GetFormatters()
        {
            var defaultMonitor = new DefaultOptionsMonitor(new DefaultConsoleLogFormatterOptions() { MultiLine = true });
            var systemdMonitor = new SystemdOptionsMonitor(new SystemdConsoleLogFormatterOptions() { });
            var formatters = new List<IConsoleLogFormatter>() {
                new DefaultConsoleLogFormatter(defaultMonitor),
                new SystemdConsoleLogFormatter(systemdMonitor) };
            return formatters;
        }

        private class DefaultOptionsMonitor : IOptionsMonitor<DefaultConsoleLogFormatterOptions>
        {
            private DefaultConsoleLogFormatterOptions _options;
            private event Action<DefaultConsoleLogFormatterOptions, string> _onChange;

            public DefaultOptionsMonitor(DefaultConsoleLogFormatterOptions options)
            {
                _options = options;
            }

            public DefaultConsoleLogFormatterOptions Get(string name) => _options;

            public IDisposable OnChange(Action<DefaultConsoleLogFormatterOptions, string> listener)
            {
                _onChange += listener;
                return null;
            }

            public DefaultConsoleLogFormatterOptions CurrentValue => _options;

            public void Set(DefaultConsoleLogFormatterOptions options)
            {
                _options = options;
                _onChange?.Invoke(options, "");
            }
        }

        private class SystemdOptionsMonitor : IOptionsMonitor<SystemdConsoleLogFormatterOptions>
        {
            private SystemdConsoleLogFormatterOptions _options;
            private event Action<SystemdConsoleLogFormatterOptions, string> _onChange;

            public SystemdOptionsMonitor(SystemdConsoleLogFormatterOptions options)
            {
                _options = options;
            }

            public SystemdConsoleLogFormatterOptions Get(string name) => _options;

            public IDisposable OnChange(Action<SystemdConsoleLogFormatterOptions, string> listener)
            {
                _onChange += listener;
                return null;
            }

            public SystemdConsoleLogFormatterOptions CurrentValue => _options;

            public void Set(SystemdConsoleLogFormatterOptions options)
            {
                _options = options;
                _onChange?.Invoke(options, "");
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ConsoleLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to create <see cref="ConsoleLogger"/> instances with.</param>
        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, ConsoleLogger>();
            _formatters = new ConcurrentDictionary<string, IConsoleLogFormatter>(GetFormatters().ToDictionary(f => f.Name));

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

        private void ReloadLoggerOptions(ConsoleLoggerOptions options)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, ConsoleLogger> logger in _loggers)
            {
                logger.Value.Options = options;
            }
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, loggerName => new ConsoleLogger(name, _messageQueue)
            {
                Options = _options.CurrentValue,
                ScopeProvider = _scopeProvider
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

            foreach (System.Collections.Generic.KeyValuePair<string, ConsoleLogger> logger in _loggers)
            {
                logger.Value.ScopeProvider = _scopeProvider;
            }

        }
    }
}
#pragma warning restore CS0618
