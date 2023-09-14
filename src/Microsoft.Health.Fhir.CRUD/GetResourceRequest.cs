// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using EnsureThat;
using MediatR;
using Microsoft.Health.Fhir.Common;

namespace Microsoft.Health.Fhir.CRUD
{
    public class GetResourceRequest : IRequest<RawResourceElement>
    {
        public GetResourceRequest(ResourceKey resourceKey)
        {
            EnsureArg.IsNotNull(resourceKey, nameof(resourceKey));

            ResourceKey = resourceKey;
        }

        public GetResourceRequest(string type, string id)
        {
            EnsureArg.IsNotNull(type, nameof(type));
            EnsureArg.IsNotNull(id, nameof(id));

            ResourceKey = new ResourceKey(type, id);
        }

        public GetResourceRequest(string type, string id, string versionId)
        {
            EnsureArg.IsNotNull(type, nameof(type));
            EnsureArg.IsNotNull(id, nameof(id));
            EnsureArg.IsNotNull(versionId, nameof(versionId));

            ResourceKey = new ResourceKey(type, id, versionId);
        }

        public ResourceKey ResourceKey { get; }
    }
}
