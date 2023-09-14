// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.Health.Fhir.Common.Interfaces;

namespace Microsoft.Health.Fhir.Common
{
    public class RawResourceElement : IResourceElement
    {
        public RawResource RawResource { get; protected set; }

        public FhirResourceFormat Format { get; protected set; }

        public string Id { get; protected set; }

        public string VersionId { get; protected set; }

        public string InstanceType { get; protected set; }

        public DateTimeOffset? LastUpdated { get; protected set; }
    }
}
