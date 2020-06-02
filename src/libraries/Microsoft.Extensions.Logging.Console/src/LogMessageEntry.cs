// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    internal readonly struct LogMessageEntry
    {
        public LogMessageEntry(string message, string timeStamp = null, string levelString = null, ConsoleColor? levelBackground = null, ConsoleColor? levelForeground = null, ConsoleColor? messageColor = null, bool logAsError = false)
        {
            TimeStamp = timeStamp;
            LevelString = levelString;
            LevelBackground = levelBackground;
            LevelForeground = levelForeground;
            MessageColor = messageColor;
            Message = message;
            LogAsError = logAsError;
        }

        public readonly string TimeStamp;
        public readonly string LevelString;
        public readonly ConsoleColor? LevelBackground;
        public readonly ConsoleColor? LevelForeground;
        public readonly ConsoleColor? MessageColor;
        public readonly string Message;
        public readonly bool LogAsError;
    }
    internal readonly struct LogMessageEntry2
    {
        public LogMessageEntry2(ConsoleMessage[] messages, bool logAsError = false)
        {
            Messages = messages;
            LogAsError = logAsError;
        }

        public readonly ConsoleMessage[] Messages;
        public readonly bool LogAsError;
    }

    internal readonly struct ConsoleMessage
    {
        public ConsoleMessage(string message, ConsoleColor? background = null, ConsoleColor? foreground = null, bool logAsError = false)
        {
            Message = message;
            Background = background;
            Foreground = foreground;
            LogAsError = logAsError;
        }
        public readonly string Message;
        public readonly ConsoleColor? Background;
        public readonly ConsoleColor? Foreground;
        public readonly bool LogAsError;
    }
}
