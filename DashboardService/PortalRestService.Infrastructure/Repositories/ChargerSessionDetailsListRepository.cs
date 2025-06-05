using Newtonsoft.Json;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class ChargerSessionDetailsListRepository : OcppRepository<ChargerSessionDetailsListResponse>, IGetChargerSessionDetailsListRepository
    {
        TokenBase _tokenBase;
        private readonly ILocationRepository _locationRepository;
        public ChargerSessionDetailsListRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token, ILocationRepository locationRepository) : base(dbContext)
        {
            _tokenBase = token;
            _locationRepository = locationRepository;
        }

        async Task<PagedList<ChargerSessionDetailsList>> IGetChargerSessionDetailsListRepository.GetChargerSessionDetailsList(ChargerSessionListRequest request)
        {
            List<ChargerSessionDetailsList> res = new List<ChargerSessionDetailsList>();
            List<long> locationIdList = await _locationRepository.GetAllLocationIdByObjectId();
            string zero = "0000000000";
            res = (from c in request.chargerboxid.Count > 0 ? _dbContext.ChargingSessions.ToList().Where(o => request.chargerboxid.Contains(o.DeviceId, StringComparer.InvariantCultureIgnoreCase) && o.DeviceId != null) : _dbContext.ChargingSessions.ToList().Where(o => o.DeviceId != null)
                   join vehiclerfid in _dbContext.VehicleRFID on c.RfId equals vehiclerfid.Name
                   join vehicle in _dbContext.Vehicle on vehiclerfid.VehicleId equals vehicle.Id
                   join charger in _dbContext.Charger on c.ChargerId equals charger.Id
                   join location in _dbContext.Locations.Where(x => locationIdList.Contains(x.Id)) on charger.LocationId equals location.Id
                   select new ChargerSessionDetailsList
                   {
                       Id = c.Id,
                       Duration = "",
                       Sessionid = zero.Substring(0, (10 - c.Id.ToString().Length)) + c.Id.ToString(),
                       Usage = (Convert.ToDouble(c.EndMeterValue) < Convert.ToDouble(c.StartMeterValue)) ? 0 : Math.Round((Convert.ToDouble(c.EndMeterValue) - Convert.ToDouble(c.StartMeterValue <= 0 ? 0 : c.StartMeterValue)) / 1000, 2),
                       StartTime = c.StartTime,
                       EndTime = c.EndTime,
                       ChargingStatus = c.ChargingStatus,
                       ChargeBoxId = c.DeviceId,
                       ModifiedAt = c.ModifiedAt,
                       CreatedAt = c.CreatedAt,
                       Startmetervalue = c.StartMeterValue,
                       Endmetervalue = c.EndMeterValue,
                       Startsoc = c.StartSoc,
                       EndSoc = c.EndSoc,
                       ReasoneForStop = c.ReasonForStop,
                       AssetId = vehicle.AssetId
                   }).DistinctBy(d => d.Id).OrderByDescending(a => a.ModifiedAt).Where(s => s.ChargeBoxId != null).ToList();
            if (!string.IsNullOrEmpty(request.Fromdate) && !string.IsNullOrEmpty(request.Todate) && res.Any())
            {
                res = res.Where(o => o.StartTime >= Convert.ToDateTime(request.Fromdate) && o.StartTime <= Convert.ToDateTime(request.Todate)).ToList();
                if (request.status.Count > 0)
                    res = res.Where(o => request.status.Contains(o.ChargingStatus, StringComparer.InvariantCultureIgnoreCase)).ToList();
            }
            if (res.Count > 0)
            {
                foreach (var s in res)
                {
                    if (s.EndTime.HasValue && s.StartTime.HasValue)
                    {
                        System.TimeSpan diff1 = (TimeSpan)(s.EndTime - s.StartTime);
                        int total_seconds = (int)diff1.TotalSeconds;
                        int hours = total_seconds / (60 * 60);
                        int remaining_seconds = total_seconds - hours * (60 * 60);
                        int minutes = remaining_seconds / 60;
                        int seconds = remaining_seconds % 60;

                        s.Duration = string.Format("{0:#00}:{1:#00}:{2:#00}", hours, minutes, seconds);
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchParam))
                    res = res.Where(d => d.ChargeBoxId.ToLower() == request.SearchParam.ToLower()).ToList();                
            }
            var dataResult = PagedList<ChargerSessionDetailsList>.ToPagedList(res, request.PageNumber, request.PageSize);
            return await Task.FromResult(dataResult);
        }

    }
}
    
