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
using Microsoft.Health.Fhir.Common;
using Microsoft.Health.Fhir.Pipes;

namespace Microsoft.Health.Fhir.CRUD
{
    public class ClientPipe : IDisposable
    {
        private NamedPipeClientStream _pipeClient;
        private IRequestParser _requestParser;

        public ClientPipe(IRequestParser requestParser)
        {
            _requestParser = EnsureArg.IsNotNull(requestParser, nameof(requestParser));

            _pipeClient = new NamedPipeClientStream(".", "database-pipe", PipeDirection.InOut);
        }

        public async Task<RawResource> UpdateResource(RawResource resource, CancellationToken cancellationToken)
        {
            var request = CreateRequest(resource, RequestType.Upsert);
            return await SendRequest<RawResource>(request, cancellationToken);
        }

        public async Task<ResourceKey> DeleteResource(ResourceKey resource, CancellationToken cancellationToken)
        {
            var request = CreateRequest(resource, RequestType.Delete);
            return await SendRequest<ResourceKey>(request, cancellationToken);
        }

        public async Task<RawResource> ReadResource(ResourceKey resource, CancellationToken cancellationToken)
        {
            var request = CreateRequest(resource, RequestType.Read);
            return await SendRequest<RawResource>(request, cancellationToken);
        }

        private async Task<T> SendRequest<T>(RequestWrapper<object> request, CancellationToken cancellationToken)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.WriteLine("Connecting...");
            await _pipeClient.ConnectAsync(cancellationToken);

            Console.WriteLine("Connected to server");
            var stream = new StreamObject(_pipeClient);
            await stream.WriteObjectAsync(request, cancellationToken);
            var response = await stream.ReadObjectAsync(_requestParser, cancellationToken);

            Console.WriteLine("Finished server communication");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            _pipeClient.Close();

            return (T)response.Data;
        }

        // Writing proper dispose methods is hard. Unsure if this is correct.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pipeClient.Dispose();
            }
        }

        private static RequestWrapper<object> CreateRequest(object data, RequestType type)
        {
            return new RequestWrapper<object>()
            {
                Data = data,
                Type = type,
            };
        }
    }
}
