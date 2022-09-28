using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using System.Net;

namespace RestService.Assets.Controllers
{

    [Route("api/v1/[controller]/")]
    [ApiController]
    [Authorize]
    public class VehicleDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        TokenBase _tokenBase;
        public VehicleDashboardController(IMediator mediator, IConfiguration configuration,TokenBase token)
        {
            _mediator = mediator;
            this._configuration = configuration;
            _tokenBase = token; 
        }

        /// <summary>
        /// Get Vehicle Details by By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetVehicleByID/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VehiclesResponse>> GetVehicleByID(long id)
        {
            VehiclesResponse vehiclesResponse = new VehiclesResponse();
            VehicleByIdData vehicleByIdData = new VehicleByIdData();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                vehicleByIdData = await _mediator.Send(new GetVehicleByIdQuery(id));
                if (vehicleByIdData != null && !(string.IsNullOrEmpty(vehicleByIdData.VIN)))
                {
                    vehiclesResponse.data = vehicleByIdData;
                    vehiclesResponse.StatusMessage = "Record found!";
                }
                else
                {
                    vehiclesResponse.data = vehicleByIdData;
                    vehiclesResponse.StatusMessage = "Record not found";
                }
                vehiclesResponse.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                vehiclesResponse.StatusMessage = "Operation Failed!";
                vehiclesResponse.StatusCode = 400;
            }
            return vehiclesResponse;
        }

        /// <summary>
        /// Get Vehicles list
        /// </summary>
        /// <param name="getAllVehicleRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAllVehicle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetAllVehicleResponse>> GetAllVehicle([FromBody] GetAllVehicleRequest getAllVehicleRequest)
        {
            GetAllVehicleResponse getAll = new GetAllVehicleResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                StatusList status = new StatusList();
                List<StatusData> statusData = new List<StatusData>();
                vehicleWithPagination vehicleWithPaginatio = new vehicleWithPagination();
                vehicleWithPaginatio = await _mediator.Send(new GetAllVehicleQuery(getAllVehicleRequest));
                if (vehicleWithPaginatio.data != null && vehicleWithPaginatio.data.Count > 0)
                {
                    getAll.data = vehicleWithPaginatio.data;
                    getAll.paginationResponse = vehicleWithPaginatio.paginationResponse;
                    getAll.StatusCode = (int)HttpStatusCode.OK;
                    getAll.StatusMessage = "Record found";
                    status.Type = "Vehicle";
                    status.Count = vehicleWithPaginatio.paginationResponse.TotalCount;
                    statusData = new List<StatusData>(){
                            new StatusData () {
                                Key = "Active",
                                Value = vehicleWithPaginatio.Active,
                                Color = "#90993F",
                            },
                            new StatusData () {
                                Key = "Inactive",
                                Value = vehicleWithPaginatio.Inactive,
                                Color = "#775577",
                            }
                    };
                    getAll.statusList = status;
                    getAll.statusList.StatusData = statusData;
                    return getAll;
                }
                else
                {
                    getAll.StatusCode = 200;
                    getAll.StatusMessage = "Record  not found";
                    getAll.data = null;
                    getAll.paginationResponse = new PortalRestService.Core.PagingHelper.PaginationResponse();
                }
            }
            catch (Exception ex)
            {
                getAll.StatusMessage = "Operation Failed!";
                getAll.StatusCode =(int)HttpStatusCode.BadRequest;
            }
            return getAll;
        }
    }
}
