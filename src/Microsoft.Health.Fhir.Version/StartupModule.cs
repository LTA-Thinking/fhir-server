// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Hl7.Fhir.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.Fhir.Common.Interfaces;

namespace Microsoft.Health.Fhir.Version
{
    public static class StartupModule
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddSingleton<FhirJsonSerializer>();
            services.AddSingleton<FhirJsonParser>();
            services.AddSingleton<IFhirVersionGeneralizer, FhirVersionGeneralizer>();
            services.AddSingleton<IModelInfoProvider, VersionSpecificModelInfoProvider>();

            ModelInfoProvider.SetProvider(new VersionSpecificModelInfoProvider());
        }
    }
}
