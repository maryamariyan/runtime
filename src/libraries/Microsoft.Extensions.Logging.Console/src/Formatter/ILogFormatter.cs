// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    public interface ILogFormatter
    {
        string Name { get; }
        LogMessageEntry Format(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IExternalScopeProvider scopeProvider);
        LogMessageEntry Format<TState>(LogLevel logLevel, string logName, int eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, IExternalScopeProvider scopeProvider);
    }
}
