﻿namespace GL.HttpServer.Enums
{
    public enum MethodType
    {
        PUT = 0,
        GET = 1,
        POST = 2,
        DELETE = 3
    }

    public static class MethodTypeExtensions
    {
        public static MethodType FromString(this string methodType)
        {
            switch (methodType)
            {
                case "GET":
                    return MethodType.GET;
                case "POST":
                    return MethodType.POST;
                case "PUT":
                    return MethodType.PUT;
                case "DELETE":
                    return MethodType.DELETE;
                default:
                    return MethodType.GET;
            }
        }
    }
}