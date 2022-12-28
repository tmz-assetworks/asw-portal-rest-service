using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net.Http.Headers;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
#pragma warning disable
    
    public class LocationStatusByLocationIdRepository : OcppRepository<AllLocationStatusChartBO>, ILocationStatusByLocationIdRepository
    {
        TokenBase _tokenBase;
        public LocationStatusByLocationIdRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }
        public async Task<List<AllLocationStatusChartBO>> GetLocationStatusByLocatonId(List<int> locations, string duration)
        {

            AllLocationStatusQueryResponse obj = new AllLocationStatusQueryResponse();
            List<LocationStatusData> LocationStatus = new List<LocationStatusData>();

            List<AllLocationStatusChartBO> res = (from s in _dbContext.ChargingSessions.ToList().Where(o => o.DeviceId != null)
                                             where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                             join charger in _dbContext.Charger on s.ChargerId equals charger.Id
                                             join location in locations.Count > 0 ? _dbContext.Locations.Where(x => locations.Contains((int)(x.Id))) : _dbContext.Locations on charger.LocationId equals location.Id
                                             join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                                             join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id
                                             join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                             on location.Id equals userMap.LocationId
                                                  select new AllLocationStatusChartBO
                                                  {
                                                      LocationStatus = Status.LocationStatusName,
                                                  }).ToList<AllLocationStatusChartBO>();


            List<AllLocationStatusChartBO> finalon = null;
            if (res.Count <= 0)
            {
                finalon = getstatus();
            }
            else
            { 
                    finalon = res
                .GroupBy(x => new { x.LocationStatus })
                .Select(y => new AllLocationStatusChartBO()
                {
                    LocationStatus = y.Key.LocationStatus,
                    Counts = y.ToList().Count,
                    Color = Extensions.GetColorCodesByStatus(y.Key.LocationStatus)
                }
                ).ToList<AllLocationStatusChartBO>();
            }
           

            return finalon;


        }
        public List<AllLocationStatusChartBO> getstatus()
        {
            List<AllLocationStatusChartBO> chargingSessionByLocationBOs = new List<AllLocationStatusChartBO>();


            chargingSessionByLocationBOs.Add(new AllLocationStatusChartBO() { Color = Extensions.GetColorCodesByStatus("Live"), LocationStatus = "Live", Counts = 0 });
            chargingSessionByLocationBOs.Add(new AllLocationStatusChartBO() { Color = Extensions.GetColorCodesByStatus("Under Maintenance"), LocationStatus = "Under Maintenance", Counts = 0 });
            chargingSessionByLocationBOs.Add(new AllLocationStatusChartBO() { Color = Extensions.GetColorCodesByStatus("Upcoming"), LocationStatus = "Upcoming", Counts = 0 });
            chargingSessionByLocationBOs.Add(new AllLocationStatusChartBO() { Color = Extensions.GetColorCodesByStatus("Installed"), LocationStatus = "Installed", Counts = 0 });
            chargingSessionByLocationBOs.Add(new AllLocationStatusChartBO() { Color = Extensions.GetColorCodesByStatus("Commissioned"), LocationStatus = "Commissioned", Counts = 0 });
            chargingSessionByLocationBOs.Add(new AllLocationStatusChartBO() { Color = Extensions.GetColorCodesByStatus("Decommissioned"), LocationStatus = "Decommissioned", Counts = 0 });
            return chargingSessionByLocationBOs;
        }

    }
}
