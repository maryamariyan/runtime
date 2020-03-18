// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace Microsoft.AspNetCore.Testing
{
    [Repeat]
    public class RepeatTest
    {
        public static int _runCount = 0;

#if !SKIP_FAILING
        [Fact]
        [Repeat(5)]
        public void RepeatLimitIsSetCorrectly()
        {
            Assert.Equal(5, RepeatContext.Current.Limit);
        }

        [Fact]
        [Repeat(5)]
        public void RepeatRunsTestSpecifiedNumberOfTimes()
        {
            Assert.Equal(RepeatContext.Current.CurrentIteration, _runCount);
            _runCount++;
        }

        [Fact]
        public void RepeatCanBeSetOnClass()
        {
            Assert.Equal(10, RepeatContext.Current.Limit);
        }
#endif
    }

#if !SKIP_FAILING
    public class LoggedTestXunitRepeatAssemblyTests
    {
        [Fact]
        public void RepeatCanBeSetOnAssembly()
        {
            Assert.Equal(1, RepeatContext.Current.Limit);
        }
    }
#endif
}
