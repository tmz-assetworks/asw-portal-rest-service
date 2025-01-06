using Newtonsoft.Json;
using PortalRestService.Core.Models;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetChartDetailsListRepository : OcppRepository<ChartDetailsListResponse>, IChartDetailsListRepository
    {
        TokenBase _tokenBase;
        private readonly ILocationRepository _locationRepository;
        public GetChartDetailsListRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, ILocationRepository locationRepository, TokenBase token) : base(dbContext)
        {
            _tokenBase=token;
            _locationRepository = locationRepository;
        }

         async Task<List<ChartDetailsList>> IChartDetailsListRepository.GetChartDetailsList(ChartDetailsListRequest request)
        {
            List<ChartDetailsList> res = new List<ChartDetailsList>();
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            
                if (string.IsNullOrEmpty(request.Duration) || request.Duration.ToLower() == "string")
                    request.Duration = "1";

            List<long> locationList = await _locationRepository.GetAllLocationIdByObjectId();
            if (request.Flag.ToLower() == "chargerSession".ToLower())
            {
                if (!string.IsNullOrEmpty(request.Fromdate) || !string.IsNullOrEmpty(request.Todate) || request.status.Count > 0)
                {
                    if (!string.IsNullOrEmpty(request.Fromdate) && !string.IsNullOrEmpty(request.Todate))
                    {
                        request.Fromdate = Convert.ToDateTime(request.Fromdate).ToString("yyyy-MM-dd");
                        request.Todate = Convert.ToDateTime(request.Todate).ToString("yyyy-MM-dd");
                    }
                    
                    res = (from s in _dbContext.ChargingSessions
                           join charger in _dbContext.Charger on s.ChargerId equals charger.Id
                           join location in _dbContext.Locations.Where(x => locationList.Contains(x.Id)) on charger.LocationId equals location.Id
                           join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                           join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id

                           select new ChartDetailsList
                           {
                               Id = s.Id,
                               ChargerName = charger.ChargeBoxId,
                               UID = "",
							   ChargerType = String.Join(",", _dbContext.Port.Where(p => p.ChargerId == charger.Id && p.Connectorid==s.ConnectorId).Select(s => s.Connector.ConnectorType)),
							   FaultSince = _dbContext.ChargerStatuses.Where(x => x.ConnectorStatus.ToLower() == "faulted" && x.ChargerId==s.ChargerId && x.ConnectorId==s.ConnectorId).Count() == 0 ? "" :
                                (DateTime.Now - _dbContext.ChargerStatusHistory.Where(x => x.ConnectorStatus.ToLower() == "faulted" && x.ChargerId == s.ChargerId && x.ConnectorId == s.ConnectorId).OrderByDescending(m => m.Id).FirstOrDefault().CreatedOn).Value.Hours.ToString() + " hours",
                               FaultDescription = "",
                               TimeReported = s.StartTime,
                               LocationId = location.Id,
                               LocationName = location.LocationName,
                               ChargingStatus = (request.ChartType.ToLower() == "chargerinuse" ?
                               (
                                                                s.ChargingStatus.ToLower().Equals("completed".ToLower()) ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("cancelled".ToLower()) ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("interrupted".ToLower()) ? "Available" : "Unavailable"
                                                        ) : s.ChargingStatus


                               ),
                               
                               ChargeBoxId = charger.ChargeBoxId,
                               StartTime = Convert.ToDateTime(s.StartTime).ToString("yyyy-MM-dd"),
                               EndTime = Convert.ToDateTime(s.EndTime).ToString("yyyy-MM-dd"),
                                Startmetervalue = Math.Round(Convert.ToDecimal(s.StartMeterValue) / 1000, 2).ToString(),
                               Endmetervalue = Math.Round(Convert.ToDecimal(s.EndMeterValue) / 1000, 2).ToString(),
                               Startsoc = s.StartSoc,
                               EndSoc = s.EndSoc,

                               ReasoneForStop = s.ReasonForStop
                           }).OrderByDescending(a => a.TimeReported).ToList<ChartDetailsList>();
                    if (res != null)
                    {
                        if (!string.IsNullOrEmpty(request.Fromdate))
                        {
                            res = res.Where(o => Convert.ToDateTime(o.StartTime) >= Convert.ToDateTime(request.Fromdate) && Convert.ToDateTime(o.StartTime) <= Convert.ToDateTime(request.Todate)).ToList();
                        }
                        if (request.status.Count > 0)
                        {
                            res = res.Where(o => request.status.Contains(o.ChargingStatus, StringComparer.InvariantCultureIgnoreCase)).ToList();
                        }
                    }                    
                }
                else
                {
                    res = (from s in _dbContext.ChargingSessions.ToList()
                           where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(request.Duration)) && s.StartTime <= DateTime.Now
                           join charger in _dbContext.Charger on s.ChargerId equals charger.Id
                           join location in _dbContext.Locations.Where(x => locationList.Contains(x.Id)) on charger.LocationId equals location.Id
                           join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                           join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id
                           
                           select new ChartDetailsList
                           {
                               Id = s.Id,
                               ChargerName = charger.ChargeBoxId,
                               UID = "",
							   ChargerType = String.Join(",", _dbContext.Port.Where(p => p.ChargerId == s.ChargerId && p.Connectorid==s.ConnectorId).Select(s => s.Connector.ConnectorType)),
							   FaultSince = _dbContext.ChargerStatuses.Where(x => x.ConnectorStatus.ToLower() == "faulted" && x.ChargerId == s.ChargerId && x.ConnectorId == s.ConnectorId).Count() == 0 ? "" :
                                (DateTime.Now - _dbContext.ChargerStatusHistory.Where(x => x.ConnectorStatus.ToLower() == "faulted" && x.ChargerId == s.ChargerId && x.ConnectorId == s.ConnectorId).OrderByDescending(m => m.Id).FirstOrDefault().CreatedOn).Value.Hours.ToString() + " hours",
                               FaultDescription = "",
                               TimeReported = s.StartTime,
                               LocationId = location.Id,
                               LocationName = location.LocationName,
                               ChargingStatus = (request.ChartType.ToLower() == "chargerinuse" ?
                               (
                                                                s.ChargingStatus.ToLower().Equals("completed".ToLower()) ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("cancelled".ToLower()) ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("interrupted".ToLower()) ? "Available" : "Unavailable"
                                                        ) : s.ChargingStatus


                               ),
                               ChargeBoxId = charger.ChargeBoxId,
                               StartTime = Convert.ToDateTime(s.StartTime).ToString("yyyy-MM-dd"),
                               EndTime = Convert.ToDateTime(s.EndTime).ToString("yyyy-MM-dd"),
                               Startmetervalue = Math.Round(Convert.ToDecimal(s.StartMeterValue)/1000,2).ToString(),
                               Endmetervalue = Math.Round(Convert.ToDecimal(s.EndMeterValue) / 1000, 2).ToString(),
                               Startsoc = s.StartSoc,
                               EndSoc = s.EndSoc,

                               ReasoneForStop = s.ReasonForStop
                           }).OrderByDescending(a => a.TimeReported).ToList<ChartDetailsList>();

                    
                }                
                if (request.LocationIds.Count > 0)
                {
                    res = res.Where(o => request.LocationIds.Contains(o.LocationId)).ToList();
                }
            }
            else if (request.Flag.ToLower() == "locationstatus".ToLower())
            {
                res = (from location in request.LocationIds.Count>0 ? _dbContext.Locations.Where(x=> request.LocationIds.Contains(x.Id)) : _dbContext.Locations
                       join disp in _dbContext.Charger  on location.Id equals disp.LocationId
                       join locationstatus in _dbContext.LocationStatus on location.LocationStatusId equals locationstatus.Id
                       join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                        on location.Id equals userMap.LocationId
                       select new ChartDetailsList
                       {
                           Id = disp.Id,
                           ChargerName = disp.ChargeBoxId,
                           UID = "",
                           ChargerType = "OCPP",
                           FaultSince = disp.ChargerStatuses.ToList().Where(x => x.ConnectorStatus.ToLower() == "faulted").ToList().Count == 0 ? "" :
                                (DateTime.Now - disp.ChargerStatusHistories.Where(x => x.ConnectorStatus.ToLower() == "faulted").OrderByDescending(m => m.Id).FirstOrDefault().CreatedOn).Value.Hours.ToString() + " hours",
                           FaultDescription = "",
                           TimeReported = disp.ChargerStatuses.ToList().Where(x => x.ConnectorStatus.ToLower() == "faulted").ToList().Count == 0 ? DateTime.MinValue :
                                disp.ChargerStatusHistories.Where(x => x.ConnectorStatus.ToLower() == "faulted").OrderByDescending(m => m.Id).FirstOrDefault().CreatedOn,
                           LocationId = disp.LocationId.Value,
                           LocationName = location.LocationName,
                           ChargeBoxId = disp.ChargeBoxId,
                           LocationStatus = locationstatus.LocationStatusName
                       }
               ).OrderByDescending(a => a.LocationName).ToList<ChartDetailsList>();

            }
            if (res == null)
            {
                res=new List<ChartDetailsList>();
            }
           

            return res;


        }

        
    }
}
