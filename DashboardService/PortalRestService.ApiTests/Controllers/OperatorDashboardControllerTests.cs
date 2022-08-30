using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortalRestService.Api.Controllers;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Api.Controllers.Tests
{
    [TestClass()]
    public class OperatorDashboardControllerTests
    {
        private readonly OperatorDashboardController _operatorDashboardController;
        private readonly Mock<IMediator> _mediator;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public OperatorDashboardControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
           .Build();
            _operatorDashboardController = new OperatorDashboardController(_mediator.Object, _configuration);
            {
            }
        }

        [TestMethod()]
        public void GetAllLocationTest()
        {
            // Arrange 
            // Act
            var actionResult = _operatorDashboardController.GetAllLocation().Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as AllLocationQueryResponse).StatusCode);
            var allLocationResponse = ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as AllLocationQueryResponse).StatusCode;
            Assert.IsNotNull(allLocationResponse);
        }

        [TestMethod()]
        public void GetLocationsDispenserformapTest()
        {
            // Arrange 
            // Act
            LocationOpratorRequest locationDispenserRequest = new LocationOpratorRequest();
            locationDispenserRequest.LocationIds = new List<int> { };
            locationDispenserRequest.opratorid = "1";
            var actionResult = _operatorDashboardController.GetLocationsDispenserformap(locationDispenserRequest).Result as ActionResult<LocationsDispenserformapResponce>;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserformapResponce).StatusCode);
            var allLocationResponse = ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserformapResponce).StatusCode;
            Assert.IsNotNull(allLocationResponse);
        }

        [TestMethod()]
        public void GetLocationsDispenserDetailsTest()
        {
            // Arrange 
            LocationDispenserDetailRequest locationDispenserRequest = new LocationDispenserDetailRequest();
            locationDispenserRequest.LocationIds = new List<long> { 4 };
            locationDispenserRequest.opratorId = "1";

            // Act
            var actionResult = _operatorDashboardController.GetLocationsDispenserDetails(locationDispenserRequest).Result as ActionResult<LocationsDispenserDetailsResponce>;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(0, ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserDetailsResponce).StatusCode);
            var allLocationResponse = ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserDetailsResponce).StatusCode;
        }

        [TestMethod()]
        public async Task GetSummaryStatusTest()
        {
            //Arrange
            int locationId = 4;

            var summaryStatusDataResponse = new CardDataResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<CardData>()
                {
                     new CardData()
                     {
                          Type="type",
                          Count = 1,
                          StatusData = new List<StatusData>()
                          {
                              new StatusData()
                              {
                                  Key="key",
                                  Color="Red",
                                  Value="value"
                              }
                          },
                     }
                }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetSummaryStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(summaryStatusDataResponse);

            //Act
            var actionresult = _operatorDashboardController.GetSummaryStatus(locationId).Result as ActionResult<PortalRestService.Core.Responses.CardDataResponse>;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as CardDataResponse).StatusCode);
            var statusDataResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as CardDataResponse).StatusCode;
            Assert.IsNotNull(statusDataResponse);
        }

        [TestMethod()]
        public async Task GetSummaryDataTest()
        {
            //Arrange
            int locationId = 4;

            var summaryDataResponse = new SummaryData()
            {
                StatusCode = 200,
                Message = "Ok",
                Data = new List<SummaryDetail>()
                {
                     new SummaryDetail()
                     {
                    chargingInfustructure = new List<ChargingInfustructure>()
                    {
                        new ChargingInfustructure()
                        {
                            Key = "key",
                            Value = 1
                        }
                    },
                    EnergyPoints = new List<EnergyPoint>()
                    {
                        new EnergyPoint()
                        {
                            Key = "key",
                            Value = "val"
                        }
                    },
                    EnergyUsed = new List<EnergyUsed>()
                    {
                        new EnergyUsed()
                        {
                            Key = "key",
                            Value = "val"
                        }

                    },
                    Revenue = new List<Revenue>()
                    {
                        new Revenue()
                        {
                            Key = "key",
                            Value = ""
                        }
                    }
                }
              }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetSummaryDataQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(summaryDataResponse);

            //Act
            var actionresult = _operatorDashboardController.GetSummaryData(locationId).Result as ActionResult<PortalRestService.Core.Responses.SummaryData>;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as SummaryData).StatusCode);
            var summaryResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as SummaryData).StatusCode;
            Assert.IsNotNull(summaryResponse);
        }

        [TestMethod()]
        public async Task ChargingSessionTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest()
            {
                LocationIds = new List<int> { },
                chargerBoxId = "CH01",
                Duration = "90",
                Opratorid = ""
            };
            var chargingSessionResponse = new ChargingSessionByLocationForChartResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<ChargingSessionByLocationChartBO>()
                {
                    new ChargingSessionByLocationChartBO()
                    {
                        Color = "Red",

                        svalue = "",

                        ChargingStatus = "Active",

                        Counts = 1,

                        times = "1"
                    }
                }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetAllChargingSessionQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionResponse);

            //Act
            var actionresult = _operatorDashboardController.ChargingSession(chargerSessionRequest).Result as ActionResult<List<PortalRestService.Core.Entities.Charger.ChargingSessionByLocationForChartResponse>>;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargingSessionByLocationForChartResponse).StatusCode);
            var sessionResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargingSessionByLocationForChartResponse).StatusCode;
            Assert.IsNotNull(sessionResponse);
        }

        [TestMethod()]
        public async Task GetChargerStatusByLocationIDTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest()
            {
                LocationIds = new List<int> { },
                chargerBoxId = "",
                Duration = "1",
                Opratorid = "1"
            };
            var chargingSessionByLocationForChartResponse = new ChargerStatusForChartResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",

                data = new List<ChargerByLocationChartBO>()
                       {
                        new ChargerByLocationChartBO()
                        {
                             svalue = "1",

                             Color = "Green",

                             ChargeStatus = "Active",

                             Counts = 1,

                             times = "1"
                        }
                       }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetChargerByLocationIDQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetChargerStatusByLocationID(chargerSessionRequest) as ActionResult<List<PortalRestService.Core.Responses.ChargerStatusForChartResponse>>; ;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargerStatusForChartResponse).StatusCode);
            var sessionResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargerStatusForChartResponse).StatusCode;
            Assert.IsNotNull(sessionResponse);
        }

        [TestMethod()]
        public async Task GetEnergyUsedByLocationIDTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest()
            {
                LocationIds = new List<int> { 12 },
                chargerBoxId = "",
                Duration = "90",
                Opratorid = ""
            };
            var energyUsedsResponse = new EnergyUsedBOForChartResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<EnergyUsedsResponse>()
                       {
                            new EnergyUsedsResponse()
                            {
                                 svalue = "",
                                 Counts = 1,
                                 EndMeterValue = 0,
                                 times = "1"
                            }
                       }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetEnergyUsedsByLocationIDQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(energyUsedsResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetEnergyUsedByLocationID(chargerSessionRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as EnergyUsedBOForChartResponse).StatusCode);
            var sessionResponse = (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value;
            Assert.IsNotNull(sessionResponse);
        }

        [TestMethod()]
        public async Task GetLocationPerformingTest()
        {
            //Arrange
            var locationPerformingRequest = new PortalRestService.Core.Responses.LocationPerformingRequest()
            {
                LocationIds = new List<int> { },
                Orderby = 1,
                Duration = "90",
                Opratorid = ""
            };
            var locationPerformingChartResponse = new LocationPerformingChartResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<LocationPerformingResponse>()
                         {
                             new LocationPerformingResponse()
                             {
                                   Color="Red",
                                   LocationName="Delhi",
                                   MeterValue=1,
                                   Orderby="Asc"
                             }
                        }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetLocationPerformingQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(locationPerformingChartResponse);

            //Act
            var actionresult = _operatorDashboardController.GetLocationPerforming(locationPerformingRequest).Result as ActionResult<List<PortalRestService.Core.Responses.LocationPerformingChartResponse>>;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationPerformingChartResponse).StatusCode);
            var sessionResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationPerformingChartResponse).StatusCode;
            Assert.IsNotNull(sessionResponse);
        }
        [TestMethod()]
        public async Task GetMilesAddedByLocationTest()
        {
            //Arrange
            var milesAddedByLocationRequest = new PortalRestService.Core.Responses.MilesAddedByLocationRequest()
            {
                LocationIds = new List<int> { },
                chargerBoxId = "",
                Duration = "90",
                Opratorid = ""
            };
            var chargingSessionByLocationForChartResponse = new MilesAddedByLocationChartResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<MilesAddedByLocationResponse>()
                 {
                     new MilesAddedByLocationResponse()
                     {
                       RangeAdded=1,
                        svalue="abc",
                         Times="1"
                     }
                 }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetMilesAddedByLocationQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetMilesAddedByLocation(milesAddedByLocationRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as MilesAddedByLocationChartResponse).StatusCode);
            var sessionResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as MilesAddedByLocationChartResponse).StatusCode;
            Assert.IsNotNull(sessionResponse);
        }
        #region private methods 
        private SummaryData GetMockSummaryData()
        {
            return new SummaryData()
            {
                Message = "Ok",
                StatusCode = 1,
                Data = new List<SummaryDetail>()
                {
                      new SummaryDetail()
                      {
                           chargingInfustructure=new List<ChargingInfustructure>()
                           {
                                new ChargingInfustructure()
                                {
                                     Key="Key",
                                     Value=1
                                }
                           },
                            EnergyPoints=new List<EnergyPoint>()
                            {
                                new EnergyPoint()
                                {
                                    Key="Key1",
                                     Value="2"
                                }
                            },
                             EnergyUsed=new List<EnergyUsed>()
                             {
                                 new EnergyUsed()
                                 {
                                      Key="Key2",
                                       Value="3"
                                 }
                             },
                              Revenue=new List<Revenue>()
                              {
                                  new Revenue()
                                  {
                                       Key="Key3",
                                        Value="3"
                                  }
                              }

                      }
                }
            };
        }
        private StatusSummary GetMockStatusSummary()
        {
            return new StatusSummary()
            {
                Message = "Ok",
                StatusCode = 1,
                data = new List<StatusSummaryData>()
                   {
                        new StatusSummaryData()
                        {
                             Type="Type 1",

                             Count = 1,

                             StatusData= new List<StatusItemData>()
                              {
                                   new StatusItemData()
                                   {
                                        Key="Key",
                                        value=1
                                   }
                              },
                        }
                   }
            };
        }
        private ChargerResponse GetMockChargerResponse()
        {
            return new ChargerResponse()
            {
                chargerData = new List<ChargerData>()
                {
                    new ChargerData()
                    {
                         Count = 1,
                         StatusData=new List<StatusData>()
                          {
                              new StatusData()
                              {
                                   Key="Key",
                                   Value="Valued"
                              }
                          },
                         Type="Type 1"
                    }
                },
                Message = "Ok",
                StatusCode = 1
            };
        }
        private AllLocationQueryResponse GetMockAllLocationQueryResponse()
        {
            return new AllLocationQueryResponse()
            {
                data = new List<LocationData>()
                {
                    new LocationData()
                    {
                        Id=1,
                        LocationName="Noida"
                    },
                },
                StatusCode = 1,
                StatusMessage = "Ok"
            };
        }
        private LocationsDispenserformapResponce GetMockLocationsDispenserformapResponce()
        {
            return new LocationsDispenserformapResponce()
            {
                StatusCode = 1,
                StatusMessage = "Ok",
                data = new List<LocationsDispenser>()
                {
                    new LocationsDispenser()
                    {
                        LocationName="Noida",
                        CityName="Noida",
                        CountryName="India",
                        DispenserId=1,
                        Latitude="0",
                        Longitude="0",
                        locationId=1,
                        StateName="U.P",
                        status="Active"
                    },
                },

            };
        }
        private LocationsDispenserDetailsResponce GetMockLocationsDispenserDetailsResponce()
        {
            return new LocationsDispenserDetailsResponce()
            {
                StatusCode = 1,
                StatusMessage = "Ok",
                data = new List<LocationsDispenserDetails>()
                {
                    new LocationsDispenserDetails()
                    {
                        LocationName="Noida",
                        Address="Noida",
                        Available="Yes",
                        Connected="Yes",
                        ContactName="Smith",
                        ContactNo="767675780",
                        Faulted="No",
                        NoofPort="1",
                        DispenserId=1,
                        locationId=1,
                        status="Active"
                    },
                },

            };
        }
        #endregion

        [TestMethod()]
        public async Task GetOperatorAlertsTest()
        {
            //Arrange
            var operatorAlertRequest = new PortalRestService.Core.Responses.OperatorAlertRequest()
            {
                LocationIds = new List<int> { },
                chargerBoxIds = new List<string> { },
                operatorId = "1",
                OrderBy = "asc",
                PageNumber = 0,
                PageSize = 0,
                SearchParam = ""
            };
            var alertsResponse = new OperatorAlertResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<AlertResponse>()
                {
                  new AlertResponse()
                  {
                       Category="OCPP",
                       ChargeBoxId="1",
                       DateTime=DateTime.Now,
                       IPAddress="10.2.10.20",
                       LocationsName="Australia",
                       MessageType="Type",
                       RequestPayload="",
                       ResponsePayload=""
                  }
                },
                paginationResponse = new Core.PagingHelper.PaginationResponse()
                {
                    CurrentPage = 1,
                    HasNext = false,
                    HasPrevious = false,
                    PageSize = 10,
                    TotalCount = 10,
                    TotalPages = 50
                }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetAllAlertsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(alertsResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetOperatorAlerts(operatorAlertRequest) as ActionResult<List<PortalRestService.Core.Responses.OperatorAlertResponse>>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as OperatorAlertResponse).StatusCode);
            var alertResponse = ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as OperatorAlertResponse).StatusCode;
            Assert.IsNotNull(alertResponse);
        }

        [TestMethod()]
        public async Task GetEventLogByLocationTest()
        {
            //Arrange
            var eventLogRequest = new PortalRestService.Core.Responses.EventLogRequest()
            {
                LocationIds = new List<int> { 4 },
                ChargerBoxIds = new List<string> { },
                Opratorid = "1",
                OrderBy = "asc",
                PageNumber = 0,
                PageSize = 10,
                SearchParam = ""
            };
            if (eventLogRequest.PageSize == 0) eventLogRequest.PageSize = 10;
            if (eventLogRequest.PageNumber == 0) eventLogRequest.PageNumber = 1;
            var eventLogLocationResponse = new EventLogLocationResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<EventLogLocation>
                {
                  new EventLogLocation()
                  {
                        CreatedAt = DateTime.Now,
                         DeviceId="1",
                         EventLogDataSource= "source",
                         Id=1,
                         IsRead=true,
                         LocationId="1",
                         LocationName="Noida",
                         ModifiedAt=DateTime.Now,
                         RequestId="1",
                         RequestPayload="payload",
                         RequestType="reqype",
                         RequestTypeColor="Green",
                         ResponsePayload="respayload"
                  }
                },
                paginationResponse = new Core.PagingHelper.PaginationResponse()
                {
                    CurrentPage = 1,
                    HasNext = false,
                    HasPrevious = false,
                    PageSize = 10,
                    TotalCount = 10,
                    TotalPages = 50
                }
            };
            _mediator.Setup(md => md.Send(It.IsAny<PagedList<EventLogByLocationQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(eventLogLocationResponse);

            //Act
            var actionresult = _operatorDashboardController.GetEventLogByLocation(eventLogRequest).Result;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Value as PortalRestService.Core.Responses.EventLogLocationResponse).StatusCode));
        }

        [TestMethod()]
        public async Task UpdateOcppEventLogIsReadTest()
        {
            //Arrange
            int id = 1;

            var eventLogLocationResponse = new EventLogLocationResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<EventLogLocation>
                {
                  new EventLogLocation()
                  {
                         CreatedAt = DateTime.Now,
                         DeviceId="1",
                         EventLogDataSource= "source",
                         Id=1,
                         IsRead=true,
                         LocationId="1",
                         LocationName="Noida",
                         ModifiedAt=DateTime.Now,
                         RequestId="1",
                         RequestPayload="payload",
                         RequestType="reqype",
                         RequestTypeColor="Green",
                         ResponsePayload="respayload"
                  }
                },
                paginationResponse = new Core.PagingHelper.PaginationResponse()
                {
                    CurrentPage = 1,
                    HasNext = false,
                    HasPrevious = false,
                    PageSize = 10,
                    TotalCount = 10,
                    TotalPages = 50
                }
            };
            _mediator.Setup(md => md.Send(It.IsAny<UpdateIsReadEventLogByIDQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(eventLogLocationResponse);

            //Act
            var actionresult = _operatorDashboardController.UpdateOcppEventLogIsRead(id).Result;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Value as PortalRestService.Core.Responses.EventLogLocationResponse).StatusCode));
        }
    }

}