// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Console
{
    internal class JsonConsoleLogFormatter : IConsoleLogFormatter, IDisposable
    {
        private IDisposable _optionsReloadToken;
        
        [ThreadStatic]
        private static StringBuilder _logBuilder;

        public JsonConsoleLogFormatter(IOptionsMonitor<JsonConsoleLogFormatterOptions> options)
        {
            FormatterOptions = options.CurrentValue;
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        public string Name => ConsoleLogFormatterNames.Json;

        private string WriteJson(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider)//long[] extraData)
        {
            const int DefaultBufferSize = 1024;
            var output = new ArrayBufferWriter<byte>(DefaultBufferSize);
            using (var writer = new Utf8JsonWriter(output, FormatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();


                string timestamp = null;
                var timestampFormat = FormatterOptions.TimestampFormat;
                if (timestampFormat != null)
                {
                    var dateTime = FormatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
                    timestamp = dateTime.ToString(timestampFormat);
                }
                writer.WriteString("timestamp", timestamp);
                writer.WriteNumber("eventId", eventId);
                writer.WriteString("logLevel", GetLogLevelString(logLevel));
                writer.WriteString("category", logName);
                writer.WriteString("message", message);

                if (exception != null)
                {
                    writer.WriteStartObject("exception");
                    writer.WriteString("message", exception.Message.ToString());
                    writer.WriteString("type", exception.GetType().ToString());
                    writer.WriteStartArray("stackTrace");
                    if (exception?.StackTrace != null)
                    {
                        foreach (var xx in exception?.StackTrace?.Split(Environment.NewLine))
                        {
                            JsonSerializer.Serialize<string>(writer, xx);
                        }
                    }
                    writer.WriteEndArray();
                    writer.WriteNumber("hResult", exception.HResult);
                    writer.WriteEndObject();
                }

                GetScopeInformation(writer, scopeProvider);

                writer.WriteEndObject();

                writer.Flush();
            }
            return Encoding.UTF8.GetString(output.WrittenMemory.Span);
        }

        public LogMessageEntry Format<TState>(LogLevel logLevel, string logName, int eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider)
        {
            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                return Format(logLevel, logName, eventId, message, exception, scopeProvider);
            }
            return default;
        }

        private LogMessageEntry Format(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider)
        {
            var logBuilder = _logBuilder;
            _logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            logBuilder.Append(WriteJson(logLevel, logName, eventId, message, exception, scopeProvider));

            var formattedMessage = logBuilder.ToString();
            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _logBuilder = logBuilder;
            
            var messages = new ConsoleMessage[2] {
                new ConsoleMessage(formattedMessage, null, null),
                new ConsoleMessage(Environment.NewLine, null, null)
            };

            return new LogMessageEntry(
                messages: messages,
                logAsError: logLevel >= FormatterOptions.LogToStandardErrorThreshold
            );
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "json_trace";
                case LogLevel.Debug:
                    return "json_debug";
                case LogLevel.Information:
                    return "json_info";
                case LogLevel.Warning:
                    return "json_warn";
                case LogLevel.Error:
                    return "json_fail";
                case LogLevel.Critical:
                    return "json_critical";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private void GetScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider scopeProvider)
        {
            try
            {
                if (FormatterOptions.IncludeScopes && scopeProvider != null)
                {
                    writer.WriteStartObject("scopes");
                    scopeProvider.ForEachScope((scope, state) =>
                    {
                        if (scope is IReadOnlyList<KeyValuePair<string, object>> kvps)
                        {
                            foreach (var kvp in kvps)
                            {
                                if (kvp.Value is string ss)
                                    state.WriteString(kvp.Key, ss);
                                else
                                if (kvp.Value is int ii)
                                    state.WriteNumber(kvp.Key, ii);
                                else
                                {
                                    // check how this work
                                    state.WritePropertyName(kvp.Key);
                                    JsonSerializer.Serialize(state, kvp.Value);
                                }
                            }
                            //state is the writer
                            //JsonSerializer.Serialize(state, scope);
                        }
                        else
                        {
                            state.WriteString("noName", scope.ToString());
                        }
                    }, (writer));
                    writer.WriteEndObject();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Something went wrong" + ex.Message);
            }
        }

        internal JsonConsoleLogFormatterOptions FormatterOptions { get; set; }

        private void ReloadLoggerOptions(JsonConsoleLogFormatterOptions options)
        {
            FormatterOptions = options;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
    }
}
