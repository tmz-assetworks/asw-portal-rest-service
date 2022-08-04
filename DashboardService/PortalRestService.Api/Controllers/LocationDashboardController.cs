using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PortalRestService.Helper;
using PortalRestService.Core.Responses;
using PortalRestService.Application.Queries;
using PortalRestService.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace RestService.Assets.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    [Authorize]
    public class LocationDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
      
        public LocationDashboardController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            this._configuration = configuration;
            
        }

        [HttpGet]
        [Route("GetLocatinById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetLocatinByIdResponse>> GetLocatinById(long id)
        {
            try
            {
                string callingMethod = APIConstant.GetLocationById + id;
                HttpResponseMessage response = await Helper.GetCallAssetAPIAsync(callingMethod);
                GetLocatinByIdResponse getLocatinByIdResponse = new GetLocatinByIdResponse();
                if (response.IsSuccessStatusCode)
                {
                    var locationinfo = await response.Content.ReadAsStringAsync();
                    getLocatinByIdResponse = JsonConvert.DeserializeObject<GetLocatinByIdResponse>(locationinfo);

                    if (getLocatinByIdResponse != null && getLocatinByIdResponse.data!=null) 
                     getLocatinByIdResponse.data.Id= (int)id;

                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }

                return getLocatinByIdResponse == null ? NotFound() : this.Ok(getLocatinByIdResponse);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }


        }


        [HttpPost("LocationStatus")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AllLocationStatusChartBO>>> LocationStatus([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            LocationStatusQueryResponse locationStatusQueryResponse = new LocationStatusQueryResponse();
            try
            {
                var result = await _mediator.Send(new GetLocationStatusByLocationIdQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration));
                locationStatusQueryResponse.StatusMessage = "Record Found";
                locationStatusQueryResponse.StatusCode = 200;
                locationStatusQueryResponse.data = result;
                return result == null ? NotFound() : this.Ok(locationStatusQueryResponse);
            }
            catch (Exception ex)
            {
                locationStatusQueryResponse.StatusMessage = "Record not Found";
                locationStatusQueryResponse.StatusCode = 404;
                locationStatusQueryResponse.data = null;
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("GetDispenserByLocation")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationDispenserForLocationResponse>> GetDispenserByLocation([FromBody] List<long> Id)
        {
            LocationDispenserForLocationResponse locationStatusQueryResponse = new LocationDispenserForLocationResponse();
            try
            {
                var result = await _mediator.Send(new GetDispenserByLocationIdQuery(Id));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }


    }

}
