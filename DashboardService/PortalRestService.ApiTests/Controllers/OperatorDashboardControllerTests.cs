using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PortalRestService.Api.Controllers;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Responses;
using PortalRestService.Helpers;
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
        private readonly Mock<IHttpHelper> _mockHttpHelper;
        private readonly Mock<IConfiguration> _configuration;

        public OperatorDashboardControllerTests()
        {
            _mediator = new Mock<IMediator>();

            _mockHttpHelper = new Mock<IHttpHelper>();
            _configuration = new Mock<IConfiguration>();

            _operatorDashboardController = new OperatorDashboardController(_mediator.Object, _configuration.Object, _mockHttpHelper.Object);
            {

            }
        }

        [TestMethod()]
        public void GetAllLocationTest()
        {
            // Arrange 
            var mockAllLocation = GetMockAllLocationQueryResponse();

            var mockHandler = new Mock<DelegatingHandler>();

            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var seriaizedAllLocation = JsonConvert.SerializeObject(mockAllLocation);

            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedAllLocation) };

            _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            // Act
            var actionResult = _operatorDashboardController.GetAllLocation().Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var allLocationResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as AllLocationQueryResponse;
            Assert.IsNotNull(allLocationResponse);
        }

        [TestMethod()] 
        public void GetLocationsDispenserformapTest()
        {
            // Arrange 
            var mockAllLocation = GetMockLocationsDispenserformapResponce();

            var mockHandler = new Mock<DelegatingHandler>();

            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var seriaizedAllLocation = JsonConvert.SerializeObject(mockAllLocation);

            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedAllLocation) };

            _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            // Act
            LocationDispenserRequest locationDispenserRequest = new LocationDispenserRequest();
            locationDispenserRequest.LocationIds = new List<long> { 1 , 2};
            locationDispenserRequest.opratorid = "1";
            var actionResult = _operatorDashboardController.GetLocationsDispenserformap(locationDispenserRequest).Result as ActionResult<LocationsDispenserformapResponce>;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var allLocationResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserformapResponce;
            Assert.IsNotNull(allLocationResponse);
        }

        [TestMethod()]
        public void GetLocationsDispenserDetailsTest()
        {
            // Arrange 
            var mockAllLocation = GetMockLocationsDispenserDetailsResponce();

            var mockHandler = new Mock<DelegatingHandler>();

            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var seriaizedAllLocation = JsonConvert.SerializeObject(mockAllLocation);

            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedAllLocation) };

            _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            // Act
            LocationDispenserRequest locationDispenserRequest = new LocationDispenserRequest();
            locationDispenserRequest.LocationIds = new List<long> { 1, 2 };
            locationDispenserRequest.opratorid = "1";
            var actionResult = _operatorDashboardController.GetLocationsDispenserDetails(locationDispenserRequest).Result as ActionResult<LocationsDispenserDetailsResponce>;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var allLocationResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserDetailsResponce;
            Assert.IsNotNull(allLocationResponse);
        }

        [TestMethod()]
        public async Task GetSummaryStatusTest()
        {
            // Arrange 
            var mockStatusSummary = GetMockStatusSummary();

            var mockHandler = new Mock<DelegatingHandler>();

            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var seriaizedStatusSummary = JsonConvert.SerializeObject(mockStatusSummary);

            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedStatusSummary) };

            _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            // Act
            var actionResult = await _operatorDashboardController.GetSummaryStatus();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var summaryResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as StatusItemData;
            Assert.IsNotNull(summaryResponse);
        }

        [TestMethod()]
        public async Task GetSummaryDataTest()
        {
            // Arrange 
            var mockSummaryData = GetMockSummaryData();

            var mockHandler = new Mock<DelegatingHandler>();

            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var seriaizedSummaryData = JsonConvert.SerializeObject(mockSummaryData);

            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedSummaryData) };

            _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            // Act
            var actionResult = await _operatorDashboardController.GetSummaryData();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var summaryResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as StatusSummary;
            Assert.IsNotNull(summaryResponse);
        }

        [TestMethod()]
        public async Task ChargingSessionTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest() { LocationIds = new List<int> { 1, 2 }, Duration = "1", Opratorid = "1" };
            var chargingSessionByLocationForChartResponse = new List<ChargingSessionByLocationForChartResponse>()
            {
                    new ChargingSessionByLocationForChartResponse()
                    {
                       StatusCode = 1,
                       StatusMessage = "Ok",
                       data = new List<ChargingSessionByLocationChartBO>()
                    {
                        new ChargingSessionByLocationChartBO()
                        {
                             ChargingStatus = "Active",

                             Counts = 1,

                             times = "1"
                        }
                   }
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetAllChargingSessionQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);
          //  _mediator.Setup(md => md.Send(It.IsAny<ChargerSessionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);
            //Act
            var actionresult = await _operatorDashboardController.ChargingSession(chargerSessionRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
        }

        [TestMethod()]
        public async Task GetChargerStatusByLocationIDTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest() { LocationIds = new List<int> { 1, 2 }, Duration = "1", Opratorid = "1" };
            var chargingSessionByLocationForChartResponse = new List<ChargerStatusForChartResponse>()
            {
                    new ChargerStatusForChartResponse()
                    {
                       StatusCode = 1,
                       StatusMessage = "Ok",
                       data = new List<ChargerByLocationChartBO>()
                    {
                        new ChargerByLocationChartBO()
                        {
                             ChargeStatus = "Active",

                             Counts = 1,

                             times = "1"
                        }
                   }
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetChargerByLocationIDQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);
            
            //Act
            var actionresult = await _operatorDashboardController.GetChargerStatusByLocationID(chargerSessionRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
        }

        [TestMethod()]
        public async Task GetEnergyUsedByLocationIDTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest() { LocationIds = new List<int> { 1, 2 }, Duration = "1", Opratorid = "1" };
            var chargingSessionByLocationForChartResponse = new List<EnergyUsedBOForChartResponse>()
            {
                    new EnergyUsedBOForChartResponse()
                    {
                       StatusCode = 1,
                       StatusMessage = "Ok",
                       data = new List<EnergyUsedsResponse>()
                    {
                        new EnergyUsedsResponse()
                        {
                             EndMeterValue = 1,

                             Counts = 1,

                             times = "1"
                        }
                   }
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetEnergyUsedsByLocationIDQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetEnergyUsedByLocationID(chargerSessionRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
        }

        [TestMethod()]
        public async Task GetLocationPerformingTest()
        {
            //Arrange
            var locationPerformingRequest = new PortalRestService.Core.Responses.LocationPerformingRequest() { Orderby=1,  LocationIds = new List<int> { 1, 2 }, Duration = "1", Opratorid = "1" };
            var chargingSessionByLocationForChartResponse = new List<LocationPerformingChartResponse>()
            {
                    new LocationPerformingChartResponse()
                    {
                       StatusCode = 1,
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
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetLocationPerformingQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetLocationPerforming(locationPerformingRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
        }
        [TestMethod()]
        public async Task GetMilesAddedByLocationTest()
        {
            //Arrange
            var milesAddedByLocationRequest = new PortalRestService.Core.Responses.MilesAddedByLocationRequest() { LocationIds = new List<int> { 1, 2 }, Duration = "1", Opratorid = "1" };
            var chargingSessionByLocationForChartResponse = new List<MilesAddedByLocationChartResponse>()
            {
                    new MilesAddedByLocationChartResponse()
                    {
                       StatusCode = 1,
                       StatusMessage = "Ok",
                       data = new List<MilesAddedByLocationResponse>()
                    {
                        new MilesAddedByLocationResponse()
                        {
                             RangeAdded=1,
                             Times="1"
                        }
                   }
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetLocationPerformingQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);

            //Act
            var actionresult = await _operatorDashboardController.GetMilesAddedByLocation(milesAddedByLocationRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
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
                                     Value=2
                                }
                            },
                             EnergyUsed=new List<EnergyUsed>()
                             {
                                 new EnergyUsed()
                                 {
                                      Key="Key2",
                                       Value=3
                                 }
                             },
                              Revenue=new List<Revenue>()
                              {
                                  new Revenue()
                                  {
                                       Key="Key3",
                                        Value=3
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
                        Latitude=0,
                        Longitude=0,
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
                        Faulty="No",
                        NoofPort="1",
                        DispenserId=1,
                        locationId=1,
                        status="Active"
                    },
                },

            };
        }
        #endregion
    }

}