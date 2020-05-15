// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class ConsoleLoggerExtensions
    {
        /// <summary>
        /// Adds a console logger named 'Console' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddConsole(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.AddConsoleLogFormatter<JsonConsoleLogFormatter, JsonConsoleLogFormatterOptions>();
            builder.AddConsoleLogFormatter<SystemdConsoleLogFormatter, SystemdConsoleLogFormatterOptions>();
            builder.AddConsoleLogFormatter<CompactLogFormatter, CompactLogFormatterOptions>();
            builder.AddConsoleLogFormatter<DefaultConsoleLogFormatter, DefaultConsoleLogFormatterOptions>();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<ConsoleLoggerOptions, ConsoleLoggerProvider>(builder.Services);

            return builder;
        }

        /// <summary>
        /// Adds a console logger named 'Console' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="ConsoleLogger"/>.</param>
        public static ILoggingBuilder AddConsole(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConsole();
            builder.Services.Configure(configure);

            return builder;
        }

        public static ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this ILoggingBuilder builder)
            where TOptions : class
            where TFormatter : class, IConsoleLogFormatter
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConsoleLogFormatter, TFormatter>());
            builder.Services.AddSingleton<IConfigureOptions<TOptions>, LogFormatterOptionsSetup<TFormatter, TOptions>>();
            builder.Services.AddSingleton<IOptionsChangeTokenSource<TOptions>, LoggerProviderOptionsChangeTokenSource<TOptions, TFormatter>>();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            builder.Services.AddOptions<TOptions>().Bind(configuration.GetSection("Logging:Console:FormatterOptions"));
            builder.Services.Configure<TOptions>(configuration.GetSection("Logging:Console:FormatterOptions"));

            return builder;
        }

        public static ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
            where TOptions : class
            where TFormatter : class, IConsoleLogFormatter
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConsoleLogFormatter<TFormatter, TOptions>();

            builder.Services.Configure(configure);

            return builder;
        }
    }

    internal class LogFormatterOptionsSetup<TFormatter, TOptions> : ConfigureFromConfigurationOptions<TOptions> 
            where TOptions : class
            where TFormatter : class, IConsoleLogFormatter
    {
        public LogFormatterOptionsSetup(ILoggerProviderConfiguration<TFormatter> providerConfiguration)
            : base(providerConfiguration.Configuration)
        {
        }
    }
}
