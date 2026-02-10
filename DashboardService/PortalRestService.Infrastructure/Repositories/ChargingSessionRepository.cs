using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Models;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net.Http.Headers;
using System.Text;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
    public class ChargingSessionRepository : OcppRepository<ChargerSessionByLocationResponse>, IChargingSessionRepository
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMilesAddedByLocationQueryRepository _milesAddedByLocationQueryRepository;
        public ChargingSessionRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, ILocationRepository locationRepository, IMilesAddedByLocationQueryRepository milesAddedByLocationQueryRepository) : base(dbContext)
        {
            _locationRepository = locationRepository;
            _milesAddedByLocationQueryRepository = milesAddedByLocationQueryRepository;
        }

        public Task<ChargingSessionByLocationForChartResponse> AddAsync(ChargingSessionByLocationForChartResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ChargingSessionByLocationForChartResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task<ChargingSessionByLocationForChartResponse> UpdateAsync(ChargingSessionByLocationForChartResponse entity)
        {
            throw new NotImplementedException();
        }

        public DateTime GetStartDate(int day)
        {
            return DateTime.Now.AddDays(-day);

        }
        

        public async Task<ChargingSessionByLocationForChartResponse> GetChargerSession(List<int> locations,string duration, string ChargerBoxId)
        {
            ChargingSessionByLocationForChartResponse obj = new();

            try
            {
                DurationAndIntervalDto dto = await _milesAddedByLocationQueryRepository.durationAndIntervalAsync(duration);

                string labelType = dto.laveltype;
                TimeSpan interval = dto.interval;
                duration = dto.duration;

                List<long> locationList = await _locationRepository.GetAllLocationIdByObjectId();

                DateTime fromDate = DateTime.Now.AddDays(-Convert.ToInt32(duration));
                DateTime toDate = DateTime.Now;

                IQueryable<Core.Models.ChargingSession> sessions = _dbContext.ChargingSessions
                    .AsNoTracking()
                    .Where(s => s.StartTime >= fromDate && s.StartTime <= toDate);

                IQueryable<Charger> chargers = string.IsNullOrEmpty(ChargerBoxId)
                    ? _dbContext.Charger
                    : _dbContext.Charger.Where(x => x.ChargeBoxId.ToLower() == ChargerBoxId.ToLower());

                IQueryable<Location> locationQuery = locations.Count > 0
                    ? _dbContext.Locations.Where(x => locations.Contains((int)x.Id) && locationList.Contains(x.Id))
                    : _dbContext.Locations.Where(x => locationList.Contains(x.Id));

                var raw = await (
                    from s in sessions
                    join c in chargers on s.ChargerId equals c.Id
                    join l in locationQuery on c.LocationId equals l.Id
                    select new
                    {
                        s.ChargingStatus,
                        s.StartTime
                    }).ToListAsync();

                List<ChargingSessionByLocationChartBO> finalon;

                if (!raw.Any())
                {
                    finalon = getstatus(duration);
                }
                else
                {
                    finalon = raw
                        .Where(x => x.StartTime.HasValue)
                        .Select(x =>
                        {
                            DateTime bucket =
                                new DateTime((x.StartTime!.Value.Ticks / interval.Ticks) * interval.Ticks);

                            return new ChargingSessionByLocationChartBO
                            {
                                ChargingStatus = x.ChargingStatus,
                                SortDate = bucket,

                                svalue = labelType switch
                                {
                                    "time" => bucket.ToString("HH"),
                                    "day" => bucket.ToString("MMdd"),
                                    "date" => bucket.ToString("MMdd"),
                                    _ => bucket.ToString("MM")
                                },

                                times = labelType switch
                                {
                                    "time" => bucket.ToString("HH"),
                                    "day" => bucket.ToString("dddd"),
                                    "date" => bucket.ToString("MM-dd-yyyy"),
                                    _ => bucket.ToString("MMMM")
                                }
                            };
                        })
                        .GroupBy(x => new { x.SortDate, x.times, x.ChargingStatus })
                        .Select(g => new ChargingSessionByLocationChartBO
                        {
                            ChargingStatus = g.Key.ChargingStatus,
                            SortDate = g.Key.SortDate,
                            times = g.Key.times.Length >= 2 ? g.Key.times : "0" + g.Key.times,
                            svalue = g.Max(x => x.svalue),
                            Counts = g.Count(),
                            Color = Extensions.GetColorCodesByChargingSession(g.Key.ChargingStatus)
                        })
                        .OrderBy(x => x.SortDate)
                        .ThenBy(x => x.ChargingStatus)
                        .ToList();
                }

                obj.StatusMessage = finalon.Any()
                    ? RespnoseMessage.Record_found
                    : RespnoseMessage.Record_not_found;

                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception)
            {
                obj.StatusMessage = RespnoseMessage.Opeartion_Failed;
                obj.StatusCode = RespnoseCode.Bad_Request;
                obj.data = new List<ChargingSessionByLocationChartBO>();
            }

            return obj;
        }



        public List<ChargingSessionByLocationChartBO> getstatus(string duration)
        {
            List<ChargingSessionByLocationChartBO> chargingSessionByLocationBOs = new List<ChargingSessionByLocationChartBO>();

            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            if (duration == "1")
            {
                duration = "1";
                interval = new TimeSpan(4, 0, 0);
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "04", ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = "04" });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "08", ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = "08" });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "12", ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = "12" });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "16", ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = "16" });

            }
            if (duration == "6")
            {
                duration = "6";
                interval = new TimeSpan(24, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy"), ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddDays(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-2).ToString("MM-dd-yyyy"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-3).ToString("MM-dd-yyyy"), ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = (new DateTime((DateTime.Now.AddDays(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-4).ToString("MM-dd-yyyy"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });

            }
            else
            if (duration == "28")
            {

                interval = new TimeSpan(24 * 7, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-6).ToString("MM-dd-yyyy"), ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddDays(-6).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-12).ToString("MM-dd-yyyy"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-12).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-18).ToString("MM-dd-yyyy"), ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = (new DateTime((DateTime.Now.AddDays(-18).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-24).ToString("MM-dd-yyyy"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-24).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
            }
            else
            if (duration == "90")
            {
                interval = new TimeSpan(24, 0, 0);
                laveltype = "date";
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-1).ToString("MMMM"), ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddMonths(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-2).ToString("MMMM"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddMonths(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-3).ToString("MMMM"), ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = (new DateTime((DateTime.Now.AddMonths(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-4).ToString("MMMM"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddMonths(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });


            }
            return chargingSessionByLocationBOs;

        }
    }
}

