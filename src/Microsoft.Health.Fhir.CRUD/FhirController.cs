// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Health.Fhir.Common;

namespace Microsoft.Health.Fhir.CRUD
{
    /// <summary>
    /// FHIR Rest API
    /// </summary>
    [ValidateResourceTypeFilter]
    public class FhirController : Controller
    {
        private readonly ClientPipe _pipe;
        private readonly IFhirVersionGeneralizer _generalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirController" /> class.
        /// </summary>
        /// <param name="pipe">The mediator.</param>
        /// <param name="authorizationService">The authorization service.</param>
        /// <param name="generalizer">Functions to handle FHIR resources across versions</param>
        public FhirController(
            ClientPipe pipe,
            IAuthorizationService authorizationService,
            IFhirVersionGeneralizer generalizer)
        {
            EnsureArg.IsNotNull(pipe, nameof(pipe));
            EnsureArg.IsNotNull(authorizationService, nameof(authorizationService));
            EnsureArg.IsNotNull(generalizer, nameof(generalizer));

            _pipe = pipe;
            _generalizer = generalizer;
        }

        /// <summary>
        /// Creates a new resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        [HttpPost]
        [Route(KnownRoutes.ResourceType)]
        public async Task<IActionResult> Create([FromBody] Resource resource)
        {
            RawResource response = await _pipe.UpdateResource(_generalizer.ResourceToRawResource(resource), HttpContext.RequestAborted);

            return FhirResult.Create(new ResourceElement(_generalizer.RawResourceToITypedElement(response)), HttpStatusCode.Created, _generalizer);
        }

        /// <summary>
        /// Updates or creates a new resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        [HttpPut]
        [ValidateResourceIdFilter]
        [Route(KnownRoutes.ResourceTypeById)]
        public async Task<IActionResult> Update([FromBody] Resource resource)
        {
            RawResource response = await _pipe.UpdateResource(_generalizer.ResourceToRawResource(resource), HttpContext.RequestAborted);

            return FhirResult.Create(new ResourceElement(_generalizer.RawResourceToITypedElement(response)), HttpStatusCode.OK, _generalizer);
        }

        /// <summary>
        /// Reads the specified resource.
        /// </summary>
        /// <param name="typeParameter">The type.</param>
        /// <param name="idParameter">The identifier.</param>
        [HttpGet]
        [Route(KnownRoutes.ResourceTypeById, Name = RouteNames.ReadResource)]
        public async Task<IActionResult> Read(string typeParameter, string idParameter)
        {
            RawResource response = await _pipe.ReadResource(new ResourceKey(typeParameter, idParameter), HttpContext.RequestAborted);

            return FhirResult.Create(new ResourceElement(_generalizer.RawResourceToITypedElement(response)), HttpStatusCode.OK, _generalizer);
        }

        /// <summary>
        /// Deletes the specified resource
        /// </summary>
        /// <param name="typeParameter">The type.</param>
        /// <param name="idParameter">The identifier.</param>
        [HttpDelete]
        [Route(KnownRoutes.ResourceTypeById)]
        public async Task<IActionResult> Delete(string typeParameter, string idParameter)
        {
            var response = await _pipe.DeleteResource(new ResourceKey(typeParameter, idParameter), HttpContext.RequestAborted);

            return FhirResult.NoContent();
        }
    }
}
