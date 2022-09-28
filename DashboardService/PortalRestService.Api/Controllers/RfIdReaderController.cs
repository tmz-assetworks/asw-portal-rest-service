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
            RfIdReaderResponse rfIdReaderResponse = new RfIdReaderResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(rfIdReaderRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethod, httpContent,_tokenBase.acces_token);   // Returens Data with Pagination
                if (response.IsSuccessStatusCode)
                {
                    var rfIdReaders = await response.Content.ReadAsStringAsync();
                    rfIdReaderResponse = JsonConvert.DeserializeObject<RfIdReaderResponse>(rfIdReaders);
                    if (rfIdReaderResponse != null && rfIdReaderResponse.data != null && rfIdReaderResponse.data.Count() > 0)
                        rfIdReaderResponse.StatusMessage = "Record found.";
                    else rfIdReaderResponse.StatusMessage = "Record not found.";
                    rfIdReaderResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            catch (Exception ex)
            {
                rfIdReaderResponse.StatusCode = (int)HttpStatusCode.BadRequest;
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
                    rfIdReaderRespnse.StatusMessage = "Record found.";
                }
                else
                {
                    rfIdReaderRespnse.StatusMessage = "Record not found.";
                }
            }
            catch (Exception ex)
            {
                rfIdReaderRespnse.StatusMessage = "Operaion failed!";

            }
            return rfIdReaderRespnse;
        }

    }
}
