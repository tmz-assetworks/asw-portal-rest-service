using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortalRestService.Application;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Models;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PortalRestService.Application.Status_Indication;
using ChargerStatus = PortalRestService.Core.Models.ChargerStatus;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetSummaryStatusRepository : OcppRepository<CardDataResponse>, IGetSummaryStatusRepository
    {
        private readonly double perkwtRate = 0;
        private readonly double gasolineInKiloWatt = 0;
        private readonly double lbsofCO2emitted = 0;
        private readonly IConfiguration _configuration;
        TokenBase _tokenBase;
        public GetSummaryStatusRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, IConfiguration configuration, TokenBase tokenBase) : base(dbContext)
        {
            this._configuration = configuration;
            //_httpHelper = httpHelper;

            gasolineInKiloWatt = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("GallongasolineKiloWatt").Value);
            lbsofCO2emitted = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("lbsofCO2emitted").Value);
            perkwtRate = (double)Convert.ToDouble(this._configuration.GetSection("EneryRatePerKg").GetSection("perkwtRate").Value);
            _tokenBase = tokenBase;
        }

         public async Task<CardDataResponse> GetSummaryStatus(int locationId, bool isChargersReq)
        {
            CardDataResponse dataResponse = new CardDataResponse();

            if (locationId > 0 && isChargersReq)  // when data is requesting for location based IsChargersReq will not true.
            {
                dataResponse.data = null;
                dataResponse.StatusMessage = "Request is not valid.";
                dataResponse.StatusCode = (int)HttpStatusCode.OK;
                return dataResponse;
            }

            List<CardData> data = new List<CardData>();
            CardData cardData = null;
            try
            {
                HttpResponseMessage locatoinResponse = null;
                if (locationId == 0 && isChargersReq == false)  // not for Chargers
                {
                    
                    AllLocationStatusQueryResponse Location = new AllLocationStatusQueryResponse();
                    Location.data = ((from ob in _dbContext.Locations
                                      select new LocationStatusData
                                      {
                                          Id = ob.Id,
                                          LocationName = ob.LocationName,
                                          LocationStatus = ob.LocationStatus.LocationStatusName,
                                      }).ToList());
                    if (Location != null)
                    {
                        cardData = new CardData();
                        cardData.Type = "Locations";
                        cardData.Count = Location.data != null ? Location.data.Count : 0;

                        if (Location.data != null)
                        {
                            List<StatusData> StatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName(), Value = Location.data!=null? CommonHelpers.GetHoursTwoDigitFormat(Location.data.Where(d => d.LocationStatus.ToLower().Equals(Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName().ToLower())).ToList().Count).ToString():"", Color = ColorsEnum.LocationsColor.Commissioned.GetEnumDisplayName() },
                        new StatusData { Key = Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName(), Value = Location.data!=null? CommonHelpers.GetHoursTwoDigitFormat(Location.data.Where(d => d.LocationStatus.ToString().ToLower().Trim().Equals(Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName().ToLower().Trim())).ToList().Count).ToString():"" , Color = ColorsEnum.LocationsColor.UnderMaintenance.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName(), Value =Location.data!=null? CommonHelpers.GetHoursTwoDigitFormat(Location.data.Where(d => d.LocationStatus.ToLower().Equals(Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName().ToLower())).ToList().Count).ToString() :"" , Color = ColorsEnum.LocationsColor.Upcoming.GetEnumDisplayName()  },
                      };
                            cardData.StatusData = StatusData;
                            data.Add(cardData);
                        }
                    }
                }
                DispenserResponse objDispenser = new DispenserResponse();
                // Getting Charger/Dispenser data
                HttpResponseMessage dispenserResponse = null;
                if (locationId == 0)
                {
                    //dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetAuthAPIAsync(APIConstant.GetAllDispenser, _tokenBase.acces_token);
                    objDispenser.data = _dbContext.Charger.Join(_dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id)), m => m.LocationId, n => n.LocationId,
                (m, n) => new Dispenser
                {
                    id = m.Id,
                    assetId = m.AssetId,
                    ChargerStatus = ((from ob in _dbContext.ChargerStatuses.Where(x => x.ChargerId == m.Id)
                                         select new ChargerStatusDTO
                                         {
                                             Id = ob.Id,
                                             ChargerId = ob.ChargerId,
                                             ChargerStatus1 = ob.Chargerstatus,
                                             ConnectorId = ob.ConnectorId,
                                             ConnectorStatus = ob.ConnectorStatus,
                                             ReservationExpiryDate = ob.ReservationExpiryDate,
                                             IdTag = ob.IdTag != null ? "" : ob.IdTag,
                                             ReservationId = ob.ReservationId,
                                             ModifiedoN = ob.ModifiedAt

                                         }).ToList()),


                }).ToList();
                }
                else
                {
                    //dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetAuthAPIAsync(APIConstant.GetDispenserByLocation + "" + locationId, _tokenBase.acces_token);

                    objDispenser.data = (from location in _dbContext.Locations
                                                                 join charger in _dbContext.Charger
                                                                 on location.Id equals charger.LocationId
                                                                 join address in _dbContext.LocationAddress
                                                                 on location.LocationAddressId equals address.Id
                                                                 join Status in _dbContext.LocationStatus
                                                                 on location.LocationStatusId equals Status.Id
                                                                 where location.Id.Equals(locationId)
                                                                 select new Dispenser
                                                                 {
                                                                     id = charger.Id,
                                                                     ChargerStatus = ((from ob in _dbContext.ChargerStatuses.Where(x => x.ChargerId == charger.Id)
                                                                                          select new ChargerStatusDTO
                                                                                          {
                                                                                              Id = ob.Id,
                                                                                              ChargerId = ob.ChargerId,
                                                                                              ChargerStatus1 = ob.Chargerstatus,
                                                                                              ConnectorId = ob.ConnectorId,
                                                                                              ConnectorStatus = ob.ConnectorStatus,
                                                                                              ReservationExpiryDate = ob.ReservationExpiryDate,
                                                                                              IdTag = ob.IdTag != null ? "" : ob.IdTag,
                                                                                              ReservationId = ob.ReservationId,
                                                                                              ModifiedoN = ob.ModifiedAt

                                                                                          }).ToList()),
                                                                 }
                    ).ToList();


                }
                if (objDispenser.data.Count>0)
                {
                    cardData = new CardData();
                   
                    cardData.Type = "Chargers";
                    cardData.Count = objDispenser.data != null ? objDispenser.data.Count : 0;

                    if (objDispenser.data != null)
                    {
                        List<StatusData> StatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.ChargerStatus.Available.GetEnumDisplayName(), Value = CommonHelpers.GetHoursTwoDigitFormat(objDispenser.data.Where(d => d.ChargerStatus!=null && d.ChargerStatus.Count >0 && d.ChargerStatus[0].ChargerStatus1.ToLower().Equals(Status_Indication.ChargerStatus.Available.ToString().ToLower())).ToList().Count).ToString(), Color = ColorsEnum.ChargerStatus.Available.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargerStatus.Connected.GetEnumDisplayName(), Value = CommonHelpers.GetHoursTwoDigitFormat(objDispenser.data.Where(d => d.ChargerStatus !=null && d.ChargerStatus.Count>0  && d.ChargerStatus[0].ChargerStatus1.Replace("Unavailable","Connected").ToLower().Equals(Status_Indication.ChargerStatus.Connected.GetEnumDisplayName().ToLower())).ToList().Count).ToString()  , Color = ColorsEnum.ChargerStatus.Connected.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargerStatus.Offline.GetEnumDisplayName(), Value = CommonHelpers.GetHoursTwoDigitFormat(objDispenser.data.Where(d => d.ChargerStatus==null || d.ChargerStatus.Count==0).ToList().Count).ToString() , Color = ColorsEnum.ChargerStatus.Offline.GetEnumDisplayName() },
                       
                      };
                        cardData.StatusData = StatusData;
                        data.Add(cardData);
                    }
                    else
                    {
                        List<StatusData> StatusData = new List<StatusData>()
                    {
                    new StatusData { Key = Status_Indication.ChargerStatus.Available.GetEnumDisplayName(), Value = "0", Color = ColorsEnum.ChargerStatus.Available.GetEnumDisplayName()  },
                    new StatusData { Key = Status_Indication.ChargerStatus.Connected.GetEnumDisplayName(), Value = "0"  , Color = ColorsEnum.ChargerStatus.Connected.GetEnumDisplayName()  },
                    new StatusData { Key = Status_Indication.ChargerStatus.Offline.GetEnumDisplayName(), Value="0" , Color = ColorsEnum.ChargerStatus.Offline.GetEnumDisplayName() },

                        };
                        cardData.StatusData = StatusData;
                        data.Add(cardData);
                    }
                }
               

                cardData = new CardData();
                cardData.Type = "Charging Sessions";
                List<PortalRestService.Core.Models.ChargingSession> objChargingSession = _dbContext.ChargingSessions.ToList();

                List<int> locationIds = new List<int>()
                   {
                       locationId
                   };
                if(locationId==0)
                    locationIds = new List<int>()
                   {
                       
                   };
                LocationDispenserForLocationResponse locationsResponse = new LocationDispenserForLocationResponse();
              
                    locationsResponse.data = (from location in locationId >0 ?_dbContext.Locations.Where(x => x.Id == locationId): _dbContext.Locations
                                         join charger in _dbContext.Charger
                                         on location.Id equals charger.LocationId
                                         join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                         on location.Id equals userMap.LocationId
                                         select new LocationDispenserForLocation
                                         {
                                             locationId = location.Id,
                                             DispenserId = charger.Id,
                                             ChargeBoxId = charger.ChargeBoxId,
                                             ProtocolName = charger.ProtocolName,
                                             ChargerStatus = charger.ChargerStatuses == null || charger.ChargerStatuses.Count == 0 ? "Offline" :
                                             charger.ChargerStatuses.ToList()[0].Chargerstatus.Replace("charging", "Busy").Replace("suspendedev", "Busy").Replace("uspendedevse", "Busy")
                                            .Replace("finishing", "Busy").Replace("preparing", "Busy"),
                                             NoofPort = charger.Ports.Where(t => t.ChargerId.Equals(charger.Id)).ToList().Count == 0 ? "0" : charger.Ports.Where(t => t.ChargerId.Equals(charger.Id)).ToList().Count.ToString(),
                                             DispenserMake = charger.MakeName,
                                             DispenserModel = charger.ModelName,
                                             ConnectorType = _dbContext.Port.FirstOrDefault(p => p.ChargerId == charger.Id).Connector.ConnectorType,

                                         }).ToList<LocationDispenserForLocation>();
                
                
                if (objChargingSession != null)
                    {
                        List<LocationDispenserForLocation> datalocations = locationsResponse.data.ToList();
                        var chargingSessionsData = (from cs in objChargingSession join l in datalocations on cs.ChargerId equals l.DispenserId where l.ChargeBoxId == cs.DeviceId select cs).ToList();


                        if (objChargingSession != null)
                        {
                            cardData.Count = chargingSessionsData.Count;
                            List<StatusData> StatusData = new List<StatusData>()
                         {
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Cancelled.ToString(), Value = CommonHelpers.GetHoursTwoDigitFormat(chargingSessionsData.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Cancelled.ToString().ToLower())).ToList().Count).ToString() , Color = ColorsEnum.ChargingSessionsColor.Cancelled.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Interrupted.ToString(), Value = CommonHelpers.GetHoursTwoDigitFormat(chargingSessionsData.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Interrupted.ToString().ToLower())).ToList().Count).ToString() , Color = ColorsEnum.ChargingSessionsColor.Interrupted.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Completed.ToString(), Value = CommonHelpers.GetHoursTwoDigitFormat(chargingSessionsData.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Completed.ToString().ToLower())).ToList().Count).ToString() , Color = ColorsEnum.ChargingSessionsColor.Completed.GetEnumDisplayName()  },
                        };
                            cardData.StatusData = StatusData;

                        }
                        data.Add(cardData);
                    }

                
                
                // Charging Session end

                // Getting Error Log
                if (true)
                {
                    cardData = new CardData();
                    if (locationId == 0)
                        cardData.Type = "Active Errors";
                    else cardData.Type = "Alerts";
                    cardData.Count = 10;
                    
                    var chargerid = string.Join(",", _dbContext.ChargerStatuses.Where(r => r.ConnectorStatus.ToLower() == "Faulted").Distinct().Select(p => p.ChargerId.ToString()));
                    var chargeboxid = string.Join(",", _dbContext.Charger.Where(o => chargerid.Contains(o.Id.ToString())).Distinct().Select(p => p.ChargeBoxId.ToString()));
                    var faualtlist = (from cs in _dbContext.ErrorSeverity join l in _dbContext.FaultyErrorCode on cs.Id equals l.ErrorSeverityId where  cs.IsActive==true select l).ToList();
                    

                    List<OcppEventLog> objlogs= (from v in _dbContext.OcppEventLogs.ToList()
                                                 select new OcppEventLog
                                                 {
                                                     Id = v.Id,
                                                    RequestType = v.RequestType==null?"": v.RequestType,
                                                    DeviceId = v.DeviceId==null?"": v.DeviceId,
                                                    ResponsePayload=   v.ResponsePayload,
                                                    RequestPayload= geterror(v.RequestPayload, v.RequestType == null ? "" : v.RequestType)


                                                 }).Distinct().Where(o => chargeboxid.Contains(o.DeviceId == null ? "" : o.DeviceId.ToString())
                                                && o.RequestType.ToLower() == "StatusNotification".ToLower()
                                                ).ToList<OcppEventLog>();

                   
                    //Errors 
                    var mediumlist = faualtlist.Where(o => (Errors) o.ErrorSeverityId == Errors.Medium).Select(p=>p.Names).ToList();
                    var highlist = faualtlist.Where(o => (Errors)o.ErrorSeverityId == Errors.High).Select(p => p.Names).ToList();
                    var criticallist = faualtlist.Where(o => (Errors)o.ErrorSeverityId == Errors.Critical).Select(p => p.Names).ToList();
                    int Mediumcount = objlogs.ToList<OcppEventLog>().Where(r => mediumlist.Contains(r.RequestPayload, StringComparer.InvariantCultureIgnoreCase)).ToList<OcppEventLog>().Count();
                    int Criticalcount = objlogs.ToList<OcppEventLog>().Where(r => criticallist.Contains(r.RequestPayload, StringComparer.InvariantCultureIgnoreCase)).ToList<OcppEventLog>().Count();
                    int Highcount = objlogs.ToList<OcppEventLog>().Where(r => highlist.Contains(r.RequestPayload, StringComparer.InvariantCultureIgnoreCase)).ToList<OcppEventLog>().Count();
                    cardData.Count = Mediumcount + Criticalcount + Highcount;
                    
                    List<StatusData> ErrorStatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.Errors.Critical.ToString(), Value = Criticalcount.ToString() , Color = ColorsEnum.ErrorsColor.Critical.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.Errors.High.ToString(), Value = Highcount.ToString() , Color = ColorsEnum.ErrorsColor.High.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.Errors.Medium.ToString(), Value = Mediumcount.ToString() , Color = ColorsEnum.ErrorsColor.Medium.GetEnumDisplayName()  },
                    };
                    cardData.StatusData = ErrorStatusData;
                    data.Add(cardData);
                }
                dataResponse.data = data;
                dataResponse.StatusMessage = RespnoseMessage.Record_found;
                dataResponse.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
            }
            if (dataResponse.data == null)
                dataResponse.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.FromResult(dataResponse).Result;
        }
        public string geterror(string str, string RequestType)
        {
            string ex1 = "";
            if (RequestType.ToLower() == "StatusNotification".ToLower())
            {
                JArray jObj = JArray.Parse(str);
                string[] ex = jObj[3].ToString().Split(",");
                 ex1 = ex[2].ToString().Split(":")[1];
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                ex1 = rgx.Replace(ex1, "").Trim();
            }
           

            return ex1;
        }
    }
}
