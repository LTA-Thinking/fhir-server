// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Health.Fhir.Common;

namespace Microsoft.Health.Fhir.SqlDatabase.R4
{
    public static class Program
    {
        public static void Main(string[] args)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var host = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(contentRoot: Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.Sources.Add(new GenericConfigurationSource(() => new DictionaryExpansionConfigurationProvider(new EnvironmentVariablesConfigurationProvider())));
                })
                .UseStartup<Startup>()
                .Build();
#pragma warning restore CS8604 // Possible null reference argument.

            host.Run();
        }
    }
}
