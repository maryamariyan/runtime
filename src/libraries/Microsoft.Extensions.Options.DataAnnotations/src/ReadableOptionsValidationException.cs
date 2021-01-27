// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents an error that occurs when eager options validation fails.
    /// </summary>
    public class ReadableOptionsValidationException : Exception
    {
        /// <inheritdoc/>
        public override string Message { get; }

        /// <summary>
        /// Gets the type of the options that failed.
        /// </summary>
        public Type OptionsType { get; }

        /// <summary>
        /// Gets the validation failures.
        /// </summary>
        public IReadOnlyCollection<string> Failures { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadableOptionsValidationException"/> class.
        /// </summary>
        /// <param name="e"><see cref="OptionsValidationException"/> error to construct from.</param>
        public ReadableOptionsValidationException(OptionsValidationException e)
        {
            var optionsException = e ?? throw new ArgumentNullException(nameof(e));

            Failures = optionsException.Failures.ToList();
            OptionsType = optionsException.OptionsType;

            Message = CreateExceptionMessage();
        }

        private string CreateExceptionMessage()
        {
            var sb = new StringBuilder();
            foreach (var failure in Failures)
            {
                _ = sb.Append(failure)
                      .Append(Environment.NewLine);
            }

            return sb.Append($"  at {OptionsType.FullName}").ToString();
        }
    }
}
