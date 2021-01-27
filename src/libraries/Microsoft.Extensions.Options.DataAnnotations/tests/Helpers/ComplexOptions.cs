// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System;

namespace Microsoft.Extensions.Options.DataAnnotations.Tests.Helpers
{
    public class ComplexOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ComplexOptions()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Nested = new NestedOptions();
            Virtual = "complex";
        }

        public static string? StaticProperty { get; set; }
        public static string? ReadOnly => null;

        public NestedOptions Nested { get; set; }
        public int Integer { get; set; }
        public bool Boolean { get; set; }
        public virtual string Virtual { get; set; }
        public object Object { get; set; }

        public string PrivateSetter { get; private set; }
        public string ProtectedSetter { get; protected set; }
        public string InternalSetter { get; internal set; }
        internal string InternalProperty { get; set; }
        internal string InternalReadOnly { get; }

        protected string ProtectedProperty { get; set; }
        protected string ProtectedPrivateSet { get; private set; }
        protected string ProtectedReadOnly { get; }
        private string PrivateProperty { get; set; }
        private string PrivateReadOnly { get; }
    }
}
