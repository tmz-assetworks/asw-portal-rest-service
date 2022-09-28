using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Text;
using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PortalRestService.Infrastructure.Repositories
{
    public class LocationDispenserRepository : OcppRepository<LocationDispenserForLocationResponse>, ILocationDispenserRepository
    {
        TokenBase _tokenBase;
        public LocationDispenserRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }

        async Task<LocationDispenserForLocationResponse> ILocationDispenserRepository.GetDispenserByLocation(LocationDispensersRequest locationDispensersRequest)
        {
            LocationDispenserForLocationResponse objLocationDispneser = new LocationDispenserForLocationResponse();
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(locationDispensersRequest), Encoding.UTF8, "application/json");

            string callingMethodLocation = APIConstant.GetlocationDispensers;
            HttpResponseMessage responseSession = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethodLocation, httpContent,_tokenBase.acces_token);

            var locationData = await responseSession.Content.ReadAsStringAsync();
            objLocationDispneser = JsonConvert.DeserializeObject<LocationDispenserForLocationResponse>(locationData);
            if (objLocationDispneser.data is not null && objLocationDispneser.data.Count() > 0)
            {
                // updating charger status as per status of charger in  OCPP ChargerStatuses tables
                objLocationDispneser.data.ForEach(l => l.ChargerStatus = _dbContext.ChargerStatuses.Where(c => c.ChargerId == l.DispenserId).OrderByDescending(m => m.ModifiedAt).FirstOrDefault()?.Chargerstatus);
            }
            return objLocationDispneser;
        }
    }
}
