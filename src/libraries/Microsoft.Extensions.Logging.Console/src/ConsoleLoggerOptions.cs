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
    }
}
