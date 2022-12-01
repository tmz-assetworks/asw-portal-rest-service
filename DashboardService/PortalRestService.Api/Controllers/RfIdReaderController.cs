using PortalRestService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortalRestService.Core.Responses;
using System.Net;
using PortalRestService.Core.PagingHelper;
using Microsoft.AspNetCore.Authorization;
using PortalRestService.Helper;
using Newtonsoft.Json;
using System.Text;
using PortalRestService.Helpers;
using PortalRestService.Infrastructure.Helper;
using Microsoft.AspNetCore.Authentication;
using PortalRestService.Core.ConstantResponse;
using Serilog;

namespace RestService.Assets.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    [Authorize]
    public class RfIdReaderController : ControllerBase
    {
        private readonly IMediator _mediator;
        TokenBase _tokenBase;
        public RfIdReaderController(IMediator mediator,TokenBase token)
        {
            _mediator = mediator;
            _tokenBase = token;
        }

        /// <summary>
        /// This api it used for getting all RfId Readers with Paggination
        /// </summary>
        /// <param name="rfIdReaderRequest"></param>
        /// <returns></returns>
        [HttpPost("GetAllRfIdReaders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RfIdReaderResponse>> GetAllRfIdReaders([FromBody] RfIdReaderRequest rfIdReaderRequest)
        {
            string callingMethod = APIConstant.GetAllRfIdReaders;
            RfIdReaderResponse? rfIdReaderResponse = new RfIdReaderResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(rfIdReaderRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethod, httpContent,_tokenBase.acces_token);   // Returens Data with Pagination
                if (response.IsSuccessStatusCode)
                {
                    var rfIdReaders = await response.Content.ReadAsStringAsync();
                    rfIdReaderResponse = JsonConvert.DeserializeObject<RfIdReaderResponse>(rfIdReaders);
                    if (rfIdReaderResponse.data.Count() > 0)
                        rfIdReaderResponse.StatusMessage = RespnoseMessage.Record_found;
                    else rfIdReaderResponse.StatusMessage = RespnoseMessage.Record_not_found;

                    rfIdReaderResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    rfIdReaderResponse.StatusCode = (int)HttpStatusCode.OK;
                    rfIdReaderResponse.StatusMessage = RespnoseMessage.Record_not_found;
                }

            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                rfIdReaderResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                rfIdReaderResponse.StatusCode = RespnoseCode.Bad_Request;

               
            }
            return rfIdReaderResponse;
        }

        /// <summary>
        /// This Api is used for getting the RfIdReader details by RfIdReader Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetRfIdReaderById/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RfIdReaderDetailsResponse>> GetRfIdReaderById(long Id)
        {
            RfIdReaderDetailsResponse rfIdReaderRespnse = new RfIdReaderDetailsResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                RfIdReaderDetailsResponse rFIDReaderDetails = await _mediator.Send(new GetRfIdReaderQuery(Id));
                rfIdReaderRespnse.StatusCode = (int)HttpStatusCode.OK;
                if (rFIDReaderDetails is not null)
                {
                    rfIdReaderRespnse.StatusMessage = RespnoseMessage.Record_found;
                }
                else
                {
                    rfIdReaderRespnse.StatusMessage = RespnoseMessage.Record_not_found;
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                rfIdReaderRespnse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                rfIdReaderRespnse.StatusCode = RespnoseCode.Bad_Request;
            }
            return rfIdReaderRespnse;
        }

    }
}
