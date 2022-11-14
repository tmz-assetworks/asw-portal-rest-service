using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
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
    public class GetAllChargeBoxIDRepository : OcppRepository<ChargeBoxIDListResponse>, IGetAllChargeBoxIDRepository
    {
        TokenBase _tokenBase;
        public GetAllChargeBoxIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token) : base(dbContext)
        {
            _tokenBase=token;
        }

        public async Task<ChargeBoxIDListResponse> GetAllChargeBoxID()
        {
            ChargeBoxIDListResponse re = new ChargeBoxIDListResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            string callingMethoddispenser = APIConstant.GetDispenserByLocations;
            List<int> myList = new List<int>();
            string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
            {
                operatorid = "",
                LocationIds = myList
            });
            StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
            HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent,_tokenBase.acces_token);

            var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();
            ChargeBoxIDList chargeBoxIDList = new ChargeBoxIDList();
            try
            {
                dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);
                if (dispenserByLocationIdResponse.data.Count > 0)
                {
                    re.data = (from v in dispenserByLocationIdResponse.data
                               select new ChargeBoxIDList
                               {
                                   id = v.LocationId,
                                   chargeboxid = v.ChargeBoxId

                               }).Distinct().OrderByDescending(a => a.chargeboxid).ToList<ChargeBoxIDList>();
                    re.StatusCode = 200;
                    re.StatusMessage = RespnoseMessage.Record_found;
                }
                else
                {
                    re.StatusCode = 200;
                    re.StatusMessage = RespnoseMessage.Record_not_found;
                }
            }
            catch (Exception ex)
            {
                re.StatusMessage = RespnoseMessage.Opeartion_Failed;
                re.StatusCode = RespnoseCode.Bad_Request;
 
            }
            return re;
        }
        
    }
}
