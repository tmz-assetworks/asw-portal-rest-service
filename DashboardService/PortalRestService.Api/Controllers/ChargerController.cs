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
    public class ChargerController : ControllerBase
    {
        private readonly IMediator _mediator;
        TokenBase _tokenBase;
        public ChargerController(IMediator mediator, TokenBase token)
        {
            _mediator = mediator;
            _tokenBase = token;
        }
        [HttpPost("GetChartDetailsList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PortalRestService.Core.Responses.ChartDetailsListResponse>> GetChartDetailsList([FromBody] ChartDetailsListRequest chartDetailsListRequest)
        {
            ChartDetailsListResponse QueryResponse = new ChartDetailsListResponse();
            try
            {
                // _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");

                _tokenBase.acces_token = HttpContext != null ? await HttpContext.GetTokenAsync("access_token") : _tokenBase.acces_token;
                if (chartDetailsListRequest.PageSize == 0) chartDetailsListRequest.PageSize = 10;
                if (chartDetailsListRequest.PageNumber == 0) chartDetailsListRequest.PageNumber = 1;
                var result = await _mediator.Send(new GetChartDetailsListQuery(chartDetailsListRequest));

                if (chartDetailsListRequest.IsExport == true)
                {
                    if (result != null)
                    {
                        QueryResponse.data = result;
                        QueryResponse.StatusCode = RespnoseCode.OK;
                        QueryResponse.StatusMessage = RespnoseMessage.Record_found;
                        QueryResponse.paginationResponse = new PaginationResponse();
                    }
                    else
                    {
                        QueryResponse.data = new List<ChartDetailsList>();
                        QueryResponse.StatusCode = RespnoseCode.OK;
                        QueryResponse.StatusMessage = RespnoseMessage.Record_not_found;
                        QueryResponse.paginationResponse = new PaginationResponse();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(chartDetailsListRequest.SearchParam))
                        result = result.Where(d => d.LocationName.ToLower() == chartDetailsListRequest.SearchParam.ToLower() || d.ChargeBoxId.ToLower() == chartDetailsListRequest.SearchParam.ToLower()).ToList();

                    var dataResult = PagedList<ChartDetailsList>.ToPagedList(result,
                     chartDetailsListRequest.PageNumber,
                     chartDetailsListRequest.PageSize);


                    if (dataResult != null)
                    {

                        QueryResponse.data = dataResult;
                        QueryResponse.paginationResponse = new PortalRestService.Core.PagingHelper.PaginationResponse
                        {
                            TotalCount = dataResult.TotalCount,
                            PageSize = dataResult.PageSize,
                            CurrentPage = dataResult.CurrentPage,
                            TotalPages = dataResult.TotalPages,
                            HasNext = dataResult.HasNext,
                            HasPrevious = dataResult.HasPrevious
                        };
                        QueryResponse.StatusCode = RespnoseCode.OK;
                        QueryResponse.StatusMessage = RespnoseMessage.Record_found;
                    }
                    else
                    {

                        QueryResponse.StatusCode = RespnoseCode.OK;
                        QueryResponse.StatusMessage = RespnoseMessage.Record_not_found;
                        QueryResponse.data = new List<ChartDetailsList>();
                        QueryResponse.paginationResponse = new PaginationResponse();
                    }
                    return QueryResponse == null ? NotFound() : this.Ok(QueryResponse);
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                QueryResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                QueryResponse.StatusCode = RespnoseCode.Bad_Request;
                QueryResponse.data = new List<ChartDetailsList>();
            }
            return QueryResponse;
        }
        [HttpPost("GetChargerSessionDetailsList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PortalRestService.Core.Responses.ChargerSessionDetailsListResponse>> GetChargerSessionDetailsList([FromBody] ChargerSessionListRequest ChargerSessionRequest)
        {
            ChargerSessionDetailsListResponse QueryResponse = new ChargerSessionDetailsListResponse();
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                if (ChargerSessionRequest.PageSize == 0) ChargerSessionRequest.PageSize = 10;
                if (ChargerSessionRequest.PageNumber == 0) ChargerSessionRequest.PageNumber = 1;

                var result = await _mediator.Send(new GetChargerSessionDetailsListQuery(ChargerSessionRequest));
                if (result != null && result.Count > 0)
                {
                    QueryResponse.StatusMessage = RespnoseMessage.Record_found;
                    QueryResponse.data = result;
                    QueryResponse.paginationResponse = new PortalRestService.Core.PagingHelper.PaginationResponse
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
                    QueryResponse.StatusCode = (int)HttpStatusCode.OK;
                    QueryResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    QueryResponse.data = new List<ChargerSessionDetailsList>();
                    QueryResponse.paginationResponse = new PaginationResponse();
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                QueryResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                QueryResponse.StatusCode = RespnoseCode.Bad_Request;
                QueryResponse.data = new List<ChargerSessionDetailsList>();
            }
            return QueryResponse;
        }
        [HttpPost("GetChargerInformation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChargerInformationResponse>> GetChargerInformation([FromBody] ChargerInformationRequest chargerInformationRequest)
        {
            try
            {
                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                var result = await _mediator.Send(new GetChargerInformationQuery(chargerInformationRequest.ChargeBoxId, chargerInformationRequest.OperatorId));
                return result == null ? NotFound() : this.Ok(result);
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                return this.BadRequest($"Exception: {ex.Message}");
            }
        }
        [HttpGet("GetCommandList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PortalRestService.Core.Responses.CommandListResponse>> GetCommandList()
        {
            CommandListResponse QueryResponse = new CommandListResponse();
            List<CommandList> lin = new List<CommandList>();
            _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
            lin.Add(new CommandList() { Id = 1, value = "Authorize" });
            lin.Add(new CommandList() { Id = 2, value = "BootNotification" });
            lin.Add(new CommandList() { Id = 3, value = "Heartbeat" });
            lin.Add(new CommandList() { Id = 4, value = "StatusNotification" });
            lin.Add(new CommandList() { Id = 5, value = "GetConfiguration" });
            lin.Add(new CommandList() { Id = 6, value = "GetLocalListVersion" });
            lin.Add(new CommandList() { Id = 7, value = "ClearCache" });
            lin.Add(new CommandList() { Id = 8, value = "MeterValues" });
            lin.Add(new CommandList() { Id = 9, value = "StopTransaction" });
            lin.Add(new CommandList() { Id = 10, value = "StartTransaction" });
            lin.Add(new CommandList() { Id = 12, value = "RemoteStopTransaction" });
            lin.Add(new CommandList() { Id = 13, value = "GetCompositeSchedule" });
            lin.Add(new CommandList() { Id = 14, value = "ChangeConfiguration" });
            lin.Add(new CommandList() { Id = 15, value = "ChangeAvailability" });
            lin.Add(new CommandList() { Id = 16, value = "GetDiagnostics" });
            lin.Add(new CommandList() { Id = 17, value = "SendLocalList" });
            lin.Add(new CommandList() { Id = 18, value = "TriggerMessage" });
            lin.Add(new CommandList() { Id = 19, value = "UnlockConnector" });
            lin.Add(new CommandList() { Id = 20, value = "UpdateFirmware" });
            lin.Add(new CommandList() { Id = 21, value = "FirmwareStatusNotification" });
            lin.Add(new CommandList() { Id = 22, value = "ReserveNow" });
            lin.Add(new CommandList() { Id = 23, value = "CancelReservation" });
            lin.Add(new CommandList() { Id = 24, value = "SetChargingProfile" });
            lin.Add(new CommandList() { Id = 25, value = "Clear Charging Profile" });
            lin.Add(new CommandList() { Id = 26, value = "Diagnostics Status Notification" });
            lin.Add(new CommandList() { Id = 27, value = "Data Transfer - CS" });
            lin.Add(new CommandList() { Id = 28, value = "Data Transfer - CSMS" });
            lin.Add(new CommandList() { Id = 29, value = "Reset" });
            lin.Add(new CommandList() { Id = 30, value = "RemoteStartTransaction" });

            QueryResponse.StatusMessage = RespnoseMessage.Record_found;
            QueryResponse.data = lin;
            QueryResponse.StatusCode = (int)HttpStatusCode.OK;



            return QueryResponse;
        }
        /// <summary>
        /// Auther: Pradeep, Date 08/08/2022
        /// </summary>
        /// <param name="dispensersDetailRequest"></param>
        /// <returns></returns>
        [HttpPost("GetDispensersDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<DispensersDetailResponse> GetDispensersDetail(DispensersDetailRequest dispensersDetailRequest)
        {
            string callingMethod = APIConstant.GetDispensersList;
            DispensersDetailResponse? dispensersDetailResponse = new DispensersDetailResponse();
            try
            {

                _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
                //  StringContent httpContent = new StringContent(JsonConvert.SerializeObject(dispensersDetailRequest), Encoding.UTF8, "application/json");
                //  HttpResponseMessage response = await Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethod, httpContent,_tokenBase.acces_token);   // Returens Data with Pagination
                if (dispensersDetailRequest.PageSize == 0) dispensersDetailRequest.PageSize = 10;
                if (dispensersDetailRequest.PageNumber == 0) dispensersDetailRequest.PageNumber = 1;
                var dispensers = await _mediator.Send(new GetDispensersDetailQuery(dispensersDetailRequest));
                if (dispensers != null && dispensers.Count > 0)
                {
                    // var dispenserdetails = await response.Content.ReadAsStringAsync();
                    // dispensersDetailResponse = JsonConvert.DeserializeObject<DispensersDetailResponse>(dispenserdetails);
                    dispensersDetailResponse.StatusCode = (int)HttpStatusCode.OK;
                    dispensersDetailResponse.StatusMessage = RespnoseMessage.Record_found;
                    dispensersDetailResponse.data = dispensers;
                    dispensersDetailResponse.paginationResponse = new PortalRestService.Core.PagingHelper.PaginationResponse
                    {
                        TotalCount = dispensers.TotalCount,
                        PageSize = dispensers.PageSize,
                        CurrentPage = dispensers.CurrentPage,
                        TotalPages = dispensers.TotalPages,
                        HasNext = dispensers.HasNext,
                        HasPrevious = dispensers.HasPrevious
                    };
                    // if (dispensersDetailResponse.data.Count() > 0)
                    //     dispensersDetailResponse.StatusMessage = RespnoseMessage.Record_found;
                    // else
                    //     dispensersDetailResponse.StatusMessage = RespnoseMessage.Record_not_found;
                }
                else
                {
                    dispensersDetailResponse.StatusCode = (int)HttpStatusCode.OK;
                    dispensersDetailResponse.StatusMessage = RespnoseMessage.Record_not_found;
                }
            }
            catch (Exception ex)
            {
                Log.Information("error occurred :" + ex.Message);
                dispensersDetailResponse.StatusMessage = RespnoseMessage.Opeartion_Failed;
                dispensersDetailResponse.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return dispensersDetailResponse;

        }
        [HttpGet("GetChargeBoxIDList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PortalRestService.Core.Responses.ChargeBoxIDListResponse>> GetChargeBoxIDList()
        {
            ChargeBoxIDListResponse QueryResponse = new ChargeBoxIDListResponse();
            List<CommandList> lin = new List<CommandList>();
            _tokenBase.acces_token = await HttpContext.GetTokenAsync("access_token");
            QueryResponse = await _mediator.Send(new GetChargeBoxIDQuery());
            return QueryResponse;
        }

    }
}
