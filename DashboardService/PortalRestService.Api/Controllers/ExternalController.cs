using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
			_configuration = configuration;
		}

		[HttpPost("PaymentTransactionDetailsById")]
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
					string authapiaddress = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTHAPI_URL")) ? this._configuration.GetSection("ExternalAPI")["AUTHAPI_URL"].ToString() : Environment.GetEnvironmentVariable("AUTHAPI_URL").ToString();
					Log.Information("authapiaddress base address : " + authapiaddress);
					client.BaseAddress = new Uri(authapiaddress);
					string username = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTHAPI_USERNAME")) ? this._configuration.GetSection("ExternalAPI")["AUTHAPI_USERNAME"] : Environment.GetEnvironmentVariable("AUTHAPI_USERNAME");
					string password = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTHAPI_PASSWORD")) ? this._configuration.GetSection("ExternalAPI")["AUTHAPI_PASSWORD"] : Environment.GetEnvironmentVariable("AUTHAPI_PASSWORD");
					string usernamePassword = "{\"Username\": \"" + username + "\", \"Password\": \"" + password + "\"}";
					StringContent content = new StringContent(usernamePassword, Encoding.UTF8, "application/json");
					var responseTask = await client.PostAsync("", content);
					if (responseTask.IsSuccessStatusCode)
					{
						string tokenresult = responseTask.Content.ReadAsStringAsync().Result;
						JObject itemtoken = JObject.Parse(tokenresult);
						JArray obj = JArray.Parse(itemtoken["items"].ToString());
						access_token = obj[0].ToString();
						if (!string.IsNullOrEmpty(access_token))
						{
							using (var assetproductissuesclient = new HttpClient())
							{
								sessionAndPaymentDTO = await _mediator.Send(new ChargingSessionAndPaymentTransactionQuery(PaymentTransactionId));
								//string jsonData = JsonConvert.SerializeObject(sessionAndPaymentDTO.sessionAndPaymentData, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
								string jsonData = JsonConvert.SerializeObject(sessionAndPaymentDTO.sessionAndPaymentData);
								//JObject status1 = JObject.Parse(jsonData1);
								JObject status = JObject.Parse(jsonData);
								JArray obj1 = new JArray();
								obj1.Add(status);
								string json = "{\"body\": " + obj1 + "}";
								Log.Information("getting token from auth api  : " + access_token);
								string assetproductissuesAddress = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASSET_PRODUCT_ISSUES_URL")) ? this._configuration.GetSection("ExternalAPI")["ASSET_PRODUCT_ISSUES_URL"].ToString() : Environment.GetEnvironmentVariable("ASSET_PRODUCT_ISSUES_URL").ToString();
								Log.Information("assetproductissues base address : " + assetproductissuesAddress);
								assetproductissuesclient.BaseAddress = new Uri(assetproductissuesAddress);
								assetproductissuesclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
								content = new StringContent(json, Encoding.UTF8, "application/json");
								var response = await assetproductissuesclient.PostAsync("", content);
								if (response.IsSuccessStatusCode)
								{
									string responseresult = response.Content.ReadAsStringAsync().Result;	
									JObject jpushresponse = JObject.Parse(responseresult);
									if (Convert.ToString(jpushresponse["status"]) == "0")
									{
										Log.Information("getting REsponse " + responseresult + " from " + assetproductissuesAddress + " API ");
										sessionAndPaymentDTO.StatusMessage = RespnoseMessage.Record_Save_Successfully;
										sessionAndPaymentDTO.StatusCode = (int)HttpStatusCode.OK;

									}
									else
									{
										Log.Error("getting REsponse " + responseresult + " from " + assetproductissuesAddress + " API ");
										sessionAndPaymentDTO.StatusMessage = RespnoseMessage.Record_Not_Saved;
										sessionAndPaymentDTO.StatusCode = (int)response.StatusCode;
									}
									return sessionAndPaymentDTO;
								}
								else
								{
									string responseresult = response.Content.ReadAsStringAsync().Result;
									Log.Information("getting REsponse " + responseresult + " from " + assetproductissuesAddress + " API ");
									sessionAndPaymentDTO.StatusMessage = RespnoseMessage.Record_Not_Saved;
									sessionAndPaymentDTO.StatusCode = (int)response.StatusCode;
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
