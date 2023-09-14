// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Fhir.Common
{
    public sealed class KnownRoutes
    {
        public const string ResourceTypeRouteConstraint = "fhirResource";

        private const string ResourceTypeRouteSegment = "{" + KnownActionParameterNames.ResourceType + ":" + ResourceTypeRouteConstraint + "}";
        private const string IdRouteSegment = "{" + KnownActionParameterNames.Id + "}";

        public const string ResourceType = ResourceTypeRouteSegment;
        public const string ResourceTypeById = ResourceType + "/" + IdRouteSegment;
    }
}
