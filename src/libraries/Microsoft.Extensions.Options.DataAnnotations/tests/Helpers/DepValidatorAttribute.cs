// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System;

namespace Microsoft.Extensions.Options.DataAnnotations.Tests.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DepValidatorAttribute
        : ValidationAttribute
    {
        public string? Target { get; set; }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            _ = validationContext ?? throw new ArgumentNullException(nameof(validationContext));

            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var dep1 = type.GetProperty("Dep1")?.GetValue(instance);
#pragma warning disable CS8604 // Possible null reference argument.
            var dep2 = type.GetProperty(name: Target)?.GetValue(instance);
#pragma warning restore CS8604 // Possible null reference argument.
            if (dep1 == dep2)
            {
                return ValidationResult.Success!;
            }

            return new ValidationResult("Dep1 != " + Target, new[] { "Dep1", Target });
        }
    }
}
