using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace RFCProtocolTesting
{
    class HttpParser : IHttpRequestLineHandler, IHttpHeadersHandler
    {
        public HttpParser() { }
        public string getBody(string requestString)
        {
            byte[] requestRaw = Encoding.UTF8.GetBytes(requestString);
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(requestRaw);
            HttpParser<HttpParser> parser = new HttpParser<HttpParser>();
            HttpParser app = new HttpParser();

            parser.ParseRequestLine(app, buffer, out var consumed, out var examined);
            buffer = buffer.Slice(consumed);
            parser.ParseHeaders(app, buffer, out consumed, out examined, out var b);
            buffer = buffer.Slice(consumed);
            string body = Encoding.UTF8.GetString(buffer.ToArray());

            return body;
        }
        public void OnHeader(Span<byte> name, Span<byte> value)
        {
            Console.WriteLine("On header");
        }

        public void OnStartLine(HttpMethod method, HttpVersion version, Span<byte> target, Span<byte> path, Span<byte> query, Span<byte> customMethod, bool pathEncoded)
        {
            Console.WriteLine("On start line");
        }
    }
}
