﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Default implementation of <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
    /// </summary>
    public class DefaultServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly ServiceProviderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceProviderFactory"/> class
        /// with default options.
        /// </summary>
        public DefaultServiceProviderFactory() : this(ServiceProviderOptions.Default)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceProviderFactory"/> class
        /// with the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options to use for this instance.</param>
        public DefaultServiceProviderFactory(ServiceProviderOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
        }

        /// <inheritdoc />
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildServiceProvider(_options);
        }
    }
}
