// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup
{
    public class ServiceTableTest
    {
        [Theory]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(string))]
        [InlineData(typeof(object))]
        public void Constructor_WithImplementationType_ThrowsIfServiceTypeIsOpenGenericAndImplementationTypeIsClosed(Type type)
        {
            // Arrange
            var serviceDescriptors = new[]
            {
                new ServiceDescriptor(typeof(IList<>), type, ServiceLifetime.Transient)
            };

            // Act and Assert
            ExceptionAssert.ThrowsArgument(
                () => new CallSiteFactory(serviceDescriptors),
                "descriptors",
                $"Open generic service type '{typeof(IList<>)}' requires registering an open generic implementation type.");
        }

        public static TheoryData Constructor_WithInstance_ThrowsIfServiceTypeIsOpenGenericData =>
            new TheoryData<object>
            {
                new List<int>(),
                "Hello world",
                new object()
            };

        [Theory]
        [MemberData(nameof(Constructor_WithInstance_ThrowsIfServiceTypeIsOpenGenericData))]
        public void Constructor_WithInstance_ThrowsIfServiceTypeIsOpenGeneric(object instance)
        {
            // Arrange
            var serviceDescriptors = new[]
            {
                new ServiceDescriptor(typeof(IEnumerable<>), instance)
            };

            // Act and Assert
            var ex = ExceptionAssert.ThrowsArgument(
                () => new CallSiteFactory(serviceDescriptors),
                "descriptors",
                $"Open generic service type '{typeof(IEnumerable<>)}' requires registering an open generic implementation type.");
        }

        [Fact]
        public void Constructor_WithFactory_ThrowsIfServiceTypeIsOpenGeneric()
        {
            // Arrange
            var serviceDescriptors = new[]
            {
                new ServiceDescriptor(typeof(Tuple<>), _ => new Tuple<int>(1), ServiceLifetime.Transient)
            };

            // Act and Assert
            var ex = ExceptionAssert.ThrowsArgument(
                () => new CallSiteFactory(serviceDescriptors),
                "descriptors",
                $"Open generic service type '{typeof(Tuple<>)}' requires registering an open generic implementation type.");
        }
    }
}
