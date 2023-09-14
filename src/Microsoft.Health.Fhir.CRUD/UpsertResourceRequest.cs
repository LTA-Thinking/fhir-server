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
    public class UpsertResourceRequest : IRequest<SaveOutcome>, IRequest
    {
        public UpsertResourceRequest(ResourceElement resource)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            Resource = resource;
        }

        public ResourceElement Resource { get; }
    }
}
