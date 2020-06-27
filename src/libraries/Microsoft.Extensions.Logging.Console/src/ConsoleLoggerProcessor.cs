// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<LogMessageEntry> _messageQueue = new BlockingCollection<LogMessageEntry>(_maxQueuedMessages);
        private readonly Thread _outputThread;

        public IConsole Console;
        public IConsole ErrorConsole;

        public ConsoleLoggerProcessor()
        {
            // Start Console message queue processor
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "Console logger queue processing thread"
            };
            _outputThread.Start();
        }

        public virtual void EnqueueMessage(LogMessageEntry message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            try
            {
                WriteMessage(message);            
            }
            catch (Exception) { }
        }

        // for testing
        internal virtual void WriteMessage(LogMessageEntry entry)
        {
            var console = entry.LogAsError ? ErrorConsole : Console;
            Write(console, entry.Message.AsSpan());
            console.Flush();
        }

        // before parser -> just check if enable VT
        // parser -> support simplest case -> color sequences
        // parser -> skip escapes that it doesnt understand

        private void Write(IConsole console, ReadOnlySpan<char> span)
        {
            // parse out different color messages for Windows
            // TODO: how to know if it is enabled on windows, console mode.
            if (console is AnsiLogConsole)
            {
                console.Write(span, null, null);
                return;
            }
            const char EscapeChar = '\x1B';
            (int startIndex, int length, ConsoleColor? bg, ConsoleColor? fg) content = (-1, 0, null, null);
            ConsoleColor? color = null;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] != EscapeChar || span.Length < i + 3 || span[i + 1] != '[')
                {
                    {
                        content.length++;
                        if (content.startIndex == -1)
                        {
                            content.startIndex = i;
                        }
                    }
                }
                else if (span[i + 3] == 'm')
                {
                    // not an escape char // check more than 3 or 4
                    if (int.TryParse(span.Slice(i + 2, length: 1), out int escapeCode))
                        i += 3;
                }
                else if (span[i + 4] == 'm')
                {
                    if (int.TryParse(span.Slice(i + 2, length: 2), out int escapeCode))
                    {
                        // todo support dark colors
                        if (GetForegroundColor(escapeCode, false, out color))
                        {
                            // time for new light color fg
                            if (content.startIndex != -1)
                            {
                                console.Write(span.Slice(content.startIndex, content.length), content.bg, content.fg);
                                content.startIndex = -1;
                                content.length = 0;
                            }
                            content.fg = color;
                        }
                        else if (GetBackgroundColor(escapeCode, out color))
                        {
                            // time for new color bg
                            if (content.startIndex != -1)
                            {
                                console.Write(span.Slice(content.startIndex, content.length), content.bg, content.fg);
                                content.startIndex = -1;
                                content.length = 0;
                            }
                            content.bg = color;
                        }
                        i += 4;
                    }
                }
            }
            if (content.startIndex != -1)
            {
                console.Write(span.Slice(content.startIndex, content.length), content.bg, content.fg);
            }
        }

        private static bool GetForegroundColor(int number, bool isDark, out ConsoleColor? color)
        {
            switch (number)
            {
                case 30:
                    color = ConsoleColor.Black;
                    return true;
                case 31:
                    color = isDark ? ConsoleColor.DarkRed : ConsoleColor.Red;
                    return true;
                case 32:
                    color = isDark ? ConsoleColor.DarkGreen : ConsoleColor.Green;
                    return true;
                case 33:
                    color = isDark ? ConsoleColor.DarkYellow : ConsoleColor.Yellow;
                    return true;
                case 34:
                    color = isDark ? ConsoleColor.DarkBlue : ConsoleColor.Blue;
                    return true;
                case 35:
                    color = isDark ? ConsoleColor.DarkMagenta : ConsoleColor.Magenta;
                    return true;
                case 36:
                    color = isDark ? ConsoleColor.DarkCyan : ConsoleColor.Cyan;
                    return true;
                case 37:
                    color = isDark ? ConsoleColor.Gray : ConsoleColor.White;
                    return true;
                case 39:
                    color = null;
                    return true;
            }
            color = null;
            return false;
        }

        private static bool GetBackgroundColor(int number, out ConsoleColor? color)
        {
            switch (number)
            {
                case 40:
                    color = ConsoleColor.Black;
                    return true;
                case 41:
                    color = ConsoleColor.Red;
                    return true;
                case 42:
                    color = ConsoleColor.Green;
                    return true;
                case 43:
                    color = ConsoleColor.Yellow;
                    return true;
                case 44:
                    color = ConsoleColor.Blue;
                    return true;
                case 45:
                    color = ConsoleColor.Magenta;
                    return true;
                case 46:
                    color = ConsoleColor.Cyan;
                    return true;
                case 47:
                    color = ConsoleColor.White;
                    return true;
                case 49:
                    color = null;
                    return true;
            }
            color = null;
            return false;
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputThread.Join(1500); // with timeout in-case Console is locked by user input
            }
            catch (ThreadStateException) { }
        }
    }
}
