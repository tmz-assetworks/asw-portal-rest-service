using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net.Http.Headers;
using System.Text;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
    public class VehicleRepository : OcppRepository<GetAllVehicleResponse>, IVehicleRepository
    {
        TokenBase _tokenBase;
        
        public VehicleRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase = token;
        }
        async Task<vehicleWithPagination> IVehicleRepository.GetAllVehicle(GetAllVehicleRequest getAllVehicleRequest)
        {

            if (getAllVehicleRequest.PageSize == 0) getAllVehicleRequest.PageSize = 10;
            if (getAllVehicleRequest.PageNumber == 0) getAllVehicleRequest.PageNumber = 1;
            vehicleWithPagination vehicleWithPagination = new vehicleWithPagination();
            List<Vehicle> vehicles = new List<Vehicle>();
            GetAllVehicleResponse getAllVehicleResponse = new GetAllVehicleResponse();
            try
            {
             var data = (from v in getAllVehicleRequest.SearchParam!="" && getAllVehicleRequest!=null ? _dbContext.Vehicle.Where(d => d.Department.ToLower().Contains(getAllVehicleRequest.SearchParam.ToLower()) || d.VehicleMacAddress.ToLower().Contains(getAllVehicleRequest.SearchParam.ToLower())) :  _dbContext.Vehicle
                                                  select new Vehicle
                                                  {
                                                      id = v.Id,
                                                      VIN = v.VIN,
                                                      ModelYear = v.ModelYear,
                                                      MakeName = v.MakeName,
                                                      ModelName = v.ModelName,
                                                      LicencePlate = v.LicencePlate,
                                                      Department = v.Department,
                                                      DomicileLocation = v.DomicileLocation,
                                                      VehicleMacAddress = v.VehicleMacAddress,
                                                      UnitNumber = v.UnitNumber,
                                                      RFIDCardAssigned = String.Join(",", v.vehicleRFID.Where(rfid => rfid.VehicleId == v.Id).Select(s => s.Name)),
                                                      Status = v.IsActive,
                                                  }).OrderByDescending(a => a.id).ToList<Vehicle>();
                vehicleWithPagination.Active = data.Where(m => m.Status == true).Count().ToString();
                vehicleWithPagination.Inactive = data.Where(m => m.Status == false).Count().ToString();
                vehicleWithPagination.data = PagedList<Vehicle>.ToPagedList(data,
                getAllVehicleRequest.PageNumber,
               getAllVehicleRequest.PageSize);
                vehicleWithPagination.paginationResponse = new Core.PagingHelper.PaginationResponse
                {
                    TotalCount = vehicleWithPagination.data.TotalCount,
                    PageSize = vehicleWithPagination.data.PageSize,
                    CurrentPage = vehicleWithPagination.data.CurrentPage,
                    TotalPages = vehicleWithPagination.data.TotalPages,
                    HasNext = vehicleWithPagination.data.HasNext,
                    HasPrevious = vehicleWithPagination.data.HasPrevious
                };
               
                

                if(vehicleWithPagination.data.Count() >0)
                {
                    getAllVehicleResponse.StatusMessage = RespnoseMessage.Record_found;
                    getAllVehicleResponse.StatusCode = 200;
                    getAllVehicleResponse.paginationResponse = vehicleWithPagination.paginationResponse;
                    getAllVehicleResponse.data = new List<Vehicle>();
                }
                else
                {
                    getAllVehicleResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    getAllVehicleResponse.StatusCode = 200;
                    getAllVehicleResponse.paginationResponse = null;
                    getAllVehicleResponse.data = new List<Vehicle>();
                }


            }

            catch (Exception ex)
            {
                getAllVehicleResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                getAllVehicleResponse.StatusCode = RespnoseCode.Bad_Request;
                getAllVehicleResponse.paginationResponse = null;
                getAllVehicleResponse.data =  new List<Vehicle>();
            }
            return vehicleWithPagination;

        }
    }
}
