// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.Health.SqlServer.Configs;

namespace Microsoft.Health.Fhir.SqlDatabase.R4
{
    public class Startup
    {
        private static string instanceId;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            instanceId = $"{Configuration["WEBSITE_ROLE_INSTANCE_ID"]}--{Configuration["WEBSITE_INSTANCE_ID"]}--{Guid.NewGuid()}";

            Version.StartupModule.ConfigureService(services);
            SqlDatabaseModule.StartupModule.ConfigureService(
                services,
                config =>
                {
                    Configuration?.GetSection(SqlServerDataStoreConfiguration.SectionName).Bind(config);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (instanceId != null)
                {
                    string instanceKey = "X-Instance-Id";
                    if (!context.Response.Headers.ContainsKey(instanceKey))
                    {
                        context.Response.Headers.Add(instanceKey, new StringValues(instanceId));
                    }
                }

                await next.Invoke();
            });
            app.UseHealthChecks("/health/check");
        }
    }
}
