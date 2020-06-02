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

    internal readonly struct LME
    {
        public LME(string message, ConsoleColor? background = null, ConsoleColor? foreground = null, ColoredMessage? prev = null)
        {
            Current = new ColoredMessage(message, background, foreground);
            Previous = prev;
        }
        public readonly ColoredMessage Current;
        public readonly ColoredMessage? Previous;
    }

    internal readonly struct ColoredMessage
    {
        public ColoredMessage(string message, ConsoleColor? background = null, ConsoleColor? foreground = null)
        {
            Message = message;
            Background = background;
            Foreground = foreground;
        }
        public readonly string Message;
        public readonly ConsoleColor? Background;
        public readonly ConsoleColor? Foreground;
    }
}
