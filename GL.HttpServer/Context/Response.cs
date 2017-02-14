﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoMapper;
using GL.HttpServer.Entities;
using LiteDB;
using Newtonsoft.Json;

namespace GL.HttpServer.Context
{
    public abstract class Response
    {
        protected Response()
        {
            WriteStream = s => { };
            StatusCode = 200;
            Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};
        }

        [IgnoreMap]
        [BsonIgnore]
        [JsonIgnore]
        public int StatusCode { get; set; }

        [IgnoreMap]
        [BsonIgnore]
        [JsonIgnore]
        public IDictionary<string, string> Headers { get; set; }

        [IgnoreMap]
        [BsonIgnore]
        [JsonIgnore]
        internal Action<Stream> WriteStream { get; set; }
    }
}