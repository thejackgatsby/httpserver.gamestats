﻿using System.Linq;
using System.Net;
using System.Text;
using GL.HttpServer.Enums;
using GL.HttpServer.Extensions;

namespace GL.HttpServer.Context
{
    public class RequestContext
    {
        private readonly HttpListenerResponse listenerResponse;

        public RequestContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            listenerResponse = response;
            Request = MapRequest(request);
        }

        public Request Request { get; private set; }

        private Request MapRequest(HttpListenerRequest request)
        {
            var mapRequest = new Request
            {
                Headers = request.Headers.ToDictionary(),
                HttpMethod = request.HttpMethod.FromString(),
                InputStream = request.InputStream,
                RawUrl = request.RawUrl
            };
            return mapRequest;
        }

        public void Respond(Response response)
        {
            foreach (var header in response.Headers.Where(r => r.Key != "Content-Type"))
                listenerResponse.AddHeader(header.Key, header.Value);

            listenerResponse.ContentType = response.Headers["Content-Type"];
            listenerResponse.StatusCode = response.StatusCode;
            listenerResponse.ContentEncoding = Encoding.UTF8;

            using (var output = listenerResponse.OutputStream)
            {
                response.WriteStream(output);
            }
        }
    }
}