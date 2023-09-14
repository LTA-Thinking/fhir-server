// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.Fhir.Common.Exceptions;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.Version
{
    public class FhirVersionGeneralizer : IFhirVersionGeneralizer
    {
        private FhirJsonSerializer _serializer;
        private FhirJsonParser _parser;
        private IModelInfoProvider _modelInfoProvider;

        public FhirVersionGeneralizer(
            FhirJsonSerializer fhirJsonSerializer,
            FhirJsonParser fhirJsonParser,
            IModelInfoProvider modelInfoProvider)
        {
            _serializer = EnsureArg.IsNotNull(fhirJsonSerializer, nameof(fhirJsonSerializer));
            _parser = EnsureArg.IsNotNull(fhirJsonParser, nameof(fhirJsonParser));
            _modelInfoProvider = EnsureArg.IsNotNull(modelInfoProvider, nameof(modelInfoProvider));
        }

        public ResourceElement ToResourceElement(Base resource)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            return resource.ToTypedElement().ToResourceElement();
        }

        public Resource ToPoco(ResourceElement resource)
        {
            return ToPoco<Resource>(resource);
        }

        public T ToPoco<T>(ResourceElement resource)
            where T : Resource
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            return (T)resource.ResourceInstance ?? resource.Instance.ToPoco<T>();
        }

        public RawResource CreateRawResource(ResourceElement resource, bool keepMeta, bool keepVersion = false)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            var poco = ToPoco<Resource>(resource);

            poco.Meta = poco.Meta ?? new Meta();
            var versionId = poco.Meta.VersionId;

            try
            {
                // Clear meta version if keepMeta is false since this is set based on generated values when saving the resource
                if (!keepMeta)
                {
                    poco.Meta.VersionId = null;
                }
                else if (!keepVersion)
                {
                    // Assume it's 1, though it may get changed by the database.
                    poco.Meta.VersionId = "1";
                }

                return new RawResource(_serializer.SerializeToString(poco), FhirResourceFormat.Json, keepMeta);
            }
            finally
            {
                if (!keepMeta)
                {
                    poco.Meta.VersionId = versionId;
                }
            }
        }

        public ITypedElement RawResourceToITypedElement(RawResource rawResource)
        {
            EnsureArg.IsNotNull(rawResource, nameof(rawResource));

            using TextReader reader = new StringReader(rawResource.Data);
            using JsonReader jsonReader = new JsonTextReader(reader);
            try
            {
                ISourceNode sourceNode = FhirJsonNode.Read(jsonReader);
                return _modelInfoProvider.ToTypedElement(sourceNode);
            }
            catch (FormatException ex)
            {
                var issue = new OperationOutcomeIssue(
                    OperationOutcomeConstants.IssueSeverity.Fatal,
                    OperationOutcomeConstants.IssueType.Invalid,
                    ex.Message);

                throw new ResourceNotValidException(new OperationOutcomeIssue[] { issue });
            }
        }

        public RawResource ResourceToRawResource(Base resource)
        {
            return new RawResource(_serializer.SerializeToString(resource), FhirResourceFormat.Json, true);
        }

        public async System.Threading.Tasks.Task SerializeAsync(Base resource, JsonWriter jsonWriter)
        {
            await _serializer.SerializeAsync(resource, jsonWriter);
        }

        public async System.Threading.Tasks.Task<T> ParseAsync<T>(JsonTextReader jsonReader)
            where T : Base
        {
            return await _parser.ParseAsync<T>(jsonReader);
        }
    }
}
