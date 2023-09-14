// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Fhir.Pipes
{
    public class RequestWrapper<T>
    {
        public T Data { get; set; }

        public RequestType Type { get; set; }
    }
}
