// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Health.Fhir.Common.Constants;
using Microsoft.Health.Fhir.Common.Interfaces;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Common
{
    public class FhirJsonOutputFormatter : TextOutputFormatter
    {
        private readonly IFhirVersionGeneralizer _fhirVersionGeneralizer;
        private readonly IArrayPool<char> _charPool;
        private readonly IModelInfoProvider _modelInfoProvider;

        public FhirJsonOutputFormatter(
            IFhirVersionGeneralizer fhirVersionGeneralizer,
            ArrayPool<char> charPool,
            IModelInfoProvider modelInfoProvider)
        {
            EnsureArg.IsNotNull(fhirVersionGeneralizer, nameof(fhirVersionGeneralizer));
            EnsureArg.IsNotNull(charPool, nameof(charPool));
            EnsureArg.IsNotNull(modelInfoProvider, nameof(modelInfoProvider));

            _fhirVersionGeneralizer = fhirVersionGeneralizer;
            _charPool = new JsonArrayPool(charPool);
            _modelInfoProvider = modelInfoProvider;

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add(KnownContentTypes.JsonContentType);
            SupportedMediaTypes.Add(KnownMediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(KnownMediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(KnownMediaTypeHeaderValues.ApplicationAnyJsonSyntax);
        }

        protected override bool CanWriteType(Type type)
        {
            EnsureArg.IsNotNull(type, nameof(type));

            return typeof(Resource).IsAssignableFrom(type) || typeof(RawResourceElement).IsAssignableFrom(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            EnsureArg.IsNotNull(context, nameof(context));
            EnsureArg.IsNotNull(selectedEncoding, nameof(selectedEncoding));

            HttpResponse response = context.HttpContext.Response;

            Resource resource = null;
            var summaryProvider = _modelInfoProvider.StructureDefinitionSummaryProvider;
            var additionalElements = new HashSet<string>();

            resource = (Resource)context.Object;

            using (TextWriter textWriter = context.WriterFactory(response.Body, selectedEncoding))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.ArrayPool = _charPool;

                await _fhirVersionGeneralizer.SerializeAsync(resource, jsonWriter);
                await jsonWriter.FlushAsync();
            }
        }
    }
}
