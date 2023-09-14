// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Common
{
    public sealed class KnownActionParameterNames
    {
        public const string ResourceType = "typeParameter";
        public const string Resource = "resource";
        public const string Id = "idParameter";
        public const string Vid = "vidParameter";
        public const string CompartmentType = "compartmentTypeParameter";
        public const string Bundle = "bundle";
    }
}
