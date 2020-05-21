// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.Logging.Console
{
    public class JsonConsoleLogFormatterOptions : BaseOptions
    {
        public JsonConsoleLogFormatterOptions() { }
        public JsonWriterOptions JsonWriterOptions { get; set; }
        public JsonSerializerOptions JsonSerializerOptions { get; set; }
    }
}
