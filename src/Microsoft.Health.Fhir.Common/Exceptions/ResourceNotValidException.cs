// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using EnsureThat;
using FluentValidation.Results;

namespace Microsoft.Health.Fhir.Common.Exceptions
{
    public class ResourceNotValidException : FhirException
    {
        public ResourceNotValidException(IEnumerable<OperationOutcomeIssue> validationFailures)
        {
            EnsureArg.IsNotNull(validationFailures, nameof(validationFailures));

            foreach (var failure in validationFailures)
            {
                Issues.Add(failure);
            }
        }

        public ResourceNotValidException(IEnumerable<ValidationFailure> validationFailures)
        {
            EnsureArg.IsNotNull(validationFailures, nameof(validationFailures));

            foreach (var failure in validationFailures)
            {
                if (failure is FhirValidationFailure fhirValidationFailure)
                {
                    if (fhirValidationFailure.IssueComponent != null)
                    {
                        Issues.Add(fhirValidationFailure.IssueComponent);
                    }
                }
                else
                {
                    string[] expression = string.IsNullOrEmpty(failure.PropertyName) ? null : new[] { failure.PropertyName };

                    Issues.Add(new OperationOutcomeIssue(
                            OperationOutcomeConstants.IssueSeverity.Error,
                            OperationOutcomeConstants.IssueType.Invalid,
                            diagnostics: failure.ErrorMessage,
                            expression: expression));
                }
            }
        }
    }
}
