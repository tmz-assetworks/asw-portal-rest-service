using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
    public class DispensersDetailRepository : OcppRepository<DispensersDetail>, IDispenserDetailRepository
    {
        TokenBase _tokenBase;
        public DispensersDetailRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            this._tokenBase = tokenBase;
        }

        public Task<PagedList<DispensersDetail>> GetDispensersDetail(DispensersDetailRequest dispensersDetailRequest)
        {
            List<DispensersDetail> result = new List<DispensersDetail>();
            result = (from disp in (dispensersDetailRequest.SearchParam == null && dispensersDetailRequest.SearchParam == "") ? _dbContext.Charger :
                      _dbContext.Charger.Where(d => d.ChargeBoxId.ToLower().Contains(dispensersDetailRequest.SearchParam.ToLower()))
                      join location in _dbContext.Locations on disp.LocationId equals location.Id
                      join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                       on location.Id equals userMap.LocationId
                      select new DispensersDetail
                      {
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

            return Task.FromResult(dataResult);
        }

    }

}
