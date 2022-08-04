using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PortalRestService.Helper;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Responses;
using PortalRestService.Core.Entities.Charger;
using System.Net;
using PortalRestService.Application;
using PortalRestService.Helpers;

using PortalRestService.Infrastructure.EnumData;
using System.Text;
using PortalRestService.Infrastructure.Repositories;
using PortalRestService.Core.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace PortalRestService.Api.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    [Authorize]
    public class OperatorDashboardController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly double perkwtRate = 0;
        private readonly double gasolineInKiloWatt = 0;
        private readonly double lbsofCO2emitted = 0;
        //private readonly IHttpHelper _httpHelper;
        public OperatorDashboardController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            this._configuration = configuration;
            //_httpHelper = httpHelper;

            gasolineInKiloWatt = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("GallongasolineKiloWatt").Value);
            lbsofCO2emitted = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("lbsofCO2emitted").Value);
            perkwtRate = (double)Convert.ToDouble(this._configuration.GetSection("EneryRatePerKg").GetSection("perkwtRate").Value);
        }

        [HttpGet]
        [Route("GetAllLocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AllLocationQueryResponse>> GetAllLocation()
        {
            try
            {
                string callingMethod = APIConstant.GetAllLocationName;
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetAPIAsync(callingMethod);
                AllLocationQueryResponse alLocationQueryResponse = new AllLocationQueryResponse();
                if (response.IsSuccessStatusCode)
                {
                    var locationinfo = await response.Content.ReadAsStringAsync();
                    return Ok(JsonConvert.DeserializeObject<AllLocationQueryResponse>(locationinfo));
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


                return alLocationQueryResponse == null ? NotFound() : this.Ok(alLocationQueryResponse);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }


        }

        [HttpPost]
        [Route("GetLocationsDispenserformap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationsDispenserformapResponce>> GetLocationsDispenserformap([FromBody] LocationDispenserRequest request)
        {
            try
            {
                string callingMethod = APIConstant.Getlocationsdispenserformap;
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(request.LocationIds), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethod, httpContent);
                LocationsDispenserformapResponce locationsDispenserformapResponce = new LocationsDispenserformapResponce();
                if (response.IsSuccessStatusCode)
                {
                    var locationdispenserformapinfo = await response.Content.ReadAsStringAsync();
                    locationsDispenserformapResponce = JsonConvert.DeserializeObject<LocationsDispenserformapResponce>(locationdispenserformapinfo);

                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


                return locationsDispenserformapResponce == null ? NotFound() : this.Ok(locationsDispenserformapResponce);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("GetLocationsDispenserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationsDispenserDetailsResponce>> GetLocationsDispenserDetails([FromBody] LocationDispenserRequest request)
        {
            try
            {
                string callingMethod = APIConstant.GetLocationsDispenserDetails;
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(request.LocationIds), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethod, httpContent);
                LocationsDispenserDetailsResponce locationsDispenserDetailsResponce = new LocationsDispenserDetailsResponce();
                if (response.IsSuccessStatusCode)
                {

                    var locationdispenserdetailsinfo = await response.Content.ReadAsStringAsync();
                    locationsDispenserDetailsResponce = JsonConvert.DeserializeObject<LocationsDispenserDetailsResponce>(locationdispenserdetailsinfo);
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


                return locationsDispenserDetailsResponce == null ? NotFound() : this.Ok(locationsDispenserDetailsResponce);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Return the location, chararger , charging Session and Error
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("GetSummaryStatus/{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardDataResponse>> GetSummaryStatus(int locationId = 0)
        {
            CardDataResponse dataResponse = new CardDataResponse();
            List<CardData> data = new List<CardData>();
            CardData cardData = null;
            try
            {
                HttpResponseMessage locatoinResponse = null;
                if (locationId == 0)
                {
                    locatoinResponse = await PortalRestService.Helpers.Helper.GetCallAssetAPIAsync(APIConstant.GetAllLocation);
                    AllLocationStatusQueryResponse Location = new AllLocationStatusQueryResponse();                // Location Status
                    if (locatoinResponse != null && locatoinResponse.IsSuccessStatusCode)
                    {
                        var locationDataContent = await locatoinResponse.Content.ReadAsStringAsync();
                        Location = JsonConvert.DeserializeObject<AllLocationStatusQueryResponse>(locationDataContent);
                        cardData = new CardData();
                        cardData.Type = "Locations";
                        cardData.Count = Location.data!=null ? Location.data.Count : 0;

                        if (Location.data != null)
                        {
                            List<StatusData> StatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName(), Value = Location.data!=null? Location.data.Where(d => d.LocationStatus.ToLower().Equals(Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName().ToLower())).ToList().Count.ToString():"", Color = ColorsEnum.LocationsColor.Commissioned.GetEnumDisplayName() },
                        new StatusData { Key = Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName(), Value = Location.data!=null? Location.data.Where(d => d.LocationStatus.ToString().ToLower().Trim().Equals(Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName().ToLower().Trim())).ToList().Count.ToString():"" , Color = ColorsEnum.LocationsColor.UnderMaintenance.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName(), Value =Location.data!=null? Location.data.Where(d => d.LocationStatus.ToLower().Equals(Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName().ToLower())).ToList().Count.ToString() :"" , Color = ColorsEnum.LocationsColor.Upcoming.GetEnumDisplayName()  },
                      };
                            cardData.StatusData = StatusData;
                            data.Add(cardData);
                        }
                    }
                }

                // Getting Charger/Dispenser data
                HttpResponseMessage dispenserResponse = null;
                if (locationId == 0)
                    dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetAPIAsync(APIConstant.GetAllDispenser);
                else dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetAPIAsync(APIConstant.GetDispenserByLocation + "" + locationId);
                if (dispenserResponse.IsSuccessStatusCode)
                {
                    cardData = new CardData();
                    var dispenserData = await dispenserResponse.Content.ReadAsStringAsync();
                    DispenserResponse objDispenser = new DispenserResponse();

                    objDispenser = JsonConvert.DeserializeObject<DispenserResponse>(dispenserData);
                    cardData.Type = "Chargers";
                    cardData.Count = objDispenser.data!=null ? objDispenser.data.Count :0;

                    if (objDispenser.data != null)
                    {
                        List<StatusData> StatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.ChargerStatus.Available.GetEnumDisplayName(), Value = objDispenser.data.Where(d => d.dispenserStatus.dispenserStatusName.ToLower().Equals(Status_Indication.ChargerStatus.Available.ToString().ToLower())).ToList().Count.ToString(), Color = ColorsEnum.ChargerStatus.Available.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargerStatus.Connected.GetEnumDisplayName(), Value = objDispenser.data.Where(d => d.dispenserStatus.dispenserStatusName.ToLower().Equals(Status_Indication.ChargerStatus.Connected.GetEnumDisplayName().ToLower())).ToList().Count.ToString()  , Color = ColorsEnum.ChargerStatus.Connected.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargerStatus.Offline.GetEnumDisplayName(), Value = objDispenser.data.Where(d => d.dispenserStatus.dispenserStatusName.ToLower().Equals(Status_Indication.ChargerStatus.Offline.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargerStatus.Offline.GetEnumDisplayName() },

                      };
                        cardData.StatusData = StatusData;
                        data.Add(cardData);
                    }
                }

                // Getting Charging Session data
                HttpResponseMessage chargingSessionResponse = null;
                chargingSessionResponse = await PortalRestService.Helpers.Helper.GetCallOCPPAPIAsync(APIConstant.GetChargingSession);
                cardData = new CardData();
                cardData.Type = "Charging Session";
                List<PortalRestService.Core.Responses.ChargingSession> objChargingSession = new List<PortalRestService.Core.Responses.ChargingSession>();
                var chargingSessionData = await chargingSessionResponse.Content.ReadAsStringAsync();
                objChargingSession = JsonConvert.DeserializeObject<List<PortalRestService.Core.Responses.ChargingSession>>(chargingSessionData);

                if (locationId > 0)
                {
                    List<int> locationIds = new List<int>()
                   {
                       locationId
                   };
                    LocationDispenserForLocationResponse locationsResponse = new LocationDispenserForLocationResponse();
                    StringContent httpContent = new StringContent(JsonConvert.SerializeObject(locationIds), Encoding.UTF8, "application/json");

                    string callingMethodLocation = APIConstant.Getdispenserbylocation;
                    HttpResponseMessage responseSession = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethodLocation, httpContent);

                    var locationData = await responseSession.Content.ReadAsStringAsync();
                    locationsResponse = JsonConvert.DeserializeObject<LocationDispenserForLocationResponse>(locationData);
                    if (chargingSessionResponse != null && chargingSessionResponse.IsSuccessStatusCode)
                    {
                        List<LocationDispenserForLocation> datalocations = locationsResponse.data.ToList();
                        var chargingSessionsData = (from cs in objChargingSession join l in datalocations on cs.ChargerId equals l.DispenserId where l.locationId == locationId select cs).ToList();


                        if (objChargingSession != null)
                        {
                            cardData.Count = chargingSessionsData.Count;
                            List<StatusData> StatusData = new List<StatusData>()
                         {
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Completed.ToString(), Value = chargingSessionsData.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Completed.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargingSessionsColor.Completed.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Interrupted.ToString(), Value = chargingSessionsData.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Interrupted.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargingSessionsColor.Interrupted.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Cancelled.ToString(), Value = chargingSessionsData.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Cancelled.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargingSessionsColor.Cancelled.GetEnumDisplayName()  },
                        };
                            cardData.StatusData = StatusData;

                        }
                        data.Add(cardData);
                    }

                }
                if (chargingSessionResponse != null && locationId == 0 && chargingSessionResponse.IsSuccessStatusCode)
                {
                    cardData.Count = objChargingSession.Count;

                    if (objChargingSession != null)
                    {
                        List<StatusData> StatusData = new List<StatusData>()
                    {
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Completed.ToString(), Value = objChargingSession.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Completed.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargingSessionsColor.Completed.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Interrupted.ToString(), Value = objChargingSession.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Interrupted.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargingSessionsColor.Interrupted.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.ChargingSessionStatus.Cancelled.ToString(), Value = objChargingSession.Where(d => d.ChargingStatus.ToLower().Equals(Status_Indication.ChargingSessionStatus.Cancelled.ToString().ToLower())).ToList().Count.ToString() , Color = ColorsEnum.ChargingSessionsColor.Cancelled.GetEnumDisplayName()  },
                        };
                        cardData.StatusData = StatusData;
                        data.Add(cardData);
                    }
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
                        new StatusData { Key = Status_Indication.Errors.Critical.ToString(), Value = "5" , Color = ColorsEnum.ChargingSessionsColor.Completed.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.Errors.High.ToString(), Value = "2" , Color = ColorsEnum.ChargingSessionsColor.Interrupted.GetEnumDisplayName()  },
                        new StatusData { Key = Status_Indication.Errors.Medium.ToString(), Value = "3" , Color = ColorsEnum.ChargingSessionsColor.Cancelled.GetEnumDisplayName()  },
                        };
                    cardData.StatusData = ErrorStatusData;
                    data.Add(cardData);
                }

                dataResponse.data = data;
                dataResponse.StatusMessage = "Record found";
                dataResponse.StatusCode = (int)HttpStatusCode.OK;
                return dataResponse.data == null ? NotFound() : this.Ok(dataResponse);
            }
            catch (Exception ex)
            {
                return dataResponse.data == null ? NotFound() : this.Ok(dataResponse);
            }

            return dataResponse == null ? NotFound() : this.Ok(dataResponse);
        }


        [HttpGet]
        [Route("GetSummaryData/{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardDataResponse>> GetSummaryData(int locationId = 0)
        {
            SummaryData summaryData = new SummaryData();
            List<SummaryDetail> summaryDetails = null;
            try
            {
                // Getting Charging Session data
                summaryDetails = new List<SummaryDetail>();
                HttpResponseMessage chargingSessionResponse = await PortalRestService.Helpers.Helper.GetCallOCPPAPIAsync(APIConstant.GetChargingSession);
                if (chargingSessionResponse.IsSuccessStatusCode)
                {

                    List<PortalRestService.Core.Responses.ChargingSession> objChargingSession = new List<PortalRestService.Core.Responses.ChargingSession>();
                    var chargingSessionData = await chargingSessionResponse.Content.ReadAsStringAsync();
                    objChargingSession = JsonConvert.DeserializeObject<List<PortalRestService.Core.Responses.ChargingSession>>(chargingSessionData);

                    if (locationId > 0)
                    {
                        List<int> locationIds = new List<int>()
                             {
                                 locationId
                             };

                        LocationDispenserForLocationResponse locationsResponse = new LocationDispenserForLocationResponse();
                        StringContent httpContent = new StringContent(JsonConvert.SerializeObject(locationIds), Encoding.UTF8, "application/json");

                        string callingMethodLocation = APIConstant.Getdispenserbylocation;
                        HttpResponseMessage responseSession = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethodLocation, httpContent);

                        var locationData = await responseSession.Content.ReadAsStringAsync();
                        locationsResponse = JsonConvert.DeserializeObject<LocationDispenserForLocationResponse>(locationData);
                        if (chargingSessionResponse != null && chargingSessionResponse.IsSuccessStatusCode && locationsResponse != null && locationsResponse.data != null)
                        {
                            List<LocationDispenserForLocation> datalocations = locationsResponse.data.ToList();
                            objChargingSession = (from cs in objChargingSession join l in datalocations on cs.ChargerId equals l.DispenserId where l.locationId == locationId select cs).ToList();
                        }
                    }

                    if (objChargingSession != null)
                    {
                        SummaryDetail summaryDetail = new SummaryDetail();

                        //Type  chargingInfustructure
                        TotalLocationAndChargerResponse totalLocationAndChargerResponse = new TotalLocationAndChargerResponse();
                        HttpResponseMessage chargingInfustructureResponse = await PortalRestService.Helpers.Helper.GetCallAssetAPIAsync(APIConstant.GetTotalLocationAndCharger);

                        var dataCharginInfraData = await chargingInfustructureResponse.Content.ReadAsStringAsync();
                        totalLocationAndChargerResponse = JsonConvert.DeserializeObject<TotalLocationAndChargerResponse>(dataCharginInfraData);

                        if (totalLocationAndChargerResponse != null && totalLocationAndChargerResponse.StatusCode == (int)HttpStatusCode.OK)
                        {
                            summaryDetail.chargingInfustructure = new List<ChargingInfustructure>();
                            if (locationId == 0)
                            {

                                ChargingInfustructure chargingInfustructureTotalSites = new ChargingInfustructure();
                                chargingInfustructureTotalSites.Key = "Total Locations";
                                chargingInfustructureTotalSites.Value = totalLocationAndChargerResponse.TotalLocations;
                                summaryDetail.chargingInfustructure.Add(chargingInfustructureTotalSites);
                            }
                            if (locationId > 0)
                            {
                                List<int> locationIds = new List<int>()
                             {
                                 locationId
                             };
                                ChargingInfustructure chargingInfustructureTotalSites = new ChargingInfustructure();
                                chargingInfustructureTotalSites.Key = "Total Locations";
                                chargingInfustructureTotalSites.Value = 0;
                                summaryDetail.chargingInfustructure.Add(chargingInfustructureTotalSites);

                                LocationDispenserForLocationResponse locationsResponse = new LocationDispenserForLocationResponse();
                                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(locationIds), Encoding.UTF8, "application/json");

                                HttpResponseMessage dispenserResponse = null;
                                dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetWithBodyAPIAsync(APIConstant.Getdispenserbylocation,httpContent);
                                var dispenserData = await dispenserResponse.Content.ReadAsStringAsync();
                                DispenserResponse objDispenser = new DispenserResponse();

                                objDispenser = JsonConvert.DeserializeObject<DispenserResponse>(dispenserData);
                                ChargingInfustructure chargingInfustructureTotalLocations = new ChargingInfustructure();
                                chargingInfustructureTotalLocations.Key = "Total Chargers";
                                if (objDispenser != null && objDispenser.data != null)
                                    chargingInfustructureTotalLocations.Value = objDispenser.data.Where(d=>d.locationId==locationId).Count();
                                else chargingInfustructureTotalLocations.Value = 0;
                                summaryDetail.chargingInfustructure.Add(chargingInfustructureTotalLocations);
                            }
                            else
                            {
                                ChargingInfustructure chargingInfustructureTotalLocations = new ChargingInfustructure();
                                chargingInfustructureTotalLocations.Key = "Total Chargers";
                                chargingInfustructureTotalLocations.Value = totalLocationAndChargerResponse.TotalDispenser;
                                summaryDetail.chargingInfustructure.Add(chargingInfustructureTotalLocations);
                            }
                        }

                        // Type = "Revenue";
                        var todayChargingsession = (from data in objChargingSession.Where(c => c.CreatedAt != null && c.CreatedAt.Value.Day == DateTime.Now.Day && c.CreatedAt.Value.Year == DateTime.Now.Year) select data).ToList();       // AS-701

                        double startChargingMeter = (double)(from data in objChargingSession where data.StartMeterValue != null select data.StartMeterValue.Value).Sum();
                        double endChargingMeter = (double)(from data in objChargingSession where data.EndMeterValue != null select data.EndMeterValue.Value).Sum();

                        double billableChargingMeter = endChargingMeter - startChargingMeter;
                        if (billableChargingMeter < 0)
                            billableChargingMeter = 0;
                        double todayStartChargingMeter = (from data in objChargingSession.Where(c => c.CreatedAt.Value.Day == DateTime.Now.Day && c.CreatedAt.Value.Year == DateTime.Now.Year).Where(s => s.StartMeterValue != null) select data.StartMeterValue.Value).Sum();    // AS-701
                        double todayEndChargingMeter = (from data in objChargingSession.Where(c => c.CreatedAt.Value.Day == DateTime.Now.Day && c.CreatedAt.Value.Year == DateTime.Now.Year).Where(s => s.EndMeterValue != null) select data.EndMeterValue.Value).Sum();        // AS-701
                        if (todayEndChargingMeter < 0)
                            todayEndChargingMeter = 0;
                        double todayBillableChargingMeter = todayEndChargingMeter - todayStartChargingMeter;

                        int chargingSessionGroupBydateCount = objChargingSession.Where(c => c.CreatedAt != null).GroupBy(s => s.CreatedAt.Value.Date).ToList().Count;
                        //TotalRevenue
                        Revenue totalRevenue = new Revenue();
                        totalRevenue.Key = EnumControlTexts.DisplayingLabels.TotalRevenue.GetEnumDisplayName();
                        totalRevenue.Value = Math.Round(billableChargingMeter * perkwtRate, 2).ToString("0.00");
                        summaryDetail.Revenue = new List<Revenue>();
                        summaryDetail.Revenue.Add(totalRevenue);

                        // Daily Revenue
                        Revenue dailyRevenue = new Revenue();
                        dailyRevenue.Key = EnumControlTexts.DisplayingLabels.DailyRevenue.GetEnumDisplayName();
                        if (billableChargingMeter > 0)
                            dailyRevenue.Value = Math.Round((billableChargingMeter / chargingSessionGroupBydateCount) * perkwtRate).ToString("0.00"); //Auther:Pradeep , Date:27/07/2022,  AS-701
                        else dailyRevenue.Value = "0";
                        summaryDetail.Revenue.Add(dailyRevenue);

                        // Today 's Revenue
                        Revenue todaysRevenue = new Revenue();
                        todaysRevenue.Key = EnumControlTexts.DisplayingLabels.TodaysRevenue.GetEnumDisplayName();
                        if (todayChargingsession.Count > 0)
                            todaysRevenue.Value = Math.Round(todayBillableChargingMeter * perkwtRate, 2).ToString("0.00"); ///(double)(from data in objChargingSession.Where(c => c.CreatedAt == DateTime.Now) select data.EndMeterValue.Value).Sum();   //objChargingSession.GroupBy(c => c.EndMeterValue).Sum();  //.Sum(r => r.EndMeterValue);
                        else todaysRevenue.Value = "0";
                        summaryDetail.Revenue.Add(todaysRevenue);

                        //2  EnergyUsed
                        //TotalEnergy
                        EnergyUsed energyUsedTotalEnergy = new EnergyUsed();
                        energyUsedTotalEnergy.Key = EnumControlTexts.DisplayingLabels.TotalEnergy.GetEnumDisplayName();
                        energyUsedTotalEnergy.Value = Math.Round(billableChargingMeter, 2).ToString("0.00");
                        summaryDetail.EnergyUsed = new List<EnergyUsed>();
                        summaryDetail.EnergyUsed.Add(energyUsedTotalEnergy);
                        //DailyAverage
                        EnergyUsed dailyAverageEnergyUsed = new EnergyUsed();
                        dailyAverageEnergyUsed.Key = EnumControlTexts.DisplayingLabels.DailyAverage.GetEnumDisplayName();
                        if (billableChargingMeter > 0)
                            dailyAverageEnergyUsed.Value = Math.Round(billableChargingMeter / chargingSessionGroupBydateCount, 2).ToString("0.00");  // Date : 29/07/2022    // 
                        else todaysRevenue.Value = "0";
                        summaryDetail.EnergyUsed.Add(dailyAverageEnergyUsed);

                        // Today's 
                        EnergyUsed todaysEnergyUsed = new EnergyUsed();
                        todaysEnergyUsed.Key = EnumControlTexts.DisplayingLabels.Todays.GetEnumDisplayName();
                        todaysEnergyUsed.Value = Math.Round(todayBillableChargingMeter, 2).ToString("0.00");
                        summaryDetail.EnergyUsed.Add(todaysEnergyUsed);


                        // Energy Points
                        List<EnergyPoint> EnergyPoints = new List<EnergyPoint>();
                        EnergyPoint energyPointMTofco2Saved = new EnergyPoint();
                        energyPointMTofco2Saved.Key = EnumControlTexts.DisplayingLabels.MTofco2Saved.GetEnumDisplayName();
                        energyPointMTofco2Saved.Value = Math.Round((billableChargingMeter / gasolineInKiloWatt) * lbsofCO2emitted, 2).ToString("0.00");   // 1 gasoline = 33.705 Kilowatt
                        summaryDetail.EnergyPoints = EnergyPoints;
                        summaryDetail.EnergyPoints.Add(energyPointMTofco2Saved);

                        EnergyPoint energyPointGGEofGasSaved = new EnergyPoint();
                        energyPointGGEofGasSaved.Key = EnumControlTexts.DisplayingLabels.GGEofGasSaved.GetEnumDisplayName();
                        energyPointGGEofGasSaved.Value = Math.Round(billableChargingMeter / gasolineInKiloWatt, 2).ToString("0.00");
                        summaryDetail.EnergyPoints.Add(energyPointGGEofGasSaved);

                        //Binding all data
                        summaryDetails.Add(summaryDetail);

                    }
                }
                summaryData.Data = summaryDetails;
                summaryData.Message = "Record found";
                summaryData.StatusCode = (int)HttpStatusCode.OK;
                return summaryData.Data == null ? NotFound() : this.Ok(summaryData);
            }
            catch (Exception ex)
            {
                summaryData.Message = "Operaion failed!";
                return summaryData.Data == null ? NotFound() : this.Ok(summaryData);
            }
            return summaryData.Data == null ? NotFound() : this.Ok(summaryData);
        }

        [HttpPost("ChargingSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargingSessionByLocationForChartResponse>>> ChargingSession([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                var result = await _mediator.Send(new GetAllChargingSessionQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("GetChargerStatusByLocationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargerStatusForChartResponse>>> GetChargerStatusByLocationID([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                var result = await _mediator.Send(new GetChargerByLocationIDQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }


        [HttpPost("GetEnergyUsedByLocationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Core.Responses.EnergyUsedBOForChartResponse>>> GetEnergyUsedByLocationID([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                var result = await _mediator.Send(new GetEnergyUsedsByLocationIDQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("GetLocationPerforming")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Core.Responses.LocationPerformingChartResponse>>> GetLocationPerforming([FromBody] LocationPerformingRequest locationPerformingRequest)
        {
            try
            {
                var result = await _mediator.Send(new GetLocationPerformingQuery(locationPerformingRequest.LocationIds, locationPerformingRequest.Duration, locationPerformingRequest.Orderby));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
        [HttpPost("GetMilesAddedByLocation")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Core.Responses.MilesAddedByLocationChartResponse>>> GetMilesAddedByLocation([FromBody] MilesAddedByLocationRequest milesAddedByLocationRequest)
        {
            try
            {
                var result = await _mediator.Send(new GetMilesAddedByLocationQuery(milesAddedByLocationRequest.LocationIds, milesAddedByLocationRequest.Duration));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
    }
}
