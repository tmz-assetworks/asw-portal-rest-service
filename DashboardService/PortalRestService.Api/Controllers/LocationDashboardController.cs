using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PortalRestService.Helper;
using PortalRestService.Core.Responses;
using PortalRestService.Application.Queries;
using PortalRestService.Helpers;
using Microsoft.AspNetCore.Authorization;
using PortalRestService.Infrastructure.Helper;
using Microsoft.AspNetCore.Authentication;
using PortalRestService.Core.ConstantResponse;
using Serilog;
using PortalRestService.Core.PagingHelper;

namespace RestService.Assets.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    [Authorize]
    public class LocationDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        TokenBase _tokenBase;
        public LocationDashboardController(IMediator mediator, IConfiguration configuration,TokenBase token)
        {
            _mediator = mediator;
            this._configuration = configuration;
            this._tokenBase = token;
            
        }

        [HttpGet]
        [Route("GetLocatinById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetLocatinByIdResponse>> GetLocatinById(long id)
        {
            GetLocatinByIdResponse getLocatinByIdResponse = new GetLocatinByIdResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                LocationRequest vf= new LocationRequest();
                vf.Id = (int)id;
                getLocatinByIdResponse = await _mediator.Send(new GetLocationByIdQuery(vf));
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                getLocatinByIdResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                getLocatinByIdResponse.StatusCode = RespnoseCode.Bad_Request;
                return this.BadRequest($"Exception: {ex.Message}");
            }
            return this.Ok(getLocatinByIdResponse);
        }

        [HttpPost("LocationStatus")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AllLocationStatusChartBO>>> LocationStatus([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            LocationStatusQueryResponse locationStatusQueryResponse = new LocationStatusQueryResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetLocationStatusByLocationIdQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration));
                locationStatusQueryResponse.StatusCode = 200;
                if (result is not null && result.Count() > 0)
                {
                    locationStatusQueryResponse.StatusMessage = RespnoseMessage.Record_found;
                    locationStatusQueryResponse.data = result;
                }
                else
                {
                    locationStatusQueryResponse.StatusMessage = RespnoseMessage.Record_not_found;
                }

                return result == null ? NotFound() : this.Ok(locationStatusQueryResponse);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                locationStatusQueryResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                locationStatusQueryResponse.StatusCode = RespnoseCode.Bad_Request;
                locationStatusQueryResponse.data = new List<AllLocationStatusChartBO>();
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("GetDispenserByLocation")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationDispenserForLocationResponse>> GetDispenserByLocation([FromBody] LocationDispensersRequest request)
        {
            if (request.PageSize == 0) request.PageSize = 10;
            if (request.PageNumber == 0) request.PageNumber = 1;
            LocationDispenserForLocationResponse locationStatusQueryResponse = new LocationDispenserForLocationResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetDispenserByLocationIdQuery(request));

                var dataResult = PagedList<LocationDispenserForLocation>.ToPagedList(result.data,request.PageNumber,request.PageSize);

                if (dataResult != null)
                {

                    locationStatusQueryResponse.data = dataResult;
                    locationStatusQueryResponse.paginationResponse = new PortalRestService.Core.PagingHelper.PaginationResponse
                    {
                        TotalCount = dataResult.TotalCount,
                        PageSize = dataResult.PageSize,
                        CurrentPage = dataResult.CurrentPage,
                        TotalPages = dataResult.TotalPages,
                        HasNext = dataResult.HasNext,
                        HasPrevious = dataResult.HasPrevious
                    };
                    locationStatusQueryResponse.StatusCode = RespnoseCode.OK;
                    locationStatusQueryResponse.StatusMessage = RespnoseMessage.Record_found;
                }
                else
                {

                    locationStatusQueryResponse.StatusCode = RespnoseCode.OK;
                    locationStatusQueryResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    locationStatusQueryResponse.data = new List<LocationDispenserForLocation>();
                    locationStatusQueryResponse.paginationResponse = new PaginationResponse();
                }

                return result == null ? NotFound() : this.Ok(locationStatusQueryResponse);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
    }

}
