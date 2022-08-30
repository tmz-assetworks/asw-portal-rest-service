using Newtonsoft.Json;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
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
        public GetChartDetailsListRepository(Infrastructure.DBContext.ocpp_dbContext dbContext) : base(dbContext)
        {

        }

        public async Task<PagedList<ChartDetailsList>> GetChartDetailsList(ChartDetailsListRequest request)
        {
            List<ChartDetailsList> res = new List<ChartDetailsList>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            
                if (string.IsNullOrEmpty(request.Duration) || request.Duration.ToLower() == "string")
                    request.Duration = "1";

            string callingMethoddispenser = APIConstant.GetDispenserByLocations;
            string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
            {
                opratorid = "",
                LocationIds = request.LocationIds
            });

            StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
            HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethoddispenser, httpContent);
            var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();
            dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);


            if (request.Flag.ToLower() == "chargerSession".ToLower())
            {
               

                res = (from s in _dbContext.ChargingSessions.ToList()
                       where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(request.Duration)) && s.StartTime <= DateTime.Now
                       join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                       on s.ChargerId equals c.ChargerId
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

                           ChargeBoxId = c.ChargeBoxId,


                       }).OrderByDescending(a => a.TimeReported).ToList<ChartDetailsList>();
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

            if (!string.IsNullOrEmpty(request.SearchParam))
                res = res.Where(d => d.LocationName.ToLower() == request.SearchParam.ToLower() || d.ChargeBoxId.ToLower() == request.SearchParam.ToLower()).ToList();

            var dataResult = PagedList<ChartDetailsList>.ToPagedList(res,request.PageNumber,request.PageSize);
            return await Task.FromResult(dataResult);
        }

    }
}
