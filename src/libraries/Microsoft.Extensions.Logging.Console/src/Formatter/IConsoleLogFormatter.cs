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
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        LogMessageEntry Format<TState>(LogLevel logLevel, string logName, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider);
    }
}
