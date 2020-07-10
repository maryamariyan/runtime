
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class ConsoleLoggerExtensionsTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        public void AddConsole_NullConfigure_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new ServiceCollection()
                    .AddLogging(builder => 
                    {
                        builder.AddConsole(null);
                    }));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        public void AddSimpleConsole_NullConfigure_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new ServiceCollection()
                    .AddLogging(builder => 
                    {
                        builder.AddSimpleConsole(null);
                    }));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        public void AddSystemdConsole_NullConfigure_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new ServiceCollection()
                    .AddLogging(builder => 
                    {
                        builder.AddSystemdConsole(null);
                    }));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        public void AddJsonConsole_NullConfigure_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new ServiceCollection()
                    .AddLogging(builder => 
                    {
                        builder.AddJsonConsole(null);
                    }));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        public void AddConsoleFormatter_NullConfigure_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new ServiceCollection()
                    .AddLogging(builder => 
                    {
                        builder.AddConsoleFormatter<CustomFormatter, CustomOptions>(null);
                    }));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        [MemberData(nameof(FormatterNames))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/38337", TestPlatforms.Browser)]
        public void AddConsole_ConsoleLoggerOptionsFromConfigFile_IsReadFromLoggingConfiguration(string formatterName)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[] {
                new KeyValuePair<string, string>("Console:FormatterName", formatterName)
            }).Build();

            var loggerProvider = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConfiguration(configuration)
                    .AddConsole())
                .BuildServiceProvider()
                .GetRequiredService<ILoggerProvider>();

            var consoleLoggerProvider = Assert.IsType<ConsoleLoggerProvider>(loggerProvider);
            var logger = (ConsoleLogger)consoleLoggerProvider.CreateLogger("Category");
            Assert.Equal(formatterName, logger.Options.FormatterName);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/38337", TestPlatforms.Browser)]
        public void AddConsoleFormatter_CustomFormatter_IsReadFromLoggingConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[] {
                new KeyValuePair<string, string>("Console:FormatterName", "custom"),
                new KeyValuePair<string, string>("Console:FormatterOptions:CustomLabel", "random"),
            }).Build();

            var loggerProvider = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConfiguration(configuration)
                    .AddConsoleFormatter<CustomFormatter, CustomOptions>(fOptions => { fOptions.CustomLabel = "random"; })
                    .AddConsole(o => { o.FormatterName = "custom"; })
                )
                .BuildServiceProvider()
                .GetRequiredService<ILoggerProvider>();

            var consoleLoggerProvider = Assert.IsType<ConsoleLoggerProvider>(loggerProvider);
            var logger = (ConsoleLogger)consoleLoggerProvider.CreateLogger("Category");
            Assert.Equal("custom", logger.Options.FormatterName);
            Assert.IsType<CustomFormatter>(logger.Formatter);
            var formatter = (CustomFormatter)(logger.Formatter);
            Assert.Equal("random", formatter.FormatterOptions.CustomLabel);
        }

        private class CustomFormatter : ConsoleFormatter, IDisposable
        {
            private IDisposable _optionsReloadToken;

            public CustomFormatter(IOptionsMonitor<CustomOptions> options)
                : base("custom")
            {
                ReloadLoggerOptions(options.CurrentValue);
                _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            }

            private void ReloadLoggerOptions(CustomOptions options)
            {
                FormatterOptions = options;
            }

            public CustomOptions FormatterOptions { get; set; }
            public string CustomLog { get; set; }

            public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
            {
                CustomLog = logEntry.Formatter(logEntry.State, logEntry.Exception);
            }

            public void Dispose()
            {
                _optionsReloadToken?.Dispose();
            }
        }

        private class CustomOptions : ConsoleFormatterOptions
        {
            public string CustomLabel { get; set; }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/38337", TestPlatforms.Browser)]
        public void AddSimpleConsole_TimestampFormat_IsReadFromLoggingConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[] {
                new KeyValuePair<string, string>("Console:FormatterOptions:TimestampFormat", "HH:mm "),
            }).Build();

            var loggerProvider = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConfiguration(configuration)
                    .AddSimpleConsole(o => {})
                )
                .BuildServiceProvider()
                .GetRequiredService<ILoggerProvider>();

            var consoleLoggerProvider = Assert.IsType<ConsoleLoggerProvider>(loggerProvider);
            var logger = (ConsoleLogger)consoleLoggerProvider.CreateLogger("Category");
            Assert.Equal(ConsoleFormatterNames.Simple, logger.Options.FormatterName);
            Assert.IsType<SimpleConsoleFormatter>(logger.Formatter);
            var formatter = (SimpleConsoleFormatter)(logger.Formatter);
            Assert.Equal("HH:mm ", formatter.FormatterOptions.TimestampFormat);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/38337", TestPlatforms.Browser)]
        public void AddSystemdConsole_TimestampFormat_IsReadFromLoggingConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[] {
                new KeyValuePair<string, string>("Console:FormatterOptions:TimestampFormat", "HH:mm "),
            }).Build();

            var loggerProvider = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConfiguration(configuration)
                    .AddSystemdConsole(o => {})
                )
                .BuildServiceProvider()
                .GetRequiredService<ILoggerProvider>();

            var consoleLoggerProvider = Assert.IsType<ConsoleLoggerProvider>(loggerProvider);
            var logger = (ConsoleLogger)consoleLoggerProvider.CreateLogger("Category");
            Assert.Equal(ConsoleFormatterNames.Systemd, logger.Options.FormatterName);
            Assert.IsType<SystemdConsoleFormatter>(logger.Formatter);
            var formatter = (SystemdConsoleFormatter)(logger.Formatter);
            Assert.Equal("HH:mm ", formatter.FormatterOptions.TimestampFormat);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/38337", TestPlatforms.Browser)]
        public void AddJsonConsole_TimestampFormat_IsReadFromLoggingConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[] {
                new KeyValuePair<string, string>("Console:FormatterOptions:TimestampFormat", "HH:mm "),
            }).Build();

            var loggerProvider = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConfiguration(configuration)
                    .AddJsonConsole(o => {})
                )
                .BuildServiceProvider()
                .GetRequiredService<ILoggerProvider>();

            var consoleLoggerProvider = Assert.IsType<ConsoleLoggerProvider>(loggerProvider);
            var logger = (ConsoleLogger)consoleLoggerProvider.CreateLogger("Category");
            Assert.Equal(ConsoleFormatterNames.Json, logger.Options.FormatterName);
            Assert.IsType<JsonConsoleFormatter>(logger.Formatter);
            var formatter = (JsonConsoleFormatter)(logger.Formatter);
            Assert.Equal("HH:mm ", formatter.FormatterOptions.TimestampFormat);
        }

        public static TheoryData<string> FormatterNames
        {
            get
            {
                var data = new TheoryData<string>();
                data.Add(ConsoleFormatterNames.Simple);
                data.Add(ConsoleFormatterNames.Systemd);
                data.Add(ConsoleFormatterNames.Json);
                return data;
            }
        }
    }
}