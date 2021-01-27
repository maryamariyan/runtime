// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options.DataAnnotations.Tests.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Extensions.Options.DataAnnotations.Tests
{
    public class AcceptanceTest
    {
        public static IHostBuilder CreateHostBuilder(Action<IServiceCollection> configure)
        {
            return new HostBuilder().ConfigureServices(configure);
        }

        [Fact]
        public async Task CanValidateOptionsEagerlyWithDefaultError()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = false)
                    .Validate(o => o.Boolean)
                    .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error);
            }
        }

        [Fact]
        public async Task CanValidateOptionsEagerlyWithCustomError()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = false)
                    .Validate(o => o.Boolean, "first Boolean must be true.")
                    .ValidateEagerly();
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = true)
                    .Validate(o => !o.Boolean, "second Boolean must be false.")
                    .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error, 1, "second Boolean must be false.");
            }
        }

        [Fact]
        public async Task CanValidateOptionsEagerThanLazySameType()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = false)
                    .Validate(o => o.Boolean, "first Boolean must be true.")
                    .ValidateEagerly();
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = true)
                    .Validate(o => !o.Boolean, "second Boolean must be false.");
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error, 1, "second Boolean must be false.");
            }
        }

        [Fact]
        public async Task CanValidateOptionsLazyThanEagerSameType()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = false)
                    .Validate(o => o.Boolean, "first Boolean must be true.");
                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = true)
                    .Validate(o => !o.Boolean, "second Boolean must be false.")
                    .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error, 1, "second Boolean must be false.");
            }
        }

        [Fact]
        public async Task CanValidateOptionsLazyThanEagerDifferentTypes()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<NestedOptions>()
                    .Configure(o => o.Integer = 11)
                    .Validate(o => o.Integer > 12, "Integer");

                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = false)
                    .Validate(o => o.Boolean, "first Boolean must be true.")
                    .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error, 1, "first Boolean must be true.");
            }
        }
        [Fact]
        public async Task LoopIt()
        {
            for (int i = 0; i < 100; i++)
            {
                await CanValidateMultipleOptionsEagerly();
            }
        }

        [Fact]
        public async Task CanValidateOptionsEagerThanLazyDifferentTypes()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<NestedOptions>()
                    .Configure(o => o.Integer = 11)
                    .Validate(o => o.Integer > 12, "Integer")
                    .ValidateEagerly();

                services.AddOptions<ComplexOptions>()
                    .Configure(o => o.Boolean = false)
                    .Validate(o => o.Boolean, "first Boolean must be true.");
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<NestedOptions>(error, 1, "Integer");
            }
        }

        [Fact]
        public async Task CanValidateOptionsEagerlyWithMultipleDefaultErrors()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<ComplexOptions>()
                .Configure(o =>
                {
                    o.Boolean = false;
                    o.Integer = 11;
                })
                .Validate(o => o.Boolean)
                .Validate(o => o.Integer > 12)
                .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error, 2);
            }
        }

        [Fact]
        public async Task CanValidateOptionEagerlysWithMixedOverloads()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<ComplexOptions>()
                       .Configure(o =>
                       {
                           o.Boolean = false;
                           o.Integer = 11;
                           o.Virtual = "wut";
                       })
                       .Validate(o => o.Boolean)
                       .Validate(o => o.Virtual == null, "Virtual")
                       .Validate(o => o.Integer > 12, "Integer")
                       .ValidateEagerly();
            });
            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<ComplexOptions>(error, 3, "Virtual", "Integer");
            }
        }

        [Fact]
        public async Task CanValidateEagerlyDataAnnotationsLongSyntax()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<AnnotatedOptions>()
                    .Configure(o =>
                    {
                        o.StringLength = "111111";
                        o.IntRange = 10;
                        o.Custom = "nowhere";
                        o.Dep1 = "Not dep2";
                    })
                    .ValidateDataAnnotations()
                    .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<AnnotatedOptions>(error, 5,
                    "DataAnnotation validation failed for members: 'Required' with the error: 'The Required field is required.'.",
                    "DataAnnotation validation failed for members: 'StringLength' with the error: 'Too long.'.",
                    "DataAnnotation validation failed for members: 'IntRange' with the error: 'Out of range.'.",
                    "DataAnnotation validation failed for members: 'Custom' with the error: 'The field Custom is invalid.'.",
                    "DataAnnotation validation failed for members: 'Dep1,Dep2' with the error: 'Dep1 != Dep2'.");
            }
        }

        [Fact]
        public async Task CanValidateEagerlyMixDataAnnotationsLongSyntax()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<AnnotatedOptions>()
                    .Configure(o =>
                    {
                        o.StringLength = "111111";
                        o.IntRange = 10;
                        o.Custom = "nowhere";
                        o.Dep1 = "Not dep2";
                    })
                    .ValidateDataAnnotations()
                    .Validate(o => o.Custom != "nowhere", "I don't want to go to nowhere!")
                    .ValidateEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<AnnotatedOptions>(error, 6,
                    "DataAnnotation validation failed for members: 'Required' with the error: 'The Required field is required.'.",
                    "DataAnnotation validation failed for members: 'StringLength' with the error: 'Too long.'.",
                    "DataAnnotation validation failed for members: 'IntRange' with the error: 'Out of range.'.",
                    "DataAnnotation validation failed for members: 'Custom' with the error: 'The field Custom is invalid.'.",
                    "DataAnnotation validation failed for members: 'Dep1,Dep2' with the error: 'Dep1 != Dep2'.",
                    "I don't want to go to nowhere!");
            }
        }

        [Fact]
        public async Task CanValidateEagerlyDataAnnotationsShortSyntax()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<AnnotatedOptions>()
                    .Configure(o =>
                    {
                        o.StringLength = "111111";
                        o.IntRange = 10;
                        o.Custom = "nowhere";
                        o.Dep1 = "Not dep2";
                    })
                    .ValidateDataAnnotationsEagerly();
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<AnnotatedOptions>(error, 5,
                    "DataAnnotation validation failed for members: 'Required' with the error: 'The Required field is required.'.",
                    "DataAnnotation validation failed for members: 'StringLength' with the error: 'Too long.'.",
                    "DataAnnotation validation failed for members: 'IntRange' with the error: 'Out of range.'.",
                    "DataAnnotation validation failed for members: 'Custom' with the error: 'The field Custom is invalid.'.",
                    "DataAnnotation validation failed for members: 'Dep1,Dep2' with the error: 'Dep1 != Dep2'.");
            }
        }

        [Fact]
        public async Task CanValidateEagerlyMixDataAnnotationsShortSyntax()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<AnnotatedOptions>()
                    .Configure(o =>
                    {
                        o.StringLength = "111111";
                        o.IntRange = 10;
                        o.Custom = "nowhere";
                        o.Dep1 = "Not dep2";
                    })
                    .ValidateDataAnnotationsEagerly()
                    .Validate(o => o.Custom != "nowhere", "I don't want to go to nowhere!");
            });

            using (var host = hostBuilder.Build())
            {
                var error = await Assert.ThrowsAsync<ReadableOptionsValidationException>(async () =>
                {
                    await host.StartAsync();
                });

                ValidateFailure<AnnotatedOptions>(error, 6,
                    "DataAnnotation validation failed for members: 'Required' with the error: 'The Required field is required.'.",
                    "DataAnnotation validation failed for members: 'StringLength' with the error: 'Too long.'.",
                    "DataAnnotation validation failed for members: 'IntRange' with the error: 'Out of range.'.",
                    "DataAnnotation validation failed for members: 'Custom' with the error: 'The field Custom is invalid.'.",
                    "DataAnnotation validation failed for members: 'Dep1,Dep2' with the error: 'Dep1 != Dep2'.",
                    "I don't want to go to nowhere!");
            }
        }

        [Fact]
        public void Test_WhenValidateEagerlyThrowsIfArgumentNull()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                OptionsBuilder<AnnotatedOptions>? optionsBuilder = null;
