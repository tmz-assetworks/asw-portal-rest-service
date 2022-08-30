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
    public class ChargerSessionDetailsListRepository : OcppRepository<ChargerSessionDetailsListResponse>, IGetChargerSessionDetailsListRepository
    {
        public ChargerSessionDetailsListRepository(Infrastructure.DBContext.ocpp_dbContext dbContext) : base(dbContext)
        {

        }

        async Task<PagedList<ChargerSessionDetailsList>> IGetChargerSessionDetailsListRepository.GetChargerSessionDetailsList(ChargerSessionListRequest request)
        {
            List<ChargerSessionDetailsList> ChargingSessionslist = new List<ChargerSessionDetailsList>();
           List<ChargerSessionDetailsList> res = new List<ChargerSessionDetailsList>();



            string eventlogre = JsonConvert.SerializeObject(new OcppEventLogRequest()
            {

                chargerboxid = request.chargerboxid
            });

            res = (request.chargerboxid.Count > 0 ? _dbContext.ChargingSessions.Where(x => request.chargerboxid.Contains(x.DeviceId)) : _dbContext.ChargingSessions).Select(c => new ChargerSessionDetailsList()
            {

                Id = c.Id,
                Sessionid = c.Id.ToString().Length == 1 ? "0000000" + c.Id.ToString() : c.Id.ToString().Length == 2 ? "000000" + c.Id.ToString() :
                c.Id.ToString().Length == 3 ? "00000" + c.Id.ToString() : c.Id.ToString().Length == 4 ? "00000" + c.Id.ToString() : "",
                Duration = "" ,
                Usage = (Convert.ToDouble(c.EndMeterValue) - Convert.ToDouble(c.StartMeterValue <= 0 ? 0 : c.StartMeterValue)),
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                ChargeBoxId = c.DeviceId,
                ModifiedAt = c.ModifiedAt,
                CreatedAt = c.CreatedAt
            }
              ).OrderByDescending(a => a.ModifiedAt).Where(s => s.EndTime > s.StartTime).ToList<ChargerSessionDetailsList>();

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

            var dataResult = PagedList<ChargerSessionDetailsList>.ToPagedList(res,
              request.PageNumber,
              request.PageSize);
            return await Task.FromResult(dataResult);




        }

    }
}
    
