// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.IO;
using Azure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Health.Fhir.Common;

namespace Microsoft.Health.Fhir.CRUD.R4
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

                    var builtConfig = builder.Build();

                    var keyVaultEndpoint = builtConfig["KeyVault:Endpoint"];
                    if (!string.IsNullOrEmpty(keyVaultEndpoint))
                    {
                        var credential = new DefaultAzureCredential();
                        builder.AddAzureKeyVault(new System.Uri(keyVaultEndpoint), credential);
                    }
                })
                .UseStartup<Startup>()
                .Build();
#pragma warning restore CS8604 // Possible null reference argument.

            host.Run();
        }
    }
}
