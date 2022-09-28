using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortalRestService.Core.Entities.Charger;
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
    public class VehicleRepository : Repository<GetAllVehicleResponse>, IVehicleRepository
    {
        TokenBase _tokenBase;
        public VehicleRepository(TokenBase tokenBase) : base()
        {
            _tokenBase = tokenBase;
        }

        async Task<vehicleWithPagination> IVehicleRepository.GetAllVehicle(GetAllVehicleRequest getAllVehicleRequest)
        {
            vehicleWithPagination vehicleWithPagination = new vehicleWithPagination();
            List<Vehicle> vehicles = new List<Vehicle>();
            GetAllVehicleResponse getAllVehicleResponse = new GetAllVehicleResponse();
            try
            {
                string callingMethod = APIConstant.GetlAllVehicle;
                AllVehicle AllVehicle = new AllVehicle();
                string dd = JsonConvert.SerializeObject(getAllVehicleRequest);
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
                HttpResponseMessage responseVehicle = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethod, httpContent,_tokenBase.acces_token);

                var allVehicle = await responseVehicle.Content.ReadAsStringAsync();
                AllVehicle = JsonConvert.DeserializeObject<AllVehicle>(allVehicle);
                if (AllVehicle.data !=null && AllVehicle.data.Count > 0)
                {
                    vehicleWithPagination.data = (from v in AllVehicle.data
                                                  select new Vehicle
                                                  {
                                                      id = v.Id,
                                                      VIN = v.VIN,
                                                      ModelYear = v.VehicleModelYear.Name,
                                                      Make = v.VehicleMake.Name,
                                                      Model = v.VehicleModel.Name,
                                                      LicencePlate = v.LicencePlate,
                                                      Department = v.Department,
                                                      DomicileLocation = v.DomicileLocation,
                                                      VehicleMacAddress = v.VehicleMacAddress,
                                                      RFIDCardAssigned = String.Join(",", v.VehicleRFID.Where(rfid => rfid.VehicleId == v.Id).Select(s => s.Name)),
                                                      Status = v.IsActive,

                                                  }).OrderByDescending(a => a.id).ToList<Vehicle>();
                                                  vehicleWithPagination.Active = AllVehicle.Active;
                                                  vehicleWithPagination.Inactive = AllVehicle.Inactive;
                    vehicleWithPagination.paginationResponse = AllVehicle.paginationResponse;
                }
                else
                {
                    getAllVehicleResponse.StatusMessage = "Record not Found";
                    getAllVehicleResponse.StatusCode = 200;
                    getAllVehicleResponse.paginationResponse = null;
                    getAllVehicleResponse.data = null;
                }

            }

            catch (Exception ex)
            {
                getAllVehicleResponse.StatusMessage = "Record not Found"+ ex.Message.ToString();
                getAllVehicleResponse.StatusCode = 404;
                getAllVehicleResponse.paginationResponse = null;
                getAllVehicleResponse.data = null;
            }
            return vehicleWithPagination;

        }
    }
}
