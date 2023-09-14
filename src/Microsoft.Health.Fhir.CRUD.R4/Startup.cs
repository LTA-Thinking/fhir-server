// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Health.Fhir.CRUD.R4
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
            CRUD.StartupModule.ConfigureService(services);
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

            UseFhirServer(app);
        }

        /// <summary>
        /// Adds FHIR server functionality to the pipeline with health check filter.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        /// <returns>THe application builder instance.</returns>
        private static IApplicationBuilder UseFhirServer(IApplicationBuilder app)
        {
            EnsureArg.IsNotNull(app, nameof(app));

            app.UseStaticFiles();
            app.UseMvc();

            return app;
        }
    }
}
