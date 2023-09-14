// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Pipes
{
    // Copied from https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication

    // Defines the data protocol for reading and writing strings on our stream.
    public class StreamObject
    {
        private Stream ioStream;

        public StreamObject(Stream ioStream)
        {
            this.ioStream = ioStream;
        }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        public async Task<RequestWrapper<object>> ReadObjectAsync(IRequestParser parser, CancellationToken cancellationToken)
        {
            Console.WriteLine("Reading object");

            int type;
            type = ioStream.ReadByte();

            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            Console.WriteLine($"Length is {len}");

            var inBuffer = new byte[len];
            await ioStream.ReadAsync(inBuffer.AsMemory(0, len), cancellationToken);
            Console.WriteLine("Parsing to JSON");

            return parser.ParseRequestWrapper((RequestType)type, inBuffer);
        }

        public async Task<int> WriteObjectAsync(RequestWrapper<object> outObject, CancellationToken cancellationToken)
        {
            Console.WriteLine("Writing object");

            ioStream.WriteByte((byte)outObject.Type);

            byte[] outBuffer = new BinaryData(outObject).ToArray();
            int len = outBuffer.Length;
            if (len > ushort.MaxValue)
            {
                len = ushort.MaxValue;
            }

            Console.WriteLine("Sending length");
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));

            Console.WriteLine("Sending body");
            await ioStream.WriteAsync(outBuffer.AsMemory(0, len), cancellationToken);
            await ioStream.FlushAsync(cancellationToken);

            Console.WriteLine("Done writing");
            return outBuffer.Length + 2;
        }
#pragma warning restore CA1303 // Do not pass literals as localized parameters
    }
}
