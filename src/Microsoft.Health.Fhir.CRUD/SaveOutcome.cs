﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.Health.Fhir.Common;

namespace Microsoft.Health.Fhir.CRUD
{
    public class SaveOutcome
    {
        public SaveOutcome(RawResourceElement rawResourceElement, SaveOutcomeType outcome)
        {
            EnsureArg.IsNotNull(rawResourceElement, nameof(rawResourceElement));

            RawResourceElement = rawResourceElement;
            Outcome = outcome;
        }

        public RawResourceElement RawResourceElement { get; }

        public SaveOutcomeType Outcome { get; }
    }
}
