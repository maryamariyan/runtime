// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private void Write(IConsole console, ReadOnlySpan<char> span)
        {
            // parse out different color messages for Windows
            if (console is WindowsLogConsole)// AnsiLogConsole
            {
                console.Write(span, null, null);
                return;
            }
            const char escapeChar = '\x1B';
            int curEscapeIndex = -1;
            ConsoleColor? fg = null, bg = null;
            int contentIndex = -1;
            int contentLength = 0;
            int colorCodeLength = 0;
            ConsoleColor? color = default;
            for (int i = 0; i < span.Length; i++)
            {
                if (curEscapeIndex == -1 && span[i] == escapeChar)
                {
                    colorCodeLength++;
                    curEscapeIndex = i;
                    continue;
                }
                if (curEscapeIndex != -1)
                {
                    colorCodeLength++;
                    if (span[i] == 'm' && span[i - 3] == '[')
                    {
                        if (GetForegroundColor(span.Slice(curEscapeIndex, colorCodeLength), out color))
                        {
                            if (contentIndex != -1)
                            {
                                console.Write(span.Slice(contentIndex, contentLength), bg, fg);
                                contentIndex = -1;
                                contentLength = 0;
                            }
                            fg = color;
                            curEscapeIndex = -1;
                            colorCodeLength = 0;
                            continue;
                        }
                        if (GetBackgroundColor(span.Slice(curEscapeIndex, colorCodeLength), out color))
                        {
                            if (contentIndex != -1)
                            {
                                console.Write(span.Slice(contentIndex, contentLength), bg, fg);
                                contentIndex = -1;
                                contentLength = 0;
                            }
                            bg = color;
                            curEscapeIndex = -1;
                            colorCodeLength = 0;
                            continue;
                        }
                    }
                }
                else
                {
                    contentLength++;
                    if (contentIndex == -1)
                    {
                        contentIndex = i;
                    }
                }
            }
            if (contentIndex != -1)
            {
                console.Write(span.Slice(contentIndex, contentLength), bg, fg);
            }
        }

        private static bool GetForegroundColor(ReadOnlySpan<char> text, out ConsoleColor? color)
        {
            int number = 0;
            if (text.Length == 5)
            {
                if (text[0] == '\x1B' && text[1] == '[' && text[4] == 'm' &&
                    int.TryParse(text.Slice(start: 2, length: 2), out number))
                {
                    switch (number)
                    {
                        case 30:
                            color = ConsoleColor.Black;
                            return true;
                        case 31:
                            color = ConsoleColor.DarkRed;
                            return true;
                        case 32:
                            color = ConsoleColor.DarkGreen;
                            return true;
                        case 33:
                            color = ConsoleColor.DarkYellow;
                            return true;
                        case 34:
                            color = ConsoleColor.DarkBlue;
                            return true;
                        case 35:
                            color = ConsoleColor.DarkMagenta;
                            return true;
                        case 36:
                            color = ConsoleColor.DarkCyan;
                            return true;
                        case 37:
                            color = ConsoleColor.Gray;
                            return true;
                    }
                }
            }
            else if (text.Length == 9)
            {
                if (text[0] == '\x1B' && text[1] == '[' && text[2] == '1' && text[3] == 'm' &&
                    text[4] == '\x1B' && text[5] == '[' && text[8] == 'm' && int.TryParse(text.Slice(start: 6, length: 2), out number))
                {
                    switch (number)
                    {
                        case 31:
                            color = ConsoleColor.Red;
                            return true;
                        case 32:
                            color = ConsoleColor.Green;
                            return true;
                        case 33:
                            color = ConsoleColor.Yellow;
                            return true;
                        case 34:
                            color = ConsoleColor.Blue;
                            return true;
                        case 35:
                            color = ConsoleColor.Magenta;
                            return true;
                        case 36:
                            color = ConsoleColor.Cyan;
                            return true;
                        case 37:
                            color = ConsoleColor.White;
                            return true;
                    }
                }
            }
            else if (text.Length == 10)
            {
                if (text[0] == '\x1B' && text[1] == '[' && int.TryParse(text.Slice(start: 2, length: 2), out var firstNumber) && text[4] == 'm' &&
                       text[5] == '\x1B' && text[6] == '[' && text[9] == 'm' && int.TryParse(text.Slice(start: 7, length: 2), out number))
                {
                    //\x1B[39m\x1B[22m
                    if (firstNumber == 39 && number == 22)
                    {
                        color = default;
                        return true;
                    }
                }
            }

            color = default;
            return false;
        }
        private static bool GetBackgroundColor(ReadOnlySpan<char> text, out ConsoleColor? color)
        {
            int number = 0;
            if (text.Length == 5)
            {
                if (text[0] == '\x1B' && text[1] == '[' && text[4] == 'm' &&
                    int.TryParse(text.Slice(start: 2, length: 2), out number))
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
                            color = default;
                            return true;
                    }
                }
            }

            color = default;
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
