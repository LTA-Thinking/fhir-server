// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.Common.Interfaces
{
    public interface IFhirVersionGeneralizer
    {
        public ResourceElement ToResourceElement(Base resource);

        public Resource ToPoco(ResourceElement resource);

        public T ToPoco<T>(ResourceElement resource)
            where T : Resource;

        public RawResource CreateRawResource(ResourceElement resource, bool keepMeta, bool keepVersion = false);

        public ITypedElement RawResourceToITypedElement(RawResource rawResource);

        public RawResource ResourceToRawResource(Base resource);

        public System.Threading.Tasks.Task SerializeAsync(Base resource, JsonWriter jsonWriter);

        public System.Threading.Tasks.Task<T> ParseAsync<T>(JsonTextReader jsonReader)
            where T : Base;
    }
}
