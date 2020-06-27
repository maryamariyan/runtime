// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    internal readonly struct ConsoleMessage
    {
        public ConsoleMessage(string message, ConsoleColor? background = null, ConsoleColor? foreground = null)
        {
            Message = message;
            Background = background;
            Foreground = foreground;
        }
        public string Message { get; }
        public ConsoleColor? Background { get; }
        public ConsoleColor? Foreground { get; }
    }
}