using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Models;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.DBContext;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
   

    public class GetChargerInfoRepository : OcppRepository<ChargerInformationResponse>, IGetChargerInformationRepository
    {
        TokenBase _tokenBase;
        public GetChargerInfoRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }
        public async Task<ChargerInformationResponse> GetChargerInformation(string chargeBoxId, string OperatorId)
        {
            ChargerInformationResponse chargerInformationResponse = new ChargerInformationResponse();
            if(chargeBoxId != null && chargeBoxId.Length>0)
            {
                string callingMethoddispenser = APIConstant.GetDispenserByChargeboxId + chargeBoxId;

                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetAuthAPIAsync(callingMethoddispenser, _tokenBase.acces_token);

                var chrgerInformation = await responsedispenser.Content.ReadAsStringAsync();
                var dispenser = JsonConvert.DeserializeObject<DispenserResponse>(chrgerInformation);
                if(dispenser.data != null)
                {
                    chargerInformationResponse.StatusMessage = RespnoseMessage.Record_found;
                    chargerInformationResponse.StatusCode = (int)HttpStatusCode.OK;
                    chargerInformationResponse.data.HardwareSerialNumber = dispenser.data[0].hardwareSerialNumber;
                    chargerInformationResponse.data.ZipCode = dispenser.data[0].location.LocationAddress.PinCode;
                    chargerInformationResponse.data.Address = dispenser.data[0].location.LocationAddress.AddressLine1 + " "+dispenser.data[0].location.LocationAddress.AddressLine2;
                    chargerInformationResponse.data.Charger = dispenser.data[0].Ports.Count() > 0 ? String.Join(", ", (dispenser.data[0].Ports).Select(s => s.Connector.ConnectorType)) : "";

                    chargerInformationResponse.data.ConnectorIds = dispenser.data[0].Ports.Count() > 0 ? String.Join(", ", (dispenser.data[0].Ports).Select(s => s.ConnectorId)) : "";
                    // getting the ChargerStatus from OCPP service
                    chargerInformationResponse.data.ChargerStatus = dispenser.data[0].ChargerStatuses == null || dispenser.data[0].ChargerStatuses.Count == 0 ? "Offline" :
                     dispenser.data[0].ChargerStatuses.ToList()[0].ChargerStatus1.ToLower() == "charging" ? "Busy" :
                              dispenser.data[0].ChargerStatuses.ToList()[0].ChargerStatus1.ToLower() == "suspendedev" ||
                              dispenser.data[0].ChargerStatuses.ToList()[0].ChargerStatus1.ToLower() == "uspendedevse" ||
                               dispenser.data[0].ChargerStatuses.ToList()[0].ChargerStatus1.ToLower() == "finishing" ||
                              dispenser.data[0].ChargerStatuses.ToList()[0].ChargerStatus1.ToLower() == "preparing" ? "Occupied" :
                             dispenser.data[0].ChargerStatuses.ToList()[0].ChargerStatus1;
                    chargerInformationResponse.data.ChargerType = dispenser.data[0].Ports.Count() > 0 ? String.Join(",",(dispenser.data[0].Ports).Select(s => s.ChargerType.ChargerTypeName)) : "";
                    chargerInformationResponse.data.City = dispenser.data[0].location.LocationAddress.CityName;
                    chargerInformationResponse.data.Country = dispenser.data[0].location.LocationAddress.CountryName;
                    chargerInformationResponse.data.State = dispenser.data[0].location.LocationAddress.StateName;
                    chargerInformationResponse.data.InstalledDate = dispenser.data[0].InstallationDate;
                    chargerInformationResponse.data.ChargeBoxId = dispenser.data[0].chargeBoxId;
                    chargerInformationResponse.data.ConnectorType = dispenser.data[0].Ports.Count() > 0 ? dispenser.data[0].Ports[0].ConnectorType : 0;
                }
                else
                {
                    chargerInformationResponse.StatusCode = (int)HttpStatusCode.OK;
                    chargerInformationResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    chargerInformationResponse.data = new ChargerInfo(); 
                }
            }
            else
            {
                chargerInformationResponse.data =new  ChargerInfo();
                chargerInformationResponse.StatusCode = (int)HttpStatusCode.OK;
                chargerInformationResponse.StatusMessage = RespnoseMessage.Please_provide_ChargeBox_Id;
            }
            return chargerInformationResponse;
        }
    }
}
