using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Responses;
using PortalRestService.Helpers;
using RestService.Assets.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestService.Assets.Controllers.Tests
{
    [TestClass()]
    public class LocationDashboardControllerTests
    {
        private readonly LocationDashboardController _locationDashboardController;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IHttpHelper> _mockHttpHelper;
        private readonly Mock<IConfiguration> _configuration;
        public LocationDashboardControllerTests()
        {
            _mediator = new Mock<IMediator>();

            _mockHttpHelper = new Mock<IHttpHelper>();
            _configuration = new Mock<IConfiguration>();

            _locationDashboardController = new LocationDashboardController(_mediator.Object, _configuration.Object, _mockHttpHelper.Object);
            {

            }
        }
        [TestMethod()]
        public async Task locationstatusTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest() { LocationIds = new List<int> { 1, 2 }, Duration = "1", Opratorid = "1" };
            var chargingSessionByLocationForChartResponse = new List<LocationStatusQueryResponse>()
            {
                    new LocationStatusQueryResponse()
                    {
                       StatusCode = 200,
                       StatusMessage = "Ok",
                        data=new List<AllLocationStatusChartBO>()
                        {
                            new AllLocationStatusChartBO()
                            {
                                Counts=1,
                                Color="Green",
                                LocationStatus="Active"
                            }
                        }
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetLocationStatusByLocationIdQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);
            
            //Act
            var actionresult = await _locationDashboardController.locationstatus(chargerSessionRequest);

            // Assert 
            Assert.IsNotNull(actionresult);
        }
        [TestMethod()]
        public async Task GetDispenserByLocationTest()
        {
            //Arrange
            List<long> Ids = new List<long> { 1, 2 };
            var chargingSessionByLocationForChartResponse = new List<LocationDispenserForLocationResponse>()
            {
                    new LocationDispenserForLocationResponse()
                    {
                       StatusCode = 200,
                       StatusMessage = "Ok",
                        data=new List<LocationDispenserForLocation>()
                        {
                            new LocationDispenserForLocation()
                            {
                             ChargeBoxId="1",
                             ChargerPort="10",
                             ChargerStatus="Active",
                             ConnectorType="Conector",
                             DispenserId=1,
                             DispenserMake="DispenserMake",
                             DispenserModel="DispenserModel",
                             DispenserName="DispenserName",
                             locationId=1,
                             NoofPort="10",
                             ProtocolName="Http",
                             SerialNumber="1"
                            }
                        }
                   }
           };
            _mediator.Setup(md => md.Send(It.IsAny<List<GetDispenserByLocationIdQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargingSessionByLocationForChartResponse);

            //Act
            var actionresult = await _locationDashboardController.GetDispenserByLocation(Ids);

            // Assert 
            Assert.IsNotNull(actionresult);
        }
        [TestMethod()]
        public void GetLocatinByIdTest()
        {
            long Id = 1;
            // Arrange 
            var mockLocationById = GetMockGetLocatinByIdResponse();

            var mockHandler = new Mock<DelegatingHandler>();

            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var seriaizedAllLocation = JsonConvert.SerializeObject(mockLocationById);

            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedAllLocation) };

            _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            // Act
            var actionResult = _locationDashboardController.GetLocatinById(Id);
            //.Result as ActionResult<LocationsDispenserformapResponce>;

            // Assert
            Assert.IsNotNull(actionResult);
         //   Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
           // var allLocationResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationsDispenserformapResponce;
           // Assert.IsNotNull(allLocationResponse);
        }
        private GetLocatinByIdResponse GetMockGetLocatinByIdResponse()
        {
            return new GetLocatinByIdResponse()
            {
                StatusCode = 1,
                StatusMessage = "Ok",
                data = new Data()
                {
                    LocationName = "Noida",
                    ContactPersonName = "John",
                    CreatedBy = "Adam",
                    CreatedOn = DateTime.Now,
                    Department = new Department()
                    {
                        ContactPersonName = "Adam",
                        CreatedOn = DateTime.Now,
                        CreatedBy = "Damon",
                        Address = "Noida",
                        DepartmentName = "ChargerDept",
                        Id = 1,
                        IsActive = true,
                        ModifiedBy = "Smith",
                        ModifiedOn = DateTime.Now,
                    },
                    ModifiedBy = "Adam",
                    IsActive = false,
                    Id = 1,
                    Description = "Desc",
                    GlobalTax = "100",
                    ModifiedOn = DateTime.Now,
                    LocationAddressId = 1,
                    LocationId = 1,
                    LocationNumber = 1,
                    LocationSchedule = new List<LocationSchedule>()
                    {
                        new LocationSchedule()
                        {
                             LocationId = 1,
                              ModifiedOn= DateTime.Now,
                               CreatedBy="Flodian",
                                CreatedOn= DateTime.Now,
                                 Day="1",
                                  EndTime= DateTime.Now,
                                   Id= 1,
                                    IsActive= true,
                                     ModifiedBy="Smith",
                                      StartTime= DateTime.Now,
                        }
                    },
                     LocationStatus= new LocationStatus()
                     {
                          Id = 1,
                           LocationStatusName="Delhi",
                            CreatedBy="Adam",
                             ModifiedBy="John",
                              IsActive= false,
                               CreatedOn=DateTime.Now,
                                ModifiedOn=DateTime.Now,
                     },
                      LocationStatusId=1,
                       NetworkId=1,
                        NetworkName="NetworkName",
                         SubNetworkId=1,
                          SubNetworkName="SubNetwork",
                           TimeZone="UTC",
                            TotalCapacity="100",
                             UtilityService="1",
                           LocationAddress=new LocationAddress()
                           {
                                Id=1,
                                 IsActive=!true,
                                  ModifiedBy="John",
                                   CreatedBy="Adam",
                                    AddressLine1="Noida",
                                     AddressLine2="Delhi",
                                      AlternateMobileNumber="776668867",
                                       CityId=1,
                                        CityName="Bareilly",
                                         CountryId=1,
                                          CountryName="India",
                                           CreatedOn=DateTime.Now,
                                            Email="abc@nn.com",
                                             LandlineNumber="1020",
                                              Latitude=10,
                                               Longitude=10,
                                                MobileNumber="07865565757",
                                                 ModifiedOn=DateTime.Now,
                                                  PinCode="121222",
                                                   StateId=1,
                                                    StateName="U.P"
                           }
                },

            };
        }
    }
}