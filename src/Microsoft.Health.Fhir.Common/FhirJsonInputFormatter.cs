﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Health.Fhir.Common.Constants;
using Microsoft.Health.Fhir.Common.Interfaces;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.Common
{
    public class FhirJsonInputFormatter : TextInputFormatter
    {
        private readonly IFhirVersionGeneralizer _generalizer;
        private readonly IArrayPool<char> _charPool;

        public FhirJsonInputFormatter(IFhirVersionGeneralizer generalizer, ArrayPool<char> charPool)
        {
            EnsureArg.IsNotNull(generalizer, nameof(generalizer));
            EnsureArg.IsNotNull(charPool, nameof(charPool));

            _generalizer = generalizer;
            _charPool = new JsonArrayPool(charPool);

            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);
            SupportedMediaTypes.Add(KnownContentTypes.JsonContentType);
            SupportedMediaTypes.Add(KnownMediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(KnownMediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(KnownMediaTypeHeaderValues.ApplicationAnyJsonSyntax);
        }

        protected override bool CanReadType(Type type)
        {
            EnsureArg.IsNotNull(type, nameof(type));

            return typeof(Resource).IsAssignableFrom(type);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Reference implementation: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonInputFormatter.cs
        /// Parsing from a stream: https://github.com/ewoutkramer/fhir-net-api/blob/master/src/Hl7.Fhir.Support/Utility/SerializationUtil.cs#L134
        /// </remarks>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            EnsureArg.IsNotNull(context, nameof(context));
            EnsureArg.IsNotNull(encoding, nameof(encoding));

            var request = context.HttpContext.Request;

            using (var streamReader = context.ReaderFactory(request.Body, encoding))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                Exception delayedException = null;
                Resource model = null;

                jsonReader.DateParseHandling = DateParseHandling.None;
                jsonReader.FloatParseHandling = FloatParseHandling.Decimal;
                jsonReader.ArrayPool = _charPool;
                jsonReader.CloseInput = false;

                try
                {
                    model = await _generalizer.ParseAsync<Resource>(jsonReader);
                }
                catch (Exception ex)
                {
                    delayedException = ex;
                }

                // Some nonempty inputs might deserialize as null, for example whitespace,
                // or the JSON-encoded value "null". The upstream BodyModelBinder needs to
                // be notified that we don't regard this as a real input so it can register
                // a model binding error.
                // https://github.com/aspnet/Mvc/blob/ce66e953045d3c3c52bd6c2bd9d5385fb52eccdc/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonInputFormatter.cs#L221
                if (model == null && delayedException == null && !context.TreatEmptyInputAsDefaultValue)
                {
                    return await InputFormatterResult.NoValueAsync();
                }

                if (model != null)
                {
                    return await InputFormatterResult.SuccessAsync(model);
                }

                if (delayedException != null)
                {
                    // Add model state information to return to the client
                    context.ModelState.TryAddModelError(string.Empty, delayedException.Message);
                }

                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}
