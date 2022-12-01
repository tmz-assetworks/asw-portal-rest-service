using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Responses;
using PortalRestService.Helpers;
using PortalRestService.Infrastructure.Helper;
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
            TokenBase token = new TokenBase();
            _locationDashboardController = new LocationDashboardController(_mediator.Object, _configuration.Object, token);
            {

            }
        }
        [TestMethod()]
        public async Task LocationStatusTest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionRequest()
            {
                LocationIds = new List<int> { },
                chargerBoxId = "CH01",
                Duration = "90",
                Opratorid = ""
            };

            var locationStatusQueryResponse = new List<AllLocationStatusChartBO>()
            {
                             new AllLocationStatusChartBO()
                             {
                                Counts=1,
                                Color="Green",
                                LocationStatus="Active"
                             }
           };
            _mediator.Setup(md => md.Send(It.IsAny<GetLocationStatusByLocationIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(locationStatusQueryResponse);

            //Act
            var actionresult = _locationDashboardController.LocationStatus(chargerSessionRequest).Result as ActionResult<List<PortalRestService.Core.Responses.AllLocationStatusChartBO>>;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, ((actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as PortalRestService.Core.Responses.LocationStatusQueryResponse).StatusCode);

        }
        [TestMethod()]
        public async Task GetDispenserByLocationSuccessTest()
        {
            //Arrange
            LocationDispensersRequest Ids = new LocationDispensersRequest { };           
            var locationDispenserForLocationResponse = new LocationDispenserForLocationResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<LocationDispenserForLocation>()
                        {
                            new LocationDispenserForLocation()
                            {
                             ChargeBoxId="1",
                             ChargerStatus="Active",
                             ConnectorType="Conector",
                             DispenserId=1,
                             DispenserMake="DispenserMake",
                             DispenserModel="DispenserModel",
                             locationId=1,
                             NoofPort="10",
                             ProtocolName="Http",
                             SerialNumber="1"
                            }
                        }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetDispenserByLocationIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(locationDispenserForLocationResponse);

            //Act
            var actionResult = _locationDashboardController.GetDispenserByLocation(Ids).Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as PortalRestService.Core.Responses.LocationDispenserForLocationResponse).StatusCode);
            var dispenserByLocationResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as LocationDispenserForLocationResponse;
            Assert.IsNotNull(dispenserByLocationResponse);
        }
        [TestMethod()]
        public void GetLocatinByIdSuccessTest()
        {
            // Arrange 
            long Id = 4;
            // Act
            var actionResult = _locationDashboardController.GetLocatinById(Id).Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as PortalRestService.Core.Responses.GetLocatinByIdResponse).StatusCode);
        }
        [TestMethod()]
        public void GetLocatinByIdNotFoundTest()
        {
            // Arrange 
            long Id = 0;            
            // Act
            var actionResult = _locationDashboardController.GetLocatinById(Id).Result;
            string statusMessage = "Record not found";

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(statusMessage, ((actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as PortalRestService.Core.Responses.GetLocatinByIdResponse).StatusMessage);
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
                    DepartmentName  ="IT",
                    ModifiedBy = "Adam",
                    IsActive = false,
                    Id = 1,
                    Description = "Desc",
                    GlobalTax = "100",
                    ModifiedOn = DateTime.Now,
                    LocationAddressId = 1,
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
                             EndTime= "",
                             Id= 1,
                             IsActive= true,
                             ModifiedBy="Smith",
                             StartTime="",
                        }
                    },
                    LocationStatus = new LocationStatus()
                    {
                        Id = 1,
                        LocationStatusName = "Delhi",
                        CreatedBy = "Adam",
                        ModifiedBy = "John",
                        IsActive = false,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    },
                    LocationStatusId = 1,
                    FuelProtectType = "FullProtect",
                    TimeZone = "UTC",
                    TotalCapacity = "100",
                    UtilityService = "1",
                    LocationAddress = new LocationAddress()
                    {
                        Id = 1,
                        IsActive = !true,
                        ModifiedBy = "John",
                        CreatedBy = "Adam",
                        AddressLine1 = "Noida",
                        AddressLine2 = "Delhi",
                        //CityId = 1,
                        CityName = "Bareilly",
                        CountryId = 1,
                        CountryName = "India",
                        CreatedOn = DateTime.Now,
                        LandlineNumber = "1020",
                        Latitude = "10",
                        Longitude = "20",
                        ModifiedOn = DateTime.Now,
                        PinCode = "121222",
                        StateId = 1,
                        StateName = "U.P"
                     
                    }
                },

            };
        }
    }
}