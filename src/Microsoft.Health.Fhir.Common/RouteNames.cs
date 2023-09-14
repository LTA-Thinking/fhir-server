// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Fhir.Common
{
    public static class RouteNames
    {
        public const string Metadata = nameof(Metadata);

        public const string WellKnownSmartConfiguration = nameof(WellKnownSmartConfiguration);

        public const string ReadResource = nameof(ReadResource);

        public const string ReadResourceWithVersionRoute = nameof(ReadResourceWithVersionRoute);

        public const string SearchResources = nameof(SearchResources);

        public const string SearchAllResources = nameof(SearchAllResources);

        public const string History = nameof(History);

        public const string HistoryType = nameof(HistoryType);

        public const string HistoryTypeId = nameof(HistoryTypeId);

        public const string SearchCompartmentByResourceType = nameof(SearchCompartmentByResourceType);

        public const string AadSmartOnFhirProxyAuthorize = nameof(AadSmartOnFhirProxyAuthorize);

        public const string AadSmartOnFhirProxyCallback = nameof(AadSmartOnFhirProxyCallback);

        public const string AadSmartOnFhirProxyToken = nameof(AadSmartOnFhirProxyToken);

        public const string GetExportStatusById = nameof(GetExportStatusById);

        public const string CancelExport = nameof(CancelExport);

        public const string GetReindexStatusById = nameof(GetReindexStatusById);

        public const string GetImportStatusById = nameof(GetImportStatusById);

        public const string CancelImport = nameof(CancelImport);

        public const string PostBundle = nameof(PostBundle);

        public const string PatientEverythingById = nameof(PatientEverythingById);

        public const string ReindexOperationDefintion = nameof(ReindexOperationDefintion);

        public const string ResourceReindexOperationDefinition = nameof(ResourceReindexOperationDefinition);

        public const string ExportOperationDefinition = nameof(ExportOperationDefinition);

        public const string PatientExportOperationDefinition = nameof(PatientExportOperationDefinition);

        public const string GroupExportOperationDefinition = nameof(GroupExportOperationDefinition);

        public const string AnonymizedExportOperationDefinition = nameof(AnonymizedExportOperationDefinition);

        public const string ConvertDataOperationDefinition = nameof(ConvertDataOperationDefinition);

        public const string MemberMatchOperationDefinition = nameof(MemberMatchOperationDefinition);

        public const string PurgeHistoryDefinition = nameof(PurgeHistoryDefinition);

        public const string SearchParameterState = nameof(SearchParameterState);

        public const string SearchParameterStatusOperationDefinition = "SearchParameterStatusOperationDefinition";

        public const string PostSearchParameterState = nameof(PostSearchParameterState);

        public const string UpdateSearchParameterState = nameof(UpdateSearchParameterState);

        public const string GetBulkDeleteStatusById = nameof(GetBulkDeleteStatusById);

        public const string CancelBulkDelete = nameof(CancelBulkDelete);

        public const string BulkDeleteDefinition = nameof(BulkDeleteDefinition);
    }
}
