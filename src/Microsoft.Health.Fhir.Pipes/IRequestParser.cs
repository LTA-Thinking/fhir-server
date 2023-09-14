// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.Health.Fhir.Pipes
{
    public interface IRequestParser
    {
        public RequestWrapper<object> ParseRequestWrapper(RequestType type, byte[] data);
    }
}
