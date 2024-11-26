using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetAllChargeBoxIDRepository : OcppRepository<ChargeBoxIDListResponse>, IGetAllChargeBoxIDRepository
    {
        private readonly ILocationRepository _locationRepository;
        public GetAllChargeBoxIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, ILocationRepository locationRepository) : base(dbContext)
        {
            _locationRepository = locationRepository;
        }

        public async Task<ChargeBoxIDListResponse> GetAllChargeBoxID()
        {
            ChargeBoxIDListResponse re = new ChargeBoxIDListResponse();
            try
            {
                List<long> locationIdList = await _locationRepository.GetAllLocationIdByObjectId();
                    re.data = (from  charger in _dbContext.Charger 
                               join location in _dbContext.Locations.Where(x => locationIdList.Contains(x.Id)) on charger.LocationId equals location.Id
                               join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                               join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id
                               //join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                               //on location.Id equals userMap.LocationId
                               select new ChargeBoxIDList
                               {
                                   id = charger.Id,
                                   chargeboxid = charger.ChargeBoxId

                               }).Distinct().OrderByDescending(a => a.chargeboxid).ToList<ChargeBoxIDList>();

                    
                
            if (re.data.Count > 0)
            {
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
