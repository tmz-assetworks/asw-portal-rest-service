using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetSummaryDataRepository : OcppRepository<SummaryData>, IGetSummaryDataRepository
    {
        private readonly double perkwtRate = 0;
        private readonly double gasolineInKiloWatt = 0;
        private readonly double lbsofCO2emitted = 0;
        private readonly IConfiguration _configuration;
        private readonly ILocationRepository _locationRepository;
        TokenBase _tokenBase;
        public GetSummaryDataRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, IConfiguration configuration, TokenBase tokenBase,
            ILocationRepository locationRepository) : base(dbContext)
        {
            this._configuration = configuration;
            this._locationRepository = locationRepository;

            gasolineInKiloWatt = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("GallongasolineKiloWatt").Value);
            lbsofCO2emitted = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("lbsofCO2emitted").Value);
            perkwtRate = (double)Convert.ToDouble(this._configuration.GetSection("EneryRatePerKg").GetSection("perkwtRate").Value);
            _tokenBase = tokenBase;
        }

        public async Task<SummaryData> GetSummaryData(int locationId)
        {
            SummaryData summaryData = new SummaryData();
            List<SummaryDetail> summaryDetails = null;
            try
            {
                summaryDetails = new List<SummaryDetail>();
                List<PortalRestService.Core.Models.ChargingSession> objChargingSession = _dbContext.ChargingSessions.ToList();

                LocationDispenserForLocationResponse locationsResponse = new LocationDispenserForLocationResponse();

                if (_tokenBase.getRole().ToLower() == "admin")
                {

                    locationsResponse.data = await (from location in locationId > 0 ? _dbContext.Locations.Where(x => locationId == x.Id) :_dbContext.Locations.Where(x => x.CreatedBy == _tokenBase.getObjectId())
                                              join charger in _dbContext.Charger
                                              on location.Id equals charger.LocationId
                                              select new LocationDispenserForLocation
                                              {
                                                  DispenserId = charger.Id,
                                                  locationId = location.Id,
                                                  ChargeBoxId = locationId > 0? charger.ChargeBoxId + " (" + location.LocationName + ")" : charger.ChargeBoxId,
                                              }).ToListAsync<LocationDispenserForLocation>();
                }
                else
                {
                    locationsResponse.data = await (from location in locationId>0 ?_dbContext.Locations.Where(x => locationId == x.Id): _dbContext.Locations
                                              join charger in _dbContext.Charger
                                              on location.Id equals charger.LocationId

                                              join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                             on location.Id equals userMap.LocationId
                                              select new LocationDispenserForLocation
                                              {
                                                  DispenserId = charger.Id,
                                                  locationId = location.Id,
                                                  ChargeBoxId = charger.ChargeBoxId,
                                              }).ToListAsync<LocationDispenserForLocation>();
                }
                if (locationsResponse != null && locationsResponse.data != null)
                {
                    List<LocationDispenserForLocation> datalocations = locationsResponse.data.ToList();
                    objChargingSession = (from cs in objChargingSession join l in datalocations on cs.ChargerId equals l.DispenserId where l.ChargeBoxId == cs.DeviceId select cs).Where(t=>t.EndMeterValue>t.StartMeterValue).ToList();
                }

                SummaryDetail summaryDetail = new SummaryDetail();

                List<long> locationList = await _locationRepository.GetAllLocationIdByObjectId();

                TotalLocationAndChargerResponse totalLocationAndChargerResponse = new TotalLocationAndChargerResponse();
                totalLocationAndChargerResponse.TotalLocations = await _dbContext.Locations.Where(x => locationList.Contains(x.Id)).CountAsync(); //Join(_dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id)), p => p.Id, n => n.LocationId, (p, n) => new { p.LocationId }).Count();
                totalLocationAndChargerResponse.TotalDispenser =await _dbContext.Charger.Where(x => locationList.Contains(x.LocationId.Value)).CountAsync(); //Join(_dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id)), p => p.LocationId, n => n.LocationId, (p, n) => new { p.LocationId }).Count();


                if (totalLocationAndChargerResponse != null)
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
                        ChargingInfustructure chargingInfustructureTotalLocations = new ChargingInfustructure();
                        chargingInfustructureTotalLocations.Key = "Total Chargers";
                        if (locationsResponse != null && locationsResponse.data != null)
                            chargingInfustructureTotalLocations.Value = locationsResponse.data.Where(d => d.locationId == locationId).Count();
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

                var todayChargingsession = (from data in objChargingSession.Where(c => c.CreatedAt != null && c.CreatedAt.Value.Day == DateTime.Now.Day && c.CreatedAt.Value.Year == DateTime.Now.Year) select data).ToList();       // AS-701

                double startChargingMeter = Math.Round((double)(from data in objChargingSession where data.StartMeterValue != null select data.StartMeterValue.Value).Sum() / 1000, 2);
                double endChargingMeter = Math.Round((double)(from data in objChargingSession where data.EndMeterValue != null select data.EndMeterValue.Value).Sum() / 1000, 2);

                double billableChargingMeter = endChargingMeter - startChargingMeter;
                if (billableChargingMeter < 0)
                    billableChargingMeter = 0;
                double todayStartChargingMeter = Math.Round((double)(from data in objChargingSession.Where(c => c.CreatedAt.Value.Day == DateTime.Now.Day && c.CreatedAt.Value.Year == DateTime.Now.Year).Where(s => s.StartMeterValue != null) select data.StartMeterValue.Value).Sum() / 1000, 2);    // AS-701
                double todayEndChargingMeter = Math.Round((double)(from data in objChargingSession.Where(c => c.CreatedAt.Value.Day == DateTime.Now.Day && c.CreatedAt.Value.Year == DateTime.Now.Year).Where(s => s.EndMeterValue != null) select data.EndMeterValue.Value).Sum() / 1000, 2);        // AS-701
                if (todayEndChargingMeter < 0)
                    todayEndChargingMeter = 0;
                double todayBillableChargingMeter = todayEndChargingMeter - todayStartChargingMeter;

                int chargingSessionGroupBydateCount = objChargingSession.Where(c => c.CreatedAt != null).GroupBy(s => s.CreatedAt.Value.Date).ToList().Count;
                
                summaryDetail.Revenue = new List<Revenue>();
                var today = DateTime.Today;
                var thismonthStart = new DateTime(today.Year, today.Month, 1);
                Revenue totalRevenue = new Revenue();
                totalRevenue.Key = EnumControlTexts.DisplayingLabels.TotalRevenue.GetEnumDisplayName();
                var totalRevenueValue = await _dbContext.Locations.Where(x => locationList.Contains(x.Id)).Join(_dbContext.PaymentTransaction, userlocation => userlocation.Id,
                    trans => trans.LocationId, (userlocation, trans) => new { userlocation, trans }).SumAsync(o => o.trans.TotalAmount);
                totalRevenue.Value = string.Format("{0:#,0}", totalRevenueValue.ToString());
                summaryDetail.Revenue.Add(totalRevenue);

                Revenue dailyRevenue = new Revenue();
                dailyRevenue.Key = EnumControlTexts.DisplayingLabels.DailyRevenue.GetEnumDisplayName();
                if (totalRevenue.Value != "0")
                {
                        List<DateTime> list = await
                        (from location in _dbContext.Locations.Where(x => locationList.Contains(x.Id))
                            join payment in _dbContext.PaymentTransaction on location.Id equals payment.LocationId
                            select payment.CreatedOn).ToListAsync();
                        
                    
                    if (list.Count > 0)
                    {
                        DateTime eaeliestDate = list.Min();

                        DateTime currentdateTime = DateTime.Now;
                        int totalNumberofDays = (currentdateTime - eaeliestDate).Days + 1;
                        decimal dailyAverage = Math.Round(totalRevenueValue / totalNumberofDays, 2);
                        dailyRevenue.Value = string.Format("{0:#,0}", dailyAverage.ToString());
                    }
                    else
                    { dailyRevenue.Value = string.Format("{0:#,0}", 00); }

                }
                else
                {
                    dailyRevenue.Value = "0";

                }
                summaryDetail.Revenue.Add(dailyRevenue);

                Revenue todaysRevenue = new Revenue();
                todaysRevenue.Key = EnumControlTexts.DisplayingLabels.TodaysRevenue.GetEnumDisplayName();
                
                var todaysRevenueValue =await _dbContext.Locations.Where(x => locationList.Contains(x.Id)).Join(_dbContext.PaymentTransaction, location => location.Id,
                    payment => payment.LocationId, (location, payment) => new { location, payment }).
                    Where(o => o.payment.CreatedOn >= today).SumAsync(o => o.payment.TotalAmount);

                todaysRevenue.Value = string.Format("{0:#,0}", todaysRevenueValue.ToString());
                summaryDetail.Revenue.Add(todaysRevenue);

               
                EnergyUsed energyUsedTotalEnergy = new EnergyUsed();
                energyUsedTotalEnergy.Key = EnumControlTexts.DisplayingLabels.TotalEnergy.GetEnumDisplayName();
                energyUsedTotalEnergy.Value = string.Format("{0:#,0}", Math.Round(billableChargingMeter, 2));

                summaryDetail.EnergyUsed = new List<EnergyUsed>();
                summaryDetail.EnergyUsed.Add(energyUsedTotalEnergy);
                
                EnergyUsed dailyAverageEnergyUsed = new EnergyUsed();
                dailyAverageEnergyUsed.Key = EnumControlTexts.DisplayingLabels.DailyAverage.GetEnumDisplayName();
                if (billableChargingMeter > 0)
                    dailyAverageEnergyUsed.Value = string.Format("{0:#,0}", Math.Round(billableChargingMeter / chargingSessionGroupBydateCount, 2));  // Date : 29/07/2022    // 
                else todaysRevenue.Value = "0";
                dailyAverageEnergyUsed.Value = string.Format("{0:#,0}", dailyAverageEnergyUsed.Value);
                summaryDetail.EnergyUsed.Add(dailyAverageEnergyUsed);

                EnergyUsed todaysEnergyUsed = new EnergyUsed();
                todaysEnergyUsed.Key = EnumControlTexts.DisplayingLabels.Todays.GetEnumDisplayName();
                todaysEnergyUsed.Value = string.Format("{0:#,0}", Math.Round(todayBillableChargingMeter, 2));
                summaryDetail.EnergyUsed.Add(todaysEnergyUsed);

                List<EnergyPoint> EnergyPoints = new List<EnergyPoint>();
                EnergyPoint energyPointMTofco2Saved = new EnergyPoint();
                energyPointMTofco2Saved.Key = EnumControlTexts.DisplayingLabels.MTofco2Saved.GetEnumDisplayName();
                energyPointMTofco2Saved.Value = string.Format("{0:#,0}", Math.Round((billableChargingMeter / gasolineInKiloWatt) * lbsofCO2emitted, 2));   // 1 gasoline = 33.705 Kilowatt
                summaryDetail.EnergyPoints = EnergyPoints;
                summaryDetail.EnergyPoints.Add(energyPointMTofco2Saved);


                EnergyPoint energyPointGGEofGasSaved = new EnergyPoint();
                energyPointGGEofGasSaved.Key = EnumControlTexts.DisplayingLabels.GGEofGasSaved.GetEnumDisplayName();
                energyPointGGEofGasSaved.Value = string.Format("{0:#,0}", Math.Round(billableChargingMeter / gasolineInKiloWatt, 2));

                summaryDetail.EnergyPoints.Add(energyPointGGEofGasSaved);

                
                summaryDetails.Add(summaryDetail);

                summaryData.Data = summaryDetails;
                summaryData.Message = RespnoseMessage.Record_found;
                summaryData.StatusCode = (int)HttpStatusCode.OK;
                if (summaryData.Data == null)
                    summaryData.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                summaryData.Message = RespnoseMessage.Opeartion_Failed;
                if (summaryData.Data == null)
                    summaryData.StatusCode = (int)HttpStatusCode.NotFound;
            }
            if (summaryData.Data == null)
                summaryData.StatusCode = (int)HttpStatusCode.NotFound;

            return Task.FromResult(summaryData).Result;
        }
    }
}

