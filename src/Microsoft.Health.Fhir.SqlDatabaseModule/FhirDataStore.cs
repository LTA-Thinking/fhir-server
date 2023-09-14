// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Buffers.Text;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.SqlServer.Features.Client;

namespace Microsoft.Health.Fhir.SqlDatabaseModule
{
    public class FhirDataStore
    {
        private SqlConnectionWrapperFactory _sqlConnectionWrapperFactory;
        private IFhirVersionGeneralizer _generalizer;

        public FhirDataStore(
            SqlConnectionWrapperFactory sqlConnectionWrapperFactory,
            IFhirVersionGeneralizer generalizer)
        {
            _sqlConnectionWrapperFactory = EnsureArg.IsNotNull(sqlConnectionWrapperFactory, nameof(sqlConnectionWrapperFactory));
            _generalizer = EnsureArg.IsNotNull(generalizer, nameof(generalizer));
        }

        public async Task<RawResource> UpsertResource(RawResource resource, CancellationToken cancellationToken)
        {
            var resourceElement = new ResourceElement(_generalizer.RawResourceToITypedElement(resource));

            // var currentVersion = (await ReadResource(resource.ToResourceKey(), cancellationToken))?.VersionId;
            var resourceTypeId = await GetResourceTypeId(resourceElement.InstanceType, cancellationToken);
            var rawResource64String = Base64Encode(_generalizer.CreateRawResource(resourceElement, true, true).Data);

            using var conn = await _sqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
            using var cmd = conn.CreateNonRetrySqlCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = $"Insert into dbo.Resources values ({resourceTypeId}, '{resourceElement.Id}', {resourceElement.VersionId}, '{rawResource64String}', 0)";
            cmd.CommandTimeout = 300;
            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return resource;
        }

        public async Task<RawResource> ReadResource(ResourceKey resource, CancellationToken cancellationToken)
        {
            var resourceTypeId = await GetResourceTypeId(resource.ResourceType, cancellationToken);

            using var conn = await _sqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
            using var cmd = conn.CreateNonRetrySqlCommand();

            cmd.CommandType = System.Data.CommandType.Text;

            var command = $"Select top 1 Resource from dbo.Resources where ResourceTypeId = {resourceTypeId} and ResourceId = '{resource.Id}'";
            if (resource.VersionId != null)
            {
                command += $" and VersionNumber = {resource.VersionId}";
            }

            cmd.CommandText = command;
            cmd.CommandTimeout = 300;
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            await reader.ReadAsync(cancellationToken);
            var rawResource64String = reader.GetString(0);
            var result = Base64Decode(rawResource64String);
            var rawResource = new RawResource(result, FhirResourceFormat.Json, isMetaSet: true);
            return rawResource;
        }

        public async Task<ResourceKey> DeleteResource(ResourceKey resource, CancellationToken cancellationToken)
        {
            var resourceTypeId = await GetResourceTypeId(resource.ResourceType, cancellationToken);

            using var conn = await _sqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
            using var cmd = conn.CreateNonRetrySqlCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = $"Delete from dbo.Resources where ResourceTypeId = {resourceTypeId} and ResourceId = '{resource.Id}' and VersionNumber = {resource.VersionId}";
            cmd.CommandTimeout = 300;
            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return resource;
        }

        private async Task<int> GetResourceTypeId(string resourceType, CancellationToken cancellationToken)
        {
            using var conn = await _sqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken);
            using var cmd = conn.CreateNonRetrySqlCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = $"Select top 1 Id from dbo.ResourceTypes where Name = '{resourceType}'";
            cmd.CommandTimeout = 300;
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            await reader.ReadAsync(cancellationToken);
            return reader.GetInt32(0);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
