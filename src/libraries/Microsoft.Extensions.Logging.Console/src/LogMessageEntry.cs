// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    public readonly struct LogMessageEntry
    {
        public LogMessageEntry(ConsoleMessage[] messages, bool logAsError = false)
        {
            Messages = messages;
            LogAsError = logAsError;
        }

        public readonly ConsoleMessage[] Messages;
        public readonly bool LogAsError;
    }

    public readonly struct ConsoleMessage
    {
        public ConsoleMessage(string message, ConsoleColor? background = null, ConsoleColor? foreground = null)
        {
            Content = message;
            Background = background;
            Foreground = foreground;
        }
        public readonly string Content;
        public readonly ConsoleColor? Background;
        public readonly ConsoleColor? Foreground;
    }
}
