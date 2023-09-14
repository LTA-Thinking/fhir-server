// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.Fhir.Pipes;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.Health.Fhir.SqlDatabaseModule
{
    public class DatabaseServerPipe : BackgroundService
    {
        private Func<IScoped<FhirDataStore>> _fhirDataStoreFactory;
        private IRequestParser _requestParser;
        private const int _numThreads = 4;
        private IList<Task> _tasks;

        public DatabaseServerPipe(
            Func<IScoped<FhirDataStore>> fhirDataStoreFactory,
            IRequestParser requestParser)
        {
            _fhirDataStoreFactory = EnsureArg.IsNotNull(fhirDataStoreFactory, nameof(fhirDataStoreFactory));
            _requestParser = EnsureArg.IsNotNull(requestParser, nameof(requestParser));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _tasks = new List<Task>();
            for (int i = 0; i < _numThreads; i++)
            {
                _tasks.Add(ServerThread());
            }

            await Task.WhenAll(_tasks);
        }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        // will need to rework this, it will handle 4 requests and stop
        private async Task ServerThread()
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("database-pipe", PipeDirection.InOut, _numThreads);

            Console.WriteLine("Waiting for connection...");

            // Wait for a client to connect
            await pipeServer.WaitForConnectionAsync();

            Console.WriteLine("Connected to client");
            try
            {
                StreamObject stream = new StreamObject(pipeServer);
                var request = await stream.ReadObjectAsync(_requestParser, CancellationToken.None);
                var response = new RequestWrapper<object>()
                {
                    Type = request.Type,
                    Data = await HandleRequest(request),
                };

                await stream.WriteObjectAsync(response, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Finished client communication");

            pipeServer.Close();
        }
#pragma warning restore CA1303 // Do not pass literals as localized parameters

        private async Task<object> HandleRequest(RequestWrapper<object> request)
        {
            using IScoped<FhirDataStore> fhirDataStore = _fhirDataStoreFactory.Invoke();
            switch (request.Type)
            {
                case RequestType.Upsert:
                    return await fhirDataStore.Value.UpsertResource((RawResource)request.Data, CancellationToken.None);
                case RequestType.Read:
                    return await fhirDataStore.Value.ReadResource((ResourceKey)request.Data, CancellationToken.None);
                case RequestType.Delete:
                    return await fhirDataStore.Value.DeleteResource((ResourceKey)request.Data, CancellationToken.None);
                default:
                    throw new InvalidArgumentException();
            }
        }
    }
}
