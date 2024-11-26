using Azure;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Linq;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
    public class DispensersDetailRepository : OcppRepository<DispensersDetail>, IDispenserDetailRepository
    {
        TokenBase _tokenBase;
        private readonly ILocationRepository _locationRepository;
        public DispensersDetailRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase, ILocationRepository locationRepository) : base(dbContext)
        {
            this._tokenBase = tokenBase;
            _locationRepository = locationRepository;
        }


        public async Task<PagedList<DispensersDetail>> GetDispensersDetail(DispensersDetailRequest dispensersDetailRequest)
        {
            List<long> locationIdList = await _locationRepository.GetAllLocationIdByObjectId();
            List<DispensersDetail> result = (from disp in _dbContext.Charger                       
                      join location in _dbContext.Locations.Where(x => locationIdList.Contains(x.Id)) on disp.LocationId equals location.Id
                      //join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                      //on location.Id equals userMap.LocationId 
                                             where (!string.IsNullOrEmpty(dispensersDetailRequest.SearchParam)) ? (disp.ChargeBoxId.ToLower().Contains(dispensersDetailRequest.SearchParam.ToLower()) || disp.AssetId.ToLower().Contains(dispensersDetailRequest.SearchParam.ToLower()) || disp.MakeName.ToLower().Contains(dispensersDetailRequest.SearchParam.ToLower()) || disp.ModelName.ToLower().Contains(dispensersDetailRequest.SearchParam.ToLower()) || location.LocationName.ToLower().Contains(dispensersDetailRequest.SearchParam.ToLower())): disp.ChargeBoxId != null
                      select new DispensersDetail
                      {
                          AssetId = disp.AssetId,
                          ChargerBoxId = disp.ChargeBoxId,
                          TimeReported = disp.ChargerStatuses == null ? "" :
                          disp.ChargerStatuses.ToList().Where(x => x.ConnectorStatus.ToLower() == "faulted").ToList().Count == 0 ? "" :
                          disp.ChargerStatusHistories.Where(x => x.ConnectorStatus.ToLower() == "faulted").OrderByDescending(m => m.Id).FirstOrDefault().CreatedOn.Value.ToString("d-MM-yyyy h:mm"),
                          FaultSince = disp.ChargerStatuses.ToList().Where(x => x.ConnectorStatus.ToLower() == "faulted").ToList().Count == 0 ? "" :
                          (DateTime.Now - disp.ChargerStatusHistories.Where(x => x.ConnectorStatus.ToLower() == "faulted").OrderByDescending(m => m.Id).FirstOrDefault().CreatedOn).Value.Hours.ToString() + " hours",
                          LocationId = disp.LocationId == null ? 0 : (long)disp.LocationId,
                          State = location.LocationAddress != null ? location.LocationAddress.StateName : "",
                          ChargerType = "OCPP",
                          LocationContactName = location.LocationName,
                          LocationContactNumber = location.ContactPersonNumber,
                      }).ToList<DispensersDetail>();

            result = result != null ? result.OrderByDescending(a => a.ChargerName).ToList<DispensersDetail>() : result;

            //  Paging on Records

            var dataResult = PagedList<DispensersDetail>.ToPagedList(result,
              dispensersDetailRequest.PageNumber,
              dispensersDetailRequest.PageSize);

            return dataResult;
        }

    }

}
