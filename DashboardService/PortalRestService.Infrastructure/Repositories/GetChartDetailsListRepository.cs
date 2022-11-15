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
    public class GetChartDetailsListRepository : OcppRepository<ChartDetailsListResponse>, IChartDetailsListRepository
    {
        TokenBase _tokenBase;
        public GetChartDetailsListRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token) : base(dbContext)
        {
            _tokenBase=token;
        }

         async Task<List<ChartDetailsList>> IChartDetailsListRepository.GetChartDetailsList(ChartDetailsListRequest request)
        {
            List<ChartDetailsList> res = new List<ChartDetailsList>();
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            
                if (string.IsNullOrEmpty(request.Duration) || request.Duration.ToLower() == "string")
                    request.Duration = "1";

            string callingMethoddispenser = APIConstant.GetDispenserByLocations;
            string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
            {
                operatorid = "",
                LocationIds = request.LocationIds
            });

            StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
            HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent, _tokenBase.acces_token);
            var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();
            dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);

            //if filter use then remove defult starttime filter 
            if (request.Flag.ToLower() == "chargerSession".ToLower())
            {
                if (!string.IsNullOrEmpty(request.Fromdate) || !string.IsNullOrEmpty(request.Todate) || request.status.Count > 0)
                {

                    res = (from s in _dbContext.ChargingSessions.ToList()
                           join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                           on s.ChargerId equals c.DispenserId
                           select new ChartDetailsList
                           {
                               Id = s.Id,

                               ChargerName = c.ChargeBoxId,
                               UID = "",
                               ChargerType = c.ConnectorType,
                               FaultSince = "",
                               FaultDescription = "",
                               TimeReported = s.StartTime,
                               LocationId = c.LocationId,
                               LocationName = c.LocationName,
                               ChargingStatus = s.ChargingStatus,
                               ChargeBoxId = c.ChargeBoxId,
                               StartTime = s.StartTime,
                               EndTime = s.EndTime,
                                Startmetervalue = s.StartMeterValue,
                               Endmetervalue = s.EndMeterValue,
                               Startsoc = s.StartSoc,
                               EndSoc = s.EndSoc,

                               ReasoneForStop = s.ReasonForStop
                           }).OrderByDescending(a => a.TimeReported).ToList<ChartDetailsList>();
                    if (res != null)
                    {
                        //if (request.IsExport == false)
                        //{
                            if (!string.IsNullOrEmpty(request.Fromdate))
                            {
                                res = res.Where(o => o.StartTime >= Convert.ToDateTime(request.Fromdate) && o.EndTime <= Convert.ToDateTime(request.Todate)).ToList();
                            }
                            if (request.status.Count > 0)
                            {
                                res = res.Where(o => request.status.Contains(o.ChargingStatus)).ToList();
                            }
                        //}
                    }
                }
                else
                {
                    res = (from s in _dbContext.ChargingSessions.ToList()
                           where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(request.Duration)) && s.StartTime <= DateTime.Now
                           join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                           on s.ChargerId equals c.DispenserId
                           select new ChartDetailsList
                           {
                               Id = s.Id,
                               ChargerName = c.ChargeBoxId,
                               UID = "",
                               ChargerType = c.ConnectorType,
                               FaultSince = "",
                               FaultDescription = "",
                               TimeReported = s.StartTime,
                               LocationId = c.LocationId,
                               LocationName = c.LocationName,
                               ChargingStatus = s.ChargingStatus,
                               ChargeBoxId = c.ChargeBoxId,
                               StartTime = s.StartTime,
                               EndTime = s.EndTime,
                                Startmetervalue = s.StartMeterValue,
                               Endmetervalue = s.EndMeterValue,
                               Startsoc = s.StartSoc,
                               EndSoc = s.EndSoc,
                               ReasoneForStop = s.ReasonForStop
                           }).OrderByDescending(a => a.TimeReported).ToList<ChartDetailsList>();
                }
            }
            else if (request.Flag.ToLower() == "locationstatus".ToLower())
            {
                    res = dispenserByLocationIdResponse.data.Select(c => new ChartDetailsList()
                    {
                        Id = -1,
                        ChargerName = c.ChargeBoxId,
                        UID = "",
                        ChargerType = c.ConnectorType,
                        FaultSince = "",
                        FaultDescription = "",
                        TimeReported = DateTime.MinValue,
                        LocationId = c.LocationId,
                        LocationName = c.LocationName,
                        ChargeBoxId = c.ChargeBoxId,
                      
                    }
                 ).OrderByDescending(a => a.LocationName).ToList<ChartDetailsList>();

            }
           // var dataResult =new List<ChartDetailsList>();
            //if (!string.IsNullOrEmpty(request.SearchParam))
            //    res = res.Where(d => d.LocationName.ToLower() == request.SearchParam.ToLower() || d.ChargeBoxId.ToLower() == request.SearchParam.ToLower()).ToList();

            if (res == null)
            {
                res=new List<ChartDetailsList>();
            }
            //if (request.IsExport == true)
            //{

            //     dataResult = res;
            //        return await Task.FromResult(res);
            //}
            //else
            //{
            //     dataResult = PagedList<ChartDetailsList>.ToPagedList(res, request.PageNumber, request.PageSize);
            //       // return await Task.FromResult(dataResult);
            //}

            return res;


        }

        
    }
}
