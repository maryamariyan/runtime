// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Options for a <see cref="ConsoleLogger"/>.
    /// </summary>
    public class ConsoleLoggerOptions
    {
        /// <summary>
        /// Includes scopes when <see langword="true" />.
        /// </summary>
        public bool IncludeScopes { get; set; }

        /// <summary>
        /// Disables colors when <see langword="true" />.
        /// </summary>
        public bool DisableColors { get; set; }

        /// <summary>
        /// Gets or sets log message format. Defaults to <see cref="ConsoleLoggerFormat.Default" />.
        /// </summary>
        public ConsoleLoggerFormat Format
        {
            get
            {
                try {
                    return (ConsoleLoggerFormat) Enum.Parse(typeof(ConsoleLoggerFormat), Formatter);
                }
                catch (ArgumentException) {
                    return ConsoleLoggerFormat.Default;
                }
            }
            set
            {
                if (value == ConsoleLoggerFormat.Systemd)
                {
                    Formatter = Enum.GetName(typeof(ConsoleLoggerFormat), ConsoleLoggerFormat.Systemd);
                }
                else
                {
                    Formatter = Enum.GetName(typeof(ConsoleLoggerFormat), ConsoleLoggerFormat.Default);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Formatter { get; set; }
        
        /// <summary>
        /// Gets or sets value indicating the minimum level of messaged that would get written to <c>Console.Error</c>.
        /// </summary>
        public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.None;

        /// <summary>
        /// Gets or sets format string used to format timestamp in logging messages. Defaults to <c>null</c>.
        /// </summary>
        public string TimestampFormat { get; set; }

        /// <summary>
        /// Gets or sets indication whether or not UTC timezone should be used to for timestamps in logging messages. Defaults to <c>false</c>.
        /// </summary>
        public bool UseUtcTimestamp { get; set; }

        public JsonWriterOptions JsonWriterOptions { get; set; }
    }
}
