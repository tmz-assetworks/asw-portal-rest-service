using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetSummaryDataRepository : OcppRepository<SummaryData>, IGetSummaryDataRepository
    {
        private readonly double perkwtRate = 0;
        private readonly double gasolineInKiloWatt = 0;
        private readonly double lbsofCO2emitted = 0;
        private readonly IConfiguration _configuration;
        public GetSummaryDataRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, IConfiguration configuration) : base(dbContext)
        {
             this._configuration = configuration;
            //_httpHelper = httpHelper;

            gasolineInKiloWatt = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("GallongasolineKiloWatt").Value);
            lbsofCO2emitted = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("lbsofCO2emitted").Value);
            perkwtRate = (double)Convert.ToDouble(this._configuration.GetSection("EneryRatePerKg").GetSection("perkwtRate").Value);
        }

        public async Task<SummaryData> GetSummaryData(int locationId)
        {
            SummaryData summaryData = new SummaryData();
            List<SummaryDetail> summaryDetails = null;
            try
            {
                // Getting Charging Session data
                summaryDetails = new List<SummaryDetail>();
                //HttpResponseMessage chargingSessionResponse = await PortalRestService.Helpers.Helper.GetCallOCPPAPIAsync(APIConstant.GetChargingSession);
                //if (chargingSessionResponse.IsSuccessStatusCode)
                //{

                    List<PortalRestService.Core.Models.ChargingSession> objChargingSession =  _dbContext.ChargingSessions.ToList();
                   // var chargingSessionData = await chargingSessionResponse.Content.ReadAsStringAsync();
                   // objChargingSession = JsonConvert.DeserializeObject<List<PortalRestService.Core.Responses.ChargingSession>>(chargingSessionData);

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
                        if ( locationsResponse != null && locationsResponse.data != null)
                        {
                            List<LocationDispenserForLocation> datalocations = locationsResponse.data.ToList();
                            objChargingSession = (from cs in _dbContext.ChargingSessions.ToList() join l in datalocations on cs.ChargerId equals l.DispenserId where l.locationId == locationId select cs).ToList();
                        }
                    }

                    
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
                                dispenserResponse = await PortalRestService.Helpers.Helper.GetCallAssetWithBodyAPIAsync(APIConstant.Getdispenserbylocation, httpContent);
                                var dispenserData = await dispenserResponse.Content.ReadAsStringAsync();
                                DispenserResponse objDispenser = new DispenserResponse();

                                objDispenser = JsonConvert.DeserializeObject<DispenserResponse>(dispenserData);
                                ChargingInfustructure chargingInfustructureTotalLocations = new ChargingInfustructure();
                                chargingInfustructureTotalLocations.Key = "Total Chargers";
                                if (objDispenser != null && objDispenser.data != null)
                                    chargingInfustructureTotalLocations.Value = objDispenser.data.Where(d => d.locationId == locationId).Count();
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

                    
                //}
                summaryData.Data = summaryDetails;
                summaryData.Message = "Record found";
                summaryData.StatusCode = (int)HttpStatusCode.OK;
                if (summaryData.Data == null)
                    summaryData.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                summaryData.Message = "Operaion failed!";
                if (summaryData.Data == null)
                    summaryData.StatusCode = (int)HttpStatusCode.NotFound;
            }
            if (summaryData.Data == null)
                summaryData.StatusCode = (int)HttpStatusCode.NotFound;

            return Task.FromResult(summaryData).Result;
        }
    }
 }

       
