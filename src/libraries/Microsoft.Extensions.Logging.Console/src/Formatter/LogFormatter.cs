// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    public class LogFormatter
    {
        public virtual string Name { get; }

        public virtual LogMessageEntry Format(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
