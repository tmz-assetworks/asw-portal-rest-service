using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Models;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.DBContext;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
               
                var dispenser = _dbContext.Charger
                 .Select(m => new Dispenser
                 {
                     locationId = (int)m.LocationId,
                     id = m.Id,
                     assetId = m.AssetId,
                     endPointUrl = m.EndPointUrl,
                     firmwareVersion = m.FirmwareVersion,
                     hardwareSerialNumber = m.HardwareSerialNumber,
                     isActive = m.IsActive,
                     isAutomatic = m.IsAutomatic,
                     meterType = m.MeterType,
                     multiplePorts = m.MultiplePorts,
                     pingSchedule = m.PingSchedule,
                     readingSchedule = m.ReadingSchedule,
                     chargeBoxId = m.ChargeBoxId,
                     modelName = m.ModelName,
                     makeName = m.MakeName,
                     InstallationDate=m.InstallationDate,
                     SimCardMSIDN = m.SimCardMSIDN != null ? m.SimCardMSIDN : "",
                     OEMOrderNumber = m.OEMOrderNumber ?? "",

                     ChargerStatus = ((from ob in _dbContext.ChargerStatuses.Where(x => x.ChargerId == m.Id)
                               select new ChargerStatusDTO
                               {
                                   Id = ob.Id,
                                  ChargerId = ob.ChargerId,
                                  ChargerStatus1=ob.Chargerstatus,
                                  ConnectorId=ob.ConnectorId,
                                  ConnectorStatus=ob.ConnectorStatus,
                                  ReservationExpiryDate=ob.ReservationExpiryDate,
                                  IdTag=ob.IdTag!=null?"": ob.IdTag,
                                  ReservationId=ob.ReservationId,
                                  ModifiedoN=ob.ModifiedAt
                                   
                               }).ToList()),
                     LocationDTO = (from obls in _dbContext.Locations.Where(x => x.Id == m.LocationId)
                                    select new LocationDTO
                                    {
                                        Id = (int)obls.Id,
                                        LocationName = obls.LocationName,
                                        LocationId=obls.LocationId,
                                        LocationAddress = new LocationAddressDTO()
                                        {
                                            Id = obls.LocationAddress.Id,
                                            AddressLine1 = obls.LocationAddress.AddressLine1,
                                            AddressLine2 = obls.LocationAddress.AddressLine2,
                                            CityName = obls.LocationAddress.CityName,
                                            CountryId = obls.LocationAddress.CountryId,
                                            CountryName = obls.LocationAddress.CountryName,
                                            IsActive = obls.LocationAddress.IsActive,
                                            Latitude = obls.LocationAddress.Latitude,
                                            Longitude = obls.LocationAddress.Longitude,
                                            LandlineNumber = obls.LocationAddress.LandlineNumber,
                                            StateId = obls.LocationAddress.StateId,
                                            StateName = obls.LocationAddress.StateName,
                                            PinCode = obls.LocationAddress.PinCode
                                        }
                                    }).FirstOrDefault(),
                     Ports = ((from obpo in _dbContext.Port.Where(x => x.ChargerId == m.Id)
                               select new PortDTO
                               {
                                   Id = obpo.Id,
                                   ConnectorId = obpo.Connectorid,
                                   ChargerTypeId = obpo.ChargerTypeId,
                                   ConnectorType = obpo.ConnectorType,
                                   PortName = obpo.PortName,
                                   ConnectorDTO = new ConnectorDTO()
                                   {
                                       ConnectorType = obpo.Connector.ConnectorType,
                                       Id = obpo.Id,
                                   },
                                   ChargerTypeDTO = (from ob in _dbContext.ChargerType.Where(x => x.Id == obpo.ChargerTypeId)
                                                     select new ChargerTypeDTO
                                                     {
                                                         ChargerTypeName = ob.ChargerTypeName
                                                     }).FirstOrDefault(),
                               }).ToList()),
                 }).Where(d => d.chargeBoxId.ToLower() == chargeBoxId.ToLower()).FirstOrDefault();
                if (dispenser != null)
                {
                    chargerInformationResponse.StatusMessage = RespnoseMessage.Record_found;
                    chargerInformationResponse.StatusCode = (int)HttpStatusCode.OK;
                    chargerInformationResponse.data.AssetId = dispenser.assetId;
                    chargerInformationResponse.data.HardwareSerialNumber = dispenser.hardwareSerialNumber;
                    chargerInformationResponse.data.ZipCode = dispenser.LocationDTO.LocationAddress.PinCode;
                    chargerInformationResponse.data.Address = dispenser.LocationDTO.LocationAddress.AddressLine1 + " " + dispenser.LocationDTO.LocationAddress.AddressLine2;
                    chargerInformationResponse.data.Charger = dispenser.Ports.Count() > 0 ? String.Join(", ", (dispenser.Ports).Select(s => s.ConnectorDTO.ConnectorType)) : "";

                    chargerInformationResponse.data.ConnectorIds = dispenser.Ports.Count() > 0 ? String.Join(", ", (dispenser.Ports).Select(s => s.ConnectorId)) : "";
                    chargerInformationResponse.data.ChargerStatus = dispenser.ChargerStatus == null || dispenser.ChargerStatus.Count == 0 ? "Offline" :
                                    dispenser.ChargerStatus.ToList()[0].ChargerStatus1.Replace("charging", "Busy").Replace("suspendedev", "Busy").Replace("uspendedevse", "Busy")
                                  .Replace("finishing", "Busy").Replace("preparing", "Busy");
                    chargerInformationResponse.data.ChargerType = dispenser.Ports.Count() > 0 ? String.Join(",", (dispenser.Ports).Select(s => s.ChargerTypeDTO.ChargerTypeName)) : "";
                    chargerInformationResponse.data.City = dispenser.LocationDTO.LocationAddress.CityName;
                    chargerInformationResponse.data.Country = dispenser.LocationDTO.LocationAddress.CountryName;
                    chargerInformationResponse.data.State = dispenser.LocationDTO.LocationAddress.StateName;
                    chargerInformationResponse.data.InstalledDate = dispenser.InstallationDate;
                    chargerInformationResponse.data.ChargeBoxId = dispenser.chargeBoxId;
                    chargerInformationResponse.data.ConnectorType = dispenser.Ports.Count() > 0 ? dispenser.Ports[0].ConnectorType : 0;
                    chargerInformationResponse.data.SimCardMSIDN = dispenser.SimCardMSIDN;
                    chargerInformationResponse.data.LocationId=dispenser.LocationDTO.LocationId;
                    chargerInformationResponse.data.LocationName = dispenser.LocationDTO.LocationName;
                    chargerInformationResponse.data.ChargerMake=dispenser.makeName;
                    chargerInformationResponse.data.ChargerModel = dispenser.modelName;
                    chargerInformationResponse.data.OEMOrderNumber = dispenser.OEMOrderNumber;

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
