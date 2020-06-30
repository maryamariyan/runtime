// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Allows custom log messages formatting
    /// </summary>
    public abstract class ConsoleFormatter
    {
        protected ConsoleFormatter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name associated with the console log formatter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Formats a log message at the specified log level.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="scopeProvider">The provider of scope data.</param>
        /// <param name="textWriter">The string writer embedding ansi code for colors.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
    }
}
