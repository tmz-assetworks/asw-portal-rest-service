using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRestService.Application.Queries;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using Serilog;
using System.Net;

namespace PortalRestService.Api.Controllers
{
    [Route("api/v1/[controller]/")]
    [ApiController]
    [Authorize]
    public class OperatorDashboardController : ControllerBase
    {
        TokenBase _tokenBase;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly double perkwtRate = 0;
        private readonly double gasolineInKiloWatt = 0;
        private readonly double lbsofCO2emitted = 0;

        public OperatorDashboardController(IMediator mediator, IConfiguration configuration, TokenBase tokenBase)
        {
            _mediator = mediator;
            this._configuration = configuration;
            gasolineInKiloWatt = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("GallongasolineKiloWatt").Value);
            lbsofCO2emitted = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("lbsofCO2emitted").Value);
            perkwtRate = (double)Convert.ToDouble(this._configuration.GetSection("EneryRatePerKg").GetSection("perkwtRate").Value);
            _tokenBase = tokenBase;
        }

        /// <summary>
        /// Retrieves all locations from the database.
        /// </summary>
        /// <returns>
        /// An ActionResult of type AllLocationQueryResponse containing all locations.
        /// Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns BadRequest if an exception occurs or no data is found.
        /// </returns>
        [HttpGet]
        [Route("GetAllLocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AllLocationQueryResponse>> GetAllLocation()
        {
            AllLocationQueryResponse alLocationQueryResponse = new AllLocationQueryResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                alLocationQueryResponse = await _mediator.Send(new GetGetAllLocationQuery());
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                alLocationQueryResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                alLocationQueryResponse.StatusCode = RespnoseCode.Bad_Request;
            }
            return alLocationQueryResponse;
        }

        /// <summary>
        /// Retrieves dispenser details for locations based on the provided request parameters,
        /// specifically for use in mapping applications.
        /// </summary>
        /// <param name="request">The request object containing filters like location IDs, operator ID, etc.</param>
        /// <returns>
        /// An ActionResult of type LocationsDispenserpResponce containing dispenser details for locations.
        /// Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns BadRequest if an exception occurs or no data is found.
        /// </returns>
        [HttpPost]
        [Route("GetLocationsDispenserformap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationsDispenserpResponce>> GetLocationsDispenserformap([FromBody] LocationOpratorRequest request)
        {
            LocationsDispenserpResponce? locationsDispenserformapResponce = new LocationsDispenserpResponce();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new LocationOpratorQuery(request));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves dispenser details for locations based on the provided request parameters.
        /// </summary>
        /// <param name="request">The request object containing filters like PageSize, PageNumber, etc.</param>
        /// <returns>
        /// An ActionResult of type LocationsDispenserDetailsResponce containing dispenser details for locations.
        /// Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns BadRequest if an exception occurs or no data is found.
        /// </returns>
        [HttpPost]
        [Route("GetLocationsDispenserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationsDispenserDetailsResponce>> GetLocationsDispenserDetails([FromBody] LocationDispenserDetailRequest request)
        {
            LocationsDispenserDetailsResponce? locationsDispenserDetailsResponce = new LocationsDispenserDetailsResponce();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                if (request.PageSize == 0) request.PageSize = 10;
                if (request.PageNumber == 0) request.PageNumber = 1;
                var location = await _mediator.Send(new GetLocationsDispenserDetailsQuery(request));

                if (location != null && location.Count > 0)
                {
                    locationsDispenserDetailsResponce.StatusMessage = RespnoseMessage.Record_found;
                    locationsDispenserDetailsResponce.data = location;
                    locationsDispenserDetailsResponce.paginationResponse = new Core.PagingHelper.PaginationResponse
                    {
                        TotalCount = location.TotalCount,
                        PageSize = location.PageSize,
                        CurrentPage = location.CurrentPage,
                        TotalPages = location.TotalPages,
                        HasNext = location.HasNext,
                        HasPrevious = location.HasPrevious
                    };
                    locationsDispenserDetailsResponce.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    locationsDispenserDetailsResponce.StatusMessage = RespnoseMessage.Record_not_found;
                    locationsDispenserDetailsResponce.data = null;
                    locationsDispenserDetailsResponce.paginationResponse = new PaginationResponse();
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                locationsDispenserDetailsResponce.StatusMessage = RespnoseMessage.Opeartion_Failed;
                locationsDispenserDetailsResponce.StatusCode = RespnoseCode.Bad_Request;
            }
            return locationsDispenserDetailsResponce;
        }

        /// <summary>
        /// Retrieves summary status data for a specified location based on the provided location ID and a flag indicating if charger details are required.
        /// </summary>
        /// <param name="locationId">The ID of the location for which summary status data is requested. Defaults to 0 if not provided.</param>
        /// <param name="isChargersReq">A boolean flag indicating whether detailed charger information is required.</param>
        /// <returns>
        /// An ActionResult of type CardDataResponse. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        [HttpGet]
        [Route("GetSummaryStatus/{locationId}/{isChargersReq}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardDataResponse>> GetSummaryStatus(int locationId = 0, bool isChargersReq = false)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetSummaryStatusQuery(locationId, isChargersReq));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves summary data for a specified location based on the provided location ID.
        /// </summary>
        /// <param name="locationId">The ID of the location for which summary data is requested. Defaults to 0 if not provided.</param>
        /// <returns>
        /// An ActionResult of type SummaryData. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        [HttpGet]
        [Route("GetSummaryData/{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SummaryData>> GetSummaryData(int locationId = 0)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetSummaryDataQuery(locationId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the charging session data for specific locations based on the provided request parameters.
        /// </summary>
        /// <param name="chargerSessionRequest">The request object containing filter parameters for fetching charging session data.</param>
        /// <returns>
        /// An ActionResult of type List of ChargingSessionByLocationForChartResponse. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        [HttpPost("ChargingSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargingSessionByLocationForChartResponse>>> ChargingSession([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetAllChargingSessionQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration, chargerSessionRequest.chargerBoxId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the status of chargers for specific locations based on the provided request parameters.
        /// </summary>
        /// <param name="chargerSessionRequest">The request object containing filter parameters for fetching charger status data.</param>
        /// <returns>
        /// An ActionResult of type List of ChargerStatusForChartResponse. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        [HttpPost("GetChargerStatusByLocationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargerStatusForChartResponse>>> GetChargerStatusByLocationID([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetChargerByLocationIDQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration, chargerSessionRequest.chargerBoxId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves energy usage data for specific locations based on the provided request parameters.
        /// </summary>
        /// <param name="chargerSessionRequest">The request object containing filter parameters for fetching energy usage data.</param>
        /// <returns>
        /// An ActionResult of type List of EnergyUsedBOForChartResponse. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        [HttpPost("GetEnergyUsedByLocationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Core.Responses.EnergyUsedBOForChartResponse>>> GetEnergyUsedByLocationID([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetEnergyUsedsByLocationIDQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration, chargerSessionRequest.chargerBoxId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves performance data for locations based on the provided request parameters.
        /// </summary>
        /// <param name="locationPerformingRequest">The request object containing filter parameters for fetching location performance data.</param>
        /// <returns>
        /// An ActionResult of type List of LocationPerformingChartResponse. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        /// <response code="200">If the location performance data is successfully retrieved.</response>
        /// <response code="400">If an exception occurs during the process.</response>
        [HttpPost("GetLocationPerforming")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Core.Responses.LocationPerformingChartResponse>>> GetLocationPerforming([FromBody] LocationPerformingRequest locationPerformingRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetLocationPerformingQuery(locationPerformingRequest.LocationIds, locationPerformingRequest.Duration, locationPerformingRequest.Orderby));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the miles added by location based on the provided request parameters.
        /// </summary>
        /// <param name="milesAddedByLocationRequest">The request object containing filter parameters for fetching miles added by location.</param>
        /// <returns>
        /// An ActionResult of type List of MilesAddedByLocationChartResponse. Returns Status200OK if the operation is successful with the retrieved data.
        /// Returns NotFound if no data is found. Returns BadRequest if an exception occurs.
        /// </returns>
        /// <response code="200">If the miles added by location data is successfully retrieved.</response>
        /// <response code="400">If an exception occurs during the process.</response>
        [HttpPost("GetMilesAddedByLocation")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Core.Responses.MilesAddedByLocationChartResponse>>> GetMilesAddedByLocation([FromBody] MilesAddedByLocationRequest milesAddedByLocationRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetMilesAddedByLocationQuery(milesAddedByLocationRequest.LocationIds, milesAddedByLocationRequest.Duration, milesAddedByLocationRequest.chargerBoxId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves event logs for a specified location based on the provided request parameters.
        /// </summary>
        /// <param name="eventLogRequest">The request object containing filter and pagination parameters for fetching event logs by location.</param>
        /// <returns>
        /// An ActionResult of type EventLogLocationResponse. Returns Status200OK if the operation is successful,
        /// with either found records or a message indicating no records found.
        /// Returns BadRequest if an exception occurs.
        /// </returns>
        /// <response code="200">If the event logs are successfully retrieved or if no records are found.</response>
        /// <response code="400">If an exception occurs during the process.</response>
        [HttpPost("GetEventLogByLocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Core.Responses.EventLogLocationResponse>> GetEventLogByLocation([FromBody] EventLogRequest eventLogRequest)
        {
            EventLogLocationResponse QueryResponse = new EventLogLocationResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                if (eventLogRequest.PageSize == 0) eventLogRequest.PageSize = 10;
                if (eventLogRequest.PageNumber == 0) eventLogRequest.PageNumber = 1;

                var result = await _mediator.Send(new EventLogByLocationQuery(eventLogRequest));
                if (result != null && result.Count > 0)
                {
                    QueryResponse.StatusMessage = RespnoseMessage.Record_found;
                    QueryResponse.data = result;
                    QueryResponse.paginationResponse = new Core.PagingHelper.PaginationResponse
                    {
                        TotalCount = result.TotalCount,
                        PageSize = result.PageSize,
                        CurrentPage = result.CurrentPage,
                        TotalPages = result.TotalPages,
                        HasNext = result.HasNext,
                        HasPrevious = result.HasPrevious
                    };
                    QueryResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    QueryResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    QueryResponse.data = new List<EventLogLocation>();
                    QueryResponse.paginationResponse = new PaginationResponse();
                    QueryResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                QueryResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                QueryResponse.StatusCode = RespnoseCode.Bad_Request;

                QueryResponse.data = new List<EventLogLocation>();
            }
            return QueryResponse;
        }

        /// <summary>
        /// Retrieves a list of operator alerts based on the provided request parameters.
        /// </summary>
        /// <param name="operatorAlertRequest">The request object containing filter and pagination parameters for fetching operator alerts.</param>
        /// <returns>
        /// An ActionResult of type List & OperatorAlertResponse;. Returns Status200OK if the operation is successful.
        /// Returns BadRequest if an exception occurs.
        /// </returns>
        /// <response code="200">If the operator alerts are successfully retrieved.</response>
        /// <response code="400">If an exception occurs during the process.</response>
        [HttpPost("GetOperatorAlerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Core.Responses.OperatorAlertResponse>>> GetOperatorAlerts([FromBody] OperatorAlertRequest operatorAlertRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                if (ModelState.IsValid)
                {
                    if (operatorAlertRequest.PageSize == 0) operatorAlertRequest.PageSize = 10;
                    if (operatorAlertRequest.PageNumber == 0) operatorAlertRequest.PageNumber = 1;
                    var result = await _mediator.Send(new GetAllAlertsQuery(operatorAlertRequest));
                    return result == null ? NotFound() : this.Ok(result);
                }
                else
                {
                    return this.Ok(ModelState);
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the read status of a notification.
        /// </summary>
        /// <param name="notificationCommand">The notification command containing the details of the notification to be updated.</param>
        /// <returns>
        /// An ActionResult of type SaveNotificationResponse. Returns Status 200OK if the operation is successful.
        /// Returns BadRequest if an exception occurs.
        /// </returns>
        /// <response code="200">If the notification read status is successfully updated.</response>
        /// <response code="400">If an exception occurs during the process.</response>
        [HttpPost("UpdateNotificationIsRead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SaveNotificationResponse>> UpdateNotificationIsRead([FromBody] NotificationCommand notificationCommand)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                if (ModelState.IsValid)
                {
                    var result = await _mediator.Send(new UpdateNotificationIsReadQuery(notificationCommand));
                    return result == null ? NotFound() : this.Ok(result);
                }
                else
                {
                    return this.Ok(ModelState);
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the read status of an OCPP event log by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the OCPP event log to mark as read.</param>
        /// <returns>
        /// An ActionResult of type EventLogLocationResponse indicating the status of the update operation.
        /// Returns Status200OK if the operation is successful.
        /// Returns NotModified if the operation fails to update.
        /// </returns>
        [HttpPost("UpdateOcppEventLogIsRead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Core.Responses.EventLogLocationResponse>> UpdateOcppEventLogIsRead([FromBody] int id)
        {
            EventLogLocationResponse QueryResponse = new EventLogLocationResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new UpdateIsReadEventLogByIDQuery(id));
                QueryResponse.StatusMessage = RespnoseMessage.Record_Updated_Successfully;
                QueryResponse.StatusCode = (int)HttpStatusCode.OK;
                QueryResponse.data = new List<EventLogLocation>();
            }
            catch (Exception ex)
            {
                QueryResponse.StatusMessage = RespnoseMessage.Record_Not_Updated;
                QueryResponse.StatusCode = (int)HttpStatusCode.NotModified;
                QueryResponse.data = new List<EventLogLocation>(); ;
            }
            return QueryResponse;
        }

        /// <summary>
        /// Retrieves notification counts for the current user by user ID.
        /// </summary>
        /// <returns>Returns an ActionResult containing a NotificationResponse.</returns>
        [HttpPost("GetNotificationCountsByUserid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationResponse>> GetNotificationCountsByUserid()
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
            return await _mediator.Send(new GetNotificationCountQuery(notificationRequest));
        }

        /// <summary>
        /// Updates the read status of OCPP event logs for the given operator.
        /// </summary>
        /// <param name="eventLogIds">A list of event log IDs to be updated.</param>
        /// <returns>An ActionResult containing an EventLogLocationResponse indicating the result of the update operation.</returns>
        /// <response code="200">If the event logs were successfully updated.</response>
        /// <response code="500">If an internal server error occurs during the update operation.</response>
        [HttpPost("UpdateOcppEventLogAreReadByOperator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EventLogLocationResponse>> UpdateOcppEventLogAreReadByOperator([FromBody] List<int> eventLogIds)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new UpdateOcppEventLogAreReadByOperatorIdQuery(eventLogIds));
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("error occurred :" + ex.Message);

                EventLogLocationResponse response = new()
                {
                    StatusMessage = RespnoseMessage.Record_Not_Updated,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    data = new List<EventLogLocation>()
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }
    }
}