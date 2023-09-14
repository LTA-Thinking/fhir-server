// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Fhir.Common
{
    public static class OperationOutcomeConstants
    {
#pragma warning disable CA1034 // Nested types should not be visible
        public static class IssueSeverity
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public const string Error = nameof(Error);
            public const string Fatal = nameof(Fatal);
            public const string Information = nameof(Information);
            public const string Warning = nameof(Warning);
        }

#pragma warning disable CA1034 // Nested types should not be visible
        public static class IssueType
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public const string Conflict = nameof(Conflict);
            public const string Duplicate = nameof(Duplicate);
            public const string Exception = nameof(Exception);
            public const string Forbidden = nameof(Forbidden);
            public const string Incomplete = nameof(Incomplete);
            public const string Informational = nameof(Informational);
            public const string Invalid = nameof(Invalid);
            public const string NotFound = nameof(NotFound);
            public const string NotSupported = nameof(NotSupported);
            public const string Processing = nameof(Processing);
            public const string Required = nameof(Required);
            public const string Security = nameof(Security);
            public const string Structure = nameof(Structure);
            public const string Throttled = nameof(Throttled);
            public const string Timeout = nameof(Timeout);
            public const string TooCostly = nameof(TooCostly);
        }
    }
}
