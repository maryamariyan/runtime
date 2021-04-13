// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SourceGenerators.Tests;
using Xunit;

namespace Microsoft.Extensions.Logging.Generators.Tests
{
    [ActiveIssue("https://github.com/dotnet/runtime/issues/32743", TestRuntimes.Mono)]
    public class LoggerMessageGeneratorEmitterTests
    {
        [Fact]
        public async Task TestEmitter()
        {
            // The functionality of the resulting code is tested via LoggerMessageGeneratedCodeTests.cs
            string[] sources = Directory.GetFiles("TestClasses");
            foreach (var src in sources)
            {
                var testSourceCode = await File.ReadAllTextAsync(src).ConfigureAwait(false);

                var (d, r) = await RoslynTestUtils.RunGenerator(
                    new LoggerMessageGenerator(),
                    new[] { Assembly.GetAssembly(typeof(ILogger))!, Assembly.GetAssembly(typeof(LoggerMessageAttribute))! },
                    new[] { testSourceCode }).ConfigureAwait(false);

                Assert.Empty(d);
                _ = Assert.Single(r);
            }
        }
    }
}
