using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PortalRestService.Application;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.DBContext;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetSummaryStatusRepository : OcppRepository<CardDataResponse>, IGetSummaryStatusRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILocationRepository _locationRepository;
        private readonly IMemoryCache _cache;
        private readonly IDbContextFactory<ocpp_dbContext> _dbFactory;

        public GetSummaryStatusRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, IConfiguration configuration,
            ILocationRepository locationRepository, IMemoryCache cache, IDbContextFactory<ocpp_dbContext> dbFactory) : base(dbContext)
        {
            this._configuration = configuration;
            this._locationRepository = locationRepository;
            _cache = cache;
            _dbFactory = dbFactory;

        }

        public async Task<CardDataResponse> GetSummaryStatus(int locationId, bool isChargersReq)
        {
            var cacheKey = $"SummaryStatus:{locationId}";
            if (_cache.TryGetValue(cacheKey, out var value) && value is CardDataResponse cached)
            {
                return cached;
            }
            var stopwatch = Stopwatch.StartNew();
            var response = new CardDataResponse();

            if (locationId > 0 && isChargersReq)
            {
                return new CardDataResponse
                {
                    data =null,
                    StatusMessage = "Request is not valid.",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            try
            {
                var allowedLocationIds = await _locationRepository.GetAllLocationIdByObjectId();
                var cardDataList = new List<CardData>();

                // ==========================
                // Locations Summary (only for all locations)
                // ==========================
                if (locationId == 0)
                {
                    var locationCounts = await _dbContext.Locations.AsNoTracking()
                        .Where(l => allowedLocationIds.Contains(l.Id))
                        .GroupBy(l => l.LocationStatus.LocationStatusName)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToListAsync();

                    cardDataList.Add(new CardData
                    {
                        Type = "Locations",
                        Count = locationCounts.Sum(x => x.Count),
                        StatusData = new List<StatusData>
                {
                    new() { Key = Status_Indication.LocationStatus.Live.GetEnumDisplayName(),
                            Value = CommonHelpers.GetHoursTwoDigitFormat(locationCounts.FirstOrDefault(x => x.Status == Status_Indication.LocationStatus.Live.GetEnumDisplayName())?.Count ?? 0),
                            Color = ColorsEnum.LocationsColor.Live.GetEnumDisplayName() },
                    new() { Key = Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName(),
                            Value = CommonHelpers.GetHoursTwoDigitFormat(locationCounts.FirstOrDefault(x => x.Status == Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName())?.Count ?? 0),
                            Color = ColorsEnum.LocationsColor.UnderMaintenance.GetEnumDisplayName() },
                    new() { Key = Status_Indication.LocationStatus.Inactive.GetEnumDisplayName(),
                            Value = CommonHelpers.GetHoursTwoDigitFormat(locationCounts.FirstOrDefault(x => x.Status == Status_Indication.LocationStatus.Inactive.GetEnumDisplayName())?.Count ?? 0),
                            Color = ColorsEnum.LocationsColor.Inactive.GetEnumDisplayName() },
                    new() { Key = Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName(),
                            Value = CommonHelpers.GetHoursTwoDigitFormat(locationCounts.FirstOrDefault(x => x.Status == Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName())?.Count ?? 0),
                            Color = ColorsEnum.LocationsColor.Commissioned.GetEnumDisplayName() },
                    new() { Key = Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName(),
                            Value = CommonHelpers.GetHoursTwoDigitFormat(locationCounts.FirstOrDefault(x => x.Status == Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName())?.Count ?? 0),
                            Color = ColorsEnum.LocationsColor.Upcoming.GetEnumDisplayName() }
                }
                    });
                }


                var chargerCountsTask = GetChargerCountsAsync(locationId, allowedLocationIds);
                var activeChargerTask = GetActiveChargerCountAsync(locationId, allowedLocationIds);
                var sessionCountsTask = GetSessionCountsAsync(locationId, allowedLocationIds);
                var errorCountsTask = GetErrorCountsAsync(allowedLocationIds);

                await Task.WhenAll(chargerCountsTask, activeChargerTask, sessionCountsTask, errorCountsTask);

                var chargerCounts = await chargerCountsTask;
                var activeChargerCount = await activeChargerTask;
                var sessionCounts = await sessionCountsTask;
                var errorCountsList = await errorCountsTask;

                var chargerLookup = chargerCounts.ToDictionary(x => x.Status, x => x.Count);

                chargerLookup.TryGetValue("Available", out var available);
                chargerLookup.TryGetValue("Connected", out var connected);
                chargerLookup.TryGetValue("Offline", out var offline);

                cardDataList.Add(new CardData
                {
                    Type = "Chargers",
                    Count = activeChargerCount,
                    StatusData = new List<StatusData>
    {
        new()
        {
            Key = Status_Indication.ChargerStatus.Available.GetEnumDisplayName(),
            Value = CommonHelpers.GetHoursTwoDigitFormat(available),
            Color = ColorsEnum.ChargerStatus.Available.GetEnumDisplayName()
        },
        new()
        {
            Key = Status_Indication.ChargerStatus.Connected.GetEnumDisplayName(),
            Value = CommonHelpers.GetHoursTwoDigitFormat(connected),
            Color = ColorsEnum.ChargerStatus.Connected.GetEnumDisplayName()
        },
        new()
        {
            Key = Status_Indication.ChargerStatus.Offline.GetEnumDisplayName(),
            Value = CommonHelpers.GetHoursTwoDigitFormat(offline),
            Color = ColorsEnum.ChargerStatus.Offline.GetEnumDisplayName()
        }
    }
                });


                var sessionLookup = sessionCounts.ToDictionary(x => x.Status, x => x.Count);

                sessionLookup.TryGetValue("Cancelled", out var cancelled);
                sessionLookup.TryGetValue("Interrupted", out var interrupted);
                sessionLookup.TryGetValue("Completed", out var completed);

                cardDataList.Add(new CardData
                {
                    Type = "Charging Sessions",
                    Count = sessionCounts.Sum(x => x.Count),
                    StatusData = new List<StatusData>
    {
        new()
        {
            Key = Status_Indication.ChargingSessionStatus.Cancelled.ToString(),
            Value = CommonHelpers.GetHoursTwoDigitFormat(cancelled),
            Color = ColorsEnum.ChargingSessionsColor.Cancelled.GetEnumDisplayName()
        },
        new()
        {
            Key = Status_Indication.ChargingSessionStatus.Interrupted.ToString(),
            Value = CommonHelpers.GetHoursTwoDigitFormat(interrupted),
            Color = ColorsEnum.ChargingSessionsColor.Interrupted.GetEnumDisplayName()
        },
        new()
        {
            Key = Status_Indication.ChargingSessionStatus.Completed.ToString(),
            Value = CommonHelpers.GetHoursTwoDigitFormat(completed),
            Color = ColorsEnum.ChargingSessionsColor.Completed.GetEnumDisplayName()
        }
    }
                });

                // ==========================
                // Errors / Alerts Summary



                var errorLookup = errorCountsList.ToDictionary(x => x.Severity, x => x.Count);

                errorLookup.TryGetValue("Critical", out var critical);
                errorLookup.TryGetValue("High", out var high);
                errorLookup.TryGetValue("Medium", out var medium);

                var errorCounts = new Dictionary<string, int>
                {
                    ["Critical"] = critical,
                    ["High"]     = high,
                    ["Medium"]   = medium
                };

                cardDataList.Add(new CardData
                {
                    Type = locationId == 0 ? "Active Errors" : "Alerts",
                    Count = errorCounts.Values.Sum(),
                    StatusData = new List<StatusData>
            {
                new() { Key = "Critical", Value = CommonHelpers.GetHoursTwoDigitFormat(errorCounts["Critical"]), Color = ColorsEnum.ErrorsColor.Critical.GetEnumDisplayName() },
                new() { Key = "High", Value = CommonHelpers.GetHoursTwoDigitFormat(errorCounts["High"]), Color = ColorsEnum.ErrorsColor.High.GetEnumDisplayName() },
                new() { Key = "Medium", Value = CommonHelpers.GetHoursTwoDigitFormat(errorCounts["Medium"]), Color = ColorsEnum.ErrorsColor.Medium.GetEnumDisplayName() }
            }
                });

                // ==========================
                // Final Response
                // ==========================
                response.data = cardDataList;
                response.StatusMessage = RespnoseMessage.Record_found;
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Error in GetSummaryStatus for location {LocationId}", locationId);
                response.StatusMessage = "Internal server error.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            stopwatch.Stop();
            _cache.Set(cacheKey, response, TimeSpan.FromSeconds(15));
            Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");

            return response;
        }

        private async Task<List<(string Status, int Count)>> GetChargerCountsAsync(int locationId, List<long> allowedLocationIds)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var chargersBase = db.Charger.AsNoTracking()
                .Where(c =>
                    c.LocationId.HasValue &&
                    allowedLocationIds.Contains(c.LocationId.Value) &&
                    (locationId == 0 || c.LocationId == locationId));

            var latestStatus =
                from s in db.ChargerStatuses.AsNoTracking()
                group s by s.ChargerId into g
                select new
                {
                    ChargerId = g.Key,
                    ModifiedAt = g.Max(x => x.ModifiedAt)
                };

            var latestJoined =
                from s in db.ChargerStatuses.AsNoTracking()
                join m in latestStatus
                    on new { s.ChargerId, s.ModifiedAt }
                    equals new { m.ChargerId, m.ModifiedAt }
                select new { s.ChargerId, s.Chargerstatus };

            var chargerQuery =
                from c in chargersBase
                join s in latestJoined
                    on c.Id equals s.ChargerId into sj
                from s in sj.DefaultIfEmpty()
                select s.Chargerstatus ?? "Offline";

            var result = await chargerQuery
                .GroupBy(x => x)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return result.Select(x => (x.Status, x.Count)).ToList();
        }

        private async Task<int> GetActiveChargerCountAsync(int locationId, List<long> allowedLocationIds)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Charger.AsNoTracking()
                .Where(c =>
                    c.LocationId.HasValue &&
                    allowedLocationIds.Contains(c.LocationId.Value) &&
                    (locationId == 0 || c.LocationId == locationId) &&
                    c.IsActive)
                .CountAsync();
        }

        private async Task<List<(string Status, int Count)>> GetSessionCountsAsync(int locationId, List<long> allowedLocationIds)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var result = await (
                from cs in db.ChargingSessions.AsNoTracking()
                join c in db.Charger.AsNoTracking()
                    on cs.ChargerId equals c.Id
                where c.LocationId.HasValue
                      && allowedLocationIds.Contains(c.LocationId.Value)
                      && (locationId == 0 || c.LocationId == locationId)
                group cs by cs.ChargingStatus into g
                select new { Status = g.Key, Count = g.Count() }
            ).ToListAsync();

            return result.Select(x => (x.Status, x.Count)).ToList();
        }
        private async Task<List<(string Severity, int Count)>> GetErrorCountsAsync(List<long> allowedLocationIds)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var last24Hours = DateTime.UtcNow.AddHours(-24);

            var logs = db.OcppEventLogs.AsNoTracking()
                .Where(l => l.RequestType == "StatusNotification"
                && l.CreatedAt >= last24Hours);


            var result =
                from l in logs
                 join c in db.Charger.AsNoTracking()
                 on l.DeviceId equals c.ChargeBoxId
                 where c.LocationId.HasValue
                       && allowedLocationIds.Contains(c.LocationId.Value)
                 //&& (locationId == 0 || c.LocationId == locationId)
                 join fe in db.FaultyErrorCode.AsNoTracking()
                     on l.ErrorCode equals fe.Names
                 where fe.IsActive
                 join es in db.ErrorSeverity.AsNoTracking()
                     on fe.ErrorSeverityId equals es.Id
                 where es.IsActive
                 group es by es.Names into g
                 select new
                 {
                     Severity = g.Key,
                     Count = g.Count()
                 };


            var list = await result.ToListAsync();

            return list.Select(x => (x.Severity, x.Count)).ToList();
        }
        public static string geterror(string str, string RequestType)
        {
            if (RequestType.ToLower() != "StatusNotification".ToLower())
                return "";
            try
            {
                var jArray = JArray.Parse(str);
                var payload = jArray[3] as JObject;
                if (payload == null) return "";
                var errorCode = payload["errorCode"]?.ToString() ?? "";
                return Regex.Replace(errorCode, "[^a-zA-Z0-9 -]", "", RegexOptions.None, TimeSpan.FromSeconds(5)).Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
