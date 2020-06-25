// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLogger : ILogger
    {
        private readonly ObjectPool<StringWriter> _objectPool;
        private readonly string _name;
        private readonly ConsoleLoggerProcessor _queueProcessor;

        internal ConsoleLogger(string name, ConsoleLoggerProcessor loggerProcessor)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
            _queueProcessor = loggerProcessor;
            _objectPool = new ObjectPool<StringWriter>(() => new StringWriter());
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        internal IConsoleLogFormatter Formatter { get; set; }
        internal ConsoleLoggerOptions Options { get; set; }

        [ThreadStatic]
        internal static StringWriter _stringWriter;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                try
                {
                    if (_stringWriter == null)
                    {
                        _stringWriter = new StringWriter();
                    }

                    Formatter.Write(logLevel, _name, eventId.Id, state, exception, formatter, ScopeProvider, _stringWriter);
                    /* Formatter.Write:
                     textWriter.write("12:22 ");
                     textWriter.write("info:", green, ..);
                     textWriter.write("this is an info log", blue, null);
                     textWriter.write("<ansiRedCoded>this is in red</ansiRedCoded>")
                     textWriter.SetColor(Color.Red)
...
... 
when I pass 1000 in the buffer, manage it somehow
this is the new message I need to write it would overflow so,
create another chunk and link it to existing and ask for more buffer

                    */
                    // _stringWriter  =>  somehow get to totalBuffer contains not only buffer up top
                    // _stringWriter.Clear() -> would do Return buffer

                    // checking the stringbuilder size. I know length
                    //var samePool = ArrayPool<char>.Shared;
                    //char[] buffer = samePool.Rent(1000); // min length
                    //for (int i = 0; i < sb.Length; i++)
                    //{
                    //copyto buffer
                    //}

                    _queueProcessor.EnqueueMessage(new LogMessageEntry(_stringWriter.Text, logLevel >= Options.LogToStandardErrorThreshold));
                    _stringWriter.Clear();
                }
                finally
                {
                    //samePool.Return(buffer);
                    // don't use the reference to the buffer after returning it!
                }
            }

            // 
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
