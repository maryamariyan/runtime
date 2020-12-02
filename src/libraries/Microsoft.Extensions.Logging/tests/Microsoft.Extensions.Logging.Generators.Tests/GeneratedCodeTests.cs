// � Microsoft Corporation. All rights reserved.

namespace Microsoft.Extensions.Logging.Generators.Tests
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class GeneratedCodeTests
    {
        [Fact]
        public void BasicTests()
        {
            var logger = new MockLogger();

            logger.Reset();
            NoNamespace.CouldNotOpenSocket(logger, "microsoft.com");
            Assert.Equal(LogLevel.Critical, logger.LastLogLevel);
            Assert.Null(logger.LastException);
            Assert.Equal("Could not open socket to `microsoft.com`", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            Level1.OneLevelNamespace.CouldNotOpenSocket(logger, "microsoft.com");
            Assert.Equal(LogLevel.Critical, logger.LastLogLevel);
            Assert.Null(logger.LastException);
            Assert.Equal("Could not open socket to `microsoft.com`", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            Level1.Level2.TwoLevelNamespace.CouldNotOpenSocket(logger, "microsoft.com");
            Assert.Equal(LogLevel.Critical, logger.LastLogLevel);
            Assert.Null(logger.LastException);
            Assert.Equal("Could not open socket to `microsoft.com`", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

        }

        [Fact]
        public void EnableTest()
        {
            var logger = new MockLogger();

            logger.Reset();
            logger.Enabled = false;
            NoNamespace.CouldNotOpenSocket(logger, "microsoft.com");
            Assert.Equal(0, logger.CallCount);          // ensure the logger doesn't get called when it is disabled
        }

        [Fact]
        public void ArgTest()
        {
            var logger = new MockLogger();

            logger.Reset();
            ArgTestExtensions.Method1(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M1", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method2(logger, "arg1");
            Assert.Null(logger.LastException);
            Assert.Equal("M2", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method3(logger, "arg1", 2);
            Assert.Null(logger.LastException);
            Assert.Equal("M3 arg1 2", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method4(logger, new InvalidOperationException("A"));
            Assert.Equal("A", logger.LastException!.Message);
            Assert.Equal("M4", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method5(logger, new InvalidOperationException("A"), new InvalidOperationException("B"));
            Assert.Equal("A", logger.LastException!.Message);
            Assert.Equal("M5", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method6(logger, new InvalidOperationException("A"), 2);
            Assert.Equal("A", logger.LastException!.Message);
            Assert.Equal("M6", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method7(logger, 1, new InvalidOperationException("B"));
            Assert.Equal("B", logger.LastException!.Message);
            Assert.Equal("M7", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method8(logger, 1, 2, 3, 4, 5, 6, 7);
            Assert.Equal("M8", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            ArgTestExtensions.Method9(logger, 1, 2, 3, 4, 5, 6, 7);
            Assert.Equal("M9 1 2 3 4 5 6 7", logger.LastFormattedString);
            Assert.Equal(1, logger.CallCount);
        }

        [Fact]
        public void ReadOnlyListTest()
        {
            var logger = new MockLogger();

            logger.Reset();
            ReadOnlyListExtensions.M0(logger);
            TestCollection(0, logger);

            logger.Reset();
            ReadOnlyListExtensions.M1(logger, 0);
            TestCollection(1, logger);

            logger.Reset();
            ReadOnlyListExtensions.M2(logger, 0, 1);
            TestCollection(2, logger);

            logger.Reset();
            ReadOnlyListExtensions.M3(logger, 0, 1, 2);
            TestCollection(3, logger);

            logger.Reset();
            ReadOnlyListExtensions.M4(logger, 0, 1, 2, 3);
            TestCollection(4, logger);

            logger.Reset();
            ReadOnlyListExtensions.M5(logger, 0, 1, 2, 3, 4);
            TestCollection(5, logger);

            logger.Reset();
            ReadOnlyListExtensions.M6(logger, 0, 1, 2, 3, 4, 5);
            TestCollection(6, logger);

            logger.Reset();
            ReadOnlyListExtensions.M7(logger, 0, 1, 2, 3, 4, 5, 6);
            TestCollection(7, logger);
        }

        private static void TestCollection(int expected, MockLogger logger)
        {
            var rol = (logger.LastState as IReadOnlyList<KeyValuePair<string, object?>>)!;
            Assert.NotNull(rol);

            Assert.Equal(expected, rol.Count);
            for (int i = 0; i < expected; i++)
            {
                var kvp = new KeyValuePair<string, object?>($"p{i}", i);
                Assert.Equal(kvp, rol[i]);
            }

            int count = 0;
            foreach (var actual in rol)
            {
                var kvp = new KeyValuePair<string, object?>($"p{count}", count);
                Assert.Equal(kvp, actual);
                count++;
            }

            Assert.Equal(expected, count);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = rol[expected]);
        }

        [Fact]
        public void LevelTests()
        {
            var logger = new MockLogger();

            logger.Reset();
            LevelTestExtensions.M0(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M0", logger.LastFormattedString);
            Assert.Equal(LogLevel.Trace, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M1(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M1", logger.LastFormattedString);
            Assert.Equal(LogLevel.Debug, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M2(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M2", logger.LastFormattedString);
            Assert.Equal(LogLevel.Information, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M3(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M3", logger.LastFormattedString);
            Assert.Equal(LogLevel.Warning, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M4(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M4", logger.LastFormattedString);
            Assert.Equal(LogLevel.Error, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M5(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M5", logger.LastFormattedString);
            Assert.Equal(LogLevel.Critical, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M6(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M6", logger.LastFormattedString);
            Assert.Equal(LogLevel.None, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);

            logger.Reset();
            LevelTestExtensions.M7(logger);
            Assert.Null(logger.LastException);
            Assert.Equal("M7", logger.LastFormattedString);
            Assert.Equal(LogLevel.None, logger.LastLogLevel);
            Assert.Equal(1, logger.CallCount);
        }
    }
}
