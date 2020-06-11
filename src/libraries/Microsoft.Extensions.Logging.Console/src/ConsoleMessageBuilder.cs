// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleMessageBuilder : IConsoleMessageBuilder
    {
        private ConsoleColor? DefaultForegroundColor = ConsoleColor.Gray; // "\x1B[39m\x1B[22m";
        private ConsoleColor? DefaultBackgroundColor = ConsoleColor.Black; //"\x1B[49m";
        private ConsoleLoggerProcessor _queueProcessor;
        
        public ConsoleMessageBuilder(ConsoleLoggerProcessor queueProcessor, bool logAsError = false)
        {
            _queueProcessor = queueProcessor;
            LogAsError = logAsError;
        }

        public IConsoleMessageBuilder ResetColor()
        {
            return SetColor(DefaultBackgroundColor, DefaultForegroundColor);
        }

        public IConsoleMessageBuilder SetColor(ConsoleColor? background, ConsoleColor?  foreground)
        {
            if (WouldBeAColorChange(background, foreground))
            {
                var logBuilder = _sb;
                _sb = null;

                if (logBuilder != null && logBuilder.Length != 0)
                {
                    var message = logBuilder.ToString();
                    _queueProcessor.EnqueueMessage(new ConsoleMessage(message, _colorSetting.bg, _colorSetting.fg, LogAsError));
                    logBuilder.Clear();

                    if (logBuilder.Capacity > 1024)
                    {
                        logBuilder.Capacity = 1024;
                    }
                }
                _sb = logBuilder;

                _colorSetting.bg = background;
                _colorSetting.fg = foreground;
            }
            return this;
        }
        private bool WouldBeAColorChange(ConsoleColor? background, ConsoleColor?  foreground)
        {
            // compare against _colorSettings
            // throw new NotImplementedException("TODO: " + nameof(WouldBeAColorChange));
            return true;
        }

        private (ConsoleColor? bg, ConsoleColor? fg) _colorSetting = (null, null);

        [ThreadStatic]
        private static StringBuilder _sb = new StringBuilder();

        public IConsoleMessageBuilder Append(string message)
        {
            var logBuilder = _sb;
            _sb = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }
            logBuilder.Append(message);

            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _sb = logBuilder;
            return this;
        }

        public bool LogAsError { get; set; }

        public IConsoleMessageBuilder Build()
        {
            var logBuilder = _sb;
            _sb = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }
            var message = logBuilder.ToString();
            _queueProcessor.EnqueueMessage(new ConsoleMessage(message, _colorSetting.bg, _colorSetting.fg, LogAsError));

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _sb = logBuilder;
            ResetColor();
            return this;
        }

        public void Clear()
        {
            var logBuilder = _sb;
            _sb = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }
            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _sb = logBuilder;
            ResetColor();
        }
    }
}
