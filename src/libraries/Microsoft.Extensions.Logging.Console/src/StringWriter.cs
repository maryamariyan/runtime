// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.Logging.Console
{

    internal class StringWriter : TextWriter
    {
        private readonly bool _ignoreColors;
        private readonly StringBuilder _sbMain;

        public override Encoding Encoding => Encoding.Unicode;

        public StringWriter(bool ignoreColors = false)
        {
            _sbMain = new StringBuilder();
            _ignoreColors = ignoreColors;
        }

        public override void Write(char value)
        {
            _sbMain.Append(value);
        }

        public override void Write(string value)
        {
            _sbMain.Append(value);
        }

        public void SetColor(ConsoleColor? background, ConsoleColor? foreground)
        {
            if (!_ignoreColors)
            {
                if (background.HasValue)
                {
                    _sbMain.Append(GetBackgroundColorEscapeCode(background.Value));
                }
                
                if (foreground.HasValue)
                {
                    _sbMain.Append(GetForegroundColorEscapeCode(foreground.Value));
                }
            }
        }

        private const string DefaultForegroundColor = "\x1B[39m\x1B[22m"; // reset to default foreground color
        private const string DefaultBackgroundColor = "\x1B[49m"; // reset to the background color

        // public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
        // {
        //     SetColor(background, foreground);
        //     Write(message);
        //     SetColor(DefaultBackgroundColor, DefaultForegroundColor);
        // }

        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            // Order: backgroundcolor, foregroundcolor, Message, reset foregroundcolor, reset backgroundcolor
            if (background.HasValue)
            {
                _sbMain.Append(GetBackgroundColorEscapeCode(background.Value));
            }

            if (foreground.HasValue)
            {
                _sbMain.Append(GetForegroundColorEscapeCode(foreground.Value));
            }

            _sbMain.Append(message);

            if (foreground.HasValue)
            {
                _sbMain.Append("\x1B[39m\x1B[22m"); // reset to default foreground color
            }

            if (background.HasValue)
            {
                _sbMain.Append("\x1B[49m"); // reset to the background color
            }
        }

        public string Text
        {
            get 
            {
                // for special case only one chunk dont do ToString and use the array
                return _sbMain.ToString();
            }
        }

        // instead use buffer char[] expose length ctor inject buffer

        public void Clear()
        {
            _sbMain.Clear();
            if (_sbMain.Capacity > 1024)
            {
                _sbMain.Capacity = 1024;
            }
        }

        private static string GetForegroundColorEscapeCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1B[30m";
                case ConsoleColor.DarkRed:
                    return "\x1B[31m";
                case ConsoleColor.DarkGreen:
                    return "\x1B[32m";
                case ConsoleColor.DarkYellow:
                    return "\x1B[33m";
                case ConsoleColor.DarkBlue:
                    return "\x1B[34m";
                case ConsoleColor.DarkMagenta:
                    return "\x1B[35m";
                case ConsoleColor.DarkCyan:
                    return "\x1B[36m";
                case ConsoleColor.Gray:
                    return "\x1B[37m";
                case ConsoleColor.Red:
                    return "\x1B[1m\x1B[31m";
                case ConsoleColor.Green:
                    return "\x1B[1m\x1B[32m";
                case ConsoleColor.Yellow:
                    return "\x1B[1m\x1B[33m";
                case ConsoleColor.Blue:
                    return "\x1B[1m\x1B[34m";
                case ConsoleColor.Magenta:
                    return "\x1B[1m\x1B[35m";
                case ConsoleColor.Cyan:
                    return "\x1B[1m\x1B[36m";
                case ConsoleColor.White:
                    return "\x1B[1m\x1B[37m";
                default:
                    return DefaultForegroundColor; // default foreground color
            }
        }

        private static string GetBackgroundColorEscapeCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1B[40m";
                case ConsoleColor.Red:
                    return "\x1B[41m";
                case ConsoleColor.Green:
                    return "\x1B[42m";
                case ConsoleColor.Yellow:
                    return "\x1B[43m";
                case ConsoleColor.Blue:
                    return "\x1B[44m";
                case ConsoleColor.Magenta:
                    return "\x1B[45m";
                case ConsoleColor.Cyan:
                    return "\x1B[46m";
                case ConsoleColor.White:
                    return "\x1B[47m";
                default:
                    return DefaultBackgroundColor; // Use default background color
            }
        }
    }
}