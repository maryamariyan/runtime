// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
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
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<ConsoleLoggerOptions, ConsoleLoggerProvider>(builder.Services);
            // LoggerProviderOptions.RegisterProviderOptions<DefaultLogFormatterOptions, DefaultLogFormatter>(builder.Services);
            // add for jsonformatter and for custom... they themselves add 
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

    //     public static ILoggingBuilder AddFormatter(this ILoggingBuilder builder, Action<ILogFormatter> configure) 
    //     {
    //         // if (configure == null)
    //         // {
    //         //     throw new ArgumentNullException(nameof(configure));
    //         // }
    //         // builder.Services.AddSingleton<IConsoleLogFormatterConfigurationFactory, DefaultConsoleLogFormatterConfigurationFactory>();



    //         // builder.Services.Configure(configure);


    //         /*
    //         brainstorm
            
    //         IConsoleLogFormatterConfigurationFactory
    //         IConsoleLogFormatter
    //         IConsoleLogFormatter, an Options type, and IConsoleLogFormatterConfigurationFactory
    //         */
    //         return builder;
    //     }

    // }

    // public interface IConsoleLogFormatterConfigurationFactory
    // {
    //     ILogFormatter CreateJsonFormatter(Action<ILogFormatter> setup);
    //     // ILogFormatter CreateFormatter(string name);
    // }

    // public class DefaultConsoleLogFormatterConfigurationFactory : IConsoleLogFormatterConfigurationFactory
    // {
    //     // private readonly IOptionsMonitor<ConsoleLoggerOptions> _optionsMonitor;
    //     public DefaultConsoleLogFormatterConfigurationFactory()//IOptionsMonitor<ConsoleLoggerOptions> optionsMonitor)
    //     {
    //         // if (optionsMonitor == null)
    //         // {
    //         //     throw new ArgumentNullException(nameof(optionsMonitor));
    //         // }
    //         // _optionsMonitor = optionsMonitor;
    //     }
    //     public ILogFormatter CreateJsonFormatter(Action<ILogFormatter> setup)
    //     {
    //         var jformatter = new JsonConsoleLogFormatter();
    //         setup(jformatter);
    //         return jformatter;
    //     }
        
    // }

    // public static class FileLoggerExtensions
    // { 
    //     public static ILoggingBuilder AddJsonLogger(this ILoggingBuilder builder)
    //     {
    //         builder.AddConfiguration();
    
    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>()
    //         );
    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<IConfigureOptions<CompactJsonLoggerOptions>, FileLoggerOptionsSetup>()
    //         );
    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<IOptionsChangeTokenSource<CompactJsonLoggerOptions>, 
    //             LoggerProviderOptionsChangeTokenSource<CompactJsonLoggerOptions, FileLoggerProvider>>()
    //         );
    //         return builder;
    //     }

    //     public static ILoggingBuilder AddConsoleLoggerX(this ILoggingBuilder builder)
    //     {
    //         builder.AddConfiguration();
    //         builder.Services.AddSingleton<IConsoleLogFormatterConfigurationFactory, DefaultConsoleLogFormatterConfigurationFactory>();

    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());
    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<ILogFormatter, JsonConsoleLogFormatter>());
    
    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<IConfigureOptions<ConsoleLoggerOptions>, ConsoleLoggerOptionsSetup>()
    //         );
    //         builder.Services.TryAddEnumerable(
    //             ServiceDescriptor.Singleton<IOptionsChangeTokenSource<ConsoleLoggerOptions>, 
    //             LoggerProviderOptionsChangeTokenSource<ConsoleLoggerOptions, ConsoleLoggerProvider>>()
    //         );
    //         return builder;
    //     }
    
    //     public static ILoggingBuilder AddJsonLogger(this ILoggingBuilder builder, Action<CompactJsonLoggerOptions> configure)
    //     {
    //         if (configure == null)
    //         {
    //             throw new ArgumentNullException(nameof(configure));
    //         }
    
    //         builder.AddJsonLogger();
    //         builder.Services.Configure(configure);
    
    //         return builder;
    //     }
    // }

    // public class LogEntry
    // { 
    //     public LogEntry()
    //     {
    //         TimeStampUtc = DateTime.UtcNow;
    //         UserName = Environment.UserName;
    //     }
    
    //     static public readonly string StaticHostName = "System.Net.Dns.GetHostName()";
    
    //     public string UserName { get; private set; }
    //     public string HostName { get { return StaticHostName; } }
    //     public DateTime TimeStampUtc { get; private set; }
    //     public string Category { get; set; }
    //     public LogLevel Level { get; set; }
    //     public string Text { get; set; }
    //     public Exception Exception { get; set; }
    //     public EventId EventId { get; set; }
    //     public object State { get; set; }
    //     public string StateText { get; set; }
    //     public Dictionary<string, object> StateProperties { get; set; }
    //     public List<LogScopeInfo> Scopes { get; set; }
    // }

    // public class CompactJsonLoggerOptions
    // {    
    //     public CompactJsonLoggerOptions() { }
    //     public LogLevel LogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Information;
    //     public System.Text.Json.JsonWriterOptions JsonWriterOptions { get; set; } = true;
    // }

    // internal class FormatAgnosticConsoleLogger : ILogger
    // { 
    //     public FormatAgnosticConsoleLogger(LoggerProvider Provider, string Category)
    //     {
    //         this.Provider = Provider;
    //         this.Category = Category;
    //     }
    
    //     IDisposable ILogger.BeginScope<TState>(TState state)
    //     {
    //         return Provider.ScopeProvider.Push(state);
    //     }
    
    //     bool ILogger.IsEnabled(LogLevel logLevel)
    //     {
    //         return Provider.IsEnabled(logLevel);
    //     }
    
    //     void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, 
    //         TState state, Exception exception, Func<TState, Exception, string> formatter)
    //     {
    //         if ((this as ILogger).IsEnabled(logLevel))
    //         { 
    //             LogEntry Info = new LogEntry();
    //             Info.Category = this.Category;
    //             Info.Level = logLevel;
    //             // well, the passed default formatter function 
    //             // does not take the exception into account
    //             // SEE: https://github.com/aspnet/Extensions/blob/master/src/Logging/Logging.Abstractions/src/LoggerExtensions.cs
    //             Info.Text = exception?.Message ?? state.ToString(); // formatter(state, exception)
    //             Info.Exception = exception;
    //             Info.EventId = eventId;
    //             Info.State = state;
    
    //             // well, you never know what it really is
    //             if (state is string)   
    //             {
    //                 Info.StateText = state.ToString();
    //             }
    //             // in case we have to do with a message template, 
    //             // let's get the keys and values (for Structured Logging providers)
    //             // SEE: https://docs.microsoft.com/en-us/aspnet/core/
    //             // fundamentals/logging#log-message-template
    //             // SEE: https://softwareengineering.stackexchange.com/
    //             // questions/312197/benefits-of-structured-logging-vs-basic-logging
    //             else if (state is IEnumerable<KeyValuePair<string, object>> Properties)
    //             {
    //                 Info.StateProperties = new Dictionary<string, object>();
    
    //                 foreach (KeyValuePair<string, object> item in Properties)
    //                 {
    //                     Info.StateProperties[item.Key] = item.Value;
    //                 }
    //             }
    
    //             // gather info about scope(s), if any
    //             if (Provider.ScopeProvider != null)
    //             {
    //                 Provider.ScopeProvider.ForEachScope((value, loggingProps) =>
    //                 {
    //                     if (Info.Scopes == null)
    //                         Info.Scopes = new List<LogScopeInfo>();
    
    //                     LogScopeInfo Scope = new LogScopeInfo();
    //                     Info.Scopes.Add(Scope);
    
    //                     if (value is string)
    //                     {
    //                         Scope.Text = value.ToString();
    //                     }
    //                     else if (value is IEnumerable<KeyValuePair<string, object>> props)
    //                     {
    //                         if (Scope.Properties == null)
    //                             Scope.Properties = new Dictionary<string, object>();
    
    //                         foreach (var pair in props)
    //                         {
    //                             Scope.Properties[pair.Key] = pair.Value;
    //                         }
    //                     }
    //                 },
    //                 state); 
    //             }
    
    //             Provider.WriteLog(Info); 
    //         }
    //     }
    
    //     public LoggerProvider Provider { get; private set; }
    //     public string Category { get; private set; }
    // }

    // public abstract class LoggerProvider : IDisposable, ILoggerProvider, ISupportExternalScope
    // {
    //     private ConcurrentDictionary<string, FormatAgnosticConsoleLogger> loggers = new ConcurrentDictionary<string, FormatAgnosticConsoleLogger>();
    //     private IExternalScopeProvider fScopeProvider;
    //     protected IDisposable SettingsChangeToken;
    
    //     void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
    //     {
    //         fScopeProvider = scopeProvider;
    //     }
    
    //     ILogger ILoggerProvider.CreateLogger(string Category)
    //     {
    //         return loggers.GetOrAdd(Category,
    //         (category) => {
    //             return new FormatAgnosticConsoleLogger(this, category);
    //         });
    //     }
    
    //     void IDisposable.Dispose()
    //     {
    //         if (!this.IsDisposed)
    //         {
    //             try
    //             {
    //                 Dispose(true);
    //             }
    //             catch
    //             {
    //             }
    
    //             this.IsDisposed = true;
    //             GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor   
    //         }
    //     }
    
    //     protected virtual void Dispose(bool disposing)
    //     {
    //         if (SettingsChangeToken != null)
    //         {
    //             SettingsChangeToken.Dispose();
    //             SettingsChangeToken = null;
    //         }
    //     } 
    
    //     public LoggerProvider()
    //     {
    //     }
    
    //     ~LoggerProvider()
    //     {
    //         if (!this.IsDisposed)
    //         {
    //             Dispose(false);
    //         }
    //     }
    
    //     public abstract bool IsEnabled(LogLevel logLevel);
    
    //     public abstract void WriteLog(LogEntry Info);
    
    //     internal IExternalScopeProvider ScopeProvider
    //     {
    //         get
    //         {
    //             if (fScopeProvider == null)
    //                 fScopeProvider = new LoggerExternalScopeProvider();
    //             return fScopeProvider;
    //         }
    //     }
    
    //     public bool IsDisposed { get; protected set; }
    // }

    // // [Microsoft.Extensions.Logging.ProviderAlias("File")]
    // public class FileLoggerProvider : LoggerProvider
    // { 
    //     private void WriteLine(string Text) { }
    
    //     private void WriteLogLine()
    //     {
    //         // get LogEntry from Q -> then -> print to your output with a desired format and ordering
    //     }
    
    //     protected override void Dispose(bool disposing)
    //     {
    //         base.Dispose(disposing);
    //     }


    //     public FileLoggerProvider(IOptionsMonitor<CompactJsonLoggerOptions> Settings)
    //         : this(Settings.CurrentValue)
    //     {  
    //         // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/change-tokens
    //         SettingsChangeToken = Settings.OnChange(settings => {      
    //             this.Settings = settings;                  
    //         });
    //     }

    //     public FileLoggerProvider(CompactJsonLoggerOptions Settings)
    //     {
    //         this.Settings = Settings;
    
    //         // create the first file
    //         // BeginFile();
    
    //         // ThreadProc();
    //         JustSetup();
    //     }

    //     private void JustSetup()
    //     {
    //         // things like Q processor and thread setup
    //     }

    //     public FileLoggerProvider()
    //     {
    //         CompactJsonLoggerOptions Settings = new CompactJsonLoggerOptions()
    //         {
    //             LogLevel = LogLevel.Information,
    //             MaxFileSizeInMB = 5
    //         };
    //         this.Settings = Settings;

    //         // create the first file
    //         // BeginFile();

    //         // ThreadProc();
    //         JustSetup();
    //     }

    //     public override bool IsEnabled(LogLevel logLevel)
    //     {
    //         bool Result = logLevel != LogLevel.None
    //             && this.Settings.LogLevel != LogLevel.None
    //             && Convert.ToInt32(logLevel) >= Convert.ToInt32(this.Settings.LogLevel);
    
    //         return Result;
    //     }
    
    //     public override void WriteLog(LogEntry Info)
    //     {
    //         // InfoQueue.Enqueue(Info); // pass to Q
    //     } 
    
    //     internal CompactJsonLoggerOptions Settings { get; private set; } 
    // }

    // public class LogScopeInfo
    // { 
    //     public LogScopeInfo()
    //     {
    //     }
    
    //     public string Text { get; set; }
    //     public Dictionary<string, object> Properties { get; set; }
    // }
    
    // internal class JsonLoggerOptionsSetup : ConfigureFromConfigurationOptions<CompactJsonLoggerOptions>
    // {
    //     public JsonLoggerOptionsSetup(ILoggerProviderConfiguration<FileLoggerProvider> 
    //                                 providerConfiguration)
    //         : base(providerConfiguration.Configuration)
    //     {
    //     }
    // }
    
    // internal class ConsoleLoggerOptionsSetup : ConfigureFromConfigurationOptions<ConsoleLoggerOptions>
    // {
    //     public ConsoleLoggerOptionsSetup(ILoggerProviderConfiguration<ConsoleLoggerProvider> 
    //                                 providerConfiguration)
    //         : base(providerConfiguration.Configuration)
    //     {
    //     }
    }
}
