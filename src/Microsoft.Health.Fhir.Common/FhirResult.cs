﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Net;
using EnsureThat;

namespace Microsoft.Health.Fhir.Common
{
    /// <summary>
    /// Handles the output of a FHIR MVC Action Method
    /// </summary>
    public class FhirResult : ResourceActionResult<IResourceElement>
    {
        private IFhirVersionGeneralizer _generalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirResult" /> class.
        /// </summary>
        public FhirResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirResult" /> class.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="generalizer">Generalizer</param>
        public FhirResult(IResourceElement resource, IFhirVersionGeneralizer generalizer)
            : base(resource)
        {
            _generalizer = generalizer;
        }

        /// <summary>
        /// Creates a FHIR result with the specified parameters
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="generalizer">Generalizer</param>
        public static FhirResult Create(IResourceElement resource, HttpStatusCode statusCode = HttpStatusCode.OK, IFhirVersionGeneralizer generalizer = null)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            return new FhirResult(resource, generalizer)
            {
                StatusCode = statusCode,
            };
        }

        /// <summary>
        /// Creates a Gone response
        /// </summary>
        public static FhirResult Gone()
        {
            return new FhirResult
            {
                StatusCode = HttpStatusCode.Gone,
            };
        }

        /// <summary>
        /// Returns a NotFound response
        /// </summary>
        public static FhirResult NotFound()
        {
            return new FhirResult
            {
                StatusCode = HttpStatusCode.NotFound,
            };
        }

        /// <summary>
        /// Returns a NoContent response
        /// </summary>
        public static FhirResult NoContent()
        {
            return new FhirResult
            {
                StatusCode = HttpStatusCode.NoContent,
            };
        }

        protected override object GetResultToSerialize()
        {
            if (Result is ResourceElement && _generalizer != null)
            {
                return _generalizer.ToPoco(Result as ResourceElement);
            }
            else if (Result is RawResourceElement)
            {
                return Result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override string GetResultTypeName()
        {
            return Result?.InstanceType;
        }
    }
}
