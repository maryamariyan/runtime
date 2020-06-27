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
        private readonly StringBuilder _sbMain;
        internal bool DisableColors { get; set; }

        public override Encoding Encoding => Encoding.Unicode;

        public StringWriter(bool disableColors = false)
        {
            _sbMain = new StringBuilder();
            DisableColors = disableColors;
        }

        public override void Write(char value)
        {
            _sbMain.Append(value);
        }

        public bool SetBackgroundColor(ConsoleColor? background)
        {
            if (DisableColors || background == _colorSettings.bg)
            {
                return false;
            }
            _colorSettings.bg = background;
            if (background.HasValue)
            {
                Write(GetBackgroundColorEscapeCode(background.Value));
            }
            else
            {
                Write(DefaultBackgroundColor);
            }
            return true;
        }

        public bool SetForegroundColor(ConsoleColor? foreground)
        {
            if (DisableColors || foreground == _colorSettings.fg)
            {
                return false;
            }
            _colorSettings.fg = foreground;
            if (foreground.HasValue)
            {
                Write(GetForegroundColorEscapeCode(foreground.Value));
            }
            else
            {
                Write(DefaultForegroundColor);
            }
            return true;
        }

        // todo finally reset color using string writer for all formatters -> dispose?

        private (ConsoleColor? bg, ConsoleColor? fg) _colorSettings = (null, null);

        public void ResetColor()
        {
            SetBackgroundColor(null);
            SetForegroundColor(null);
        }

        private const string DefaultForegroundColor = "\x1B[39m\x1B[22m"; // reset to default foreground color
        private const string DefaultBackgroundColor = "\x1B[49m"; // reset to the background color

        public void WriteAndReset(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            var colorChanged = SetBackgroundColor(background);
            colorChanged = SetForegroundColor(foreground) || colorChanged;

            Write(message);

            if (colorChanged)
            {
                ResetColor();
            }
        }

        public void WriteReplacing(string oldValue, string newValue, string message)
        {
            int len = _sbMain.Length;
            _sbMain.Append(message);
            _sbMain.Replace(oldValue, newValue, len, message.Length);
        }

        public int Length
        {
            get 
            {
                return _sbMain.Length;
            }
        }

        private string _computed = null;
        public string ComputedString
        {
            get 
            {
                if (_sbMain.Length != 0)
                {
                    ResetColor();
                    _computed = _sbMain.ToString();
                    _sbMain.Clear();
                }
                return _computed;
            }
        }

        public char this[int index]
        {
            get
            {
                return _sbMain[index];
            }
        }

        public void Clear()
        {
            if (_sbMain.Length != 0)
            {
                ResetColor();
                _computed = _sbMain.ToString();
                _sbMain.Clear();
            }
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
