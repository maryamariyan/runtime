// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeAnalysis;

namespace Microsoft.Extensions.Logging.Generators
{
    internal static class DiagDescriptors
    {
        public static DiagnosticDescriptor ErrorInvalidMethodName { get; } = new (
            id: "LG0000",
            title: new LocalizableResourceString(nameof(SR.ErrorInvalidMethodNameTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorInvalidMethodNameMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor DontMentionLogLevelInMessage { get; } = new (
            id: "LG0001",
            title: new LocalizableResourceString(nameof(SR.DontMentionLogLevelInMessageTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.DontMentionLogLevelInMessageMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorInvalidParameterName { get; } = new (
            id: "LG0002",
            title: new LocalizableResourceString(nameof(SR.ErrorInvalidParameterNameTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorInvalidParameterNameMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorNestedType { get; } = new (
            id: "LG0003",
            title: new LocalizableResourceString(nameof(SR.ErrorNestedTypeTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorNestedTypeMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorMissingRequiredType { get; } = new (
            id: "LG0004",
            title: new LocalizableResourceString(nameof(SR.ErrorMissingRequiredTypeTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorMissingRequiredTypeMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorEventIdReuse { get; } = new (
            id: "LG0005",
            title: new LocalizableResourceString(nameof(SR.ErrorEventIdReuseTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorEventIdReuseMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorInvalidMethodReturnType { get; } = new (
            id: "LG0006",
            title: new LocalizableResourceString(nameof(SR.ErrorInvalidMethodReturnTypeTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorInvalidMethodReturnTypeMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorMissingLogger { get; } = new (
            id: "LG0007",
            title: new LocalizableResourceString(nameof(SR.ErrorMissingLoggerTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorMissingLoggerMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorNotStaticMethod { get; } = new (
            id: "LG0008",
            title: new LocalizableResourceString(nameof(SR.ErrorNotStaticMethodTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorNotStaticMethodMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorNotPartialMethod { get; } = new (
            id: "LG0009",
            title: new LocalizableResourceString(nameof(SR.ErrorNotPartialMethodTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorNotPartialMethodMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorMethodIsGeneric { get; } = new (
            id: "LG0010",
            title: new LocalizableResourceString(nameof(SR.ErrorMethodIsGenericTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorMethodIsGenericMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor RedundantQualifierInMessage { get; } = new (
            id: "LG0011",
            title: new LocalizableResourceString(nameof(SR.RedundantQualifierInMessageTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.RedundantQualifierInMessageMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor PassingDateTime { get; } = new (
            id: "LG0012",
            title: new LocalizableResourceString(nameof(SR.PassingDateTimeTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.PassingDateTimeMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor DontMentionExceptionInMessage { get; } = new (
            id: "LG0013",
            title: new LocalizableResourceString(nameof(SR.DontMentionExceptionInMessageTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.DontMentionExceptionInMessageMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor TemplateHasNoCorrespondingArgument { get; } = new (
            id: "LG0014",
            title: new LocalizableResourceString(nameof(SR.TemplateHasNoCorrespondingArgumentTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.TemplateHasNoCorrespondingArgumentMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ArgumentHasNoCorrespondingTemplate { get; } = new (
            id: "LG0015",
            title: new LocalizableResourceString(nameof(SR.ArgumentHasNoCorrespondingTemplateTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ArgumentHasNoCorrespondingTemplateMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorMethodHasBody { get; } = new (
            id: "LG0016",
            title: new LocalizableResourceString(nameof(SR.ErrorMethodHasBodyTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorMethodHasBodyMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ErrorMissingLogLevel { get; } = new (
            id: "LG0017",
            title: new LocalizableResourceString(nameof(SR.ErrorMissingLogLevelTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.ErrorMissingLogLevelMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor DontMentionLoggerInMessage { get; } = new (
            id: "LG0018",
            title: new LocalizableResourceString(nameof(SR.DontMentionLoggerInMessageTitle), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.DontMentionLoggerInMessageMessage), SR.ResourceManager, typeof(FxResources.Microsoft.Extensions.Logging.Generator.SR)),
            category: "LoggingGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
