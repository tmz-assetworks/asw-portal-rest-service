using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PortalRestService.Application.Queries;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Responses;
using Serilog;
using System.Dynamic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RestService.Assets.Controllers
{
#pragma warning disable
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExternalController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public ExternalController(IMediator mediator, IConfiguration configuration, ILogger<ExternalController> logger)
        {
            _mediator = mediator;
            _configuration= configuration;
        }

        [HttpGet("PaymentTransactionDetailsById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<SessionAndPaymentDTO>> GetSessionAndPaymentDetails(long PaymentTransactionId)
        {
            SessionAndPaymentDTO sessionAndPaymentDTO = new SessionAndPaymentDTO();
            try
            {              

                string access_token = string.Empty;
                using (var client = new HttpClient())
                {
                    string authapiaddress = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EXTERNAL_AUTHAPI_URL")) ? this._configuration.GetSection("ExternalAPI")["AUTHAPI_URL"].ToString() : Environment.GetEnvironmentVariable("EXTERNAL_AUTHAPI_URL").ToString();
                    Log.Information("authapiaddress base address : " + authapiaddress);
                    client.BaseAddress = new Uri(authapiaddress);
                    string username = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EXTERNAL_USERNAME")) ? this._configuration.GetSection("ExternalAPI")["USERNAME"] : Environment.GetEnvironmentVariable("EXTERNAL_USERNAME");
                    string password = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EXTERNAL_PASSWORD")) ? this._configuration.GetSection("ExternalAPI")["PASSWORD"] : Environment.GetEnvironmentVariable("EXTERNAL_PASSWORD");
                    string usernamePassword = "{\"username\": \"" + username + "\", \"password\": \"" + password + "\"}";
                    StringContent content = new StringContent(usernamePassword, Encoding.UTF8, "application/json");
                    var responseTask = await client.PostAsync("api/authentication/token", content);
                    if (responseTask.IsSuccessStatusCode)
                    {
                        string tokenresult = responseTask.Content.ReadAsStringAsync().Result;
                        if (tokenresult != null)
                        {
                            access_token = tokenresult;
                            using (var assetproductissuesclient = new HttpClient())
                            {
                                sessionAndPaymentDTO = await _mediator.Send(new ChargingSessionAndPaymentTransactionQuery(PaymentTransactionId));
                                string jsonData = string.Empty;
                                jsonData = JsonConvert.SerializeObject(sessionAndPaymentDTO.sessionAndPaymentData);

                                Log.Information("getting token from auth api  : " + access_token);
                                string assetproductissuesAddress = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EXTERNAL_ASSETPRODUCTISSUES_URL")) ? this._configuration.GetSection("ExternalAPI")["ASSET_PRODUCT_ISSUES_URL"].ToString() : Environment.GetEnvironmentVariable("EXTERNAL_ASSETPRODUCTISSUES_URL").ToString();
                                Log.Information("assetproductissues base address : " + assetproductissuesAddress);
                                assetproductissuesclient.BaseAddress = new Uri(assetproductissuesAddress);
                                assetproductissuesclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                                var response = await assetproductissuesclient.PostAsync("api/v1/assetproductissues", content);
                                if (response.IsSuccessStatusCode)
                                {
                                    sessionAndPaymentDTO.StatusMessage = RespnoseMessage.Record_Save_Successfully;
                                    sessionAndPaymentDTO.StatusCode = (int)HttpStatusCode.OK;
                                    return sessionAndPaymentDTO;
                                }
                                else
                                {
                                    sessionAndPaymentDTO.StatusMessage = RespnoseMessage.Record_Not_Saved;
                                    sessionAndPaymentDTO.StatusCode = 400;
                                    return sessionAndPaymentDTO;
                                }
                            }
                        }
                        else
                        {
                            Log.Information("Not getting access token");
                            sessionAndPaymentDTO.StatusMessage = "Not getting access token ";
                            sessionAndPaymentDTO.StatusCode = 400;
                            sessionAndPaymentDTO.sessionAndPaymentData = null;
                            return sessionAndPaymentDTO;
                        }
                    }
                    else
                    {
                        Log.Information("Not getting access token : " + responseTask.ReasonPhrase.ToString());
                        sessionAndPaymentDTO.StatusMessage = "Not getting access token : " + responseTask.ReasonPhrase.ToString();
                        sessionAndPaymentDTO.StatusCode = 400;
                        sessionAndPaymentDTO.sessionAndPaymentData = null;
                        return sessionAndPaymentDTO;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Information("Getting Exeption in ChargingSessionAndPaymentTransactionController : " + ex.Message);
                sessionAndPaymentDTO.StatusMessage = "Getting Exeption in ChargingSessionAndPaymentTransactionController : " + ex.Message;
                sessionAndPaymentDTO.StatusCode = 500;
                sessionAndPaymentDTO.sessionAndPaymentData = null;
                return sessionAndPaymentDTO;
            }
        }
    }
}
