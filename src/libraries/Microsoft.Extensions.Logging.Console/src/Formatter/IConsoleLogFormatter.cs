// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    public interface IConsoleLogFormatter
    {

        /// <summary>
        /// Gets the name associated with the console log formatter.
        /// </summary>
        string Name { get; }

        IConsoleLogFormatterOptions UpdateWith(IConsoleLogFormatterOptions options);

        /// <summary>
        /// Formats a log message at the specified log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="logName">The log name.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <see cref="string"/> message of the <paramref name="state"/> and <paramref name="exception"/>.</param>
        /// <param name="scopeProvider">The provider of scope data.</param>
        /// <param name="consoleMessageBuilder">The provider of scope data.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        void Format<TState>(LogLevel logLevel, string logName, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider, IConsoleMessageBuilder consoleMessageBuilder);
    }

    public interface IConsoleLogFormatterOptions
    {
        /// <summary>
        /// Includes scopes when <see langword="true" />.
        /// </summary>
        bool IncludeScopes { get; set; }

        /// <summary>
        /// Gets or sets value indicating the minimum level of messages that would get written to <c>Console.Error</c>.
        /// </summary>
        Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get; set; }
        
        /// <summary>
        /// Gets or sets format string used to format timestamp in logging messages. Defaults to <c>null</c>.
        /// </summary>
        string TimestampFormat { get; set; }
        
        /// <summary>
        /// Gets or sets indication whether or not UTC timezone should be used to for timestamps in logging messages. Defaults to <c>false</c>.
        /// </summary>
        bool UseUtcTimestamp { get; set; }
    }
}
