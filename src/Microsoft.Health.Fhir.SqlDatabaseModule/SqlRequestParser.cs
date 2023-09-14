// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.Fhir.Pipes;

namespace Microsoft.Health.Fhir.SqlDatabaseModule
{
    public class SqlRequestParser : IRequestParser
    {
        public RequestWrapper<object> ParseRequestWrapper(RequestType type, byte[] data)
        {
            switch (type)
            {
                case RequestType.Read:
                case RequestType.Delete:
                    var requestWithResourceKey = new BinaryData(data).ToObjectFromJson<RequestWrapper<ResourceKey>>();
                    return new RequestWrapper<object>()
                    {
                        Type = type,
                        Data = requestWithResourceKey.Data,
                    };
                case RequestType.Upsert:
                    var requestWithResource = new BinaryData(data).ToObjectFromJson<RequestWrapper<RawResource>>();

                    return new RequestWrapper<object>()
                    {
                        Type = type,
                        Data = requestWithResource.Data,
                    };
                default:
                    throw new ArgumentException("Unrecognized request type");
            }
        }
    }
}
