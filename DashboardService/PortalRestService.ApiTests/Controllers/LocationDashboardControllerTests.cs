using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PortalRestService.Helpers;
using RestService.Assets.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestService.Assets.Controllers.Tests
{
    [TestClass()]
    public class LocationDashboardControllerTests
    {
        private readonly LocationDashboardController _locationDashboardController;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IHttpHelper> _mockHttpHelper;
        public LocationDashboardControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new Mock<IConfiguration>();
            _mockHttpHelper = new Mock<IHttpHelper>();

            _locationDashboardController = new LocationDashboardController(_mediator.Object, _configuration.Object, _mockHttpHelper.Object)
            {

            };
            {

            }
        }
        //[TestMethod]
        //public async Task GetSummaryStatus_OK_Result_Test()
        //{
        //    // Arrange 
        //    var mockStatusSummary = GetMockChargerResponse();

        //    var mockHandler = new Mock<DelegatingHandler>();

        //    mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

        //    var seriaizedStatusSummary = JsonConvert.SerializeObject(mockStatusSummary);

        //    var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(seriaizedStatusSummary) };

        //    _mockHttpHelper.Setup(mockHttp => mockHttp.GetCallMockAPIAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

        //    // Act
        //    var actionResult = await _locationDashboardController.GetSummaryStatus();

        //    // Assert
        //    Assert.IsNotNull(actionResult);
        //    Assert.AreEqual(200, (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
        //    var summaryResponse = (actionResult.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value as StatusItemData;
        //    Assert.IsNotNull(summaryResponse);
        //}
        //[TestMethod()]
        //public void GetLocatinByIdTest()
        //{

        //}
        //#region private methods 
        //private StatusSummary GetMockStatusSummary()
        //{
        //    return new StatusSummary()
        //    {
        //        Message = "Ok",
        //        StatusCode = 1,
        //        StatusSummaryDataList = new List<StatusSummaryData>()
        //           {
        //                new StatusSummaryData()
        //                {
        //                     Type="Type 1",

        //                     Count = 1,

        //                     StatusDataList= new List<StatusItemData>()
        //                      {
        //                           new StatusItemData()
        //                           {
        //                                Key="Key",
        //                                value=1
        //                           }
        //                      },
        //                }
        //           }
        //    };
        //}

        //private ChargerResponse GetMockChargerResponse()
        //{
        //    return new ChargerResponse()
        //    {
        //        ChargerDataList = new List<ChargerData>()
        //        {
        //            new ChargerData()
        //            {
        //                 Count = 1,
        //                 StatusData=new List<StatusData>()
        //                  {
        //                      new StatusData()
        //                      {
        //                           Key="Key",
        //                           Value="Valued"
        //                      }
        //                  },
        //                 Type="Type 1"
        //            }
        //        },
        //        Message = "Ok",
        //        StatusCode = 1
        //    };
        //}
        //#endregion
    }
}