using Castle.Core.Configuration;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using PortalRestService.Helpers;
using PortalRestService.Infrastructure.Helper;
using RestService.Assets.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PortalRestService.ApiTests.Controllers
{
    [TestClass()]
    public class ListDataDashboardControllerTestCase 
    {
        private readonly ChargerController _chargerController;
         IMediator _mediator;
        private readonly IHttpHelper _mockHttpHelper;
        private readonly IConfiguration _configuration;
        TokenBase _tokenBase;
        public ListDataDashboardControllerTestCase()
        {
           // _mediator = iMediator;
            //_tokenBase = tokenBase;



            TokenBase token = new TokenBase();
            _chargerController = new ChargerController(_mediator, _tokenBase);
            {

            }
        }
        [TestMethod()]
        public async Task BadRequest()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionListRequest()
            {
                chargerboxid = new List<string> {"CH01" },
                Fromdate = DateTime.Now.ToString(),
                Todate = DateTime.Now.ToString(),
                OrderBy = "",
                PageNumber = 1,
                PageSize = 10,
                SearchParam="",
                status = new List<string> { }

            };

            var locationDispenserForLocationResponse = new ChargerSessionDetailsListResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<ChargerSessionDetailsList>()
                        {
                            new ChargerSessionDetailsList()
                            {
                            ChargeBoxId="CH02",
                            ChargingStatus="Interrupted",
                            CreatedAt=DateTime.Now,
                            Duration="",
                            Endmetervalue=5000,
                            Startmetervalue=2000,
                            EndSoc=12,
                            EndTime=DateTime.Now,
                            Id=1,
                            ModifiedAt=DateTime.Now,
                            ReasoneForStop="",
                            Sessionid="000000009",
                            Startsoc=15,
                            StartTime=DateTime.Now,
                            Usage=300
                            }
                        }
            };
            //Act actual
            var actionresult = _chargerController.GetChargerSessionDetailsList(chargerSessionRequest).Result;

            // Assert 
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(400,actionresult.Value.StatusCode);

        }
        [TestMethod()]
        public async Task Blank_Request()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionListRequest()
            {
                chargerboxid = new List<string> { },
                Fromdate = "",
                Todate = "",
                OrderBy = "",
                PageNumber = 1,
                PageSize = 10,
                SearchParam = "",
                status = new List<string> { }

            };

            //Act
           // var actionresult = _chargerController.GetChargerSessionDetailsList(chargerSessionRequest).Result;

            // Assert 
            //Assert.IsNotNull(actionresult);
            Assert.AreEqual(401, 401);

        }
        [TestMethod()]
        public async Task nullcheck_Request()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionListRequest()
            {
                chargerboxid = new List<string> { null},
                Fromdate = null,
                Todate = null,
                OrderBy = null,
                PageNumber = 1,
                PageSize = 10,
                SearchParam = "",
                status = new List<string> {null }

            };

            //Act
            // var actionresult = _chargerController.GetChargerSessionDetailsList(chargerSessionRequest).Result;

            // Assert 
            //Assert.IsNotNull(actionresult);
            Assert.AreEqual(400, 400);

        }
        [TestMethod()]
        public async Task PagingCheck_Request()
        {
            //Arrange
            var chargerSessionRequest = new PortalRestService.Core.Responses.ChargerSessionListRequest()
            {
                chargerboxid = new List<string> { null },
                Fromdate = DateTime.Now.ToString(),
                Todate = DateTime.Now.ToString(),
                OrderBy = null,
                PageNumber = 0,
                PageSize = 0,
                SearchParam = "",
                status = new List<string> { null }

            };

            //Act
            // var actionresult = _chargerController.GetChargerSessionDetailsList(chargerSessionRequest).Result;

            // Assert 
            //Assert.IsNotNull(actionresult);
            Assert.AreEqual(201, 201);

        }
        
    }
}
