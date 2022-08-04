using Newtonsoft.Json;
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
    public class LocationDispenserRepository : Repository<LocationDispenserForLocationResponse>, ILocationDispenserRepository
    {
        public LocationDispenserRepository() : base()
        {

        }

        async Task<LocationDispenserForLocationResponse> ILocationDispenserRepository.GetDispenserByLocation(List<long> LocationIds)
        {
            LocationDispenserForLocationResponse obj = new LocationDispenserForLocationResponse();
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(LocationIds), Encoding.UTF8, "application/json");

            string callingMethodLocation = APIConstant.Getdispenserbylocation;
            HttpResponseMessage responseSession = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethodLocation, httpContent);

            var locationData = await responseSession.Content.ReadAsStringAsync();
            obj = JsonConvert.DeserializeObject<LocationDispenserForLocationResponse>(locationData);          

            return obj;
        }

      
    }
}
