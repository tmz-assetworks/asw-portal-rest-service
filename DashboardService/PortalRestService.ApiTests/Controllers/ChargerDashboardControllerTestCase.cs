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
            token.acces_token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiJzcG46NzY5OGNiZWQtN2Q5Zi00M2IzLWI5Y2QtYTRmMDliOWI1NWVkIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNzQ0YWE4YjAtYmI5OS00OTgyLTkwM2YtNTIzMjgyMTZiNGJlLyIsImlhdCI6MTY2OTg4Njk0NCwibmJmIjoxNjY5ODg2OTQ0LCJleHAiOjE2Njk4OTIyMzIsImFjciI6IjEiLCJhaW8iOiJBVFFBeS84VEFBQUExQTlEbHBoOVBJM3BEcUpWZjFOUktNRHpPV2RoU3piRkdUWDZINm0zR3hXVWVpNmZWSGdaMWRzZ3hlOUtPR3pCIiwiYW1yIjpbInB3ZCJdLCJhcHBpZCI6Ijc2OThjYmVkLTdkOWYtNDNiMy1iOWNkLWE0ZjA5YjliNTVlZCIsImFwcGlkYWNyIjoiMSIsImZhbWlseV9uYW1lIjoib3BlcmF0b3IiLCJnaXZlbl9uYW1lIjoib3BlcmF0b3IiLCJpcGFkZHIiOiI1Mi4xNDIuMTcyLjIyIiwibmFtZSI6Im9wZXJhdG9yIiwib2lkIjoiZjRlOWI0MTktYzdkYy00MmI2LTkyYmMtZjIwNzcwNzE2N2YyIiwicmgiOiIwLkFWVUFzS2hLZEptN2drbVFQMUl5Z2hhMHZ1M0xtSGFmZmJORHVjMms4SnViVmUySUFPdy4iLCJyb2xlcyI6WyJPcGVyYXRvciJdLCJzY3AiOiJBcHBSb2xlQXNzaWdubWVudC5SZWFkV3JpdGUuQWxsIERpcmVjdG9yeS5BY2Nlc3NBc1VzZXIuQWxsIERpcmVjdG9yeS5SZWFkLkFsbCBEaXJlY3RvcnkuUmVhZFdyaXRlLkFsbCBEaXJlY3RvcnkuV3JpdGUuUmVzdHJpY3RlZCBlbWFpbCBHcm91cC5SZWFkLkFsbCBHcm91cC5SZWFkV3JpdGUuQWxsIElkZW50aXR5VXNlckZsb3cuUmVhZFdyaXRlLkFsbCBvZmZsaW5lX2FjY2VzcyBvcGVuaWQgcHJvZmlsZSBUZWFtU2V0dGluZ3MuUmVhZFdyaXRlLkFsbCBVc2VyLkV4cG9ydC5BbGwgVXNlci5JbnZpdGUuQWxsIFVzZXIuTWFuYWdlSWRlbnRpdGllcy5BbGwgVXNlci5SZWFkIFVzZXIuUmVhZC5BbGwgVXNlci5SZWFkQmFzaWMuQWxsIFVzZXIuUmVhZFdyaXRlIFVzZXIuUmVhZFdyaXRlLkFsbCBVc2VyQXV0aGVudGljYXRpb25NZXRob2QuUmVhZCBVc2VyQXV0aGVudGljYXRpb25NZXRob2QuUmVhZC5BbGwgVXNlckF1dGhlbnRpY2F0aW9uTWV0aG9kLlJlYWRXcml0ZSBVc2VyQXV0aGVudGljYXRpb25NZXRob2QuUmVhZFdyaXRlLkFsbCIsInN1YiI6IksyYW43OF9kSEx1T2tjdWtMdEFjanAxZlg5Tk9ZZTZKei02SGRacWI4MVkiLCJ0aWQiOiI3NDRhYThiMC1iYjk5LTQ5ODItOTAzZi01MjMyODIxNmI0YmUiLCJ1bmlxdWVfbmFtZSI6Im9wZXJhdG9yQGRldm9wc3Rla21pbmR6Lm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im9wZXJhdG9yQGRldm9wc3Rla21pbmR6Lm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6IldnMzVaeTBSWVU2dnJMRmN2ajhOQWciLCJ2ZXIiOiIxLjAifQ.g3DPuouxmb2VODht1ylRGr7l7PuHDoyGejBEMceTmcJyM-jo_ZAiEnFLRrWEsCfSqTuDE8HZvG7auxd447uVEbhaQV_qsWp2MQbS2KTDMQMZw2PAWclcWHp-A51FKWbwcVUqlvevFRW9u-isA95C9zRuL6hbhxlonarn1v8BKa5CtNfXvkIqfJvrV5NwHT1z62fMWrL6CfsykB8lQUnwu1UvgKkw--qOxSfAgtRpPZ2CiozWpdoIaVkeTAT75eOY0D_jCaR3mfMDv0VUUpNBxRXCFYa7WIRua6IxhbP7cBjZ30Fl5FjMcMwAgAfNY-3tNWMfcUDrXHAmPcvED5D_HA";
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
            ChartDetailsListRequest.LocationIds = new List<long>(new long[] { 43 }); ;
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
                LocationIds = new List<long>(new long[] { }), 
            Opratorid = "",

            OrderBy = "",
            PageNumber = 0,
            SearchParam = "",
            PageSize = 0,
            Fromdate = "",
            Todate = "",

            ChargeBoxId = "",
            status = new List<string>(new string[] { }),
            Flag = "chargersession",
            Duration = "90",
            IsExport = false,
            ChartType = "",
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
                         ChargeBoxId="",
                         ChargingStatus="",
                         Endmetervalue="0",
                         EndSoc=0,
                         EndTime="",
                         ReasoneForStop="",
                         Startmetervalue="0",
                         Startsoc = 0,
                         StartTime="",
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

            _mediator.Setup(md => md.Send(It.IsAny<ChartDetailsList>(), It.IsAny<CancellationToken>())).ReturnsAsync(_ChartDetailsListResponse);
            var actionresult =  chargerController.GetChartDetailsList(_ChartDetailsListRequest).Result as ActionResult<ChartDetailsListResponse>;
            //Act
            //var actionresult = await chargerController.GetChartDetailsList(_ChartDetailsListRequest) as ActionResult<PortalRestService.Core.Responses.ChartDetailsListResponse>;

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
