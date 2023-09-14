// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.Fhir.Pipes;

namespace Microsoft.Health.Fhir.CRUD
{
    public static class StartupModule
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddTransient<ClientPipe>();
            services.AddSingleton<IRequestParser, CrudRequestParser>();

            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.RespectBrowserAcceptHeader = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset;
                })
                .AddRazorRuntimeCompilation();

            // Adds provider to serve embedded razor views
            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Add(new EmbeddedFileProvider(typeof(CodePreviewModel).Assembly));
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add(KnownRoutes.ResourceTypeRouteConstraint, typeof(ResourceTypesRouteConstraint));
            });

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Add<FormatterConfiguration>()
                .Singleton()
                .AsSelf()
                .AsService<IPostConfigureOptions<MvcOptions>>();

            services.Add<FhirJsonInputFormatter>()
                .Singleton()
                .AsSelf()
                .AsService<TextInputFormatter>();

            services.Add<FhirJsonOutputFormatter>()
                .Singleton()
                .AsSelf()
                .AsService<TextOutputFormatter>();

            services.AddHttpContextAccessor();
            services.AddHealthChecks();
        }
    }
}
