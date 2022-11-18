using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using RestService.Assets.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.ApiTests.Controllers
{
    [TestClass()]
    public class ChargerDashboardControllerTestCase
    {
        private readonly ChargerController chargerController;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly Mock<IMediator> _mediator;
        public ChargerDashboardControllerTestCase()
        {
            _mediator = new Mock<IMediator>();
            TokenBase token = new TokenBase();
            //this._configuration = configuration;
            _configuration = new ConfigurationBuilder()
               .AddInMemoryCollection()
               .Build();
            chargerController = new ChargerController(_mediator.Object,token);
            {
            }
        }
        //===============================Test case Details page=========================================
        [TestMethod()]
        public void BadRequest_ChartDetails()
        {


            ChartDetailsListResponse QueryResponse = new ChartDetailsListResponse();

            ChartDetailsListRequest ChartDetailsListRequest = new ChartDetailsListRequest();
            ChartDetailsListRequest.LocationIds = null;
            ChartDetailsListRequest.Opratorid = null;

            ChartDetailsListRequest.OrderBy = null;
            ChartDetailsListRequest.PageNumber = 0;
            ChartDetailsListRequest.SearchParam = null;
            ChartDetailsListRequest.PageSize = 0;

            //if (ChartDetailsListRequest.PageSize == 0) ChartDetailsListRequest.PageSize = 10;
            //if (ChartDetailsListRequest.PageNumber == 0) ChartDetailsListRequest.PageNumber = 1;


            //Act

            //var result = chargerController.GetChartDetailsList(ChartDetailsListRequest).Result;

            //JObject jObj = JObject.Parse(Class1.GetValue(result));

            //string StatusCode = jObj["StatusCode"].ToString();
            //string Message = jObj["statusMessage"].ToString();

            //Assert  
         //   Assert.AreEqual(result.GetType().GetProperty("StatusCode").GetValue(result, null).ToString(), "400");


        }
        [TestMethod()]
        public void Paging_ChartDetails()
        {

            //checking defualt paging parameter handle or not 
            ChartDetailsListResponse QueryResponse = new ChartDetailsListResponse();

            ChartDetailsListRequest ChartDetailsListRequest = new ChartDetailsListRequest();
            ChartDetailsListRequest.LocationIds = new List<int>(new int[] { 43 }); ;
            ChartDetailsListRequest.Opratorid = "";

            ChartDetailsListRequest.OrderBy = null;
            ChartDetailsListRequest.PageNumber = 0;
            ChartDetailsListRequest.SearchParam = "";
            ChartDetailsListRequest.PageSize = 0;

            var result = chargerController.GetChartDetailsList(ChartDetailsListRequest).Result;

            //JObject jObj = JObject.Parse(Class1.GetValue(result));

            //string StatusCode = jObj["StatusCode"].ToString();
            //string Message = jObj["statusMessage"].ToString();

            //Assert  
            Assert.AreEqual(result.GetType().GetProperty("StatusCode").GetValue(result, null).ToString(), "200");


        }
        [TestMethod()]
        public void LocationIds_ChartDetails()
        {

            //passing null to loaction id 
            ChartDetailsListResponse QueryResponse = new ChartDetailsListResponse();

            ChartDetailsListRequest ChartDetailsListRequest = new ChartDetailsListRequest();
            ChartDetailsListRequest.LocationIds = null;
            ChartDetailsListRequest.Opratorid = "";

            ChartDetailsListRequest.OrderBy = null;
            ChartDetailsListRequest.PageNumber = 0;
            ChartDetailsListRequest.SearchParam = "";
            ChartDetailsListRequest.PageSize = 0;

            var result = chargerController.GetChartDetailsList(ChartDetailsListRequest).Result;

            //JObject jObj = JObject.Parse(Class1.GetValue(result));

            //string StatusCode = jObj["StatusCode"].ToString();
            //string Message = jObj["statusMessage"].ToString();

            //Assert  
            Assert.AreEqual(result.GetType().GetProperty("StatusCode").GetValue(result, null).ToString(), "200");


        }
        [TestMethod()]
        public async Task GetChargerInformationTest()
        {
            //Arrange
            var chargerInformationRequest = new PortalRestService.Core.Responses.ChargerInformationRequest()
            {
                ChargeBoxId = "CH01",
                OperatorId = "",
            };
            var chargerInformationResponse = new ChargerInformationResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new ChargerInfo()
                {
                    ChargeBoxId = "WIT202101021234",
                    Charger = "CHFAST CHARGER",
                    ChargerStatus = "FAULT",
                    ChargerType = "PUBLIC",
                    Address = "794 WALNUTWOOD DR BROOKLYN",
                    City = "BROOKLYN",
                    Country = "USA",
                    InstalledDate = DateTime.Now,
                    HardwareSerialNumber = "SN0101010102",
                    State = "NEW YORK",
                    ZipCode = "201301"
                },
            };
            _mediator.Setup(md => md.Send(It.IsAny<GetChargerInformationQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(chargerInformationResponse);

            //Act
            var actionresult = await chargerController.GetChargerInformation(chargerInformationRequest) as ActionResult<PortalRestService.Core.Responses.ChargerInformationResponse>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var alertResponse = (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargerInformationResponse;
            Assert.IsNotNull(alertResponse);
        }
        [TestMethod()]
        public async Task GetChartDetailsListTest()
        {
            //Arrange
            var _ChartDetailsListRequest = new PortalRestService.Core.Responses.ChartDetailsListRequest()
            {
                LocationIds= new List<int> { },
                Duration="90",
                Flag= "chargerSession",
                Opratorid = "",
                PageNumber = 0,
                PageSize = 0,
                SearchParam = "",
                OrderBy = ""
            };
            var _ChartDetailsListResponse = new ChartDetailsListResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<ChartDetailsList>()
                {
                    new ChartDetailsList()
                    {    Id=1,
                         ChargerName="",
                         UID="",
                         ChargerType="",
                         FaultSince="",
                         FaultDescription="",
                         TimeReported=DateTime.Now,
                         LocationId=90,
                         LocationName="",
                         ChargeBoxId=""
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
            _mediator.Setup(md => md.Send(It.IsAny< PagedList<GetChartDetailsListQuery>>(), It.IsAny<CancellationToken>())).ReturnsAsync(_ChartDetailsListResponse);

            //Act
            var actionresult = await chargerController.GetChartDetailsList(_ChartDetailsListRequest) as ActionResult<PortalRestService.Core.Responses.ChartDetailsListResponse>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var alertResponse = (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargerInformationResponse;
            Assert.IsNotNull(alertResponse);
        }
        [TestMethod()]
        public async Task GetChargerSessionDetailsListTest()
        {
            var GetChargerSessionListRequest = new ChargerSessionListRequest()
            {
                chargerboxid = new List<string> { },
                OrderBy = "",
                PageNumber = 0,
                PageSize = 0,
                SearchParam = ""

            };
            var ChargerSessionDetailsListResponse = new ChargerSessionDetailsListResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<ChargerSessionDetailsList>()
                {
                    new ChargerSessionDetailsList()
                    {
                     ChargeBoxId="",
                     CreatedAt=DateTime.Now,
                     Duration="",
                     EndTime=DateTime.Now,
                     Id=1,
                     ModifiedAt=DateTime.Now,
                     Sessionid="",
                     StartTime=DateTime.Now,
                     Usage=2

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
            _mediator.Setup(md => md.Send(It.IsAny<GetChargerSessionDetailsListQuery>(), It.IsAny<CancellationToken>()));//.ReturnsAsync(ChargerSessionDetailsListResponse);
            var actionresult = await chargerController.GetChargerSessionDetailsList(GetChargerSessionListRequest);// as ActionResult<PortalRestService.Core.Responses.ChargerInformationResponse>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(200, (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
            var alertResponse = (actionresult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as ChargerInformationResponse;
            Assert.IsNotNull(alertResponse);
        }
        [TestMethod()]
        public async Task GetCommandListTest()
        {
            //Arrange           
            var CommandListResponse = new CommandListResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<CommandList>()
                {
                 new CommandList()
                 {
                     Id = 1,
                     value=""
                 }
                },
            };
            var actionresult = await chargerController.GetCommandList();// as ActionResult<PortalRestService.Core.Responses.ChargerInformationResponse>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(CommandListResponse.StatusCode, (actionresult.Value).StatusCode);
            //var alertResponse = (actionresult.Value);
            Assert.IsNotNull(actionresult.Value);
        }
        [TestMethod()]
        public async Task GetDispensersDetailTest()
        {
            //Arrange
            var DispensersDetailRequest = new PortalRestService.Core.Responses.DispensersDetailRequest()
            {
                OrderBy="",
                PageNumber = 0,
                PageSize = 0,
                SearchParam=""
            };
            var DispensersDetailResponse = new DispensersDetailResponse()
            {
                StatusCode = 200,
                StatusMessage = "Ok",
                data = new List<DispensersDetail>()
                {
                    new DispensersDetail()
                    {
                     ChargerBoxId="",
                     ChargerName="",
                     ChargerType="",
                     FaultSince = "",
                     LocationContactName = "",
                     LocationContactNumber = "",
                     LocationId=90,
                     State = "",
                     TimeReported = ""
                    }
                },
                paginationResponse=new PaginationResponse()
                {
                     CurrentPage = 1,
                     HasNext=true,
                     HasPrevious=true,
                     PageSize=10,
                     TotalCount=10,
                     TotalPages=10
                }
            };
            var actionresult = chargerController.GetDispensersDetail(DispensersDetailRequest).Result;// as ActionResult<PortalRestService.Core.Responses.ChargerInformationResponse>;

            // Assert
            Assert.IsNotNull(actionresult);
            Assert.AreEqual(DispensersDetailResponse.StatusCode, (actionresult.StatusCode ));
            Assert.IsNotNull(actionresult.data);
        }
    }
}
