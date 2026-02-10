using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Models;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;

namespace PortalRestService.Infrastructure.Repositories
{
#pragma warning disable
    public class EnergyUsedByLocationIDRepository : OcppRepository<EnergyUsedBOForChartResponse>, IEnergyUsedByLocationIDRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string OccpIp = String.Empty;
        private readonly ILocationRepository _locationRepository;
        private readonly IMilesAddedByLocationQueryRepository _milesAddedByLocationQueryRepository;
        public EnergyUsedByLocationIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, IConfiguration configuration, ILocationRepository locationRepository, IMilesAddedByLocationQueryRepository milesAddedByLocationQueryRepository) : base(dbContext)
        {
            this._configuration = configuration;
            OccpIp = this._configuration.GetSection("OccpIp").GetSection("ip").Value;
            _locationRepository = locationRepository;
            _milesAddedByLocationQueryRepository = milesAddedByLocationQueryRepository;
        }

        public async Task<EnergyUsedBOForChartResponse> GetEnergyUsedByLocationID( List<int> location, string duration, string chargeBoxId)
        {
            EnergyUsedBOForChartResponse obj = new();

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
                    .Where(s =>
                        s.StartTime >= fromDate &&
                        s.StartTime <= toDate &&
                        s.EndMeterValue > 0);

                IQueryable<Charger> chargers = string.IsNullOrEmpty(chargeBoxId)
                    ? _dbContext.Charger
                    : _dbContext.Charger.Where(x => x.ChargeBoxId.ToLower() == chargeBoxId.ToLower());

                IQueryable<Location> locations = location.Count > 0
                    ? _dbContext.Locations.Where(x => location.Contains((int)x.Id) && locationList.Contains(x.Id))
                    : _dbContext.Locations.Where(x => locationList.Contains(x.Id));

                var raw = await (
                    from s in sessions
                    join c in chargers on s.ChargerId equals c.Id
                    join l in locations on c.LocationId equals l.Id
                    select new
                    {
                        s.StartTime,
                        s.StartMeterValue,
                        s.EndMeterValue
                    }).ToListAsync();

                List<EnergyUsedsResponse> finalon;

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

                            return new
                            {
                                SortDate = bucket,
                                Start = x.StartMeterValue ?? 0,
                                End = x.EndMeterValue ?? 0,

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
                        .GroupBy(x => new { x.SortDate, x.times })
                        .Select(g => new EnergyUsedsResponse
                        {
                            SortDate = g.Key.SortDate,
                            times = g.Key.times.Length >= 2 ? g.Key.times : "0" + g.Key.times,
                            svalue = g.Max(x => x.svalue),
                            EndMeterValue = Convert.ToInt32(
                                (g.Sum(c => c.End) - g.Sum(c => c.Start)) / 1000)
                        })
                        .OrderBy(x => x.SortDate)
                        .ToList();
                }

                obj.StatusMessage = RespnoseMessage.Record_found;
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch
            {
                obj.StatusMessage = RespnoseMessage.Faild;
                obj.StatusCode = 404;
                obj.data = new List<EnergyUsedsResponse>();
            }

            return obj;
        }





        public List<EnergyUsedsResponse> getstatus(string duration)
        {
            List<EnergyUsedsResponse> chargingSessionByLocationBOs = new List<EnergyUsedsResponse>();

            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            if (duration == "1")
            {
                duration = "1";
                interval = new TimeSpan(4, 0, 0);
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = "04", EndMeterValue = 0, svalue = "04" });


            }
            if (duration == "6")
            {
                duration = "6";
                interval = new TimeSpan(24, 0, 0);
                laveltype = "day";

                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-1).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-2).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-3).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-4).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });

            }
            else
            if (duration == "28")
            {

                interval = new TimeSpan(24 * 7, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-6).ToString("MM-dd-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-6).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-12).ToString("MM-dd-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-12).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-18).ToString("MM-dd-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-18).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-24).ToString("MM-dd-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-24).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
            }
            else
            if (duration == "90")
            {
                interval = new TimeSpan(24, 0, 0);
                laveltype = "month";
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-1).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-2).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-3).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-4).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });


            }
            return chargingSessionByLocationBOs;

        }
    }
}
