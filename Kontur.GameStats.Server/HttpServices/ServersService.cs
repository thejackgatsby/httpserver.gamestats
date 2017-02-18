﻿using System;
using System.Collections.Generic;
using System.Linq;
using GL.HttpServer.Attributes;
using GL.HttpServer.Context;
using GL.HttpServer.Database;
using GL.HttpServer.Extensions;
using GL.HttpServer.HttpServices;
using GL.HttpServer.Managers;
using GL.HttpServer.Types;
using Kontur.GameStats.Server;
using Kontur.GameStats.Server.Dto;
using Kontur.GameStats.Server.DTO;
using Kontur.GameStats.Server.Entities;
using Match = System.Text.RegularExpressions.Match;

namespace Kontur.GameStats.Server.HttpServices
{
    [HttpService("servers")]
    public class ServersService : IHttpService
    {
        /// <summary>
        ///     Advertise запрос от игрового сервера
        /// </summary>
        [PutOperation("info", "/<endpoint>/info")]
        public EmptyResponse PutServerInfo(Endpoint endpoint, ServerInfoDto body)
        {
            using (var unit = new UnitOfWork())
            {
                try
                {
                    var existServer = unit.Repository<Entities.Server>().Find(a => a.Endpoint == endpoint.ToString()).FirstOrDefault();
                    if (existServer != null)
                    {
                        existServer.Info = body.ToEntity<ServerInfo>();
                        unit.Repository<Entities.Server>().Update(existServer);
                    }
                    else
                    {
                        var fullInfo = new Entities.Server();
                        fullInfo.Endpoint = endpoint.ToString();
                        fullInfo.Info = body.ToEntity<ServerInfo>();
                        unit.Repository<Entities.Server>().Add(fullInfo);
                    }
                    return new EmptyResponse(200);
                }
                catch (Exception)
                {
                    return new EmptyResponse(500);
                }
            }
        }

        /// <summary>
        ///     Запись информации о завершенном матче
        /// </summary>
        [PutOperation("matches", "/<endpoint>/matches/<timestamp>")]
        public EmptyResponse PutMatchInfo(Endpoint endpoint, DateTime? timestamp, MatchResultDto body)
        {
            using (var unit = new UnitOfWork())
            {
                try
                {
                    var match = new Entities.Match();
                    match.Server = endpoint.ToString();
                    match.TimeStamp = timestamp;
                    match.Results = body.ToEntity<MatchResult>();
                    unit.Repository<Entities.Match>().Add(match);
                    return new EmptyResponse(200);
                }
                catch (Exception ex)
                {
                    return new EmptyResponse(500);
                }
            }
        }

        /// <summary>
        ///     Получить информацию о сервере
        /// </summary>
        [GetOperation("info", "/<endpoint>/info")]
        public ServerInfoDto GetServerInfo(Endpoint endpoint)
        {
            using (var unit = new UnitOfWork())
            {
                var response = new ServerInfoDto();
                try
                {
                    var existServer = unit.Repository<Entities.Server>().FindOne(a => a.Endpoint == endpoint.ToString());
                    if (existServer?.Info != null)
                    {
                        return existServer.Info.ToDto<ServerInfoDto>();
                    }
                    response.StatusCode = 404;
                    return response;
                }
                catch (Exception)
                {
                    response.StatusCode = 500;
                    return response;
                }
            }
        }

        /// <summary>
        ///     Получить информацию о серверах
        /// </summary>
        [GetOperation("info", "/info")]
        public JsonList<ServerDto> GetAllServersInfo()
        {
            using (var unit = new UnitOfWork())
            {
                var model = new JsonList<ServerDto>();
                try
                {
                    var allServers = unit.Repository<Entities.Server>().FindAll();
                    model = allServers.ToJsonList<ServerDto>();
                    return model;
                }
                catch (Exception)
                {
                    model.StatusCode = 500;
                    return model;
                }
            }
        }

        /// <summary>
        ///     Получение информации о завершенном матче
        /// </summary>
        [GetOperation("matches", "/<endpoint>/matches/<timestamp>")]
        public MatchResultDto GetMatchInfo(Endpoint endpoint, DateTime timestamp)
        {
            using (var unit = new UnitOfWork())
            {
                var response = new MatchResultDto();
                try
                {
                    var match = unit.Repository<Entities.Match>()
                        .FindOne(a => a.Server == endpoint.ToString() && a.TimeStamp == timestamp);
                    if (match?.Results != null)
                    {
                        return match.Results.ToDto<MatchResultDto>();
                    }
                    response.StatusCode = 404;
                    return response;
                }
                catch(Exception)
                {
                    response.StatusCode = 500;
                    return response;
                }
            }
        }

        /// <summary>
        ///     Получение статистики о играх на сервере
        /// </summary>
        [GetOperation("stats", "/<endpoint>/stats")]
        public FullServerStatsDto GetServerStats(Endpoint endpoint)
        {
            using (var unit = new UnitOfWork(true))
            {
                var response = new FullServerStatsDto();
                try
                {
                    var server = unit.Repository<Entities.Server>().FindOne(a => a.Endpoint == endpoint.ToString());
                    if (server == null)
                    {
                        response.StatusCode = 404;
                        return response;
                    }
                    response.Endpoint = endpoint.ToString();
                    response.Name = server.Info.Name;
                    var matches = unit.Repository<Entities.Match>().Find(a => a.Server == endpoint.ToString());
                    response.TotalMatchesPlayed = matches.Count;
                    
                    var byDay = matches.Where(a => a.TimeStamp != null).GroupBy(match => match.TimeStamp.Value.DayOfYear).ToList();
                    response.MaximumMatchesPerDay = byDay.Max(a => a.ToList().Count);
                    response.AverageMatchesPerDay = byDay.Average(a => a.ToList().Count);

                    response.MaximumPopulation = matches.Max(a => a.Results.ScoreBoard.Count);
                    response.AveragePopulation = matches.Average(a => a.Results.ScoreBoard.Count);

                    var maps = matches.Where(a => a.Results != null).Select(match => match.Results.Map).ToList();
                    var dictMaps = new Dictionary<string, int>();
                    foreach (var map in maps)
                    {
                        if (dictMaps.ContainsKey(map))
                            dictMaps[map]++;
                        else dictMaps.Add(map, 1);
                    }
                    var orderMaps = dictMaps.OrderByDescending(a => a.Value).Select(a => a.Key).Take(5);
                    response.Top5Maps.AddRange(orderMaps);

                    var gameModes = matches.Where(a => a.Results != null).Select(match => match.Results.GameMode).ToList();
                    var dictModes = new Dictionary<string, int>();
                    foreach (var mode in gameModes)
                    {
                        if (dictModes.ContainsKey(mode))
                            dictModes[mode]++;
                        else dictModes.Add(mode, 1);
                    }
                    var orderModes = dictModes.OrderByDescending(a => a.Value).Select(a => a.Key).Take(5);
                    response.Top5GameModes.AddRange(orderModes);
                    return response;
                }
                catch(Exception ex)
                {
                    response.StatusCode = 500;
                    return response;
                }
            }
        }
    }
}