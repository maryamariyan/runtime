// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Console
{
    public interface IConsoleMessageBuilder
    {
        IConsoleMessageBuilder ResetColor();
        IConsoleMessageBuilder SetColor(ConsoleColor? background, ConsoleColor?  foreground);
        IConsoleMessageBuilder Append(string message);
        IConsoleMessageBuilder Build();
        void Clear();
        bool LogAsError { get; set; }
    }
}
