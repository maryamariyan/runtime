// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Tests;
using Xunit;

namespace Microsoft.Extensions.Logging.Generators.Tests
{
    [ActiveIssue("https://github.com/dotnet/runtime/issues/32743", TestRuntimes.Mono)]
    public class LoggerMessageGeneratorEmitterTests
    {
        [Theory]
        [InlineData("TestClasses")]
#if HAS_EXTENDED_SUPPORT            
        [InlineData("TestClassesDisabled")]
#endif
        public async Task TestEmitter(string folder)
        {
            // The functionality of the resulting code is tested via LoggerMessageGeneratedCodeTests.cs
            string[] sources = Directory.GetFiles(folder);
            foreach (var src in sources)
            {
                var testSourceCode = await File.ReadAllTextAsync(src).ConfigureAwait(false);

                var (d, r) = await RoslynTestUtils.RunGenerator(
                    new LoggerMessageGenerator(),
                    new[] { typeof(ILogger).Assembly, typeof(LoggerMessageAttribute).Assembly },
                    new[] { testSourceCode }).ConfigureAwait(false);

                Assert.Empty(d);
                Assert.Single(r);
            }
        }

        [Fact]
        public async Task TestBaseline_TestWithOneParam_Success()
        {
            string testSourceCode = @"
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class TestWithOneParam
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = ""M0 {a1}"")]
        public static partial void M0(ILogger logger, int a1);
    }
}";
            await VerifyAgainstBaselineUsingFile("TestWithOneParam.generated.txt", testSourceCode);
        }

        [Fact]
        public async Task TestBaseline_TestWithTwoParams_Success()
        {
            string testSourceCode = @"
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class TestWithTwoParams
    {
        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = ""M2{p0}{p1}"")]
        public static partial void M2(ILogger logger, int p0, int p1);
    }
}";
            await VerifyAgainstBaselineUsingFile("TestWithTwoParams.generated.txt", testSourceCode);
        }

#if HAS_EXTENDED_SUPPORT
        [Fact]
        public async Task TestBaseline_TestWithMoreThan6Params_Success()
        {
            string testSourceCode = @"
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class TestWithMoreThan6Params
    {
        [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = ""M9 {p1} {p2} {p3} {p4} {p5} {p6} {p7}"")]
        public static partial void Method9(ILogger logger, int p1, int p2, int p3, int p4, int p5, int p6, int p7);
    }
}";
            await VerifyAgainstBaselineUsingFile("TestWithMoreThan6Params.generated.txt", testSourceCode, conditionallySupported: true);
        }

        [Fact]
        public async Task TestBaseline_TestWithDynamicLogLevel_Success()
        {
            string testSourceCode = @"
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class TestWithDynamicLogLevel
    {
        [LoggerMessage(EventId = 9, Message = ""M9"")]
        public static partial void M9(LogLevel level, ILogger logger);
    }
}";
            await VerifyAgainstBaselineUsingFile("TestWithDynamicLogLevel.generated.txt", testSourceCode, conditionallySupported: true);
        }

        [Fact]
        public async Task TestBaseline_TestWithEnumerableParam_Success()
        {
            string testSourceCode = @"
using System.Collections;
using System.Collections.Generic;
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class TestWithEnumerableParam
    {
        [LoggerMessage(EventId = 12, Level = LogLevel.Error, Message = ""M2{p0}{p1}"")]
        public static partial void LogMethod(ILogger logger, int p0, IEnumerable<int> p1);
    }
}";
            await VerifyAgainstBaselineUsingFile("TestWithEnumerableParam.generated.txt", testSourceCode, conditionallySupported: true);
        }

        [Fact]
        public async Task TestBaseline_CaseInsensitiveArguments_Success()
        {
            string testSourceCode = @"
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    internal static partial class TestWithCaseInsensitiveArguments
    {
        [LoggerMessage(EventId = 12, Level = LogLevel.Error, Message = ""M2 {A1} {a2} {A3} {a4} {A5}"")]
        public static partial void LogMethod(ILogger logger, int a1, int a2, int a3, int a4, int a5);
    }
}";
            await VerifyAgainstBaselineUsingFile("TestWithCaseInsensitiveArguments.generated.txt", testSourceCode, conditionallySupported: true);
        }
#endif

        [Fact]
        public async Task NoLoggerMessageAttributeUsage_NothingGetsGenerated()
        {
            var testSourceCode = $@"
namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{{
    internal static class ClassWithNoLoggerMessageAttributeUsage {{ }}
}}";
            var (d, r) = await RoslynTestUtils.RunGenerator(
                new LoggerMessageGenerator(),
                new[] { typeof(ILogger).Assembly, typeof(LoggerMessageAttribute).Assembly },
                new[] { testSourceCode }).ConfigureAwait(false);

            Assert.Empty(d);
            Assert.Empty(r);
        }

        private async Task VerifyAgainstBaselineUsingFile(string filename, string testSourceCode, bool conditionallySupported = false)
        {
            string folderName = conditionallySupported ? "BaselinesDisabled" : "Baselines";
            string[] expectedLines = await File.ReadAllLinesAsync(Path.Combine(folderName, filename)).ConfigureAwait(false);

            var (d, r) = await RoslynTestUtils.RunGenerator(
                new LoggerMessageGenerator(),
                new[] { typeof(ILogger).Assembly, typeof(LoggerMessageAttribute).Assembly },
                new[] { testSourceCode }).ConfigureAwait(false);

            Assert.Empty(d);
            Assert.Single(r);

            Assert.True(CompareLines(expectedLines, r[0].SourceText,
                out string errorMessage), errorMessage);
        }

        private bool CompareLines(string[] expectedLines, SourceText sourceText, out string message)
        {
            if (expectedLines.Length != sourceText.Lines.Count)
            {
                message = string.Format("Line numbers do not match. Expected: {0} lines, but generated {1}",
                    expectedLines.Length, sourceText.Lines.Count);
                return false;
            }
            int index = 0;
            foreach (TextLine textLine in sourceText.Lines)
            {
                string expectedLine = expectedLines[index];
                if (!expectedLine.Equals(textLine.ToString(), StringComparison.Ordinal))
                {
                    message = string.Format("Line {0} does not match.{1}Expected Line:{1}{2}{1}Actual Line:{1}{3}",
                        textLine.LineNumber, Environment.NewLine, expectedLine, textLine);
                    return false;
                }
                index++;
            }
            message = string.Empty;
            return true;
        }
    }
}