#pragma warning disable CS8604 // Possible null reference argument.
                optionsBuilder.ValidateEagerly();
#pragma warning restore CS8604 // Possible null reference argument.
            });

            _ = Assert.Throws<ArgumentNullException>(() => { _ = hostBuilder.Build(); });
        }

        [Fact]
        public async Task Test_IVfalidationSuccessful()
        {
            var hostBuilder = CreateHostBuilder(services =>
            {
                services.AddOptions<AnnotatedOptions>()
                    .Configure(o =>
                    {
                        o.Required = "required";
                        o.StringLength = "1111";
                        o.IntRange = 0;
                        o.Custom = "USA";
                        o.Dep1 = "dep";
                        o.Dep2 = "dep";
                    })
                    .ValidateDataAnnotationsEagerly()
                    .Validate(o => o.Custom != "nowhere", "I don't want to go to nowhere!");
            });

            using (var host = hostBuilder.Build())
            {
                try
                {
                    await host.StartAsync();
                }
    #pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
    #pragma warning restore CA1031 // Do not catch general exception types
                {
                    Assert.True(false, "Expected no exception, but got: " + ex.Message);
                }
            }
        }

        private static void ValidateFailure(Type type, ReadableOptionsValidationException e, int count = 1, params string[] errorsToMatch)
        {
            Assert.Equal(type, e.OptionsType);

            Assert.Equal(count, e.Failures.Count);

            // Check for the error in any of the failures
            foreach (var error in errorsToMatch)
            {
#if NETCOREAPP
                Assert.True(e.Failures.FirstOrDefault(predicate: f => f.Contains(error, StringComparison.CurrentCulture)) != null, "Did not find: " + error);
#else
                Assert.True(e.Failures.FirstOrDefault(predicate: f => f.IndexOf(error, StringComparison.CurrentCulture) >= 0) != null, "Did not find: " + error);
#endif
            }
        }

        private static void ValidateFailure<TOptions>(ReadableOptionsValidationException e, int count = 1, params string[] errorsToMatch)
        {
            ValidateFailure(typeof(TOptions), e, count, errorsToMatch);
        }
    }
}
