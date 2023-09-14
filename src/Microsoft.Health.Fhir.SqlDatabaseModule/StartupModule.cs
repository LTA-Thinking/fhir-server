// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Pipes;
using Microsoft.Health.SqlServer.Api.Registration;
using Microsoft.Health.SqlServer.Configs;
using Microsoft.Health.SqlServer.Features.Schema;
using Microsoft.Health.SqlServer.Registration;

namespace Microsoft.Health.Fhir.SqlDatabaseModule
{
    public static class StartupModule
    {
        private enum SchemaVersion
        {
            None = 0,
        }

        public static void ConfigureService(IServiceCollection services, Action<SqlServerDataStoreConfiguration> configureAction = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            services.AddScoped();

            services.AddSqlServerConnection(configureAction);
            services.AddSqlServerManagement<SchemaVersion>();
            services.AddSqlServerApi();

            services.Add(provider => new SchemaInformation((int)SchemaVersion.None, (int)SchemaVersion.None))
                .Singleton()
                .AsSelf()
                .AsImplementedInterfaces();

            services.Add<FhirDataStore>()
                .Scoped()
                .AsSelf()
                .AsImplementedInterfaces()
                .AsFactory<IScoped<FhirDataStore>>();

            services.AddSingleton<IMediator, Mediator>();
            services.AddSingleton<IRequestParser, SqlRequestParser>();

            services.Add<DatabaseServerPipe>()
                .Singleton()
                .AsSelf()
                .AsImplementedInterfaces();
        }
    }
}
