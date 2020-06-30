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

        /// <summary>
        /// Add and configure a console log formatter named 'json' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="ConsoleLogger"/> options for the built-in default log formatter.</param>
        public static ILoggingBuilder AddDefaultConsoleLogFormatter(this ILoggingBuilder builder, Action<DefaultConsoleLogFormatterOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConsole();
            builder.Services.Configure(configure);

            Action<ConsoleLoggerOptions> configureFormatter = (options) => { options.FormatterName = ConsoleLogFormatterNames.Default; };
            builder.Services.Configure(configureFormatter);

            return builder;
        }

        /// <summary>
        /// Add and configure a console log formatter named 'json' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="ConsoleLogger"/> options for the built-in json log formatter.</param>
        public static ILoggingBuilder AddJsonConsoleLogFormatter(this ILoggingBuilder builder, Action<JsonConsoleLogFormatterOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConsole();
            builder.Services.Configure(configure);

            Action<ConsoleLoggerOptions> configureFormatter = (options) => { options.FormatterName = ConsoleLogFormatterNames.Json; };
            builder.Services.Configure(configureFormatter);

            return builder;
        }

        /// <summary>
        /// Add and configure a console log formatter named 'systemd' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="ConsoleLogger"/> options for the built-in systemd log formatter.</param>
        public static ILoggingBuilder AddSystemdConsoleLogFormatter(this ILoggingBuilder builder, Action<SystemdConsoleLogFormatterOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConsole();
            builder.Services.Configure(configure);

            Action<ConsoleLoggerOptions> configureFormatter = (options) => { options.FormatterName = ConsoleLogFormatterNames.Systemd; };
            builder.Services.Configure(configureFormatter);

            return builder;
        }

        /// <summary>
        /// Adds a custom console logger formatter 'TFormatter' to be configured with options 'TOptions'.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this ILoggingBuilder builder)
            where TOptions : SystemdConsoleLogFormatterOptions
            where TFormatter : class, IConsoleLogFormatter
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConsoleLogFormatter, TFormatter>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, LogFormatterOptionsSetup<TFormatter, TOptions>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, LoggerProviderOptionsChangeTokenSource<TOptions, TFormatter>>());

            // TODO: Move configure and bind Console:Logging:FormatterName elsewhere
            // var configuration = new ConfigurationBuilder()
            //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //     .Build();
            // builder.Services.AddOptions<TOptions>().Bind(configuration.GetSection("Logging:Console:FormatterOptions"));
            // builder.Services.Configure<TOptions>(configuration.GetSection("Logging:Console:FormatterOptions"));

            return builder;
        }

        /// <summary>
        /// Adds a custom console logger formatter 'TFormatter' to be configured with options 'TOptions'.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
        public static ILoggingBuilder AddConsoleLogFormatter<TFormatter, TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
            where TOptions : SystemdConsoleLogFormatterOptions
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
