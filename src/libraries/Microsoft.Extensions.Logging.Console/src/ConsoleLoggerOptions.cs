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
        [System.ObsoleteAttribute("ConsoleLoggerOptions.DisableColors has been deprecated. Please use ColoredConsoleLogFormatterOptions.DisableColors instead.", false)]
        public bool DisableColors { get; set; }

        /// <summary>
        /// Gets or sets log message format. Defaults to <see cref="ConsoleLoggerFormat.Default" />.
        /// </summary>
        [System.ObsoleteAttribute("ConsoleLoggerOptions.Format has been deprecated. Please use ConsoleLoggerOptions.FormatterName instead.", false)]
        public ConsoleLoggerFormat Format
        {
            get
            {
                if (FormatterName != null && FormatterName.Equals(ConsoleLogFormatterNames.Systemd, StringComparison.OrdinalIgnoreCase))
                    return ConsoleLoggerFormat.Systemd;
                return ConsoleLoggerFormat.Default;
            }
            set
            {
                if (value == ConsoleLoggerFormat.Systemd)
                {
                    FormatterName = ConsoleLogFormatterNames.Systemd;
                }
                else
                {
                    FormatterName = ConsoleLogFormatterNames.Default;
                }
            }
        }

        public string FormatterName { get; set; }

        [System.ObsoleteAttribute("ConsoleLoggerOptions.IncludeScopes has been deprecated..", false)]
        public bool IncludeScopes { get; set; }

        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get; set; }

        [System.ObsoleteAttribute("ConsoleLoggerOptions.TimestampFormat has been deprecated..", false)]
        public string TimestampFormat { get; set; }

        [System.ObsoleteAttribute("ConsoleLoggerOptions.UseUtcTimestamp has been deprecated..", false)]
        public bool UseUtcTimestamp { get; set; }
    }
}
