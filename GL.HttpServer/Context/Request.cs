﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GL.HttpServer.Enums;
using GL.HttpServer.Types;

namespace GL.HttpServer.Context
{
    public class Request
    {
        public Request()
        {
            Parameters = new List<UrlParameter>();
        }

        public MethodType HttpMethod { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public Stream InputStream { get; set; }
        public string RawUrl { get; set; }
        public string UnescapedUrl { get; set; }
        public List<UrlParameter> Parameters { get; set; }

        public int ContentLength
        {
            get
            {
                var length = 0;
                IEnumerable<string> contentLengthString;
                if (Headers.TryGetValue("Content-Length", out contentLengthString))
                {
                    var firstValue = contentLengthString.FirstOrDefault();
                    if (firstValue != null)
                        int.TryParse(firstValue, out length);
                }
                return length;
            }
        }

        public string GetServiceName()
        {
            return UnescapedUrl.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .FirstOrDefault();
        }
    }
}