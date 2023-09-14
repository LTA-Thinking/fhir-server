// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.Health.Fhir.Common
{
    public interface IResourceElement
    {
        string Id { get; }

        string VersionId { get; }

        string InstanceType { get; }

        DateTimeOffset? LastUpdated { get; }
    }
}
