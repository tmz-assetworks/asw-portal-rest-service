using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
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
                    locatoinResponse = await PortalRestService.Helpers.Helper.GetCallAssetAuthAPIAsync(APIConstant.GetAllLocation,_tokenBase.acces_token);
                    AllLocationStatusQueryResponse Location = new AllLocationStatusQueryResponse();                // Location Status
                    if (locatoinResponse != null && locatoinResponse.IsSuccessStatusCode)
                    {
                        var locationDataContent = await locatoinResponse.Content.ReadAsStringAsync();
                        Location = JsonConvert.DeserializeObject<AllLocationStatusQueryResponse>(locationDataContent);
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

                // Getting Charger/Dispenser data
                HttpResponseMessage dispenserResponse = null;
                if (locationId == 0)
                    dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetAuthAPIAsync(APIConstant.GetAllDispenser,_tokenBase.acces_token);
                else dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetAuthAPIAsync(APIConstant.GetDispenserByLocation + "" + locationId,_tokenBase.acces_token);
                if (dispenserResponse.IsSuccessStatusCode)
                {
                    cardData = new CardData();
                    var dispenserData = await dispenserResponse.Content.ReadAsStringAsync();
                    DispenserResponse objDispenser = new DispenserResponse();

                    objDispenser = JsonConvert.DeserializeObject<DispenserResponse>(dispenserData);
                    cardData.Type = "Chargers";
                    cardData.Count = objDispenser.data != null ? objDispenser.data.Count : 0;

                    if (objDispenser.data != null)
                    {
                        List<StatusData> StatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.ChargerStatus.Available.GetEnumDisplayName(), Value = CommonHelpers.GetHoursTwoDigitFormat(objDispenser.data.Where(d => d.ChargerStatuses !=null && d.ChargerStatuses.Count >0 && d.ChargerStatuses[0].ChargerStatus1.ToLower().Equals(Status_Indication.ChargerStatus.Available.ToString().ToLower())).ToList().Count).ToString(), Color = ColorsEnum.ChargerStatus.Available.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargerStatus.Connected.GetEnumDisplayName(), Value = CommonHelpers.GetHoursTwoDigitFormat(objDispenser.data.Where(d => d.ChargerStatuses !=null && d.ChargerStatuses.Count>0  && d.ChargerStatuses[0].ChargerStatus1.Replace("Unavailable","Connected").ToLower().Equals(Status_Indication.ChargerStatus.Connected.GetEnumDisplayName().ToLower())).ToList().Count).ToString()  , Color = ColorsEnum.ChargerStatus.Connected.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargerStatus.Offline.GetEnumDisplayName(), Value = CommonHelpers.GetHoursTwoDigitFormat(objDispenser.data.Where(d => d.ChargerStatuses ==null || d.ChargerStatuses.Count==0).ToList().Count).ToString() , Color = ColorsEnum.ChargerStatus.Offline.GetEnumDisplayName() },
                       
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
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(locationIds), Encoding.UTF8, "application/json");

                string callingMethodLocation = APIConstant.Getdispenserbylocation;
                HttpResponseMessage responseSession = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethodLocation, httpContent, _tokenBase.acces_token);

                var locationData = await responseSession.Content.ReadAsStringAsync();
                locationsResponse = JsonConvert.DeserializeObject<LocationDispenserForLocationResponse>(locationData);

                
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
                        cardData.Type = "Errors";
                    else cardData.Type = "Alerts";
                    cardData.Count = 10;
                    List<StatusData> ErrorStatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.Errors.Critical.ToString(), Value = "05" , Color = ColorsEnum.ErrorsColor.Critical.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.Errors.High.ToString(), Value = "02" , Color = ColorsEnum.ErrorsColor.High.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.Errors.Medium.ToString(), Value = "03" , Color = ColorsEnum.ErrorsColor.Medium.GetEnumDisplayName()  },
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
    }
}
