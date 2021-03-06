﻿using GL.HttpServer.Cache;
using GL.HttpServer.Database;
using Kontur.GameStats.Server.DTO.CacheInfo;
using Kontur.GameStats.Server.Entities;

namespace Kontur.GameStats.Server.CacheLoaders
{
    public class ServerStatsCacheLoader : ICacheLoader
    {
        public void Load()
        {
            using (var unit = new UnitOfWork())
            {
                var servers = unit.Repository<Entities.Server>().FindAll();
                if (servers == null)
                    return;
                foreach (var server in servers)
                {
                    ServerStatsTempInfo serverStats;
                    if (!MemoryCache.Cache<ServerStatsTempInfo>().TryGetValue(server.Endpoint, out serverStats))
                    {
                        serverStats = new ServerStatsTempInfo();
                    }
                    var matches = unit.Repository<Match>().Find(a => a.Server == server.Endpoint.ToString());
                    foreach (var match in matches)
                    {
                        serverStats.Update(match);
                    }
                    MemoryCache.Cache<ServerStatsTempInfo>().PutAsync(server.Endpoint, serverStats);
                }
            }
        }
    }
}
