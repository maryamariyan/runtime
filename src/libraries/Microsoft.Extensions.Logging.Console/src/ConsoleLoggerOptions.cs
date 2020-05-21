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
    public class ConsoleLoggerOptions : BaseOptions
    {
        [System.ObsoleteAttribute("ConsoleLoggerOptions.DisableColors has been deprecated. Please use DefaultConsoleLogFormatterOptions.DisableColors instead.", false)]
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

        /// <summary>
        /// 
        /// </summary>
        public string FormatterName { get; set; }
    }

    public abstract class BaseOptions
    {
        public bool IncludeScopes { get; set; }
        
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get; set; }
        
        public string TimestampFormat { get; set; }
        
        public bool UseUtcTimestamp { get; set; }
    }
}
