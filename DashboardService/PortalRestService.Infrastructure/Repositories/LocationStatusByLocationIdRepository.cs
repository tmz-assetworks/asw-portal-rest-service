using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net.Http.Headers;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
#pragma warning disable
    public class LocationStatusByLocationIdRepository : Repository<AllLocationStatusChartBO>, ILocationStatusByLocationIdRepository
    {
        public LocationStatusByLocationIdRepository() : base()
        {

        }
        public async Task<List<AllLocationStatusChartBO>> GetLocationStatusByLocatonId(List<int> location, string duration)
        {

            AllLocationStatusQueryResponse obj = new AllLocationStatusQueryResponse();
            List<LocationStatusData> LocationStatus = new List<LocationStatusData>();



            string callingMethodLocation = APIConstant.GetAllLocation;
            HttpResponseMessage responseSession = await Helpers.Helper.GetCallAssetAPIAsync(callingMethodLocation);

            var locationData = await responseSession.Content.ReadAsStringAsync();
            obj = JsonConvert.DeserializeObject<AllLocationStatusQueryResponse>(locationData);


            List<AllLocationStatusChartBO> finalon = null;
            finalon = obj.data
            .GroupBy(x => new { x.LocationStatus })
            .Select(y => new AllLocationStatusChartBO()
            {
                LocationStatus = y.Key.LocationStatus,
                Counts = y.ToList().Count,
                Color = Extensions.GetColorCodesByStatus(y.Key.LocationStatus)
            }
            ).ToList<AllLocationStatusChartBO>();

            return finalon;


        }

    }
}
