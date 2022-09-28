using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PortalRestService.Helper;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Responses;
using PortalRestService.Core.Entities.Charger;
using System.Net;
using PortalRestService.Application;
using PortalRestService.Helpers;

using PortalRestService.Infrastructure.EnumData;
using System.Text;
using PortalRestService.Infrastructure.Repositories;
using PortalRestService.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Infrastructure.Helper;
using Microsoft.AspNetCore.Authentication;

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
        //private readonly IHttpHelper _httpHelper;

        public OperatorDashboardController(IMediator mediator, IConfiguration configuration, TokenBase tokenBase)
        {
            _mediator = mediator;
            this._configuration = configuration;
            gasolineInKiloWatt = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("GallongasolineKiloWatt").Value);
            lbsofCO2emitted = (double)Convert.ToDouble(this._configuration.GetSection("GasolineIoKiloWatt").GetSection("lbsofCO2emitted").Value);
            perkwtRate = (double)Convert.ToDouble(this._configuration.GetSection("EneryRatePerKg").GetSection("perkwtRate").Value);
            _tokenBase = tokenBase;
        }

        [HttpGet]
        [Route("GetAllLocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AllLocationQueryResponse>> GetAllLocation()
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                string callingMethod = APIConstant.GetAllLocationName;
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetAuthAPIAsync(callingMethod,_tokenBase.acces_token);
                AllLocationQueryResponse alLocationQueryResponse = new AllLocationQueryResponse();
                if (response.IsSuccessStatusCode)
                {
                    var locationinfo = await response.Content.ReadAsStringAsync();
                    return Ok(JsonConvert.DeserializeObject<AllLocationQueryResponse>(locationinfo));
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


                return alLocationQueryResponse == null ? NotFound() : this.Ok(alLocationQueryResponse);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }


        }

        [HttpPost]
        [Route("GetLocationsDispenserformap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationsDispenserformapResponce>> GetLocationsDispenserformap([FromBody] LocationOpratorRequest request)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                string callingMethod = APIConstant.Getlocationsdispenserformap;
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(request.LocationIds), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethod, httpContent,_tokenBase.acces_token);
                LocationsDispenserformapResponce locationsDispenserformapResponce = new LocationsDispenserformapResponce();
                if (response.IsSuccessStatusCode)
                {
                    var locationdispenserformapinfo = await response.Content.ReadAsStringAsync();
                    locationsDispenserformapResponce = JsonConvert.DeserializeObject<LocationsDispenserformapResponce>(locationdispenserformapinfo);

                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


                return locationsDispenserformapResponce == null ? NotFound() : this.Ok(locationsDispenserformapResponce);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Locations with Paginaiton
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks> Auther : Pradeep , Date : 08/08/2022</remarks>
        [HttpPost]
        [Route("GetLocationsDispenserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LocationsDispenserDetailsResponce>> GetLocationsDispenserDetails([FromBody] LocationDispenserDetailRequest request)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                string callingMethod = APIConstant.GetLocationsDispenserDetails;
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethod, httpContent,_tokenBase.acces_token);   // Returens Data with Pagination
                LocationsDispenserDetailsResponce locationsDispenserDetailsResponce = new LocationsDispenserDetailsResponce();
                if (response.IsSuccessStatusCode)
                {

                    var locationdispenserdetailsinfo = await response.Content.ReadAsStringAsync();
                    locationsDispenserDetailsResponce = JsonConvert.DeserializeObject<LocationsDispenserDetailsResponce>(locationdispenserdetailsinfo);
                    if (locationsDispenserDetailsResponce != null && locationsDispenserDetailsResponce.data != null && locationsDispenserDetailsResponce.data.Count() > 0)
                        locationsDispenserDetailsResponce.StatusMessage = "Record found.";
                    else locationsDispenserDetailsResponce.StatusMessage = "Record not found.";
                    locationsDispenserDetailsResponce.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
                return locationsDispenserDetailsResponce == null ? NotFound() : this.Ok(locationsDispenserDetailsResponce);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Return the location, chararger , charging Session and Error
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("GetSummaryStatus/{locationId}/{isChargersReq}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardDataResponse>> GetSummaryStatus(int locationId = 0, bool isChargersReq = false)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetSummaryStatusQuery(locationId,isChargersReq));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }


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
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("ChargingSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChargingSessionByLocationForChartResponse>>> ChargingSession([FromBody] ChargerSessionRequest chargerSessionRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetAllChargingSessionQuery(chargerSessionRequest.LocationIds, chargerSessionRequest.Duration,chargerSessionRequest.chargerBoxId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

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
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }


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
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

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
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
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
                var result = await _mediator.Send(new GetMilesAddedByLocationQuery(milesAddedByLocationRequest.LocationIds, milesAddedByLocationRequest.Duration,milesAddedByLocationRequest.chargerBoxId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
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
                    QueryResponse.StatusMessage = "Record found";
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
                    QueryResponse.StatusMessage = "Record not found";
                    QueryResponse.data = null;
                    QueryResponse.paginationResponse = new PaginationResponse();
                    QueryResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                //return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                QueryResponse.StatusMessage = "Operation failed!";
                QueryResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                QueryResponse.data = null;
            }
            return QueryResponse;
        }

        /// <summary>
        /// Operator Alert with pagination and filter
        /// </summary>
        /// <param name="operatorAlertRequest"></param>
        /// <returns></returns>
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
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("UpdateOcppEventLogIsRead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Core.Responses.EventLogLocationResponse>> UpdateOcppEventLogIsRead([FromBody] int id)
        {
            EventLogLocationResponse QueryResponse = new EventLogLocationResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new UpdateIsReadEventLogByIDQuery(id));
                QueryResponse.StatusMessage = "updated successfully!";
                QueryResponse.StatusCode = (int)HttpStatusCode.OK;
                QueryResponse.data = null;
            }
            catch (Exception ex)
            {
                QueryResponse.StatusMessage = "Not updated";
                QueryResponse.StatusCode = (int)HttpStatusCode.NotFound;
                QueryResponse.data = null;
            }
            return QueryResponse;
        }


        ///// <summary>
        ///// Auther: Pradeep, Date 08/08/2022
        ///// </summary>
        ///// <param name="dispensersDetailRequest"></param>
        ///// <returns></returns>
        //[HttpPost("GetDispensersDetail")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<DispensersDetailResponse> GetDispensersDetail(DispensersDetailRequest dispensersDetailRequest)
        //{
        //    string callingMethod = APIConstant.GetDispensersList;
        //    DispensersDetailResponse dispensersDetailResponse = new DispensersDetailResponse();
        //    try
        //    {
        //        StringContent httpContent = new StringContent(JsonConvert.SerializeObject(dispensersDetailRequest), Encoding.UTF8, "application/json");
        //        HttpResponseMessage response = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethod, httpContent);   // Returens Data with Pagination

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var dispenserdetails = await response.Content.ReadAsStringAsync();
        //            dispensersDetailResponse = JsonConvert.DeserializeObject<DispensersDetailResponse>(dispenserdetails);
        //            if (dispensersDetailResponse != null && dispensersDetailResponse.data != null && dispensersDetailResponse.data.Count() > 0)
        //                dispensersDetailResponse.StatusMessage = "Record found.";
        //            else dispensersDetailResponse.StatusMessage = "Record not found.";
        //            dispensersDetailResponse.StatusCode = (int)HttpStatusCode.OK;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Internal server Error");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dispensersDetailResponse.StatusCode = (int)HttpStatusCode.BadRequest;
        //    }
        //    return dispensersDetailResponse;


        //}
        
        /*[HttpPost("GetChargerSessionDetailsList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Core.Responses.ChargerSessionDetailsListResponse>> GetChargerSessionDetailsList([FromBody] ChargerSessionListRequest ChargerSessionRequest)
        {
            ChargerSessionDetailsListResponse QueryResponse = new ChargerSessionDetailsListResponse();
            try
            {
                if (ChargerSessionRequest.PageSize == 0) ChargerSessionRequest.PageSize = 10;
                if (ChargerSessionRequest.PageNumber == 0) ChargerSessionRequest.PageNumber = 1;

                var result = await _mediator.Send(new GetChargerSessionDetailsListQuery(ChargerSessionRequest));
                if (result != null && result.Count > 0)
                {
                    QueryResponse.StatusMessage = "Record found";
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
                    QueryResponse.StatusMessage = "Record not found";
                    QueryResponse.data = null;
                    QueryResponse.paginationResponse = new PaginationResponse();
                }               
            }
            catch (Exception ex)
            {
                QueryResponse.StatusMessage = "Operation failed!";
                QueryResponse.StatusCode = (int)HttpStatusCode.NotFound;
                QueryResponse.data = null;
            }
            return QueryResponse;
        }*/


    }
}
