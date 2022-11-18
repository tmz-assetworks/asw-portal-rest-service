using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using RestService.Assets.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.ApiTests.Controllers
{
    [TestClass()]
    public class VehicleDashboardControllerTestCase
    {
        //===============================Mediator connection to controller===================================
        private readonly VehicleDashboardController _VehicleDashboardController;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly Mock<IMediator> _mediator;
        public VehicleDashboardControllerTestCase()
        {
            _mediator = new Mock<IMediator>();
            TokenBase token = new TokenBase();
            //this._configuration = configuration;
            _configuration = new ConfigurationBuilder()
               .AddInMemoryCollection()
               .Build();
            _VehicleDashboardController = new VehicleDashboardController(_mediator.Object, _configuration, token);
            {
            }
        }
        //===================================Test Case========================================================
        [TestMethod()]
        public async Task GetVehicleByIDTest()
        {
            //Arrange
            long id = 10;           
            var getVehiclesResponse = new VehiclesResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new VehicleByIdData()
                {
                    department = "",
                    domicileLocation = "",
                    licencePlate = "",
                    MakeName = "",
                    ModelName = "",
                    ModelYear = 2022,
                    rfId = "",
                    Status = true,                  
                    vehicleMacAddress = "",
                    VIN = ""
                },
            };
            var actionresult = _VehicleDashboardController.GetVehicleByID(id).Result;
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(getVehiclesResponse.StatusCode, (actionresult.Value).StatusCode);
            Assert.IsNotNull(actionresult.Value.data);
        }
        [TestMethod()]
        public async Task GetAllVehicleTest()
        {
            var _GetAllVehicleRequest = new GetAllVehicleRequest()
            {
                opratorid = "",
                OrderBy = "",
                PageNumber = 1,
                PageSize = 10,
                SearchParam = ""
            };

            var vehicleWithPaginationData = new vehicleWithPagination()
            {
                data = new List<Vehicle>()
                {
                    new Vehicle()
                    {
                     id=10,
                     Status=true,
                     Department="Dept",
                     DomicileLocation="DomLocation",
                     LicencePlate="LicPlate",
                     MakeName="Make",
                     ModelName="Model",
                     ModelYear=2022,
                     RFIDCardAssigned="CardAssigned",
                     VehicleMacAddress="VcleMacAddress",
                     VIN="VIN"
                    }
                },
                paginationResponse = new PaginationResponse()
                {
                    CurrentPage = 1,
                    HasNext = true,
                    HasPrevious = true,
                    PageSize = 10,
                    TotalCount = 10,
                    TotalPages = 10
                }
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetAllVehicleQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vehicleWithPaginationData);

            // Act
            var actionresult = _VehicleDashboardController.GetAllVehicle(_GetAllVehicleRequest).Result as ActionResult<PortalRestService.Core.Responses.GetAllVehicleResponse>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, (actionresult.Value.StatusCode));
        }
    }
}
