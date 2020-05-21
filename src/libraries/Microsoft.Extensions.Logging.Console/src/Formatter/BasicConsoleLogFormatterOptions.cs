// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    public class BasicConsoleLogFormatterOptions
    {
        public BasicConsoleLogFormatterOptions() { }

        public bool IncludeScopes { get; set; }
        
        public Microsoft.Extensions.Logging.LogLevel LogToStandardErrorThreshold { get; set; }
        
        public string TimestampFormat { get; set; }
        
        public bool UseUtcTimestamp { get; set; }
    }
}
