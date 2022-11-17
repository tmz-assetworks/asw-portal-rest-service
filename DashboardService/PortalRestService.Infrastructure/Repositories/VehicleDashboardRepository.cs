using Microsoft.Extensions.Configuration;
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
    public class VehicleDashboardRepository : Repository<VehicleByIdData>, IVehicleDashboardRepository
    {
        TokenBase _tokenBase;
        public VehicleDashboardRepository(TokenBase token) : base()
        {
            _tokenBase = token;
        }

        // Get Vehicle detail by vehicleId
        // Auther:ATUL, Date : 
        public async Task<VehicleByIdData> VehicleDetailsById(long id)

        {
            VehicleByIdData vehicleByIdData = new VehicleByIdData();
            VehiclesResponse vehiclesResponse = new VehiclesResponse();
            try
            {
                string str = APIConstant.GetVehicleByID + id;
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetAuthAPIAsync(str, _tokenBase.acces_token);
                VehicleResponse? getVehicleByIdResponse = new VehicleResponse();

                if (response.IsSuccessStatusCode)
                {
                    var vehicleinfo = await response.Content.ReadAsStringAsync();
                    getVehicleByIdResponse = JsonConvert.DeserializeObject<VehicleResponse>(vehicleinfo);
                    if (getVehicleByIdResponse.data != null)
                    {

                        vehicleByIdData = new VehicleByIdData()
                        {
                            VIN = getVehicleByIdResponse.data.VIN,
                            ModelYear = getVehicleByIdResponse.data.ModelYear,
                            MakeName =getVehicleByIdResponse.data.MakeName,
                            ModelName =getVehicleByIdResponse.data.ModelName,
                            licencePlate = getVehicleByIdResponse.data.licencePlate,
                            department = getVehicleByIdResponse.data.department,
                            domicileLocation = getVehicleByIdResponse.data.domicileLocation,
                            vehicleMacAddress = getVehicleByIdResponse.data.vehicleMacAddress,
                            Status = getVehicleByIdResponse.data.isActive,
                            rfId = getVehicleByIdResponse.data.vehicleRFID != null ? String.Join(",", (getVehicleByIdResponse.data.vehicleRFID).Select(x => x.name)) : "",
                            applicableSubscriptionPlans = getVehicleByIdResponse.data.applicableSubscriptionPlans,
                        };
                    }
                }

                return vehicleByIdData;

            }
            catch (Exception ex)
            {
                vehiclesResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                vehiclesResponse.StatusCode = RespnoseCode.Bad_Request;
            }
            return vehicleByIdData;

        }

    }
}