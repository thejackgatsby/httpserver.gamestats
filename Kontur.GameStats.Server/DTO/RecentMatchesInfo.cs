﻿using System;
using System.Collections.Generic;
using GL.HttpServer.Context;

namespace Kontur.GameStats.Server.DTO
{
    public class RecentMatchesInfo : JsonResponse
    {
        public RecentMatchesInfo()
        {
            Results = new List<MatchInfo>();
        }

        public string Server { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<MatchInfo> Results { get; set; }
    }
}