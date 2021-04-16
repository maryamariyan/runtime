// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class CollectionTestExtensions
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "M0")]
        public static partial void M0(ILogger logger);

        [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "M1{p0}")]
        public static partial void M1(ILogger logger, int p0);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "M2{p0}{p1}")]
        public static partial void M2(ILogger logger, int p0, int p1);

        [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "M3{p0}{p1}{p2}")]
        public static partial void M3(ILogger logger, int p0, int p1, int p2);

        [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "M4{p0}{p1}{p2}{p3}")]
        public static partial void M4(ILogger logger, int p0, int p1, int p2, int p3);

        [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "M5{p0}{p1}{p2}{p3}{p4}")]
        public static partial void M5(ILogger logger, int p0, int p1, int p2, int p3, int p4);

        [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "M6{p0}{p1}{p2}{p3}{p4}{p5}")]
        public static partial void M6(ILogger logger, int p0, int p1, int p2, int p3, int p4, int p5);

#if HAS_EXTENDED_SUPPORT
        [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "M7{p0}{p1}{p2}{p3}{p4}{p5}{p6}")]
        public static partial void M7(ILogger logger, int p0, int p1, int p2, int p3, int p4, int p5, int p6);

        [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "M8{p0}{p1}{p2}{p3}{p4}{p5}{p6}{p7}")]
        public static partial void M8(ILogger logger, int p0, int p1, int p2, int p3, int p4, int p5, int p6, int p7);

        [LoggerMessage(EventId = 9, Message = "M8{p0}{p1}")]
        public static partial void M9(ILogger logger, LogLevel level, int p0, System.Exception ex, int p1);
#endif
    }
}
